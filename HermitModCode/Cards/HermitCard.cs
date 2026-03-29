using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HermitMod.Character;
using HermitMod.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

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

    /// <summary>
    /// Custom keyword tooltips for this card. Override to add tooltips for Bruise, Rugged, etc.
    /// Cards with HasDeadOn automatically get the Dead On tooltip.
    /// </summary>
    protected virtual IEnumerable<CardKeyword> CustomKeywords => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (HasDeadOn)
                yield return HoverTipFactory.FromKeyword(HermitKeywords.DeadOn);
            foreach (var kw in CustomKeywords)
                yield return HoverTipFactory.FromKeyword(kw);
        }
    }

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
