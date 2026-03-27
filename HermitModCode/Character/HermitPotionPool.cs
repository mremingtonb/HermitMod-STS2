using BaseLib.Abstracts;
using HermitMod.Extensions;
using Godot;

namespace HermitMod.Character;

public class HermitPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Hermit.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
