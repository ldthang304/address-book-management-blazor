namespace AddressBookManagement.Commons.Utils
{
    public class StringUtil
    {
        public static string SplitCamelCase(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? string.Empty
                : System.Text.RegularExpressions.Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        }
    }
}
