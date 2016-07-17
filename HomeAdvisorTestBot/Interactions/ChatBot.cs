using HomeAdvisorTestBot.Models;
using HomeAdvisorTestBot.NaturalLanguage;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HomeAdvisorTestBot.Interactions {
    public class ChatBot {

        public async Task<Activity> HandleChat(Activity message) {
            Activity replyToConversation;
            replyToConversation = message.CreateReply(await GetReplyFromLuis(message.Text));

            replyToConversation.Recipient = message.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();
            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            cardContentList.Add("PigLatin", "https://stanleyyelnats.wikispaces.com/file/view/jhan171l2.JPG/30641276/361x347/jhan171l2.JPG");
            cardContentList.Add("Pork Shoulder", "https://bigoven-res.cloudinary.com/image/upload/t_recipe-256/slow-roasted-glazed-pork-shoulder-1319864.jpg");
            cardContentList.Add("Bacon", "http://images.bigoven.com/image/upload/t_recipe-256/smoked-bacon-wrappped-meat-loaf.jpg");
            foreach (KeyValuePair<string, string> cardContent in cardContentList) {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: cardContent.Value));
                List<CardAction> cardButtons = new List<CardAction>();
                CardAction plButton = new CardAction() {
                    Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                    Type = "openUrl",
                    Title = "WikiPedia Page"
                };
                cardButtons.Add(plButton);
                HeroCard plCard = new HeroCard() {
                    Title = $"I'm a hero card about {cardContent.Key}",
                    Subtitle = $"{cardContent.Key} Wikipedia Page",
                    Images = cardImages,
                    Buttons = cardButtons
                };
                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;




            return replyToConversation;
        }

        private async Task<string> GetReplyFromLuis(string messageText) {
            string toReturn = string.Empty;
            HomeAdvisorLuis requestDetails = await LuisClient.ParseUserInput(messageText);
            var request = new ServiceRequest(requestDetails);
            toReturn = ConstructReplyFromServiceRequest(request);

            return toReturn;
        }


        private string ConstructReplyFromServiceRequest(ServiceRequest request) {
            string toReturn = "You need help with ";

            // TODO: Save states based on data retrieved (e.g. if service == null, but day exists, store the desired day and request the service, etc.)
            if (request.Service != null) {
                if (request.Service.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
                    request.Service += "ing";
                toReturn += request.Service;
                
                if (request.Target != null) {
                    toReturn += $" your {request.Target}";
                }

                if (request.Day != null) {
                    toReturn += $" on {request.Day}";
                }
            }
            else {
                toReturn = "Sorry, we couldn't find any related service. Try something like 'I need to remodel my kitchen' or 'I need my AC repaired'";
            }
            return toReturn;
        }

       


        private Activity GetGreetingReply(Activity messageFromUser) {
            Activity toReturn = messageFromUser.CreateReply("Hello there!");
            SetGreetingSent(messageFromUser);
            return toReturn;
        }

        private Activity GetSecondReply(Activity messageFromUser) {
            Activity toReturn = messageFromUser.CreateReply("Glad to have you back!");

            return toReturn;
        }

        private async Task<bool> DidSendGreeting(Activity message) {
            StateClient stateClient = message.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(message.ChannelId, message.From.Id);
            bool didSendGreeting = userData.GetProperty<bool?>("SentGreeting") ?? false;
            return didSendGreeting;
        }

        private async void SetGreetingSent(Activity message) {
            StateClient stateClient = message.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(message.ChannelId, message.From.Id);
            userData.SetProperty<bool>("SentGreeting", true);
            await stateClient.BotState.SetUserDataAsync(message.ChannelId, message.From.Id, userData);

        }
    }
}