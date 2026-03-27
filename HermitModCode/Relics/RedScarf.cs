using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you apply a new debuff to an enemy, gain 3 Block.
/// Requires Harmony patch on power application for full implementation.
/// </summary>
public sealed class RedScarf : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
}
