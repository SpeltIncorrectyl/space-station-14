namespace Content.Server.Squish;

[RegisterComponent]
public sealed class SquishOnInventoryUsedComponent: Component
{
    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(0.2);

    [DataField]
    public double Magnitude = 0.75;
}
