using HermitMod.Cards;
using HermitMod.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

public class Feint() : HermitCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int Blk = 6;
    private const int UpgradeBlk = 3;
    private const int BruiseAmt = 2;
    private const int UpgradeBruise = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)Blk, ValueProp.Move), new PowerVar<BruisePower>((decimal)BruiseAmt)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        // Apply Bruise to ALL enemies
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<BruisePower>(enemy, DynamicVars.Cards.BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradeBlk);
        DynamicVars.Cards.UpgradeValueBy(UpgradeBruise);
    }
}
