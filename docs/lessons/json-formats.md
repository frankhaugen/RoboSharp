# JSON direction: lessons and content packs

These examples are **schema direction** for v1, not a committed on-disk contract. Final names, required fields, and validation rules should be aligned with implementation and world file format.

## Lesson file

```json
{
  "format": "RoboSharpLesson",
  "version": 1,
  "id": "lesson-01",
  "title": "Move to the Star",
  "description": "Move the robot to the goal tile.",
  "builtinProfileName": "BasicMovement",
  "worldFile": "worlds/lesson-01.world.json",
  "goals": {
    "requiredGoals": [
      { "kind": "ReachGoalTileGoal", "actorId": 1 }
    ],
    "optionalGoals": [
      { "kind": "MaxStepsGoal", "maxSteps": 10 }
    ]
  },
  "help": {
    "concepts": ["movement", "sequencing"],
    "examples": [
      {
        "title": "Move twice",
        "code": "move()\nmove()"
      }
    ],
    "highlightedBuiltins": ["Move", "TurnLeft"]
  },
  "hints": [
    {
      "id": "hint-01",
      "title": "Try moving first",
      "content": "Use move() to go forward.",
      "level": "Gentle"
    }
  ],
  "uiPolicy": {
    "showTokensPanel": false,
    "showSyntaxTreePanel": false,
    "showBoundTreePanel": false,
    "showIlPanel": false,
    "showWorldPanel": true,
    "showOutputPanel": true,
    "showMetricsPanel": false,
    "showGoalPanel": true,
    "pauseAtEntryByDefault": false,
    "allowDebugging": true
  },
  "progression": {
    "nextLessonId": "lesson-02",
    "previousLessonId": null,
    "prerequisiteLessonIds": []
  }
}
```

## Content pack file

```json
{
  "format": "RoboSharpContentPack",
  "version": 1,
  "id": "intro",
  "title": "RoboSharp Intro",
  "description": "First steps with movement, conditions, and output.",
  "contentVersion": "1.0.0",
  "lessonFiles": [
    "lessons/lesson-01.json",
    "lessons/lesson-02.json"
  ],
  "worldFiles": [
    "worlds/lesson-01.world.json",
    "worlds/lesson-02.world.json"
  ]
}
```

## Next step

Specify **world file schema**, **lesson schema validation**, and how paths resolve relative to the pack root. Cross-link from [../toolchain/project-format.md](../toolchain/project-format.md) when that doc exists.
