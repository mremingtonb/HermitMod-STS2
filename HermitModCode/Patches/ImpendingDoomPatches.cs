using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using HermitMod.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Patches;

/// <summary>
/// At end of player's turn, if any Impending Doom card is in the Dead On position
/// (middle of hand), deal 13 damage to ALL creatures (enemies and player).
/// Patches Hook.BeforeTurnEnd which receives CombatState and CombatSide.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeTurnEnd))]
public static class ImpendingDoomEndOfTurnPatch
{
    private const int DoomDamage = 13;

    [HarmonyPostfix]
    public static async Task Postfix(Task __result, CombatState combatState, CombatSide side)
    {
        // Wait for the original method to complete
        await __result;

        // Only trigger on player turn end
        if (side != CombatSide.Player) return;
        if (combatState == null) return;

        // Check each player's hand for Impending Doom in Dead On position
        foreach (var player in combatState.Players)
        {
            if (player?.Creature?.IsDead == true) continue;

            var handPile = PileType.Hand.GetPile(player);
            if (handPile == null) continue;

            var handCards = handPile.Cards;
            if (handCards == null || handCards.Count == 0) continue;

            int handSize = handCards.Count;

            for (int i = 0; i < handCards.Count; i++)
            {
                if (handCards[i] is ImpendingDoom && DeadOnHelper.IsMiddlePosition(i, handSize))
                {
                    // Impending Doom is Dead On — deal damage to EVERYONE
                    await CreatureCmd.TriggerAnim(player.Creature, "Cast", player.Character.CastAnimDelay);

                    // Deal damage to all creatures (enemies + player)
                    var allTargets = combatState.HittableEnemies.ToList();
                    allTargets.Add(player.Creature);

                    var netId = LocalContext.NetId;
                    if (!netId.HasValue) break;
                    var ctx = new HookPlayerChoiceContext(player, netId.Value,
                        MegaCrit.Sts2.Core.Entities.Multiplayer.GameActionType.Combat);

                    foreach (var target in allTargets)
                    {
                        if (target?.IsDead == true) continue;
                        await CreatureCmd.Damage(ctx, target, DoomDamage,
                            ValueProp.Move, player.Creature, handCards[i]);
                    }

                    break; // Only trigger once even if multiple Impending Dooms are Dead On
                }
            }
        }
    }
}
