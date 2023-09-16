namespace KegMonitor.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime ToUtcKindDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
