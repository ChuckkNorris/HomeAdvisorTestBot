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

        public async Task<Activity> HandleChat(ConnectorClient connector, Activity message) {
            Activity replyToConversation;
            replyToConversation = message.CreateReply(await GetReplyFromLuis(message.Text));
            await connector.Conversations.SendToConversationAsync(replyToConversation);
            Activity proCards = AddRoofHeroCards(replyToConversation);
            await connector.Conversations.SendToConversationAsync(proCards);
            return replyToConversation;
        }

        private Activity AddRoofHeroCards(Activity replyToConversation) {
            Activity heroCards = replyToConversation.CreateReply();
            heroCards.Type = "message";
            heroCards.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            heroCards.Attachments = new List<Attachment>();
            List<Professional> pros = Professional.GetProfessionalMockData();
            foreach (var pro in pros) {
                Attachment professionalCard = pro.ToAttachment();
                heroCards.Attachments.Add(professionalCard);
            }
            return heroCards;
        }

        private Activity GetConfirmOrderCard(Activity replyToConversation) {
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            replyToConversation.Attachments = new List<Attachment>();
            List<Professional> pros = Professional.GetProfessionalMockData();
            var selectedPro = pros.FirstOrDefault(x => x.FullName == replyToConversation.Text?.Split('-')?[0]);
            Attachment selectedProCard = selectedPro.ToAttachment();
            replyToConversation.Attachments.Add(selectedProCard);
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
                if (!request.Service.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
                    request.Service += "ing";
                toReturn += request.Service;
                
                if (request.Target != null) {
                    toReturn += $" your {request.Target}";
                }

                if (request.Day != null) {
                    toReturn += $" on {request.Day}";
                }

                toReturn += "? Here are some pros that can help you:";
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