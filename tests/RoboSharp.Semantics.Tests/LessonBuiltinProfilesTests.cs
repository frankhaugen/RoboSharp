using RoboSharp.Semantics;

namespace RoboSharp.Semantics.Tests;

public class LessonBuiltinProfilesTests
{
    [Test]
    public async Task BasicMovement_Excludes_Print()
    {
        var p = LessonBuiltinProfiles.GetProvider(LessonBuiltinProfiles.BasicMovementId);
        await Assert.That(p.IsAvailable(BuiltinId.Move)).IsTrue();
        await Assert.That(p.IsAvailable(BuiltinId.Print)).IsFalse();
    }

    [Test]
    public async Task Full_Includes_All_Builtins()
    {
        var p = LessonBuiltinProfiles.GetProvider(LessonBuiltinProfiles.FullId);
        await Assert.That(p.IsAvailable(BuiltinId.TakeLast)).IsTrue();
    }
}
