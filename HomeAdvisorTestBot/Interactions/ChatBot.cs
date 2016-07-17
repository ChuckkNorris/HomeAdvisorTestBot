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
            return message.CreateReply(await GetReplyFromLuis(message.Text));
            if (!await DidSendGreeting(message)) {
                replyToConversation = GetGreetingReply(message);
            }
            else {
                replyToConversation = GetSecondReply(message);
            }
            return replyToConversation;
        }

        private async Task<string> GetReplyFromLuis(string messageText) {
            string toReturn = string.Empty;
            HomeAdvisorLuis requestDetails = await LuisClient.ParseUserInput(messageText);
            switch (requestDetails.intents[0].intent) {
                case "RequestService":
                    toReturn = $"I see you want to request a service to {GetRequestedService(requestDetails.entities)} your {GetRequestedTarget(requestDetails.entities)} - Is that correct?";
                    break;
            }
            return toReturn;

        }

        private string GetRequestedService(HomeAdvisorTestBot.NaturalLanguage.Entity[] entities) {
            return entities.FirstOrDefault(x => x.type == "RequestedService").entity;
        }

        private string GetRequestedTarget(HomeAdvisorTestBot.NaturalLanguage.Entity[] entities) {
            return entities.FirstOrDefault(x => x.type == "Target").entity;
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