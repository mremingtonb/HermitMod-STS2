using HermitMod.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// At the start of each turn, gain 1 energy.
/// Uses AfterEnergyReset so it fires AFTER energy is refueled,
/// preventing the bonus energy from being overwritten.
/// </summary>
public sealed class DentedPlate : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner) return;
        Flash();
        await PlayerCmd.GainEnergy(1m, Owner);
    }
}
