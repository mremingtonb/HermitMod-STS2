using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

/// <summary>
/// Your next Dead On effect this turn triggers twice. Exhaust.
/// Applies SnipePower buff — no damage dealt.
/// </summary>
public sealed class Snipe : HermitCard
{
    private const int SnipeAmount = 1;
    private const int UpgradedSnipeAmount = 2;

    public Snipe() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SnipePower>((decimal)SnipeAmount)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<SnipePower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        int amount = IsUpgraded ? UpgradedSnipeAmount : SnipeAmount;
        await PowerCmd.Apply<SnipePower>(Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SnipePower"].UpgradeValueBy(UpgradedSnipeAmount - SnipeAmount);
    }
}
