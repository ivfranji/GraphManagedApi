namespace Microsoft.Graph.ManagedAPI.FunctionalTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Graph.Exchange;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Outlook task test definition.
    /// </summary>
    internal static class OutlookTaskTestDefinition
    {
        /// <summary>
        /// CRUD operation for OutlookTasks.
        /// </summary>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        public static async Task CreateReadUpdateDeleteOutlookTask(ExchangeService exchangeService)
        {
            // GRAPH API doesn't support Application Permission with OutlookTasks currently.
            return;

            //string subject = Guid.NewGuid().ToString();
            //exchangeService.LoggingEnabled = true;
            //exchangeService.LogFlag = LogFlag.All;

            //OutlookTask task = new OutlookTask(exchangeService);
            //task.Body = new ItemBody()
            //{
            //    ContentType = BodyType.Html,
            //    Content = "This is test task."
            //};

            //task.Subject = subject;
            //task.AssignedTo = AppConfig.MailboxB;
            //task.Importance = Importance.High;
            //task.DueDateTime = new DateTimeTimeZone()
            //{
            //    DateTime = DateTime.Now.AddDays(2).ToString(),
            //    TimeZone = "UTC"
            //};

            //await task.SaveAsync();
            //Assert.IsNotNull(task.Id);

            //task.Importance = Importance.Low;
            //task.DueDateTime = new DateTimeTimeZone()
            //{
            //    DateTime = DateTime.Now.AddDays(5).ToString(),
            //    TimeZone = "UTC"

            //};

            //await task.UpdateAsync();
            //Assert.AreEqual(
            //    Importance.Low,
            //    task.Importance);

            //SearchFilter searchFilter = new SearchFilter.IsEqualTo(
            //    OutlookTaskObjectSchema.Subject,
            //    subject);

            //FindItemResults<OutlookTask> tasks = await exchangeService.FindItems<OutlookTask>(
            //    new OutlookTaskView(), 
            //    searchFilter);

            //Assert.AreEqual(
            //    1,
            //    tasks.TotalCount);

            //IList<OutlookTask> completedTasks = await task.Complete();
            //Assert.AreEqual(
            //    1,
            //    completedTasks.Count);

            //await task.DeleteAsync();
        }
    }

    /// <summary>
    /// Outlook category test definition.
    /// </summary>
    internal static class OutlookCategoryTestDefinition
    {
        /// <summary>
        /// CRUD operation for Outlook categories.
        /// </summary>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        public static async Task CreateReadUpdateOutlookCategory(ExchangeService exchangeService)
        {
            string displayName = Guid.NewGuid().ToString();
            OutlookCategory category = new OutlookCategory(exchangeService);
            category.Color = CategoryColor.Preset18;
            category.DisplayName = displayName;

            await category.SaveAsync();

            FindItemResults<OutlookCategory> categories = await exchangeService.FindItems(
                new OutlookCategoryView(), 
                null);

            bool found = false;
            foreach (OutlookCategory outlookCategory in categories)
            {
                if (outlookCategory.DisplayName == displayName)
                {
                    found = true;
                }
            }

            Assert.IsTrue(found);
            await category.DeleteAsync();
        }
    }
}