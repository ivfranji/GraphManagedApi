namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Graph.Exchange;
    using Search;
    using VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Message rule test definition.
    /// </summary>
    internal static class MessageRuleTestDefinition
    {
        /// <summary>
        /// MessageRule CRUD operation.
        /// </summary>
        /// <param name="exchangeService"></param>
        /// <returns></returns>
        public static async Task CreateReadUpdateDeleteMessageRule(ExchangeService exchangeService)
        {
            string ruleName = Guid.NewGuid().ToString();
            MessageRule rule = new MessageRule(exchangeService);
            rule.DisplayName = ruleName;
            rule.Sequence = 1;
            rule.IsEnabled = true;
            rule.Conditions = new MessageRulePredicates()
            {
                SenderContains = new List<string>() { "testUser" }
            };
            rule.Actions = new MessageRuleActions()
            {
                ForwardTo = new List<Recipient>()
                {
                    new Recipient()
                    {
                        EmailAddress = new EmailAddress()
                        {
                            Address = "test@domain.com"
                        }
                    }
                },

                StopProcessingRules = true
            };

            await rule.SaveAsync();

            SearchFilter searchFilter = new SearchFilter.IsEqualTo(
                MessageRuleObjectSchema.DisplayName,
                ruleName);

            FindItemResults<MessageRule> rules = await exchangeService.FindItems(
                new MessageRuleView(), 
                searchFilter);

            Assert.AreEqual(
                1,
                rules.TotalCount);

            Assert.IsTrue(rules.Items[0].IsEnabled);

            rule.IsEnabled = false;
            await rule.UpdateAsync();

            rules = await exchangeService.FindItems(
                new MessageRuleView(),
                searchFilter);

            Assert.AreEqual(
                1,
                rules.TotalCount);

            Assert.IsFalse(rules.Items[0].IsEnabled);

            await rule.DeleteAsync();
        }
    }
}