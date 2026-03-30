using HermitMod.Cards;
using HermitMod.Character;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

public sealed class Gestalt : HermitCard
{
    private const int RuggedAmt = 2;
    private const int UpgradedRuggedAmt = 3;
    private const int VulnAmt = 1;
    private const int UpgradedVulnAmt = 2;

    public Gestalt() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RuggedPower>((decimal)RuggedAmt),
        new PowerVar<VulnerablePower>((decimal)VulnAmt)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<CardKeyword> CustomKeywords => [HermitKeywords.Rugged];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    private int CurrentRugged => IsUpgraded ? UpgradedRuggedAmt : RuggedAmt;
    private int CurrentVuln => IsUpgraded ? UpgradedVulnAmt : VulnAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RuggedPower>(Owner.Creature, CurrentRugged, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(Owner.Creature, CurrentVuln, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RuggedPower"].UpgradeValueBy(UpgradedRuggedAmt - RuggedAmt);
        DynamicVars["VulnerablePower"].UpgradeValueBy(UpgradedVulnAmt - VulnAmt);
    }
}
