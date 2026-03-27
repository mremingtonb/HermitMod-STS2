using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you defeat an Elite encounter, heal 7 HP and gain 35 gold.
/// </summary>
public sealed class BrokenTooth : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    // Elite victory hooks would need to be implemented via Harmony patches
    // on the combat victory/reward system
}
