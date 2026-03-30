namespace RoboSharp.Runtime;

public enum InterpreterStepKind
{
    /// <summary>One instruction executed; execution may continue.</summary>
    Advanced,

    /// <summary>Call stack empty after normal completion.</summary>
    Completed,

    /// <summary>Structured fault (stack, bounds, etc.).</summary>
    Faulted,

    /// <summary><see cref="RoboInterpreterSession.RunToEnd"/> stopped due to step budget.</summary>
    StepLimitExceeded,
}
