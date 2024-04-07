using Robust.Shared.Serialization;

namespace Content.Shared.Flash;

[Serializable, NetSerializable]
public enum WallFlashVisualStates : byte
{
    Lights,
    Flashing,
    Bulb,
}
