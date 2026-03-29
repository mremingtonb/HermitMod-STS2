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
/// Replaces the Ironclad placeholder in the merchant scene with an animated Hermit image.
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

                // Hide the spine animation (first child)
                if (merchantChar.GetChildCount() > 0)
                {
                    var spineChild = merchantChar.GetChild(0);
                    if (spineChild is Node2D spineNode)
                        spineNode.Visible = false;
                }

                // Add our animated Hermit
                var texture = GD.Load<Texture2D>("res://HermitMod/images/charui/hermit_merchant.png");
                if (texture != null)
                {
                    var animNode = new HermitMerchantAnim();
                    animNode.Texture = texture;
                    animNode.Scale = new Vector2(0.4f, 0.4f);
                    animNode.Position = new Vector2(0, -150);
                    merchantChar.AddChild(animNode);
                }
            }
        }
        catch
        {
            // Silently fail — worst case the Ironclad placeholder shows
        }
    }
}

/// <summary>
/// Animated sprite for the Hermit in the merchant scene.
/// Adds subtle idle breathing (vertical bob) and gentle sway.
/// </summary>
public partial class HermitMerchantAnim : Sprite2D
{
    private float _time;
    private Vector2 _basePosition;
    private float _baseRotation;
    private bool _initialized;

    public override void _Ready()
    {
        _basePosition = Position;
        _baseRotation = Rotation;
        _initialized = true;
    }

    public override void _Process(double delta)
    {
        if (!_initialized) return;

        _time += (float)delta;

        // Breathing bob — slow vertical oscillation
        float breathY = Mathf.Sin(_time * 1.8f) * 3.0f;

        // Gentle body sway — very slow subtle rotation
        float sway = Mathf.Sin(_time * 0.7f) * 0.008f;

        // Slight horizontal drift synced with sway
        float driftX = Mathf.Sin(_time * 0.7f) * 1.5f;

        Position = _basePosition + new Vector2(driftX, breathY);
        Rotation = _baseRotation + sway;
    }
}
