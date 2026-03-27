using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you lose HP during enemy turn, draw 1 card.
/// Requires Harmony patch on damage system for full implementation.
/// </summary>
public sealed class RyeStalk : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
}
