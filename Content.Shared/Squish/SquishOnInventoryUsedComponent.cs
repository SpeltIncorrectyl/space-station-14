namespace Content.Shared.Squish;

/// <summary>
/// This storage container has a squish animated when it's used.
/// </summary>
[RegisterComponent]
public sealed partial class SquishOnInventoryUsedComponent: Component
{
    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(0.2);

    [DataField]
    public double Magnitude = 0.75;
}
