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
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
