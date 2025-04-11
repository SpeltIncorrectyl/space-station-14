using Robust.Shared.GameStates;

namespace Content.Shared.Squish;

/// <summary>
/// An entity with this component will do a quick squish animation.
/// The component is temporary and is removed after the squish.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SquishComponent: Component
{
    /// <summary>
    /// When did the squish start?
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan SquishStart;

    /// <summary>
    /// How long will the squish take?
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan SquishDuration;

    /// <summary>
    /// Magnitude of the squish, as a percentage.
    /// A 50% squish means that the item will be horizontally scaled by 2x and vertically scaled by 0.5x at the peak of the squish.
    /// </summary>
    [DataField, AutoNetworkedField]
    public double SquishMagnitude;
}
