---
name: generate-pathfinder-avatar
description: Generate and integrate the next missing Pathfinder character avatar using the project's matrix, prompts, conversion, catalog, tests, and batch-note workflow without visual QA. Use whenever the user asks to generate, create, continue, add, or regenerate a thematic Pathfinder avatar or avatar batch; always determine the next eligible matrix row before generating anything so existing or partially integrated assets are not duplicated.
---

# Generate Pathfinder Avatar

Follow the repository workflow from selection through integration. Treat
`MemoryBank/10_workflow/avatar_assets.md` as normative and do not substitute a
generic image-generation process.

## 1. Load project rules

1. Determine and activate the Git repository root as required by `AGENTS.md`.
2. Read `MemoryBank/10_workflow/avatar_assets.md` completely.
3. Use these sources of truth without copying or inventing metadata:
   - `MemoryBank/90_research/avatar_asset_matrix.md`
   - `MemoryBank/90_research/avatar_generation_prompts.md`
   - `CharacterManagement.Application/Avatars/AvatarCatalog.cs`
   - `MemoryBank/90_research/avatar_batches/`

## 2. Select before generating

Run the bundled read-only selector from the repository root:

```powershell
python .agents/skills/generate-pathfinder-avatar/scripts/find_next_avatar.py
```

Interpret the result strictly:

- Exit `0`: use exactly the reported matrix row.
- Exit `2`: stop generation. Reconcile the reported partial state or duplicate
  first; never generate another image for that row.
- Exit `3`: the matrix is fully implemented; report completion and do not
  generate.
- Any other failure: inspect the selector or repository layout; do not guess the
  next ID.

Before generation, also search the latest batch notes and temporary inputs for
the selected `AvatarId` and asset filename. If an unfinished or user-provided
source already exists, inspect and resume it instead of generating a duplicate.

State the selected `AvatarId`, ancestry, class, gender, variant, and asset path
before invoking the generator. Never renumber rows or choose a later row merely
because it is more convenient.

## 3. Generate once

1. Build the English production prompt from the exact selected matrix row and
   `avatar_generation_prompts.md`.
2. Apply every constant constraint, the selected ancestry/class/variant
   profiles, and the full negative prompt.
3. Use the available image-generation skill/tool for a square `1024x1024` or
   larger PNG master.
4. Use the first successfully generated result. Do not open it for visual
   inspection, create crop or `48x48` previews, score its appearance, or
   regenerate it based on visual quality.
5. Verify only that the source file exists, is decodable, square, large enough,
   and has a supported opaque color mode. Stop only on a generation or technical
   file failure.

Do not delete a user-provided source. Keep intermediate PNG files out of Git by
default.

## 4. Convert and integrate generated work

Unless the user explicitly limits the request to producing a source image,
complete the entire accepted-asset workflow in one change:

1. Convert without stretching to RGB WebP `512x512`, quality `88`, method `6`.
2. Add the file at the exact matrix path.
3. Add the exact `AvatarDefinition` to `AvatarCatalog`.
4. Update `AvatarSelectorTests` for covered and still-uncovered combinations.
5. Add the next batch note with provenance and technical validation results.
6. Update the catalog-volume task note when the accepted boundary changes.

Never register a missing or technically invalid file, reuse an ID, or reassign avatars of
existing characters.

## 5. Verify

Run all checks required by `avatar_assets.md`, including the focused
`AvatarSelectorTests`, WebP format/dimensions/mode verification,
`git diff --check`, and `git status --short`.

Run the selector again. The new selected row must advance past the connected
asset and no partial-state error may remain. Report the integrated `AvatarId`,
paths, technical file validation, and test result.
