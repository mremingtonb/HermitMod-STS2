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
/// Deal 7 damage. If last card triggered Dead On, draw a card.
/// Upgrade: Retain.
/// </summary>
public sealed class CalledShot : HermitCard
{
    public override bool HasDeadOn => true;

    private const int DamageAmount = 7;
    private const int DrawAmount = 1;

    public CalledShot() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar((decimal)DamageAmount, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(ctx);

        // If the previous card triggered Dead On (counter > 0 means at least one Dead On happened)
        if (DeadOnHelper.DeadOnTriggersThisTurn > 0)
        {
            await CardPileCmd.Draw(ctx, DrawAmount, Owner, false);
        }
    }

    protected override void OnUpgrade()
    {
        // Gain Retain keyword (handled by CanonicalKeywords)
    }
}
