using HermitMod.Cards;
using HermitMod.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Cards;

/// <summary>
/// Apply 3 Vulnerable. Dead On: Also apply 3 Weak.
/// Upgrade: 4 Vulnerable, 4 Weak.
/// </summary>
public sealed class MarkedTarget : HermitCard
{
    public override bool HasDeadOn => true;

    private const int VulnAmt = 3;
    private const int UpgradedVulnAmt = 4;
    private const int WeakAmt = 3;
    private const int UpgradedWeakAmt = 4;

    public MarkedTarget() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>((decimal)VulnAmt)];

    private int CurrentVuln => IsUpgraded ? UpgradedVulnAmt : VulnAmt;
    private int CurrentWeak => IsUpgraded ? UpgradedWeakAmt : WeakAmt;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<VulnerablePower>(play.Target, CurrentVuln, Owner.Creature, this);

        if (DeadOnHelper.IsDeadOn)
        {
            DeadOnHelper.IncrementDeadOnCount();
            await PowerCmd.Apply<WeakPower>(play.Target, CurrentWeak, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(UpgradedVulnAmt - VulnAmt);
    }
}
