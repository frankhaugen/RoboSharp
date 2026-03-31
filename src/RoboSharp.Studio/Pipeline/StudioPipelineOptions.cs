using RoboSharp.Semantics;
using RoboSharp.World;

namespace RoboSharp.Studio.Pipeline;

/// <summary>Lesson profile + world factory passed through Build/Run so Studio matches kid-friendly presets.</summary>
public sealed record StudioPipelineOptions(
    IBuiltinProfileProvider BuiltinProfile,
    Func<RobotWorld> CreateRunWorld,
    string ProfileLabel,
    string WorldPresetLabel);
