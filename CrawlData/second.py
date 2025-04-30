import json
with open("output.json", "w", encoding="utf-8") as outfile:
    raw = json.loads(outfile)
    # json.dump(final_filtered_json, outfile, indent=2, ensure_ascii=False)

# Bước 1: Giải mã JSON từ string kiểu escape
clean_json = json.loads(raw)

# Bước 2: In ra cho đẹp
print(json.dumps(clean_json, indent=2, ensure_ascii=False))
