using HermitMod.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace HermitMod.Cards;

public class Defend_Hermit() : HermitCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    private const int Block = 5;
    private const int UpgradeBlock = 3;

    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar((decimal)Block, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UpgradeBlock);
    }
}
