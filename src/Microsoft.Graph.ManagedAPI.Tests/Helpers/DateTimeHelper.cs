namespace Microsoft.Graph.ManagedAPI.Tests
{
    using System;

    /// <summary>
    /// Date time helpers.
    /// </summary>
    internal static class DateTimeHelper
    {
        /// <summary>
        /// Get formatted date/time.
        /// </summary>
        /// <param name="hoursToAdd"></param>
        /// <returns></returns>
        internal static DateTime GetFormattedDateTime(int hoursToAdd = 2)
        {
            DateTime dateTime = DateTime.UtcNow.AddHours(hoursToAdd);
            DateTime roundDateTime = new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute - (dateTime.Minute % 15),
                0);

            return roundDateTime;
        }
    }
}
