using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Play one of the top 3 cards in your draw pile. Dead On: Also trigger its Dead On effect.
/// Upgrade: Top 5 cards.
/// </summary>
public sealed class Cheat : HermitCard
{
    public override bool HasDeadOn => true;

    private const int CardCount = 3;
    private const int UpgradedCardCount = 5;

    public Cheat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(CardCount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Simplified: Draw 1 card (full implementation would let player choose from top N)
        await CardPileCmd.Draw(ctx, 1, Owner, false);
    }

    protected override void OnUpgrade() { }
}
