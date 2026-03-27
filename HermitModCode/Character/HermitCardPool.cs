using BaseLib.Abstracts;
using HermitMod.Extensions;
using Godot;

namespace HermitMod.Character;

public class HermitCardPool : CustomCardPoolModel
{
    public override string Title => Hermit.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Dark purple card back
    public override float H => 0.75f;
    public override float S => 0.6f;
    public override float V => 0.5f;

    public override Color DeckEntryCardColor => new("6B3A6B");

    public override bool IsColorless => false;
}
