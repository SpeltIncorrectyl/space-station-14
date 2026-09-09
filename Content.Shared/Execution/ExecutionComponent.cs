using Robust.Shared.GameStates;

namespace Content.Shared.Execution;

/// <summary>
/// Added to entities that can be used to execute another target.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ExecutionComponent : Component
{
    /// <summary>
    /// How long will the execution do-after take?
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public TimeSpan ExecutionTime;

    /// <summary>
    /// What message is popup-ed when the execution do-after begins?
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public LocId BeforeExecutionMessage;

    /// <summary>
    /// What message is popup-ed when the execution successfully concludes and kills the victim?
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public LocId AfterExecutionMessage;

    /// <summary>
    /// What message is popup-ed when the execution do-after begins, and the user is executing themselves?
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public LocId BeforeSelfExecutionMessage;

    /// <summary>
    /// What message is popup-ed when the self-execution successfully concludes and kills the user?
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public LocId AfterSelfExecutionMessage;
}
