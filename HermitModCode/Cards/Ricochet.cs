using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Deal 3 damage. Repeat on a random enemy for each Dead On effect this turn.
/// Upgrade: 5 damage.
/// </summary>
public sealed class Ricochet : HermitCard
{
    private const int DamageAmount = 3;
    private const int UpgradedDamageAmount = 5;

    public Ricochet() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        // Always hit once
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(ctx);

        // Repeat for each Dead On effect triggered this turn
        int repeats = DeadOnHelper.DeadOnTriggersThisTurn;
        for (int i = 0; i < repeats; i++)
        {
            var enemies = CombatState.HittableEnemies;
            if (enemies.Count == 0) break;

            // Pick a random enemy
            var randomIndex = new System.Random().Next(enemies.Count);
            var target = enemies[randomIndex];

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .Execute(ctx);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UpgradedDamageAmount - DamageAmount);
    }
}
