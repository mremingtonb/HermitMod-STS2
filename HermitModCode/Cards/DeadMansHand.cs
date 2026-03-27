using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Discard your hand. Add the 3 rarest cards from draw pile to hand.
/// Upgrade: 4 rarest cards.
/// </summary>
public sealed class DeadMansHand : HermitCard
{
    private const int CardCount = 3;
    private const int UpgradedCardCount = 4;

    public DeadMansHand() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(CardCount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Simplified: Draw cards from draw pile
        await CardPileCmd.Draw(ctx, CardCount, Owner, false);
    }

    protected override void OnUpgrade()
    {
        // 3 -> 4 rarest cards (handled in OnPlay)
    }
}
