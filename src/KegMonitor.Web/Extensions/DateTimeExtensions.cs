namespace KegMonitor.Web
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundUp(this DateTime date, TimeSpan span)
        {
            return new DateTime((date.Ticks + span.Ticks - 1) / span.Ticks * span.Ticks, date.Kind);
        }
    }
}
