using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Gain 1 Dexterity. This costs 1 more this combat.
/// Upgrade: Gain 2 Dexterity.
/// </summary>
public sealed class Maintenance : HermitCard
{
    private const int DexAmount = 1;
    private const int UpgradedDexAmount = 2;

    public Maintenance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>((decimal)DexAmount)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int amount = IsUpgraded ? UpgradedDexAmount : DexAmount;
        await PowerCmd.Apply<DexterityPower>(Owner.Creature, amount, Owner.Creature, this);

        // This card costs 1 more each time it's played this combat
        EnergyCost.UpgradeBy(1);
        EnergyCost.FinalizeUpgrade();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DexterityPower"].UpgradeValueBy(UpgradedDexAmount - DexAmount);
    }
}
