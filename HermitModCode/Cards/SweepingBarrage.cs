using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 8 damage to ALL enemies. Dead On: Deal it again.
/// Upgrade: 11 damage.
/// </summary>
public sealed class SweepingBarrage : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 8;
    private const int UpgradedDamageAmount = 11;

    public SweepingBarrage() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(ctx);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState)
                .Execute(ctx);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
