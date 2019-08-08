namespace Microsoft.Graph.ManagedAPI.Tests.CoreJson
{
    using System.Collections.Generic;
    using System.Text;
    using Graph.ChangeTracking;
    using Microsoft.Graph.CoreJson;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConverterTests
    {
        /// <summary>
        /// Converter.
        /// </summary>
        private static Converter converter = new Converter();

        /// <summary>
        /// Test IList deserialization.
        /// </summary>
        [TestMethod]
        public void Test_IListDeserialization()
        {
            string recipientsJson = @"
                {
                  ""value"" : [
                    {
                      ""emailAddress"": {
                        ""address"": ""a@a.com"",
                        ""name"": ""A A""
                      }
                    },
                    {
                      ""emailAddress"": {
                        ""address"": ""b@b.com"",
                        ""name"": ""B B""
                      }
                    }
                  ]
                }";

            IList<Recipient> recipients = this.Convert<IList<Recipient>>(recipientsJson);

            Assert.AreEqual(
                "a@a.com",
                recipients[0].EmailAddress.Address);
            Assert.AreEqual(
                "A A",
                recipients[0].EmailAddress.Name);

            Assert.AreEqual(
                "b@b.com",
                recipients[1].EmailAddress.Address);
            Assert.AreEqual(
                "B B",
                recipients[1].EmailAddress.Name);
        }

        /// <summary>
        /// Test IList deserialization to entity.
        /// </summary>
        [TestMethod]
        public void Test_IListDeserializationToEntity()
        {
            string toRecipientsJson = @"
                {
                  ""id"": ""abcd"",
                  ""toRecipients"" : [
                    {
                      ""emailAddress"": {
                        ""address"": ""a@a.com"",
                        ""name"": ""A A""
                      }
                    },
                    {
                      ""emailAddress"": {
                        ""address"": ""b@b.com"",
                        ""name"": ""B B""
                      }
                    }
                  ]
                }";

            Message message = this.Convert<Message>(toRecipientsJson);
            Assert.IsNotNull(message);

            Assert.AreEqual(
                "a@a.com",
                message.ToRecipients[0].EmailAddress.Address);
            Assert.AreEqual(
                "A A",
                message.ToRecipients[0].EmailAddress.Name);

            Assert.AreEqual(
                "b@b.com",
                message.ToRecipients[1].EmailAddress.Address);
            Assert.AreEqual(
                "B B",
                message.ToRecipients[1].EmailAddress.Name);
        }

        /// <summary>
        /// Test message with attachment deserialization.
        /// </summary>
        [TestMethod]
        public void Test_MessageWithAttachmentDeserialization()
        {
            string serializedMessage = @"
                {
                      ""@odata.type"": ""#Microsoft.Graph.Message"",
                      ""id"": ""rootMessageId"",
                      ""subject"": ""message with attachs"",
                      ""isRead"": ""true"",
                      ""attachments"": [
                        {
                          ""@odata.type"": ""#Microsoft.Graph.ReferenceAttachment"",
                          ""id"": ""referenceAttachment=="",
                          ""sourceUrl"": ""https://myweb.com""
                        },
                        {
                          ""contentBytes"": ""dGVzdCBjYXNlIHdvcmtz"",
                          ""@odata.type"": ""#Microsoft.Graph.FileAttachment"",
                          ""contentType"": ""ct"",
                          ""size"": 0,
                          ""isInline"": false,
                          ""id"": ""fileAttachment=="",
                          ""name"": ""test.txt""
                        },
                        {
                          ""@odata.type"": ""#Microsoft.Graph.ItemAttachment"",
                          ""id"": ""itemAttachment=="",
                          ""item"":{
                            ""id"": ""attachmentitemId"",
                            ""changeKey"": ""ck=="",
                            ""subject"": ""attachment item subject"",
                            ""isRead"": ""true"",
                            ""@odata.type"": ""#Microsoft.Graph.Message""
                          }
                        }
                    ]
                 }";

            Message msg = this.Convert<Message>(serializedMessage);
            Assert.AreEqual(
                3,
                msg.Attachments.Count);

            foreach (Attachment attachment in msg.Attachments)
            {
                if (attachment is ReferenceAttachment refAttach)
                {
                    Assert.AreEqual(
                        "https://myweb.com",
                        refAttach.SourceUrl);

                    Assert.AreEqual(
                        "referenceAttachment==",
                        refAttach.Id);
                }
                else if (attachment is FileAttachment fileAttach)
                {
                    Assert.AreEqual(
                        "test.txt",
                        fileAttach.Name);

                    byte[] contentBytes = System.Convert.FromBase64String(fileAttach.ContentBytes);
                    string content = Encoding.UTF8.GetString(contentBytes);
                    Assert.AreEqual(
                        "test case works",
                        content);
                }
                else if (attachment is ItemAttachment itemAttach)
                {
                    Message attachMsg = (Message)itemAttach.Item;
                    Assert.AreEqual(
                        "attachmentitemId",
                        attachMsg.Id);

                    Assert.IsTrue(attachMsg.IsRead);
                    Assert.AreEqual(
                        "ck==",
                        attachMsg.ChangeKey);
                }
                else
                {
                    Assert.Fail("We shouldn't be here...");
                }
            }
        }

        /// <summary>
        /// Test message with event attachment deserialization.
        /// </summary>
        [TestMethod]
        public void Test_MessageWithEventAttachmentDeserialization()
        {
            string serializedMessage = @"{
              ""@odata.type"": ""#Microsoft.Graph.Message"",
              ""id"": ""msgId"",
              ""subject"": ""message with attachs"",
              ""isRead"": ""true"",
              ""attachments"": [
                {
                  ""@odata.type"": ""#Microsoft.Graph.ItemAttachment"",
                  ""id"": ""itemAttachment=="",
                  ""item"":{
                    ""id"": ""attachmentitemId"",
                    ""changeKey"": ""ck=="",
                    ""subject"": ""attachment item subject"",
                    ""@odata.type"": ""#Microsoft.Graph.Event"",
                    ""attendees"": [
                      {
                        ""emailAddress"":{
                          ""address"": ""att1@a.com""
                        }
                      },
                      {
                        ""emailAddress"": {
                          ""address"": ""att2@a.com""
                        }
                      }
                    ],
                    ""body"": {
                      ""Content"": ""this is event"",
                      ""ContentType"": ""Html""
                    },
                    ""start"":{
                      ""dateTime"": ""2019-01-01T12:00:00"",
                      ""TimeZone"": ""Pacific Standard Time""
                    },
                    ""end"": {
                      ""dateTime"": ""2019-01-01T04:00:00"",
                      ""TimeZone"": ""Pacific Standard Time""
                    }
                  }
                }
              ]
            }";

            Message msg = this.Convert<Message>(serializedMessage);

            Assert.AreEqual(
                1,
                msg.Attachments.Count);
            ItemAttachment messageAttach = (ItemAttachment)msg.Attachments[0];
            Event messageAttachEvent = (Event)messageAttach.Item;
            Assert.AreEqual(
                2,
                messageAttachEvent.Attendees.Count);

            Assert.IsNotNull(messageAttachEvent.Start.DateTime);
            Assert.IsNotNull(messageAttachEvent.End.DateTime);
        }

        /// <summary>
        /// Test only changed properties serialized.
        /// </summary>
        [TestMethod]
        public void Test_OnlyChangedPropertiesAreSerialized()
        {
            Message msg = new Message();
            msg.Subject = "Hey!";

            string serializedMsg = this.Convert(msg);
            Assert.AreEqual(
                this.GetJsonMessageWithRootObject(),
                serializedMsg);

            serializedMsg = this.Convert(msg, null, false);
            Assert.AreEqual(
                this.GetJsonMessageWithoutRootObject(),
                serializedMsg);
        }

        /// <summary>
        /// Test only changed properties and additional properties are serialized.
        /// </summary>
        [TestMethod]
        public void Test_OnlyChangedPropertiesAndAdditionalPropertiesAreSerialized()
        {
            Message msg = new Message();
            msg.Subject = "Hey!";
            msg.ToRecipients.Add(new Recipient()
            {
                EmailAddress = new EmailAddress()
                {
                    Address = "a@a.com"
                }
            });

            Dictionary<string, object> additionalProperties = new Dictionary<string, object>();
            additionalProperties.Add("Comment", "this is comment");
            additionalProperties.Add("ItemsCount", 10);

            string serializedObject = this.Convert(msg, additionalProperties);
            Assert.AreEqual(
                this.GetJsonMessageWithAdditionalPropertiesWithRootObject(),
                serializedObject);

            serializedObject = this.Convert(msg, additionalProperties, false);
            Assert.AreEqual(
                this.GetJsonMessageWithAdditionalPropertiesWithoutRootObject(),
                serializedObject);
        }

        /// <summary>
        /// Test MessageRule deserialization.
        /// </summary>
        [TestMethod]
        public void Test_MessageRuleDeserialization()
        {
            string messageRuleDefinition = @"
            {
              ""id"": ""AQAAAJBPvO0="",
              ""displayName"": ""09457c9f-1393-470d-aeb5-8075be93f8f4"",
              ""sequence"": 1,
              ""isEnabled"": true,
              ""hasError"": false,
              ""isReadOnly"": false,
              ""conditions"": {
                ""senderContains"": [
                  ""TESTUSER""
                ]
              },
              ""actions"": {
                ""stopProcessingRules"": true,
                ""forwardTo"": [
                  {
                    ""emailAddress"": {
                      ""name"": ""test@domain.com"",
                      ""address"": ""test@domain.com""
                    }
                  }
                ]
              }
            }";

            MessageRule messageRule = this.Convert<MessageRule>(messageRuleDefinition);
        }

        /// <summary>
        /// Test message rule actions deserialization.
        /// </summary>
        [TestMethod]
        public void Test_MessageRuleActionsDeserialization()
        {
            string messageRuleActionsDefinition = 
                @"{
                    ""stopProcessingRules"": true,
                    ""forwardTo"": [
                      {
                        ""emailAddress"": {
                          ""name"": ""test@domain.com"",
                          ""address"": ""test@domain.com""
                        }
                      }
                    ]
                }";

            MessageRuleActions actions = this.Convert<MessageRuleActions>(messageRuleActionsDefinition);
        }

        /// <summary>
        /// Convert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        private T Convert<T>(string content)
        {
            return ConverterTests.converter.Convert<T>(content);
        }

        /// <summary>
        /// Convert change trackable object to string.
        /// </summary>
        /// <param name="changeTracker"></param>
        /// <param name="additionalProperties"></param>
        /// <param name="appendRootObject"></param>
        /// <returns></returns>
        private string Convert(IPropertyChangeTracking changeTracker, Dictionary<string, object> additionalProperties = null, bool appendRootObject = true)
        {
            return ConverterTests.converter.Convert(changeTracker, additionalProperties, appendRootObject);
        }

        /// <summary>
        /// Get json message without root object.
        /// </summary>
        /// <returns></returns>
        private string GetJsonMessageWithoutRootObject()
        {
            return @"{""Subject"":""Hey!""}";
        }

        /// <summary>
        /// Get json message with root object.
        /// </summary>
        /// <returns></returns>
        private string GetJsonMessageWithRootObject()
        {
            return @"{""Message"":{""Subject"":""Hey!""}}";
        }

        /// <summary>
        /// Get json message with additional properties with root object.
        /// </summary>
        /// <returns></returns>
        private string GetJsonMessageWithAdditionalPropertiesWithRootObject()
        {
            return
                @"{""Message"":{""Subject"":""Hey!"",""ToRecipients"":[{""emailAddress"":{""address"":""a@a.com""}}]},""Comment"":""this is comment"",""ItemsCount"":10}";
        }

        /// <summary>
        /// Get json message with additional properties without root object.
        /// </summary>
        /// <returns></returns>
        private string GetJsonMessageWithAdditionalPropertiesWithoutRootObject()
        {
            return "{\"Subject\":\"Hey!\",\"ToRecipients\":[{\"emailAddress\":{\"address\":\"a@a.com\"}}],\"Comment\":\"this is comment\",\"ItemsCount\":10}";
        }
    }
}
