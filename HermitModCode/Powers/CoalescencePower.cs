using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// At the end of your turn, Retain up to X cards.
/// Wears off at end of turn.
/// Uses the same pattern as WellLaidPlansPower from the base game.
/// </summary>
public sealed class CoalescencePower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private static bool RetainFilter(CardModel card) => !card.ShouldRetainThisTurn;

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner?.Player) return;
        if (!Hook.ShouldFlush(player.Creature.CombatState, player)) return;

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount),
            context: choiceContext,
            player: Owner.Player,
            filter: RetainFilter,
            source: this
        )).ToList();

        if (selected.Count == 0) return;

        foreach (var card in selected)
        {
            card.GiveSingleTurnRetain();
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Combat.CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
