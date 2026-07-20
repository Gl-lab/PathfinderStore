#!/usr/bin/env python3
"""Find the first Pathfinder avatar matrix row not yet implemented."""

from __future__ import annotations

import argparse
import re
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path


@dataclass(frozen=True)
class AvatarRow:
    number: int
    avatar_id: str
    asset_path: str
    ancestry: str
    class_id: str
    gender: str
    variant: str
    batch_key: str


def clean_cell(value: str) -> str:
    return value.strip().strip("`")


def find_repo_root(explicit_root: str | None) -> Path:
    if explicit_root:
        return Path(explicit_root).resolve()

    result = subprocess.run(
        ["git", "rev-parse", "--show-toplevel"],
        check=True,
        capture_output=True,
        text=True,
    )
    return Path(result.stdout.strip()).resolve()


def parse_matrix(matrix_path: Path) -> list[AvatarRow]:
    rows: list[AvatarRow] = []
    for line in matrix_path.read_text(encoding="utf-8").splitlines():
        cells = [clean_cell(cell) for cell in line.strip().strip("|").split("|")]
        if len(cells) != 11 or not cells[0].isdigit():
            continue
        rows.append(
            AvatarRow(
                number=int(cells[0]),
                avatar_id=cells[1],
                asset_path=cells[2],
                ancestry=cells[3],
                class_id=cells[4],
                gender=cells[5],
                variant=cells[6],
                batch_key=cells[10],
            )
        )

    if not rows:
        raise RuntimeError(f"No avatar rows found in {matrix_path}")
    return rows


def occurrence_count(text: str, literal: str) -> int:
    return len(re.findall(re.escape(literal), text))


def describe(row: AvatarRow) -> str:
    return (
        f"#{row.number}: {row.avatar_id}\n"
        f"AssetPath: {row.asset_path}\n"
        f"Ancestry: {row.ancestry}\n"
        f"ClassId: {row.class_id}\n"
        f"Gender: {row.gender}\n"
        f"Variant: {row.variant}\n"
        f"BatchKey: {row.batch_key}"
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo-root", help="Pathfinder Git repository root")
    args = parser.parse_args()

    repo_root = find_repo_root(args.repo_root)
    matrix_path = repo_root / "MemoryBank/90_research/avatar_asset_matrix.md"
    catalog_path = repo_root / "CharacterManagement.Application/Avatars/AvatarCatalog.cs"
    public_root = repo_root / "pathfinder.frontend/public"

    rows = parse_matrix(matrix_path)
    catalog = catalog_path.read_text(encoding="utf-8")

    for row in rows:
        asset_file = public_root / row.asset_path.lstrip("/")
        asset_exists = asset_file.is_file()
        id_count = occurrence_count(catalog, f'"{row.avatar_id}"')
        path_count = occurrence_count(catalog, f'"{row.asset_path}"')

        implemented = asset_exists and id_count == 1 and path_count == 1
        absent = not asset_exists and id_count == 0 and path_count == 0

        if implemented:
            continue
        if absent:
            print("NEXT_AVATAR")
            print(describe(row))
            return 0

        print("INCONSISTENT_AVATAR_STATE", file=sys.stderr)
        print(describe(row), file=sys.stderr)
        print(f"AssetExists: {asset_exists}", file=sys.stderr)
        print(f"CatalogIdOccurrences: {id_count}", file=sys.stderr)
        print(f"CatalogPathOccurrences: {path_count}", file=sys.stderr)
        print("Reconcile this row before generating anything.", file=sys.stderr)
        return 2

    print("AVATAR_MATRIX_COMPLETE")
    print(f"ImplementedRows: {len(rows)}")
    return 3


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (OSError, RuntimeError, subprocess.SubprocessError) as error:
        print(f"SELECTOR_ERROR: {error}", file=sys.stderr)
        raise SystemExit(1)
