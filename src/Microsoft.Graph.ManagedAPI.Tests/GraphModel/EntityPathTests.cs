namespace Microsoft.Graph.ManagedAPI.Tests.GraphModel
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Entity path tests.
    /// </summary>
    [TestClass]
    public class EntityPathTests
    {
        /// <summary>
        /// Test entity path properties.
        /// </summary>
        [TestMethod]
        public void Test_MessageEntityPath()
        {
            EntityPath entityPath = new EntityPath(typeof(Message));
            Assert.IsTrue(entityPath.IsRootContainer);
            Assert.AreEqual("", entityPath.Id);
            Assert.AreEqual("Messages", entityPath.Path);
            Assert.AreEqual("Messages", entityPath.RootContainer);
            Assert.IsNull(entityPath.SubEntity);

            entityPath = new EntityPath("Abc", typeof(MailFolder));
            Assert.IsFalse(entityPath.IsRootContainer);
            Assert.AreEqual("Abc", entityPath.Id);
            Assert.AreEqual("MailFolders/Abc", entityPath.Path);
            Assert.AreEqual("MailFolders", entityPath.RootContainer);
            Assert.IsNull(entityPath.SubEntity);

            entityPath = new EntityPath("Abc", typeof(MailFolder));
            entityPath.SubEntity = new EntityPath(typeof(Message));
            Assert.IsFalse(entityPath.IsRootContainer);
            Assert.AreEqual("Abc", entityPath.Id);
            Assert.AreEqual("MailFolders/Abc/Messages", entityPath.Path);
            Assert.AreEqual("MailFolders", entityPath.RootContainer);

            Assert.IsTrue(entityPath.SubEntity.IsRootContainer);
            Assert.AreEqual("", entityPath.SubEntity.Id);
            Assert.AreEqual("Messages", entityPath.SubEntity.Path);
            Assert.AreEqual("Messages", entityPath.SubEntity.RootContainer);
        }

        /// <summary>
        /// Test if constructor prevents creating invalid paths.
        /// </summary>
        [TestMethod]
        public void Test_EntityPathThrowsErrorOnInvalidConstruct()
        {
            EntityPath entityPath;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                // only types derived from Entity can use entity path.
                entityPath = new EntityPath(typeof(string));
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                entityPath = new EntityPath((Type)null);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                entityPath = new EntityPath("", typeof(Event));
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                entityPath = new EntityPath((Entity)null);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                entityPath = new EntityPath(new Event());
            });
        }

        /// <summary>
        /// Test OutlookTask entity path.
        /// </summary>
        [TestMethod]
        public void Test_OutlookTaskEntityPath()
        {
            EntityPath entityPath = new EntityPath(typeof(OutlookTask));
            Assert.IsTrue(entityPath.IsRootContainer);
            Assert.AreEqual("Outlook/Tasks", entityPath.RootContainer);
            Assert.AreEqual("Outlook/Tasks", entityPath.Path);
            Assert.IsNull(entityPath.SubEntity);

            entityPath = new EntityPath("abcd", typeof(OutlookTask));
            Assert.IsFalse(entityPath.IsRootContainer);
            Assert.AreEqual("Outlook/Tasks", entityPath.RootContainer);
            Assert.AreEqual("Outlook/Tasks/abcd", entityPath.Path);
            Assert.IsNull(entityPath.SubEntity);
        }

        /// <summary>
        /// Test OutlookCategory entity path.
        /// </summary>
        [TestMethod]
        public void Test_OutlookCategoryEntityPath()
        {
            EntityPath entityPath = new EntityPath(typeof(OutlookCategory));
            Assert.IsTrue(entityPath.IsRootContainer);
            Assert.AreEqual("Outlook/MasterCategories", entityPath.RootContainer);
            Assert.AreEqual("Outlook/MasterCategories", entityPath.Path);
            Assert.IsNull(entityPath.SubEntity);

            entityPath = new EntityPath("abcd", typeof(OutlookCategory));
            Assert.IsFalse(entityPath.IsRootContainer);
            Assert.AreEqual("Outlook/MasterCategories", entityPath.RootContainer);
            Assert.AreEqual("Outlook/MasterCategories/abcd", entityPath.Path);
            Assert.IsNull(entityPath.SubEntity);
        }

        /// <summary>
        /// Test event message entity path.
        /// </summary>
        [TestMethod]
        public void Test_EventMessageEntityPath()
        {
            EntityPath entityPath = new EntityPath(typeof(EventMessage));
            Assert.IsTrue(entityPath.IsRootContainer);
            Assert.AreEqual("", entityPath.Id);
            Assert.AreEqual("Messages", entityPath.Path);
            Assert.AreEqual("Messages", entityPath.RootContainer);
            Assert.IsNull(entityPath.SubEntity);
        }

        /// <summary>
        /// Test contact entity path.
        /// </summary>
        [TestMethod]
        public void Test_ContactEntityPath()
        {
            Contact contact = new Contact()
            {
                Id = "cId"
            };

            EntityPath entityPath = new EntityPath(contact);
            Assert.IsFalse(entityPath.IsRootContainer);
            Assert.AreEqual("Contacts", entityPath.RootContainer);
            Assert.AreEqual("Contacts/cId", entityPath.Path);
            Assert.AreEqual("cId", entityPath.Id);
            Assert.IsNull(entityPath.SubEntity);
        }

        /// <summary>
        /// Test message rule entity path.
        /// </summary>
        [TestMethod]
        public void Test_MessageRuleEntityPath()
        {
            MessageRule rule = new MessageRule()
            {
                Id = "abc"
            };

            EntityPath entityPath = new EntityPath(rule);
            Assert.IsFalse(entityPath.IsRootContainer);
            Assert.AreEqual("MailFolders/Inbox/MessageRules", entityPath.RootContainer);
            Assert.AreEqual("abc", entityPath.Id);
            Assert.AreEqual("MailFolders/Inbox/MessageRules/abc", entityPath.Path);
            Assert.IsNull(entityPath.SubEntity);
        }
    }
}
