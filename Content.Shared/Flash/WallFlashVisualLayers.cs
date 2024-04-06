using Robust.Shared.Serialization;

namespace Content.Shared.Flash;

[Serializable, NetSerializable]
public enum WallFlashVisualLayers : byte
{
    BaseLayer,
    LightLayer,
    BulbLayer,
    FlashLayer
}
