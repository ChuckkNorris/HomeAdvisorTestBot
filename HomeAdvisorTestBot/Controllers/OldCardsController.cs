using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;

using System.Collections.Generic;



namespace HomeAdvisorTestBot.Controllers {
   [BotAuthentication]
    public class MessagessController : ApiController {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity message) {
            if (message.Type == ActivityTypes.Message) {
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                // calculate something for us to return
                int length = (message.Text ?? string.Empty).Length;

                Activity replyToConversation = CreateCard(message);

                await connector.Conversations.SendToConversationAsync(replyToConversation);
            }
            else {
                HandleSystemMessage(message);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        //internal static IDialog<SandwichOrder> MakeRootDialog() {
        //    return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildForm));
        //}
        //[ResponseType(typeof(void))]
        //public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity) {
        //    if (activity != null) {
        //        // one of these will have an interface and process it
        //        switch (activity.GetActivityType()) {
        //            case ActivityTypes.Message:
        //                await Conversation.SendAsync(activity, MakeRootDialog);
        //                break;
        //            case ActivityTypes.ConversationUpdate:
        //            case ActivityTypes.ContactRelationUpdate:
        //            case ActivityTypes.Typing:
        //            case ActivityTypes.DeleteUserData:
        //            default:
        //                Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
        //                break;
        //        }
        //    }
        //    return activity.CreateReply();
        //}

        private Activity CreateCard(Activity message) {
            Activity replyToConversation = message.CreateReply("Should go to conversation, with a thumbnail card");
            replyToConversation.Recipient = message.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: "https://upload.wikimedia.org/wikipedia/commons/f/ff/Aminah_Cendrakasih%2C_c._1959%2C_by_Tati_Photo_Studio.jpg"));
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction() {
                Value = "https://en.wikipedia.org/wiki/Pig_Latin",
                Type = "openUrl",
                Title = "WikiPedia Page"
            };
            cardButtons.Add(plButton);
            ThumbnailCard plCard = new ThumbnailCard() {
                Title = "I'm a thumbnail card",
                Subtitle = "Pig Latin Wikipedia Page",
                Images = cardImages,
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            //   plAttachment.ContentType = "application/vnd.microsoft.card.heros";

            replyToConversation.Attachments.Add(plAttachment);
            return replyToConversation;
        }

        private Activity CreateReceipt(Activity message) {
            Activity replyToConversation = message.CreateReply("Receipt card");
            replyToConversation.Recipient = message.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: "https://<ImageUrl1>"));
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction() {
                Value = "https://en.wikipedia.org/wiki/Pig_Latin",
                Type = "openUrl",
                Title = "WikiPedia Page"
            };
            cardButtons.Add(plButton);
            ReceiptItem lineItem1 = new ReceiptItem() {
                Title = "Pork Shoulder",
                Subtitle = "8 lbs",
                Text = null,
                Image = new CardImage(url: "https://<ImageUrl1>"),
                Price = "16.25",
                Quantity = "1",
                Tap = null
            };
            ReceiptItem lineItem2 = new ReceiptItem() {
                Title = "Bacon",
                Subtitle = "5 lbs",
                Text = null,
                Image = new CardImage(url: "https://<ImageUrl2>"),
                Price = "34.50",
                Quantity = "2",
                Tap = null
            };
            List<ReceiptItem> receiptList = new List<ReceiptItem>();
            receiptList.Add(lineItem1);
            receiptList.Add(lineItem2);
            ReceiptCard plCard = new ReceiptCard() {
                Title = "I'm a receipt card, isn't this bacon expensive?",
                Buttons = cardButtons,
                Items = receiptList,
                Total = "275.25",
                Tax = "27.52"
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            return replyToConversation;
           

        }

        private Activity HandleSystemMessage(Activity message) {
            if (message.Type == ActivityTypes.DeleteUserData) {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate) {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate) {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing) {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping) {
            }

            return null;
        }
    }
}