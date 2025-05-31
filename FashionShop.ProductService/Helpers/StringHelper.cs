namespace FashionShop.ProductService.Helpers
{
    public static class StringHelper
    {
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string[] vietnamese = new string[]
            {
            "aáàạảãâấầậẩẫăắằặẳẵ",
            "AÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "eéèẹẻẽêếềệểễ",
            "EÉÈẸẺẼÊẾỀỆỂỄ",
            "iíìịỉĩ",
            "IÍÌỊỈĨ",
            "oóòọỏõôốồộổỗơớờợởỡ",
            "OÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "uúùụủũưứừựửữ",
            "UÚÙỤỦŨƯỨỪỰỬỮ",
            "yýỳỵỷỹ",
            "YÝỲỴỶỸ",
            "đ",
            "Đ"
            };

            string[] latin = new string[]
            {
            "a", "A", "e", "E", "i", "I", "o", "O", "u", "U", "y", "Y", "d", "D"
            };

            for (int i = 0; i < vietnamese.Length; i++)
            {
                for (int j = 0; j < vietnamese[i].Length; j++)
                {
                    text = text.Replace(vietnamese[i][j], latin[i][0]);
                }
            }

            return text;

        }
    }
}
