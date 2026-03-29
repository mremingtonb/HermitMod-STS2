using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Powers;

/// <summary>
/// Whenever a debuff is applied to you, gain X Strength.
/// Uses the AfterPowerAmountChanged virtual hook (no Harmony patch needed).
/// </summary>
public sealed class DeterminationPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // Only respond to debuffs applied to our owner
        if (power.Owner != Owner) return;

        // Only debuffs
        if (power.Type != PowerType.Debuff) return;

        // Only when the debuff amount increased (positive application)
        if (amount <= 0) return;

        // Grant Strength equal to this power's amount
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}
