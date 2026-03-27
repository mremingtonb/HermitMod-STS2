using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// Rugged: Reduces next instance(s) of unblocked attack damage to 2.
/// Each stack = one instance of damage reduction.
/// Damage modification is handled via Harmony patches.
/// </summary>
public sealed class RuggedPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
