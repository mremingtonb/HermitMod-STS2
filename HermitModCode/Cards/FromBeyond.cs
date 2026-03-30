using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// For each card in your exhaust pile, a random enemy loses 10 HP. Exhaust.
/// Upgrade: 15 HP per card.
/// </summary>
public sealed class FromBeyond : HermitCard
{
    private const int DamageAmount = 10;
    private const int UpgradedDamageAmount = 15;

    public FromBeyond() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);

        // Count cards in exhaust pile
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        int exhaustCount = exhaustPile?.Cards.Count ?? 0;

        // For each exhausted card, a random enemy loses HP
        var rng = new System.Random();
        for (int i = 0; i < exhaustCount; i++)
        {
            var enemies = CombatState.HittableEnemies;
            if (enemies.Count == 0) break;

            var randomIndex = rng.Next(enemies.Count);
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
