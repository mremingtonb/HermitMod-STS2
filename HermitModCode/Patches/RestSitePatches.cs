using System.Linq;
using Godot;
using HarmonyLib;
using HermitMod.Character;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HermitMod.Patches;

/// <summary>
/// Replaces the Ironclad placeholder in the rest site (campfire) scene
/// with a seated animated Hermit character model.
/// </summary>
[HarmonyPatch(typeof(NRestSiteRoom), "_Ready")]
public static class RestSiteRoomPatch
{
    [HarmonyPostfix]
    static void Postfix(NRestSiteRoom __instance)
    {
        try
        {
            foreach (var restChar in __instance.Characters)
            {
                if (restChar?.Player?.Character is not Hermit) continue;

                // Hide all SpineSprite children (the Ironclad placeholder)
                foreach (var child in restChar.GetChildren().OfType<Node2D>())
                {
                    if (child.GetClass() == "SpineSprite")
                    {
                        child.Visible = false;
                    }
                }

                // Add the seated Hermit
                var hermitNode = HermitRestSiteBuilder.Build();
                restChar.AddChild(hermitNode);
            }
        }
        catch
        {
            // Silently fail — worst case the Ironclad placeholder shows
        }
    }
}

/// <summary>
/// Builds a seated version of the Hermit for the rest site / campfire scene.
/// Body is lowered, legs are tucked/bent, gun arm rests on knee,
/// and the idle animation is a gentle breathing motion.
/// </summary>
public static class HermitRestSiteBuilder
{
    private const string CharDir = "res://HermitMod/images/char/";

    // Seated waist is much lower than standing (-48 in combat)
    private const float WaistY = -20f;

    public static Node2D Build()
    {
        var root = new Node2D();
        root.Name = "HermitRestSite";
        root.Position = new Vector2(0, 30);
        root.Scale = new Vector2(1.8f, 1.8f);

        // Shadow beneath
        var shadow = CreateSprite("Shadow", CharDir + "shadow.png", new Vector2(0, -5));
        shadow.Scale = new Vector2(0.8f, 0.6f); // Flatter shadow for sitting
        root.AddChild(shadow);

        // Visuals container (matches node path structure for animations)
        var visuals = new Node2D();
        visuals.Name = "Visuals";
        visuals.Scale = new Vector2(1.3f, 1.3f);
        root.AddChild(visuals);

        // Waist (main pivot) — lower to the ground for seated pose
        var waist = new Node2D();
        waist.Name = "Waist";
        waist.Position = new Vector2(2, WaistY);
        visuals.AddChild(waist);

        // Legs — rotated forward to simulate sitting on the log
        var rightLeg = CreateSprite("RightLeg", CharDir + "leg_right.png", new Vector2(12, 30));
        rightLeg.Rotation = 0.7f; // Bent forward
        waist.AddChild(rightLeg);

        var leftLeg = CreateSprite("LeftLeg", CharDir + "leg_left.png", new Vector2(-10, 32));
        leftLeg.Rotation = 0.5f; // Bent forward
        waist.AddChild(leftLeg);

        // Body — slight forward lean
        var body = CreateSprite("Body", CharDir + "body.png", new Vector2(0, -5));
        body.Offset = new Vector2(0, -50);
        waist.AddChild(body);

        // Right arm — resting on knee
        var rightArm = CreateSprite("RightArm", CharDir + "right_hand.png", new Vector2(32, -60));
        rightArm.Rotation = 0.5f; // Resting downward
        waist.AddChild(rightArm);

        // Left arm (with gun) — resting, gun pointing away/downward
        var leftArm = new Node2D();
        leftArm.Name = "LeftArm";
        leftArm.Position = new Vector2(-30, -65);
        leftArm.Rotation = -0.6f; // Arm angled outward, away from head
        waist.AddChild(leftArm);

        var handGun = CreateSprite("HandGun", CharDir + "hand_gun.png", new Vector2(-5, 5));
        leftArm.AddChild(handGun);

        var gun = CreateSprite("Gun", CharDir + "gun.png", new Vector2(-12, -12));
        gun.Rotation = 0.8f; // Gun barrel pointing down and away
        gun.Scale = new Vector2(0.85f, 0.85f);
        leftArm.AddChild(gun);

        // Head — tilted slightly, looking toward the fire
        var head = new Node2D();
        head.Name = "Head";
        head.Position = new Vector2(-2, -100);
        waist.AddChild(head);

        var hat = CreateSprite("Hat", CharDir + "hat.png", new Vector2(8, -40));
        head.AddChild(hat);

        var eye = CreateSprite("Eye", CharDir + "eye.png", new Vector2(18, -10));
        head.AddChild(eye);

        // Gentle seated idle animation (breathing only, very subtle)
        var animPlayer = CreateSeatedIdleAnimation();
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

    private static AnimationPlayer CreateSeatedIdleAnimation()
    {
        var player = new AnimationPlayer();
        player.Name = "AnimPlayer";

        var anim = new Animation();
        anim.Length = 3.0f; // Slower cycle for a restful feel
        anim.LoopMode = Animation.LoopModeEnum.Linear;

        // Gentle breathing — slight vertical bob on the waist
        int t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(2, WaistY));
        anim.TrackInsertKey(t, 1.5f, new Vector2(2, WaistY + 2f));
        anim.TrackInsertKey(t, 3.0f, new Vector2(2, WaistY));

        // Very subtle body lean (relaxed sway)
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.04f); // Slight forward lean
        anim.TrackInsertKey(t, 1.5f, 0.06f);
        anim.TrackInsertKey(t, 3.0f, 0.04f);

        // Head gentle nod
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:position");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, new Vector2(-2, -100));
        anim.TrackInsertKey(t, 1.5f, new Vector2(-2, -98.5f));
        anim.TrackInsertKey(t, 3.0f, new Vector2(-2, -100));

        // Very slow head tilt (gazing at fire)
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/Head:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.05f);
        anim.TrackInsertKey(t, 1.5f, 0.09f);
        anim.TrackInsertKey(t, 3.0f, 0.05f);

        // Left arm very slight sway (gun resting, pointing away)
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/LeftArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, -0.6f);
        anim.TrackInsertKey(t, 1.5f, -0.57f);
        anim.TrackInsertKey(t, 3.0f, -0.6f);

        // Right arm very slight movement (resting on knee)
        t = anim.AddTrack(Animation.TrackType.Value);
        anim.TrackSetPath(t, "Visuals/Waist/RightArm:rotation");
        anim.TrackSetInterpolationType(t, Animation.InterpolationType.Cubic);
        anim.TrackInsertKey(t, 0f, 0.5f);
        anim.TrackInsertKey(t, 1.5f, 0.53f);
        anim.TrackInsertKey(t, 3.0f, 0.5f);

        var lib = new AnimationLibrary();
        lib.AddAnimation("idle", anim);
        player.AddAnimationLibrary("", lib);

        return player;
    }
}
