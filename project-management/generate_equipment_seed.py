import json
import re
from pathlib import Path

# Path to your markdown table and output JSON
MD_PATH = Path(r'd:/Dev/GameSupport/Exanima/project-management/Exanima_Equipment_Stats.md')
JSON_PATH = Path(r'd:/Dev/GameSupport/Exanima/ExanimaToolsApp/project-management/equipment_seed.json')

# Define variant templates
VARIANTS = [
    ("Crude", "Worn"),
    ("Decent", "Fair"),
    ("Well Made", "Good"),
]

# Helper to round to nearest 0.5
pip = lambda x: round(float(x) * 2) / 2

def parse_markdown_table(md_lines):
    items = []
    header = []
    for line in md_lines:
        if line.strip().startswith('|') and not line.strip().startswith('|---'):
            cols = [c.strip() for c in line.strip().split('|')[1:-1]]
            if not header:
                header = cols
            else:
                item = dict(zip(header, cols))
                items.append(item)
    return items

def stat_dict_from_row(row):
    stats = {}
    for k, v in row.items():
        if k in ("Name", "Type", "Category", "Subcategory", "Min Rank", "Description"):
            continue
        try:
            val = float(v)
            stats[k] = pip(val)
        except (ValueError, TypeError):
            pass
    return stats

def main():
    with open(MD_PATH, encoding='utf-8') as f:
        lines = f.readlines()
    items = parse_markdown_table(lines)
    out = []
    for row in items:
        min_rank_str = row.get("Min Rank", "0")
        try:
            min_rank = int(min_rank_str)
        except Exception:
            continue  # skip rows where Min Rank is not an int
        base_name = row.get("Name")
        base_type = row.get("Type") or row.get("Category")
        desc = row.get("Description", "")
        stats = stat_dict_from_row(row)
        for i, (quality, condition) in enumerate(VARIANTS):
            name = f"{base_name} ({quality})"
            variant_stats = {k: pip(v * (0.9 if i==0 else 1.0 if i==1 else 1.1)) for k, v in stats.items()}
            out.append({
                "Name": name,
                "Type": base_type,
                "MinRank": min_rank,
                "Quality": quality,
                "Condition": condition,
                "Description": desc,
                "Stats": variant_stats
            })
    with open(JSON_PATH, 'w', encoding='utf-8') as f:
        json.dump(out, f, indent=2, ensure_ascii=False)
    print(f"Wrote {len(out)} items to {JSON_PATH}")

if __name__ == "__main__":
    main()
