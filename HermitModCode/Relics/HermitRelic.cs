using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HermitMod.Character;
using HermitMod.Extensions;
using Godot;

namespace HermitMod.Relics;

[Pool(typeof(HermitRelicPool))]
public abstract class HermitRelic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "oldlocket.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "oldlocket_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "oldlocket.png".BigRelicImagePath();
        }
    }
}
