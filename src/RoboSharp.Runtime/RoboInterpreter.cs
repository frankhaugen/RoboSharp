using RoboSharp.IL;
using RoboSharp.World;

namespace RoboSharp.Runtime;

/// <summary>Deterministic fake-IL interpreter; built-ins mutate <see cref="RobotWorld"/> and writers.</summary>
public sealed class RoboInterpreter
{
    public ExecutionResult Run(RoboProgram program, RobotWorld world, TextWriter stdout, TextWriter stderr)
    {
        var engine = new RoboInterpreterEngine();
        var init = engine.Initialize(program, world, stdout, stderr);
        if (init is not null)
            return init;

        while (true)
        {
            var r = engine.ExecuteNext(program);
            if (r is null)
                continue;
            return r;
        }
    }
}
