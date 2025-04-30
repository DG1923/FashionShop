import requests
import sys
import json
from dotenv import load_dotenv
import os
import uuid
import time
from google import genai
from google.genai import types

# Load biến môi trường
load_dotenv()
API_KEY = os.getenv("API_GEMINI")

# Prompt chuẩn của bạn
systemPrompt = """
You are a super intelligent AI specialized in crawling and structuring e-commerce product data.
You are a Python developer and will always return output as a JSON array with the exact format below.

Your **input** is a JSON or HTML content that contains product information. Your job is to:
- Extract all relevant product data (name, description, base price, discount, main image, colors, sizes, etc.).
- Map each image correctly with its corresponding color. For example, images[0] corresponds to color[0], images[1] to color[1], and so on.
- Generate output strictly in the format below:

  {
    "id": "string (Guid)",
    "name": "string",
    "description": "string",
    "basePrice": number,
    "discountedPrice": number,
    "mainImageUrl": "string",
    "discountDisplayDTO": {
      "id": "string (Guid)",
      "name": "string",
      "discountPercent": number,
      "isActive": boolean
    },
    "productColorsDisplayDTO": [
      {
        "id": "string (Guid)",
        "colorName": "string",
        "colorCode": "string",
        "imageUrlColor": "string (match with the correct color)",
        "productVariationDisplayDTOs": [
          {
            "id": "string (Guid)",
            "size": "string",
            "description": "string",
            "quantity": number,
            "imageUrlVariation": "string"
          }
        ]
      }
    ],
    "productCategoryDisplayDTO": {
      "id": "string (Guid)",
      "name": "string",
      "imageUrl": "string"
    }
  }

Important Notes:
- Always ensure correct image-to-color mapping based on index or associated color name.
- If color has multiple sizes or variations, list them all under 'productVariationDisplayDTOs'.
- Use UUID format for all IDs (use Python's uuid4() if generating).
- Skip null values and ensure the JSON is clean and valid.
- with url image like /images/abc.jpg, plesase add prefix domain is https://media3.coolmate.me/
- With id is a string, please convert to Guid format. You can generate a new GUID for .net 
"""


# Load dữ liệu đầu vào
with open("products.json", "r", encoding="utf-8") as file:
    raw = json.load(file)
    json_data = raw.get("data", [])

# Lọc ra các trường cần thiết
def filter_json(data):
    return {
        "id": data.get("id"),
        "basePrice": data.get("price"),
        "compare_price": data.get("compare_price"),
        "images": data.get("images"),
        "name": data.get("title"),
        "options": data.get("options"),
        "options_value": data.get("options_value"),
        "variants": data.get("variants"),
    }

filtered_json = [filter_json(item) for item in json_data]

# Khởi tạo Gemini
client = genai.Client(api_key=API_KEY)
chat = client.chats.create(
    model="gemini-2.0-flash-lite",
    config=types.GenerateContentConfig(system_instruction=systemPrompt)
)

# Hàm để extract JSON trong code block markdown
def extract_json_from_markdown(text):
    import re
    pattern = r"```json\s*(.*?)```"
    matches = re.findall(pattern, text, re.DOTALL)
    if matches:
        return matches[0].strip()
    return None

# Gửi từng item lên Gemini và xử lý
final = []
for idx, item in enumerate(filtered_json, start=1):
    json_string = json.dumps(item, ensure_ascii=False)
    try:
        response = chat.send_message(f"Convert to expert JSON. Here is input json -> {json_string}")

        markdown_content = response.text
        extracted_json_str = extract_json_from_markdown(markdown_content)

        if extracted_json_str:
            try:
                parsed_json = json.loads(extracted_json_str)
                final.append(parsed_json)
                print(f"✅ Parsed item {idx}/{len(filtered_json)}")
            except json.JSONDecodeError as je:
                print(f"⚠️ JSON lỗi ở item {idx}: {je}")
                final.append({"error": "json_parse_error", "original": extracted_json_str})
        else:
            print(f"❌ Không tìm thấy code block ở item {idx}")
            final.append({"error": "no_json_block", "original": markdown_content})

        # Nghỉ để tránh bị spam quá
        time.sleep(1.5)

    except Exception as e:
        print(f"🔥 Lỗi khi gửi Gemini ở item {idx}: {e}")
        final.append({"error": str(e)})

# Ghi kết quả ra file đẹp gọn
with open("output.json", "w", encoding="utf-8") as outfile:
    json.dump(final, outfile, ensure_ascii=False, indent=2)
