namespace address_book_backend.Commons.Utils
{
    public static class DateTimeUtil
    {
        public static DateTime Now => DateTime.Now;
        public static string CustomFormat => "yyyy-MM-dd HH:mm:ss";
        public static string ISOFormatDateOnly => "yyyy-MM-dd";
        public static string USFormatDate => "MM/dd/yyyy";
        public static string EUFormatDate => "dd/MM/yyyy";
        public static string ISO8601Format => "yyyy-MM-ddTHH:mm:ss";

        public static string Format(DateTime dateTime, string format)
        {
            return dateTime.ToString(format);
        }
    }
}
