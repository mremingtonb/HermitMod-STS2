using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Powers;

public sealed class HighNoonPower : HermitPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;

        var cardName = cardPlay.Card.GetType().Name;
        bool isStrike = cardName.Contains("Strike");
        bool isDefend = cardName.Contains("Defend");

        if (isStrike || isDefend)
        {
            Flash();
            await CardPileCmd.Draw(context, Amount, Owner.Player!, false);
        }
    }
}
