using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Powers;

/// <summary>
/// Tracks temporary Strength from Scope In. Removes the Strength at end of turn.
/// </summary>
public sealed class ScopeInStrengthPower : HermitPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            Flash();
            int strToRemove = Amount;
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<StrengthPower>(Owner, -strToRemove, Owner, null);
        }
    }
}
