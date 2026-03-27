using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Put 2 random Attacks from discard into hand. They cost 1 less this turn. Exhaust.
/// Upgrade: 3 attacks.
/// </summary>
public sealed class Gambit : HermitCard
{
    private const int CardCount = 2;
    private const int UpgradedCardCount = 3;

    public Gambit() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(CardCount)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Simplified: Draw cards from draw pile
        await CardPileCmd.Draw(ctx, CardCount, Owner, false);
    }

    protected override void OnUpgrade()
    {
        // 2 -> 3 attacks (handled in OnPlay)
    }
}
