using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http;


using Microsoft.Bot.Connector;
using HomeAdvisorTestBot.Interactions;

namespace HomeAdvisorTestBot.Controllers {



    [BotAuthentication]
    public class MessagesController : ApiController {

    //    private readonly ChatBot _chatBot;

        public MessagesController() {
         //   _chatBot = chatBot;
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity message) {
            ChatBot chat = new ChatBot();
            if (message.Type == ActivityTypes.Message) {
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));

                Activity replyToConversation = await chat.HandleChat(connector, message);

               // await connector.Conversations.SendToConversationAsync(replyToConversation);
            }
            else {
                HandleSystemMessage(message);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

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

        private Activity GetGreetingReply(Activity messageFromUser) {
            Activity toReturn = messageFromUser.CreateReply("Hello there!");
          //  SetGreetingSent(messageFromUser);
            return toReturn;
        }

        public async Task<bool> PutStuff(Activity message) {
            StateClient stateClient = message.GetStateClient();
            return true;
        }

        //public async Task<string> DestroyStuff(Activity message) {
        //    return "";
        //    // return true;
        //}

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