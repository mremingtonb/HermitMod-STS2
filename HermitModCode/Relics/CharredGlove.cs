using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Relics;

/// <summary>
/// Whenever you draw a Curse, your next attack deals 3 more damage.
/// </summary>
public sealed class CharredGlove : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner?.Creature != Owner?.Creature) return;
        if (card.Type == CardType.Curse)
        {
            Flash();
            // TODO: Apply a temporary damage bonus to next attack
        }
    }
}
