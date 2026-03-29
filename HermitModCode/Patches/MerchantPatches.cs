using System.Collections.Generic;
using Godot;
using HarmonyLib;
using HermitMod.Character;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace HermitMod.Patches;

/// <summary>
/// Replaces the Ironclad placeholder in the merchant scene with the full animated
/// Hermit character model (same body parts and idle animation as combat).
/// </summary>
[HarmonyPatch(typeof(NMerchantRoom), "AfterRoomIsLoaded")]
public static class MerchantRoomPatch
{
    [HarmonyPostfix]
    static void Postfix(NMerchantRoom __instance)
    {
        try
        {
            var playersField = AccessTools.Field(typeof(NMerchantRoom), "_players");
            var visualsField = AccessTools.Field(typeof(NMerchantRoom), "_playerVisuals");

            if (playersField == null || visualsField == null) return;

            var players = playersField.GetValue(__instance) as List<Player>;
            var visuals = visualsField.GetValue(__instance) as List<NMerchantCharacter>;

            if (players == null || visuals == null) return;

            for (int i = 0; i < players.Count && i < visuals.Count; i++)
            {
                if (players[i].Character is not Hermit) continue;

                var merchantChar = visuals[i];
                if (merchantChar == null) continue;

                // Hide the spine animation (first child — Ironclad placeholder)
                if (merchantChar.GetChildCount() > 0)
                {
                    var spineChild = merchantChar.GetChild(0);
                    if (spineChild is Node2D spineNode)
                        spineNode.Visible = false;
                }

                // Build the full animated Hermit using the same visuals as combat
                var hermitNode = HermitMerchantBuilder.Build();
                merchantChar.AddChild(hermitNode);
            }
        }
        catch
        {
            // Silently fail — worst case the Ironclad placeholder shows
        }
    }
}

/// <summary>
/// Builds a merchant-scene version of the Hermit using the same body-part sprites
/// and idle animation as the combat model, but without NCreatureVisuals or marker nodes.
/// Scaled and positioned appropriately for the merchant scene.
/// </summary>
public static class HermitMerchantBuilder
{
    private const string CharDir = "res://HermitMod/images/char/";
    private const float WaistY = -48f;

    public static Node2D Build()
    {
        // Root container positioned/scaled for the merchant scene
        var root = new Node2D();
        root.Name = "HermitMerchant";
        root.Position = new Vector2(0, -10);
        root.Scale = new Vector2(1.3f, 1.3f);

        // Shadow at ground level
        var shadow = CreateSprite("Shadow", CharDir + "shadow.png", new Vector2(0, -5));
        root.AddChild(shadow);

        // Visuals container (matches combat structure so animation paths work)
        var visuals = new Node2D();
        visuals.Name = "Visuals";
        visuals.Scale = new Vector2(1.3f, 1.3f);
        root.AddChild(visuals);

        // Waist (main pivot)
        var waist = new Node2D();
        waist.Name = "Waist";
        waist.Position = new Vector2(2, WaistY);
        visuals.AddChild(waist);

        // Legs
        var rightLeg = CreateSprite("RightLeg", CharDir + "leg_right.png", new Vector2(18, 43));
        waist.AddChild(rightLeg);

        var leftLeg = CreateSprite("LeftLeg", CharDir + "leg_left.png", new Vector2(-20, 45));
        waist.AddChild(leftLeg);

        // Body
        var body = CreateSprite("Body", CharDir + "body.png", new Vector2(0, -5));
        body.Offset = new Vector2(0, -50);
        waist.AddChild(body);

        // Right arm
        var rightArm = CreateSprite("RightArm", CharDir + "right_hand.png", new Vector2(38, -82));
        rightArm.Rotation = 0.264f;
        waist.AddChild(rightArm);

        // Left arm (with gun)
        var leftArm = new Node2D();
        leftArm.Name = "LeftArm";
        leftArm.Position = new Vector2(-30, -88);
        leftArm.Rotation = -0.15f;
        waist.AddChild(leftArm);

        var handGun = CreateSprite("HandGun", CharDir + "hand_gun.png", new Vector2(-5, 5));
        leftArm.AddChild(handGun);

        var gun = CreateSprite("Gun", CharDir + "gun.png", new Vector2(-12, -12));
        gun.Rotation = 0.3f;
        gun.Scale = new Vector2(0.85f, 0.85f);
        leftArm.AddChild(gun);

        // Head
        var head = new Node2D();
        head.Name = "Head";
        head.Position = new Vector2(-2, -105);
        waist.AddChild(head);

        var hat = CreateSprite("Hat", CharDir + "hat.png", new Vector2(8, -40));
        head.AddChild(hat);

        var eye = CreateSprite("Eye", CharDir + "eye.png", new Vector2(10, -10));
        head.AddChild(eye);

        // Idle animation (same as combat idle)
        var animPlayer = CreateIdleAnimationPlayer();
        root.AddChild(animPlayer);
        animPlayer.CallDeferred("play", "idle");

        return root;
    }

    private static Sprite2D CreateSprite(string name, string texturePath, Vector2 position)
    {
        var sprite = new Sprite2D();
        sprite.Name = name;
        sprite.Position = position;
        sprite.Texture = GD.Load<Texture2D>(texturePath);
        return sprite;
    }

    private static AnimationPlayer CreateIdleAnimationPlayer()
    {
        var player = new AnimationPlayer();
        player.Name = "AnimPlayer";

        var anim = new Animation();
        anim.Length = 1.6666f;
        anim.LoopMode = Animation.LoopModeEnum.Linear;

        // Waist gentle sway
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 0.8333f, new Vector2(6.84f, WaistY));
        anim.TrackInsertKey(t, 1.6666f, new Vector2(2, WaistY));

        // Head bob
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-2, -105));
        anim.TrackInsertKey(t, 0.8333f, new Vector2(-2, -103.2f));
        anim.TrackInsertKey(t, 1.6666f, new Vector2(-2, -105));

        // Head tilt
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0f);
        anim.TrackInsertKey(t, 0.8333f, 0.111f);
        anim.TrackInsertKey(t, 1.6666f, 0f);

        // Left arm sway
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-30, -88));
        anim.TrackInsertKey(t, 0.8333f, new Vector2(-34, -94));
        anim.TrackInsertKey(t, 1.6666f, new Vector2(-30, -88));

        // Left arm rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, -0.15f);
        anim.TrackInsertKey(t, 0.8333f, -0.35f);
        anim.TrackInsertKey(t, 1.6666f, -0.15f);

        // Right arm rotation
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/RightArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.264f);
        anim.TrackInsertKey(t, 0.8333f, 0.518f);
        anim.TrackInsertKey(t, 1.6666f, 0.264f);

        // Gun wobble
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm/Gun:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.4f);
        anim.TrackInsertKey(t, 1.1667f, 0.154f);
        anim.TrackInsertKey(t, 1.6667f, 0.4f);

        var lib = new AnimationLibrary();
        lib.AddAnimation("idle", anim);
        player.AddAnimationLibrary("", lib);

        return player;
    }
}
