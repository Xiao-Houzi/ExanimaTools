import json
import re
from pathlib import Path

# Path to your markdown table and output JSON
MD_PATH = Path(r'd:/Dev/GameSupport/Exanima/project-management/Exanima_Equipment_Stats.md')
JSON_PATH = Path(r'd:/Dev/GameSupport/Exanima/DevAssets/equipment_seed.json')

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
                if len(cols) != len(header):
                    print(f"Skipping row with wrong column count: {cols}")
                    continue
                item = dict(zip(header, cols))
                items.append(item)
    print(f"Parsed {len(items)} items from table.")
    return items

def stat_dict_from_row(row, table_type):
    stats = {}
    if table_type == "shields":
        for k, v in row.items():
            if k in ("Name", "Type", "Category", "Min Rank", "Description"): continue
            if k in ("Block", "Coverage", "Durability", "Weight"):
                try: stats[k] = pip(float(v))
                except (ValueError, TypeError): pass
    elif table_type == "armour":
        for k, v in row.items():
            if k in ("Name", "Type/Slot", "Category", "Min Rank", "Description"): continue
            if k in ("Weight", "Slash", "Crush", "Pierce", "Coverage", "Encumbrance"):
                try: stats[k] = pip(float(v))
                except (ValueError, TypeError): pass
    else:
        for k, v in row.items():
            if k in ("Name", "Type", "Category", "Subcategory", "Min Rank", "Description"): continue
            try: stats[k] = pip(float(v))
            except (ValueError, TypeError): pass
    return stats

def main():
    with open(MD_PATH, encoding='utf-8') as f:
        lines = f.readlines()
    out = []
    table_items = []
    current_table = None
    parsing_table = False
    table_buffer = []
    for idx, line in enumerate(lines):
        if line.strip().startswith('## '):
            # Only recognize main equipment sections
            section = line.strip().replace('## ', '').lower()
            if section.startswith('weapons'):
                current_table = 'weapons'
            elif section.startswith('shields'):
                current_table = 'shields'
            elif section.startswith('armour'):
                current_table = 'armour'
            else:
                current_table = None
            parsing_table = False
            table_buffer = []
            continue
        if current_table and line.strip().startswith('|'):
            parsing_table = True
            table_buffer.append(line)
        elif parsing_table and not line.strip().startswith('|'):
            # End of table, parse it
            items = parse_markdown_table(table_buffer)
            for item in items:
                item['__table'] = current_table
            table_items.extend(items)
            parsing_table = False
            table_buffer = []
        elif parsing_table:
            table_buffer.append(line)
    # Catch any table at EOF
    if parsing_table and table_buffer:
        items = parse_markdown_table(table_buffer)
        for item in items:
            item['__table'] = current_table
        table_items.extend(items)
    print(f"Found {len(table_items)} table rows before filtering.")
    seen_keys = set()
    for row in table_items:
        min_rank_str = row.get("Min Rank", "0")
        try:
            min_rank = int(min_rank_str)
        except Exception:
            print(f"Skipping row with invalid Min Rank: {row}")
            continue
        base_name = row.get("Name")
        if not base_name or base_name.lower().startswith("none"):
            print(f"Skipping row with name: {base_name}")
            continue
        table_type = row.get("__table", "").lower()
        if table_type == "shields":
            base_type = row.get("Type") or row.get("Category")
            desc = row.get("Description", "")
            stats = stat_dict_from_row(row, table_type)
            subcategory = None
        elif table_type == "armour":
            base_type = row.get("Type/Slot") or row.get("Type") or row.get("Category")
            desc = row.get("Description", "")
            stats = stat_dict_from_row(row, table_type)
            subcategory = None
        else:
            base_type = row.get("Type") or row.get("Type/Slot") or row.get("Category")
            desc = row.get("Description", "")
            stats = stat_dict_from_row(row, table_type)
            subcategory = row.get("Subcategory") if "Subcategory" in row else None
        # Determine TopType from table name (robust)
        if table_type == "weapons":
            top_type = "Weapon"
        elif table_type == "armour":
            top_type = "Armour"
        elif table_type == "shields":
            top_type = "Shield"
        else:
            # Fallback inference (should not be needed)
            if base_type and ("shield" in base_type.lower()):
                top_type = "Shield"
            elif base_type and ("armour" in base_type.lower()):
                top_type = "Armour"
            elif base_name and ("shield" in base_name.lower()):
                top_type = "Shield"
            elif base_name and ("armour" in base_name.lower()):
                top_type = "Armour"
            else:
                top_type = "Weapon"  # fallback
        if len(out) < 10:
            print(f"DEBUG: Row '{base_name}' table_type='{table_type}' assigned TopType='{top_type}'")
        for i, (quality, condition) in enumerate(VARIANTS):
            name = base_name.strip()
            key = (name, base_type, subcategory, quality, condition)
            if key in seen_keys:
                print(f"Skipping duplicate: {key}")
                continue
            seen_keys.add(key)
            variant_stats = {k: pip(v * (0.9 if i==0 else 1.0 if i==1 else 1.1)) for k, v in stats.items()}
            item = {
                "Name": name,
                "TopType": top_type,
                "Type": base_type,
                "Category": base_type,
                "MinRank": min_rank,
                "Quality": quality,
                "Condition": condition,
                "Description": desc,
                "Stats": variant_stats
            }
            if subcategory:
                item["Subcategory"] = subcategory
            out.append(item)
    print(f"Writing {len(out)} items to {JSON_PATH}")
    with open(JSON_PATH, 'w', encoding='utf-8') as f:
        json.dump(out, f, indent=2, ensure_ascii=False)
    print(f"Wrote {len(out)} items to {JSON_PATH}")
    print(f"Table items by table type:")
    from collections import Counter
    table_type_counter = Counter(row.get("__table", "") for row in table_items)
    for t, c in table_type_counter.items():
        print(f"  {t}: {c} rows")

if __name__ == "__main__":
    main()
