namespace Microsoft.Graph.ManagedAPI.Tests.FunctionalTests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Graph.ManagedAPI.Tests.Auth;
    using Microsoft.Graph.Exchange;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Functional tests.
    /// </summary>
    [TestClass]
    public class FunctionalTestRunner
    {
        /// <summary>
        /// Exchange context.
        /// </summary>
        private static ExchangeServiceContext exchangeContext;

        /// <summary>
        /// Class init.
        /// </summary>
        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            FunctionalTestRunner.exchangeContext = new ExchangeServiceContext(
                new TestAuthenticationProvider(), 
                "FunctionalTestCase");
        }

        #region User tests

        [TestMethod]
        public async Task Test_GetUser()
        {
            await this.RunAsMailboxA(UserTestDefinition.GetUser);
        }

        #endregion

        #region Message tests

        /// <summary>
        /// Get message with SingleValueExtendedProperties.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_GetMessageWithSingleExtendedProperties()
        {
            await this.RunAsMailboxB(MessageTestDefinition.GetMessageWithSingleExtendedProperties);
        }

        /// <summary>
        /// Send message from mailbox A to mailbox B.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_SendMessageFromMailboxAToMailboxB()
        {
            await this.RunWithMailboxAMailboxB(MessageTestDefinition.SendMessageFromMailboxAToMailboxB);
        }

        /// <summary>
        /// Test sync messages.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_SyncMessages()
        {
            await this.RunAsMailboxA(MessageTestDefinition.SyncMessages);
        }

        /// <summary>
        /// Test create, read, update, delete message.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteMessage()
        {
            await this.RunAsMailboxA(MessageTestDefinition.CreateReadUpdateDeleteMessage);
        }

        /// <summary>
        /// Test find message.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_FindMessage()
        {
            await this.RunAsMailboxA(MessageTestDefinition.FindMessage);
        }

        /// <summary>
        /// Test create, read, update, delete extended properties.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteExtendedProperties()
        {
            await this.RunAsMailboxA(MessageTestDefinition.CreateReadUpdateDeleteExtendedProperties);
        }

        #endregion

        #region MailFolder tests

        /// <summary>
        /// Test sync mail folders.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_SyncMailFolders()
        {
            await this.RunAsMailboxA(MailFolderTestDefinition.SyncMailFolders);
        }

        /// <summary>
        /// Test create, read, update, delete <see cref="MailFolder"/>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteMailFolder()
        {
            await this.RunAsMailboxB(MailFolderTestDefinition.CreateReadUpdateDeleteMailFolder);
        }

        /// <summary>
        /// Test get mail folders.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_GetMailFolders()
        {
            await this.RunAsMailboxB(MailFolderTestDefinition.GetMailFolders);
        }

        /// <summary>
        /// Test get extended property from <see cref="MailFolder"/>.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_GetExtendedPropertyFromFolder()
        {
            await this.RunAsMailboxB(MailFolderTestDefinition.GetExtendedPropertyFromFolder);
        }

        #endregion

        #region Contacts tests

        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteContact()
        {
            await this.RunAsMailboxA(ContactTestDefinition.CreateReadUpdateDeleteContact);
        }

        #endregion

        #region Events tests

        /// <summary>
        /// CRUD for events.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteEvent()
        {
            await this.RunWithMailboxAMailboxB(EventTestDefinition.CreateReadUpdateDeleteEvent);
        }

        #endregion

        #region OutlookTasks tests

        /// <summary>
        /// OutlookTask CRUD operations.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteOutlookTask()
        {
            await this.RunAsMailboxA(OutlookTaskTestDefinition.CreateReadUpdateDeleteOutlookTask);
        }

        #endregion

        #region OutlookItem tests

        [TestMethod]
        public async Task Test_FindEventItems()
        {
            await this.RunAsMailboxA(OutlookItemTestDefinition.FindEventItems);
        }

        [TestMethod]
        public async Task Test_FindMessageItems()
        {
            await this.RunAsMailboxA(OutlookItemTestDefinition.FindMessageItems);
        }

        [TestMethod]
        public async Task Test_FindContactItems()
        {
            await this.RunAsMailboxA(OutlookItemTestDefinition.FindContactItems);
        }

        #endregion

        #region MessageRule tests

        /// <summary>
        /// MessageRule CRUD.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test_CreateReadUpdateDeleteMessageRule()
        {
            await this.RunAsMailboxA(MessageRuleTestDefinition.CreateReadUpdateDeleteMessageRule);
        }

        #endregion

        #region OutlookCategory tests

        [TestMethod]
        public async Task Test_CreateReadUpdateOutlookCategory()
        {
            await this.RunAsMailboxA(OutlookCategoryTestDefinition.CreateReadUpdateOutlookCategory);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Run test case as mailbox 'A'.
        /// </summary>
        /// <param name="testCase">Test case to run.</param>
        /// <returns></returns>
        private async Task RunAsMailboxA(Func<ExchangeService, Task> testCase)
        {
            await testCase(this.MailboxA);
        }

        /// <summary>
        /// Run test case as mailbox 'B'.
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns></returns>
        private async Task RunAsMailboxB(Func<ExchangeService, Task> testCase)
        {
            await testCase(this.MailboxB);
        }

        /// <summary>
        /// Run test case and provide two sessions to it.
        /// </summary>
        /// <param name="testCase">Test case to run.</param>
        /// <returns></returns>
        private async Task RunWithMailboxAMailboxB(Func<ExchangeService, ExchangeService, Task> testCase)
        {
            await testCase(
                this.MailboxA,
                this.MailboxB);
        }

        /// <summary>
        /// Mailbox A exchange service.
        /// </summary>
        private ExchangeService MailboxA
        {
            get { return this.GetExchangeService(Mailbox.A); }
        }

        /// <summary>
        /// Mailbox B exchange service.
        /// </summary>
        private ExchangeService MailboxB
        {
            get { return this.GetExchangeService(Mailbox.B); }
        }

        /// <summary>
        /// Get exchange service.
        /// </summary>
        /// <param name="mailbox">Mailbox.</param>
        /// <returns></returns>
        private ExchangeService GetExchangeService(Mailbox mailbox)
        {
            string mailboxAddress = mailbox == Mailbox.A
                ? AppConfig.MailboxA
                : AppConfig.MailboxB;

            return FunctionalTestRunner.exchangeContext[mailboxAddress];
        }

        #endregion

        #region Private class/enum definitions

        /// <summary>
        /// Mailbox.
        /// </summary>
        private enum Mailbox
        {
            A,
            B
        }

        #endregion
    }
}
