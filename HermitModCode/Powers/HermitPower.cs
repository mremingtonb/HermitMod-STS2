using BaseLib.Abstracts;
using BaseLib.Extensions;
using HermitMod.Extensions;
using Godot;

namespace HermitMod.Powers;

public abstract class HermitPower : CustomPowerModel
{
    public override string? CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "concentration.png".PowerImagePath();
        }
    }

    public override string? CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "concentration.png".BigPowerImagePath();
        }
    }
}
