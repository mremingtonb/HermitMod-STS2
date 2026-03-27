using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Prevent your first lethal HP loss each combat.
/// </summary>
public sealed class PetGhost : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    // Lethal prevention requires deep hooks into the damage/death system.
    // This serves as the relic definition; full implementation would need
    // Harmony patches on the death check.
}
