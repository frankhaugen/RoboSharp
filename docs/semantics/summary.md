# Summary — Language + Semantics

Lock these seams:

1. Source model (Language)  
2. Token model (Language)  
3. Lexer (Language)  
4. Syntax facts (Language)  
5. Parser (Language)  
6. Syntax tree (Language)  
7. Symbol model (Semantics)  
8. Type system (Semantics)  
9. Builtin signature model (Semantics + catalog in Language docs)  
10. Binder / semantic analyzer (Semantics)  
11. Bound tree (Semantics)  
12. Diagnostic model (both: parse vs semantic)  

## Design rules

- Parser knows syntax, not lesson availability  
- Syntax tree preserves what was written  
- Bound tree represents validated meaning  
- Arrays stay minimal and function-based, with no member model  
- Built-ins are profile-provided capabilities, not keywords  

Next step in implementation: project-by-project specs for `RoboSharp.Language` and `RoboSharp.Semantics` with concrete types and first-pass parser/binder flow.
