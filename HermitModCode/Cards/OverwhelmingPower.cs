using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Apply Overwhelming Power to self. Exhaust.
/// </summary>
public sealed class OverwhelmingPower : HermitCard
{
    public OverwhelmingPower() : base(1, CardType.Power, CardRarity.Rare, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new CardsVar(2), new HpLossVar(3m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<OverwhelmingPowerPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
