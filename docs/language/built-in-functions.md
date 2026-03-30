# Built-in functions (catalog)

Built-ins are **called like normal functions** in source. The parser does **not** special-case them; **availability** is decided in semantic analysis from the active profile ([../semantics/builtins-and-profiles.md](../semantics/builtins-and-profiles.md)).

## v1 built-ins (teaching names)

Robot:

- `move()`
- `turnLeft()`
- `turnRight()`
- `pick()`
- `drop()`
- `frontIsClear()`
- `leftIsClear()`
- `rightIsClear()`

General:

- `print(value)`

Collections:

- `count(array)`
- `add(array, item)`
- `getLast(array)`
- `takeLast(array)`

Keep beginner-friendly names for v1; do not switch to `Push`/`Pop`/`Peek` in the first pass.

Type/symbol definitions and `BuiltinId`: [../semantics/builtins-and-profiles.md](../semantics/builtins-and-profiles.md).
