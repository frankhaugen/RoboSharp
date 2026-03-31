namespace RoboSharp.World;

/// <summary>After a run, evaluates a simple primary-goal rule and a kid-friendly score.</summary>
public static class LessonGoalEvaluator
{
    public readonly record struct Result(bool ReachedGoal, int Score, string SummaryForKids);

    /// <summary>
    /// If <see cref="WorldMetadata.PrimaryGoalPosition"/> is null, treats as free play (participation score only).
    /// Otherwise checks whether the primary robot stands on the goal tile.
    /// </summary>
    public static Result Evaluate(RobotWorld world, int ilInstructionSteps)
    {
        var meta = world.Metadata;
        var actorId = meta.PrimaryActorId ?? 1;
        if (!world.ActorsById.TryGetValue(actorId, out var actor))
            return new Result(false, 0, "No robot found — something went wrong with the world.");

        var goal = meta.PrimaryGoalPosition;
        if (goal is null)
        {
            var participation = Math.Clamp(ilInstructionSteps, 0, 1) * 10;
            return new Result(
                true,
                participation,
                "Free play map — no goal tile. Experiment with move() and turns! (Score is just for fun here.)");
        }

        var reached = actor.Position.X == goal.Value.X && actor.Position.Y == goal.Value.Y;
        var score = ComputeScore(reached, ilInstructionSteps);
        string summary;
        if (reached)
        {
            summary =
                $"Nice work — you reached the goal! Score: {score}. " +
                "(Higher score when you use fewer program steps.)";
        }
        else
        {
            summary =
                $"Keep going! Robot at ({actor.Position.X},{actor.Position.Y}), goal at ({goal.Value.X},{goal.Value.Y}). " +
                "Score: 0 until you land on the goal.";
        }

        return new Result(reached, score, summary);
    }

    private static int ComputeScore(bool reachedGoal, int steps)
    {
        if (!reachedGoal)
            return 0;

        var baseScore = 100;
        var penalty = Math.Min(40, steps / 8);
        return Math.Max(60, baseScore - penalty);
    }
}
