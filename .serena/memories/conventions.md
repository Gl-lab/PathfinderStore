# Conventions
- AGENTS.md is authoritative; mandatory MemoryBank files must be read each session.
- C#: never var; explicit access modifiers; fields _camelCase and readonly when possible; sealed leaf classes; no this.; braces always with opening brace on new line; spaces inside parentheses/brackets; each chained call on its own line; constructors block-bodied, accessors expression-bodied; explicit parentheses in compound expressions.
- Files use UTF-8, 4 spaces, no final newline.
- Project docs use repository-relative paths only.
- Character-derived values are computed server-side and exposed with explanatory read models; do not persist or accept computable values from clients.
- EF migrations only through dotnet ef; never hand-edit migration files.
- Vikunja is task source of truth; read task before status update and preserve description in every partial update.