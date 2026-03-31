namespace RoboSharp.Locales;

/// <summary>Starter <c>.robo</c> bodies for <see cref="IStudioLessonCatalog"/> (locale-neutral syntax).</summary>
public static class StudioLessonSharedExamples
{
    public const string FirstMoves =
        """
        // Lesson: first moves — smallest program that does something.
        move();
        turnLeft();
        move();

        """;

    public const string Steering =
        """
        // Lesson: steering — face each direction then step forward.
        move();
        turnRight();
        move();
        turnRight();
        move();

        """;

    public const string LoopsAndPrint =
        """
        // Lesson: loops and print
        integer i = 0;
        while (i < 4)
        {
            print(i);
            move();
            i = i + 1;
        }

        """;

    public const string Sensing =
        """
        // Lesson: sensing — go forward while the path is clear
        while (frontIsClear())
        {
            move();
        }
        turnLeft();
        move();

        """;

    public const string FullLanguage =
        """
        // Lesson: full toolbox — top-level + small procedure
        DoSquare();

        void DoSquare()
        {
            integer side = 0;
            while (side < 4)
            {
                move();
                turnLeft();
                side = side + 1;
            }
        }

        """;
}
