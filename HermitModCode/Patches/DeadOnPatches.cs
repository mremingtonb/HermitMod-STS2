using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using HermitMod.Powers;

namespace HermitMod.Patches;

/// <summary>
/// Tracks Dead On state for the current card play.
/// Dead On triggers when a card is played from the middle position of the hand.
/// For odd hand sizes: the exact middle position.
/// For even hand sizes: either of the two center positions.
///
/// Patches Hook.BeforeCardPlayed to capture the card's position before it leaves the hand,
/// and Hook.AfterCardPlayed to reset the flag after the card finishes playing.
/// Also patches Hook.BeforeSideTurnStart to reset the per-turn trigger counter.
/// </summary>
public static class DeadOnHelper
{
    private static bool _currentCardIsDeadOn;
    private static int _deadOnTriggersThisTurn;

    public static bool IsDeadOn => _currentCardIsDeadOn;
    public static int DeadOnTriggersThisTurn => _deadOnTriggersThisTurn;

    public static void IncrementDeadOnCount()
    {
        _deadOnTriggersThisTurn++;
    }

    public static void ResetTurnCount()
    {
        _deadOnTriggersThisTurn = 0;
    }

    /// <summary>
    /// Determines if a card at the given index is in the middle of a hand of the given size.
    /// For odd hand sizes: exact middle position.
    /// For even hand sizes: either of the two middle positions.
    /// </summary>
    public static bool IsMiddlePosition(int cardIndex, int handSize)
    {
        if (handSize <= 0) return false;
        if (handSize == 1) return true;

        int middle = handSize / 2;
        if (handSize % 2 == 1)
        {
            return cardIndex == middle;
        }
        else
        {
            return cardIndex == middle - 1 || cardIndex == middle;
        }
    }

    internal static void SetDeadOn(bool value)
    {
        _currentCardIsDeadOn = value;
    }
}

/// <summary>
/// Prefix patch on Hook.BeforeCardPlayed to capture the card's hand position
/// before it is removed from the hand. Sets DeadOnHelper flag accordingly.
/// Also checks for ConcentrationPower — if present, Dead On auto-triggers.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCardPlayed))]
public static class DeadOnBeforeCardPlayedPatch
{
    [HarmonyPrefix]
    public static void Prefix(CardPlay cardPlay)
    {
        // Default to false
        DeadOnHelper.SetDeadOn(false);

        if (cardPlay?.Card?.Owner == null) return;

        var owner = cardPlay.Card.Owner;

        // Check if the owner has ConcentrationPower — if so, Dead On always triggers
        var concentrationPower = owner.Creature?.Powers?
            .OfType<ConcentrationPower>()
            .FirstOrDefault();

        if (concentrationPower != null && concentrationPower.Amount > 0)
        {
            DeadOnHelper.SetDeadOn(true);
            return;
        }

        // Get the hand pile and find the card's index before it is removed
        var handCards = PileType.Hand.GetPile(owner).Cards;
        if (handCards == null) return;

        var cardList = handCards.ToList();
        int handSize = cardList.Count;
        int cardIndex = cardList.IndexOf(cardPlay.Card);

        if (cardIndex < 0) return;

        bool isMiddle = DeadOnHelper.IsMiddlePosition(cardIndex, handSize);
        DeadOnHelper.SetDeadOn(isMiddle);
    }
}

/// <summary>
/// Postfix patch on Hook.AfterCardPlayed to reset the Dead On flag
/// after the card play has fully resolved.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
public static class DeadOnAfterCardPlayedPatch
{
    [HarmonyPostfix]
    public static void Postfix(CombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DeadOnHelper.SetDeadOn(false);
    }
}

/// <summary>
/// Patch on Hook.BeforeSideTurnStart to reset the Dead On trigger counter
/// at the start of each turn.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeSideTurnStart))]
public static class DeadOnTurnResetPatch
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        DeadOnHelper.ResetTurnCount();
    }
}
