using BaseLib.Abstracts;
using BaseLib.Utils;
using HermitMod.Character;
using HermitMod.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HermitMod.Cards;

/// <summary>
/// Apply 1 Vulnerable to ALL enemies. Retain.
/// </summary>
[Pool(typeof(HermitCardPool))]
public sealed class MementoCard : CustomCardModel
{
    public MementoCard() : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies) { }
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override string CustomPortraitPath => "memento_card.png".BigCardImagePath();
    public override string PortraitPath => "memento_card.png".CardImagePath();
    public override string BetaPortraitPath => "beta/memento_card.png".CardImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(1m)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars["VulnerablePower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() { }
}
