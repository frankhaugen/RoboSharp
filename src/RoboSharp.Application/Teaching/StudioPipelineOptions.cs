using RoboSharp.Semantics;
using RoboSharp.World;

namespace RoboSharp.Application.Teaching;

/// <summary>Lesson profile + world factory for teaching pipeline build/run.</summary>
public sealed record StudioPipelineOptions(
    IBuiltinProfileProvider BuiltinProfile,
    Func<RobotWorld> CreateRunWorld,
    string ProfileLabel,
    string WorldPresetLabel);
