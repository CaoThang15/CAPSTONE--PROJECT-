using System.Text;
using System.Text.RegularExpressions;

namespace SMarket.DataAccess.DTOs.Common
{
    public class Helpers
    {
        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            temp = regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            temp = temp.Replace(" ", "-");
            temp = temp.Replace("_", "-");
            temp = temp.Replace(":", "");
            temp = temp.Replace("\\", "");
            temp = temp.Replace("/", "");
            temp = temp.Replace("\"", "");
            temp = temp.Replace("*", "");
            temp = temp.Replace("?", "");
            temp = temp.Replace(">", "");
            temp = temp.Replace("<", "");
            temp = temp.Replace("||", "");
            return temp;
        }

        public static string GenerateSlug(string title)
        {
            title = ConvertToUnSign(title).ToLower().Trim();

            title = title.Replace("_", "-");

            title = Regex.Replace(title, @"[^a-z0-9-]", "");

            title = Regex.Replace(title, @"-+", "-").Trim('-');

            return title;
        }
    }
}