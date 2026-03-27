using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// Tracks bounty gold accumulated. Visual counter only.
/// Normal Enemy: 15 gold. Elite Enemy: 40 gold. Boss: 100 gold.
/// </summary>
public sealed class BountyPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
