using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you remove or Transform a card from your deck, heal 15 HP.
/// </summary>
public sealed class StraightRazor : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // Card removal/transform hooks happen outside combat
    // Would need Harmony patches on the card removal screen
}
