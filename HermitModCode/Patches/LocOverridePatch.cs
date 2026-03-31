using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace HermitMod.Patches;

/// <summary>
/// Overrides stale localization entries from the PCK with up-to-date values.
/// The contributor's PCK contains scene files we need, but its localization JSONs
/// are outdated. This patch merges corrected entries after LocManager loads.
/// </summary>
[HarmonyPatch(typeof(LocManager), "Initialize")]
public static class LocOverridePatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            var instance = LocManager.Instance;
            if (instance == null) return;

            // Access the _tables field via reflection
            var tablesField = AccessTools.Field(typeof(LocManager), "_tables");
            if (tablesField == null) return;

            var tables = tablesField.GetValue(instance) as Dictionary<string, LocTable>;
            if (tables == null) return;

            // Override card descriptions
            if (tables.TryGetValue("cards", out var cardsTable))
            {
                cardsTable.MergeWith(GetCardOverrides());
            }

            // Override power descriptions
            if (tables.TryGetValue("powers", out var powersTable))
            {
                powersTable.MergeWith(GetPowerOverrides());
            }

            // Override/add ancient dialogues
            if (tables.TryGetValue("ancients", out var ancientsTable))
            {
                ancientsTable.MergeWith(GetAncientOverrides());
            }

            MainFile.Logger.Info("Localization overrides applied successfully.");
        }
        catch (Exception ex)
        {
            MainFile.Logger.Info($"LocOverridePatch error: {ex.Message}");
        }
    }

    private static Dictionary<string, string> GetCardOverrides()
    {
        return new Dictionary<string, string>
        {
            // Coalescence: remove broken {CoalescencePower:diff()} reference
            ["HERMITMOD-COALESCENCE.description"] =
                "Gain {Block:diff()} Block.\n[gold]Retain[/gold] up to 2 cards this turn.",
            ["HERMITMOD-COALESCENCE.upgradeDescription"] =
                "Gain {Block:diff()} Block.\n[gold]Retain[/gold] up to 3 cards this turn.",

            // Virtue: use DynamicVar for upgrade diff display
            ["HERMITMOD-VIRTUE.description"] =
                "Reduce each debuff on you by {Reduce:diff()}.",

            // Final Canter: updated Curse scaling description
            ["HERMITMOD-FINAL_CANTER.description"] =
                "Deal {Damage:diff()} damage. Deals 3 more for each [gold]Curse[/gold] in all piles.\n[gold]Retain[/gold].",
            ["HERMITMOD-FINAL_CANTER.upgradeDescription"] =
                "Deal {Damage:diff()} damage. Deals 4 more for each [gold]Curse[/gold] in all piles.\n[gold]Retain[/gold].",

            // Dead Or Alive: updated gold amounts
            ["HERMITMOD-DEAD_OR_ALIVE.description"] =
                "Deal {Damage:diff()} damage. If Fatal, gain 15 Gold.",
            ["HERMITMOD-DEAD_OR_ALIVE.upgradeDescription"] =
                "Deal {Damage:diff()} damage. If Fatal, gain 25 Gold.",

            // Flash Powder: corrected Str loss
            ["HERMITMOD-FLASH_POWDER.description"] =
                "Gain {Block:diff()} Block. ALL enemies lose 2 [gold]Strength[/gold].",

            // Cheat: draw-then-choose
            ["HERMITMOD-CHEAT.description"] =
                "Draw the top {Cards:diff()} cards. Choose one to play.",

            // Malice: exhaust + conditional AoE
            ["HERMITMOD-MALICE.description"] =
                "[gold]Exhaust[/gold] a card.\nDeal {Damage:diff()} damage.\nIf you Exhaust a Curse, deal {Damage:diff()} damage to ALL enemies instead.",

            // Glare: uses DynamicVar for upgrade diff
            ["HERMITMOD-GLARE.description"] =
                "Apply {WeakPower:diff()} [gold]Weak[/gold] and [gold]Vulnerable[/gold].",

            // Vantage: use DynamicVar for draw/upgrade count
            ["HERMITMOD-VANTAGE.description"] =
                "Gain {Block:diff()} Block.\n[gold]Dead On[/gold]: Draw and Upgrade {DrawUpgrade:diff()} card(s).",

            // Virtue: use DynamicVar for reduce amount
            ["HERMITMOD-VIRTUE.description"] =
                "Reduce each debuff on you by {Reduce:diff()}.",
        };
    }

    private static Dictionary<string, string> GetPowerOverrides()
    {
        return new Dictionary<string, string>
        {
            // Remove stale CoalescencePower references if they cause issues
            ["HERMITMOD-COALESCENCE_POWER.description"] =
                "At the end of your turn, [gold]Retain[/gold] up to {Amount} card(s).",
        };
    }

    private static Dictionary<string, string> GetAncientOverrides()
    {
        return new Dictionary<string, string>
        {
            // The Architect (existing)
            ["THE_ARCHITECT.talk.HERMITMOD-HERMIT.0-0r.char"] = "Eat my dusty ass!",
            ["THE_ARCHITECT.talk.HERMITMOD-HERMIT.0-0r.next"] = "...",
            ["THE_ARCHITECT.talk.HERMITMOD-HERMIT.0-1r.ancient"] = "How rude!",
            ["THE_ARCHITECT.talk.HERMITMOD-HERMIT.0-attack"] = "...",

            // Darv
            ["DARV.talk.HERMITMOD-HERMIT.0-0.ancient"] = "Hey there, stranger! You look like you've seen better days. Lucky for you, Darv's got just the thing.",
            ["DARV.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["DARV.talk.HERMITMOD-HERMIT.1-0.ancient"] = "Back again, eh? The hat gives you away every time.",
            ["DARV.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["DARV.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "Darv always has something for a lone wanderer.",
            ["DARV.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",

            // Orobas
            ["OROBAS.talk.HERMITMOD-HERMIT.0-0.ancient"] = "A solitary soul... how delightful. Tell me, what drives one to wander alone?",
            ["OROBAS.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["OROBAS.talk.HERMITMOD-HERMIT.1-0.ancient"] = "You return. The silence around you speaks louder than words.",
            ["OROBAS.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["OROBAS.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "The lone gunslinger. I wonder what you seek this time.",
            ["OROBAS.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",

            // Pael
            ["PAEL.talk.HERMITMOD-HERMIT.0-0.ancient"] = "You carry an old weight on your shoulders. Perhaps I can lighten it.",
            ["PAEL.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["PAEL.talk.HERMITMOD-HERMIT.1-0.ancient"] = "The wanderer returns. Your path is a winding one.",
            ["PAEL.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["PAEL.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "Welcome back, Hermit. Shall we trade once more?",
            ["PAEL.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",

            // Tanx
            ["TANX.talk.HERMITMOD-HERMIT.0-0.ancient"] = "Hah! A loner with a gun. You remind me of someone I used to know.",
            ["TANX.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["TANX.talk.HERMITMOD-HERMIT.1-0.ancient"] = "Still alive? Impressive. Most don't make it back.",
            ["TANX.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["TANX.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "You again. Let's make this quick.",
            ["TANX.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",

            // Tezcatara
            ["TEZCATARA.talk.HERMITMOD-HERMIT.0-0.ancient"] = "The dust of the road clings to you. Come, rest a moment.",
            ["TEZCATARA.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["TEZCATARA.talk.HERMITMOD-HERMIT.1-0.ancient"] = "Your footsteps are becoming familiar to me.",
            ["TEZCATARA.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["TEZCATARA.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "The Hermit walks this path again. Fate is persistent.",
            ["TEZCATARA.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",

            // Vakuu
            ["VAKUU.talk.HERMITMOD-HERMIT.0-0.ancient"] = "Interesting... you carry the scent of gunpowder and solitude.",
            ["VAKUU.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["VAKUU.talk.HERMITMOD-HERMIT.1-0.ancient"] = "The lone wanderer. Your resolve is admirable.",
            ["VAKUU.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["VAKUU.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "Back once more. The spire draws us all in circles.",
            ["VAKUU.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",

            // Nonupeipe
            ["NONUPEIPE.talk.HERMITMOD-HERMIT.0-0.ancient"] = "A traveler! And one who prefers their own company, I see.",
            ["NONUPEIPE.talk.HERMITMOD-HERMIT.0-0.next"] = "...",
            ["NONUPEIPE.talk.HERMITMOD-HERMIT.1-0.ancient"] = "Oh, it's you again! The quiet one with the hat.",
            ["NONUPEIPE.talk.HERMITMOD-HERMIT.1-0.next"] = "...",
            ["NONUPEIPE.talk.HERMITMOD-HERMIT.2-0r.ancient"] = "Welcome, welcome! Nonupeipe remembers a friend.",
            ["NONUPEIPE.talk.HERMITMOD-HERMIT.2-0r.next"] = "...",
        };
    }
}
