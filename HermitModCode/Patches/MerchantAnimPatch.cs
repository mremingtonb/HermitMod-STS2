using Godot;
using HarmonyLib;
using HermitMod.Character;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace HermitMod.Patches;

/// <summary>
/// Adds a subtle breathing animation to the Hermit's merchant scene character.
/// The contributor's Spine skeleton has empty idle_loop/relaxed_loop animations,
/// so we supplement with a Godot tween-based bob after _Ready runs.
/// </summary>
[HarmonyPatch(typeof(NMerchantCharacter), "_Ready")]
public static class MerchantAnimPatch
{
    [HarmonyPostfix]
    public static void Postfix(NMerchantCharacter __instance)
    {
        try
        {
            // Only apply to the Hermit's merchant scene
            var scenePath = __instance.SceneFilePath;
            if (scenePath == null || !scenePath.Contains("hermit"))
                return;

            // Get the SpineSprite (first child)
            var sprite = __instance.GetChildCount() > 0 ? __instance.GetChild(0) : null;
            if (sprite == null) return;

            // Create a looping breathing tween on the sprite
            var tween = __instance.CreateTween();
            tween.SetLoops(); // infinite loop

            var basePos = ((Node2D)sprite).Position;

            // Gentle bob up and down
            tween.TweenProperty(sprite, "position",
                new Vector2(basePos.X, basePos.Y - 3f), 1.5)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

            tween.TweenProperty(sprite, "position",
                basePos, 1.5)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);
        }
        catch
        {
            // Don't crash the merchant scene if animation fails
        }
    }
}
