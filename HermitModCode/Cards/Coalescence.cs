using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Gain 7 Block. Retain up to 2 cards this turn.
/// Upgrade: 10 Block, Retain 3.
/// </summary>
public sealed class Coalescence : HermitCard
{
    private const int BlockAmount = 7;
    private const int UpgradedBlockAmount = 10;
    private const int RetainCount = 2;
    private const int UpgradedRetainCount = 3;

    public Coalescence() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar((decimal)BlockAmount, ValueProp.Move),
        new PowerVar<CoalescencePower>((decimal)RetainCount)
    ];

    private int CurrentRetainCount => IsUpgraded ? UpgradedRetainCount : RetainCount;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        await PowerCmd.Apply<CoalescencePower>(Owner.Creature, CurrentRetainCount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradedBlockAmount - BlockAmount);
        DynamicVars["CoalescencePower"].UpgradeValueBy(UpgradedRetainCount - RetainCount);
    }
}
