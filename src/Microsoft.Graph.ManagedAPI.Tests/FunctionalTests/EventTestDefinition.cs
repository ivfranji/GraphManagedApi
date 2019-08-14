namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Graph.Exchange;
    using Search;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Event test definition.
    /// </summary>
    internal static class EventTestDefinition
    {
        /// <summary>
        /// CRUD operation for event.
        /// </summary>
        /// <param name="exchangeServiceA"></param>
        /// <param name="exchangeServiceB"></param>
        /// <returns></returns>
        public static async Task CreateReadUpdateDeleteEvent(ExchangeService exchangeServiceA, ExchangeService exchangeServiceB)
        {
            string subject = Guid.NewGuid().ToString();
            Event calendarEvent = new Event(exchangeServiceA);
            calendarEvent.Body = new ItemBody()
            {
                Content = "test",
                ContentType = BodyType.Html
            };

            calendarEvent.Subject = subject;
            calendarEvent.Start = new DateTimeTimeZone()
            {
                DateTime = DateTimeHelper.GetFormattedDateTime().ToString("yyyy-MM-ddThh:mm:ss"),
                TimeZone = "Central European Standard Time"
            };

            calendarEvent.End = new DateTimeTimeZone()
            {
                DateTime = DateTimeHelper.GetFormattedDateTime(5).ToString("yyyy-MM-ddThh:mm:ss"),
                TimeZone = "Central European Standard Time"
            };

            calendarEvent.Attendees = new List<Attendee>()
            {
                new Attendee()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = AppConfig.MailboxB
                    }
                }
            };

            await calendarEvent.SaveAsync();
            DateTime created = DateTime.Now;

            Thread.Sleep(8000); // allow item to be delivered to mailbox b
            SearchFilter subjectFilter = new SearchFilter.IsEqualTo(
                EventObjectSchema.Subject,
                subject);

            FindItemResults<Event> items = await exchangeServiceB.FindItems<Event>(new EventView(), subjectFilter);

            Assert.AreEqual(
                1,
                items.TotalCount);


            Event meeting = (Event)items.Items[0];
            await meeting.Decline(
                "no comment",
                true);

            await calendarEvent.DeleteAsync();
        }
    }
}