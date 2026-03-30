# Semantic diagnostics

Messages should be plain, direct, non-academic, localized to the real problem.

## Name

- The name 'x' does not exist in the current scope.
- The function 'pick' is not available in this lesson.
- The function 'fly' does not exist.

## Type

- Cannot assign a value of type 'number' to a variable of type 'integer'.
- Condition must be of type 'bool'.
- Index must be of type 'integer'.

## Call

- Function 'move' expects 0 arguments but got 1.
- Argument 2 must be of type 'integer' but got 'string'.

## Declaration

- A variable named 'x' is already declared in this scope.
- A function named 'add' is already declared.

## Return

- Return value must be of type 'integer'.
- Return is not allowed at the top level.

Parse-time diagnostics live in the Language layer alongside the parser.
