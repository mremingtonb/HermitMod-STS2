using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you gain Weak, Frail or Vulnerable, gain 1 less.
/// Requires Harmony patch on power application for full implementation.
/// </summary>
public sealed class Horseshoe : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
}
