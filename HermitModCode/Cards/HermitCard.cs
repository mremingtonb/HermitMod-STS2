using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HermitMod.Character;
using HermitMod.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace HermitMod.Cards;

[Pool(typeof(HermitCardPool))]
public abstract class HermitCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    /// <summary>
    /// Whether this card has a Dead On effect. Override to true in cards with Dead On mechanics.
    /// Used to show gold glow when the card is in the middle of the hand.
    /// </summary>
    public virtual bool HasDeadOn => false;

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
