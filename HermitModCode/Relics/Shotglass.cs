using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// First 2 times you use a potion each combat, gain a random potion.
/// You can only use 2 potions each combat.
/// </summary>
public sealed class Shotglass : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // Complex potion interaction - would need hooks into potion usage system
    // For now, serves as relic definition
}
