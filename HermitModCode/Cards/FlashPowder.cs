using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Gain 6 Block. ALL enemies lose 2 Strength. Exhaust.
/// Upgrade: 9 Block, lose 3 Strength.
/// </summary>
public sealed class FlashPowder : HermitCard
{
    private const int BlockAmount = 6;
    private const int UpgradedBlockAmount = 9;
    private const int StrengthLoss = 2;
    private const int UpgradedStrengthLoss = 3;

    public FlashPowder() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)BlockAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private int CurrentStrengthLoss => IsUpgraded ? UpgradedStrengthLoss : StrengthLoss;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(enemy, -CurrentStrengthLoss, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
    }
}
