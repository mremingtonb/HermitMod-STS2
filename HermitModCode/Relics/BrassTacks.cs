using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace HermitMod.Relics;

/// <summary>
/// At the end of your turn, gain 2 Block.
/// </summary>
public sealed class BrassTacks : HermitRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Creature.Side || Owner?.Creature == null) return;

        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 2, ValueProp.Unpowered, null, false);
    }
}
