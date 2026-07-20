# Project conventions

## Authority
- `AGENTS.md` is the single agent-instruction source.
- Before C# edits read `MemoryBank/10_workflow/feedback_csharp_style.md`; `.editorconfig` is executable formatting/naming policy.

## C# invariants
- No `var`; explicit types.
- All fields `_camelCase`; constants/methods/properties/types `PascalCase`; events `OnPascalCase`.
- Explicit access modifiers; `readonly` wherever possible; `sealed` unless inheritance intended; no `this.`.
- Braces always, opening brace on new line; `else`/`catch`/`finally` on new lines.
- Spaces inside call/declaration/control-flow/cast/index brackets: `Foo( arg )`, `if ( x )`, `arr[ i ]`.
- Explicit parentheses in binary/relational expressions.
- Method chains: one call per line. Constructors never expression-bodied; accessors expression-bodied.
- Prefer object/collection initializers, pattern matching, throw expressions.
- UTF-8, 4 spaces, no final newline (per `.editorconfig`).

## Repository/docs/tasks
- Project documentation uses only repository-relative paths/links.
- Do not touch Store without explicit task.
- EF migrations only through `dotnet ef`, never hand-edit migration files.
- Vikunja mutations follow `MemoryBank/10_workflow/vikunja.md`: list first, preserve description on update, verify UTF-8/HTML after write.
- Preserve unrelated user changes in dirty worktrees.
