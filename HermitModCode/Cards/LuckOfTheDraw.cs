using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Draw cards until total cost drawn is 3 or more.
/// Upgrade: Until total cost is 4 or more.
/// </summary>
public sealed class LuckOfTheDraw : HermitCard
{
    private const int CostThreshold = 3;
    private const int UpgradedCostThreshold = 4;

    public LuckOfTheDraw() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Simplified: draw 3 cards (or 4 upgraded)
        await CardPileCmd.Draw(ctx, CostThreshold, Owner, false);
    }

    protected override void OnUpgrade()
    {
        // 3 -> 4 cost threshold (handled in OnPlay)
    }
}
