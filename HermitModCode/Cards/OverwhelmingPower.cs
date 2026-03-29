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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3), new CardsVar(2), new HpLossVar(3m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Gain energy
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);

        // Draw cards
        await CardPileCmd.Draw(ctx, (int)DynamicVars.Cards.BaseValue, Owner, false);

        // Apply the debuff: lose HP when ending turn with 0 energy
        await PowerCmd.Apply<OverwhelmingPowerPower>(Owner.Creature, (int)DynamicVars["HpLoss"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
