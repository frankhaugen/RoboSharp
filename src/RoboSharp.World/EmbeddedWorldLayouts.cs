namespace RoboSharp.World;

/// <summary>ASCII premade worlds — edit these strings to ship new maps without JSON yet.</summary>
public static class EmbeddedWorldLayouts
{
    /// <summary>8×6 bordered room with a goal in the corner — good first challenge.</summary>
    public static readonly string[] ReachGoalCorner =
    [
        "########",
        "#@.....#",
        "#......#",
        "#.....G#",
        "#......#",
        "########",
    ];

    /// <summary>10×8 with a wall obstacle the robot must go around.</summary>
    public static readonly string[] AroundWall =
    [
        "##########",
        "#@.......#",
        "#..####..#",
        "#..#..#..#",
        "#........#",
        "#.......G#",
        "#........#",
        "##########",
    ];

    /// <summary>12×9 simple corridor maze.</summary>
    public static readonly string[] CorridorMaze =
    [
        "############",
        "#@.........#",
        "#.###.###..#",
        "#.#...#....#",
        "#.#.###.##.#",
        "#.....#....#",
        "#.###.#..#G#",
        "#........#.#",
        "############",
    ];

    /// <summary>Wide open 14×10 for experiments.</summary>
    public static readonly string[] OpenPlayground =
    [
        "##############",
        "#@...........#",
        "#............#",
        "#.....G......#",
        "#............#",
        "#............#",
        "#............#",
        "#............#",
        "##############",
    ];
}
