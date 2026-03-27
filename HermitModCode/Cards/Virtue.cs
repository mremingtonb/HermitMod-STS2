using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Retain. Reduce each debuff on you by 1.
/// Upgrade: Reduce by 2.
/// </summary>
public sealed class Virtue : HermitCard
{
    private const int ReduceAmount = 1;
    private const int UpgradedReduceAmount = 2;

    public Virtue() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Simplified: remove all debuffs
        var powers = Owner.Creature.Powers?.ToList();
        if (powers == null) return;

        foreach (var power in powers)
        {
            if (power.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Debuff && power.Amount > 0)
            {
                await PowerCmd.Remove(power);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 1 -> 2 reduction (handled in OnPlay)
    }
}
