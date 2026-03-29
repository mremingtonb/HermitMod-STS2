using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 14 damage. Ignores Block.
/// Upgrade: 20 damage.
/// </summary>
public sealed class PiercingRound : HermitCard
{
    private const int DamageAmount = 14;
    private const int UpgradedDamageAmount = 20;

    public PiercingRound() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move | ValueProp.Unblockable)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        // Use CreatureCmd.Damage with Unblockable flag since DamageCmd.Attack doesn't support it
        await CreatureCmd.Damage(ctx, play.Target, DynamicVars.Damage.BaseValue,
            ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
