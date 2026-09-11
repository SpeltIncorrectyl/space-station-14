using Robust.Shared.GameStates;

namespace Content.Shared.Execution;

/// <summary>
/// Forbid a weapon or projectile from being guaranteed to kill.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ForbidExecutionKillComponent : Component;