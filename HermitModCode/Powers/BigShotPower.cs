using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

/// <summary>
/// Smoking Barrel: Whenever you trigger a Dead On effect, your next attack deals X more damage.
/// Damage modification is handled via Harmony patches.
/// </summary>
public sealed class BigShotPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _bonusDamage;

    public int BonusDamage => _bonusDamage;

    public void AddBonus()
    {
        _bonusDamage += Amount;
        Flash();
    }

    public int ConsumeBonus()
    {
        int bonus = _bonusDamage;
        _bonusDamage = 0;
        return bonus;
    }
}
