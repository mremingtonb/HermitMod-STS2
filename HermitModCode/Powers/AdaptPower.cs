using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// At the start of your turn, you can Exhaust a card to gain 8 Block.
/// Implementation note: In STS2 this is handled via card selection prompts.
/// </summary>
public sealed class AdaptPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // The card selection and exhaust-for-block mechanic would need
    // a more complex implementation with player choice prompts.
    // For now, this serves as the power tracker.
}
