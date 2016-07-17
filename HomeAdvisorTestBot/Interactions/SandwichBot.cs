//using Microsoft.Bot.Builder.FormFlow;
//using Microsoft.Bot.Builder.FormFlow.Advanced;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Web;

//namespace HomeAdvisorTestBot.Interactions {
//    public enum SandwichOptions {
//        [Describe(Image = @"https://placeholdit.imgix.net/~text?txtsize=12&txt=BLT&w=50&h=40&txttrack=0&txtclr=000&txtfont=bold")]
//        BLT, BlackForestHam, BuffaloChicken, ChickenAndBaconRanchMelt, ColdCutCombo, MeatballMarinara,
//        OvenRoastedChicken, RoastBeef,
//        [Terms(@"rotis\w* style chicken", MaxPhrase = 3)]
//        RotisserieStyleChicken, SpicyItalian, SteakAndCheese, SweetOnionTeriyaki, Tuna,
//        TurkeyBreast, Veggie
//    };
//    public enum LengthOptions { SixInch, FootLong };
//    public enum BreadOptions { NineGrainWheat, NineGrainHoneyOat, Italian, ItalianHerbsAndCheese, Flatbread };
//    public enum CheeseOptions { American, MontereyCheddar, Pepperjack };
//    public enum ToppingOptions {
//        // This starts at 1 because 0 is the "no value" value
//        [Terms("except", "but", "not", "no", "all", "everything")]
//        Everything = 1,
//        Avocado, BananaPeppers, Cucumbers, GreenBellPeppers, Jalapenos,
//        Lettuce, Olives, Pickles, RedOnion, Spinach, Tomatoes
//    };
//    public enum SauceOptions {
//        ChipotleSouthwest, HoneyMustard, LightMayonnaise, RegularMayonnaise,
//        Mustard, Oil, Pepper, Ranch, SweetOnion, Vinegar
//    };

//    [Serializable]
//    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")]
//    [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}")]
//    // [Template(TemplateUsage.EnumSelectOne, "What kind of {&} would you like on your sandwich? {||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
//    public class SandwichOrder {
//        [Prompt("What kind of {&} would you like? {||}")]
//        // [Prompt("What kind of {&} would you like? {||}", ChoiceFormat ="{1}")]
//        // [Prompt("What kind of {&} would you like?")]
//        public SandwichOptions? Sandwich;

//        [Prompt("What size of sandwich do you want? {||}")]
//        public LengthOptions? Length;

//        public BreadOptions? Bread;

//        // An optional annotation means that it is possible to not make a choice in the field.
//        [Optional]
//        public CheeseOptions? Cheese;

//        [Optional]
//        public List<ToppingOptions> Toppings { get; set; }

//        [Optional]
//        public List<SauceOptions> Sauces;

//        [Optional]
//        [Template(TemplateUsage.NoPreference, "None")]
//        public string Specials;

//        public string DeliveryAddress;

//        [Pattern("(\\(\\d{3}\\))?\\s*\\d{3}(-|\\s*)\\d{4}")]
//        public string PhoneNumber;

//        [Optional]
//        public DateTime? DeliveryTime;

//        [Numeric(1, 5)]
//        [Optional]
//        [Describe("your experience today")]
//        public double? Rating;

//        public static IForm<SandwichOrder> BuildForm() {
//            OnCompletionAsyncDelegate<SandwichOrder> processOrder = async (context, state) => {
//                await context.PostAsync("We are currently processing your sandwich. We will message you the status.");
//            };

//            return new FormBuilder<SandwichOrder>()
//                        .Message("Welcome to the sandwich order bot!")
//                        .Field(nameof(Sandwich))
//                        .Field(nameof(Length))
//                        .Field(nameof(Bread))
//                        .Field(nameof(Cheese))
//                        .Field(nameof(Toppings),
//                            validate: async (state, value) => {
//                                var values = ((List<object>)value).OfType<ToppingOptions>();
//                                var result = new ValidateResult { IsValid = true, Value = values };
//                                if (values != null && values.Contains(ToppingOptions.Everything)) {
//                                    result.Value = (from ToppingOptions topping in Enum.GetValues(typeof(ToppingOptions))
//                                                    where topping != ToppingOptions.Everything && !values.Contains(topping)
//                                                    select topping).ToList();
//                                }
//                                return result;
//                            })
//                        .Message("For sandwich toppings you have selected {Toppings}.")
//                        .Field(nameof(SandwichOrder.Sauces))
//                        .Field(new FieldReflector<SandwichOrder>(nameof(Specials))
//                            .SetType(null)
//                            .SetActive((state) => state.Length == LengthOptions.FootLong)
//                            .SetDefine(async (state, field) => {
//                                field
//                                    .AddDescription("cookie", "Free cookie")
//                                    .AddTerms("cookie", "cookie", "free cookie")
//                                    .AddDescription("drink", "Free large drink")
//                                    .AddTerms("drink", "drink", "free drink");
//                                return true;
//                            }))
//                        .Confirm(async (state) => {
//                            var cost = 0.0;
//                            switch (state.Length) {
//                                case LengthOptions.SixInch: cost = 5.0; break;
//                                case LengthOptions.FootLong: cost = 6.50; break;
//                            }
//                            return new PromptAttribute($"Total for your sandwich is {cost:C2} is that ok?");
//                        })
//                        .Field(nameof(SandwichOrder.DeliveryAddress),
//                            validate: async (state, response) => {
//                                var result = new ValidateResult { IsValid = true, Value = response };
//                                var address = (response as string).Trim();
//                                if (address.Length > 0 && (address[0] < '0' || address[0] > '9')) {
//                                    result.Feedback = "Address must start with a number.";
//                                    result.IsValid = false;
//                                }
//                                return result;
//                            })
//                        .Field(nameof(SandwichOrder.DeliveryTime), "What time do you want your sandwich delivered? {||}")
//                        .Confirm("Do you want to order your {Length} {Sandwich} on {Bread} {&Bread} with {[{Cheese} {Toppings} {Sauces}]} to be sent to {DeliveryAddress} {?at {DeliveryTime:t}}?")
//                        .AddRemainingFields()
//                        .Message("Thanks for ordering a sandwich!")
//                        .OnCompletion(processOrder)
//                        .Build();
//        }

//        public static IForm<JObject> BuildJsonForm() {
//            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Bot.Sample.AnnotatedSandwichBot.AnnotatedSandwich.json")) {
//                var schema = JObject.Parse(new StreamReader(stream).ReadToEnd());
//                return new FormBuilderJson(schema)
//                    .AddRemainingFields()
//                    .Build();
//            }
//        }

//        public static IForm<JObject> BuildJsonFormExplicit() {
//            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Bot.Sample.AnnotatedSandwichBot.AnnotatedSandwich.json")) {
//                var schema = JObject.Parse(new StreamReader(stream).ReadToEnd());
//                OnCompletionAsyncDelegate<JObject> processOrder = async (context, state) => {
//                    await context.PostAsync(DynamicSandwich.Processing);
//                };
//                var builder = new FormBuilderJson(schema);
//                return builder
//                            .Message("Welcome to the sandwich order bot!")
//                            .Field("Sandwich")
//                            .Field("Length")
//                            .Field("Ingredients.Bread")
//                            .Field("Ingredients.Cheese")
//                            .Field("Ingredients.Toppings",
//                            validate: async (state, response) => {
//                                var value = (IList<object>)response;
//                                var result = new ValidateResult() { IsValid = true };
//                                if (value != null && value.Contains("Everything")) {
//                                    result.Value = (from topping in new string[] {
//                                    "Avocado", "BananaPeppers", "Cucumbers", "GreenBellPeppers",
//                                    "Jalapenos", "Lettuce", "Olives", "Pickles",
//                                    "RedOnion", "Spinach", "Tomatoes"}
//                                                    where !value.Contains(topping)
//                                                    select topping).ToList();
//                                }
//                                else {
//                                    result.Value = value;
//                                }
//                                return result;
//                            }
//                            )
//                            .Message("For sandwich toppings you have selected {Ingredients.Toppings}.")
//                            .Field("Ingredients.Sauces")
//                            .Field(new FieldJson(builder, "Specials")
//                                .SetType(null)
//                                .SetActive((state) => (string)state["Length"] == "FootLong")
//                                .SetDefine(async (state, field) => {
//                                    field
//                                    .AddDescription("cookie", DynamicSandwich.FreeCookie)
//                                    .AddTerms("cookie", Language.GenerateTerms(DynamicSandwich.FreeCookie, 2))
//                                    .AddDescription("drink", DynamicSandwich.FreeDrink)
//                                    .AddTerms("drink", Language.GenerateTerms(DynamicSandwich.FreeDrink, 2));
//                                    return true;
//                                }))
//                            .Confirm(async (state) => {
//                                var cost = 0.0;
//                                switch ((string)state["Length"]) {
//                                    case "SixInch": cost = 5.0; break;
//                                    case "FootLong": cost = 6.50; break;
//                                }
//                                return new PromptAttribute(string.Format(DynamicSandwich.Cost, cost));
//                            })
//                            .Field("DeliveryAddress",
//                                validate: async (state, value) => {
//                                    var result = new ValidateResult { IsValid = true, Value = value };
//                                    var address = (value as string).Trim();
//                                    if (address.Length > 0 && (address[0] < '0' || address[0] > '9')) {
//                                        result.Feedback = DynamicSandwich.BadAddress;
//                                        result.IsValid = false;
//                                    }
//                                    return result;
//                                })
//                            .Field("DeliveryTime", "What time do you want your sandwich delivered? {||}")
//                            .Confirm("Do you want to order your {Length} {Sandwich} on {Ingredients.Bread} {&Ingredients.Bread} with {[{Ingredients.Cheese} {Ingredients.Toppings} {Ingredients.Sauces}]} to be sent to {DeliveryAddress} {?at {DeliveryTime:t}}?")
//                            .AddRemainingFields()
//                            .Message("Thanks for ordering a sandwich!")
//                            .OnCompletion(processOrder)
//                    .Build();
//            }
//        }

//        // Cache of culture specific forms. 
//        private static ConcurrentDictionary<CultureInfo, IForm<SandwichOrder>> _forms = new ConcurrentDictionary<CultureInfo, IForm<SandwichOrder>>();

//        public static IForm<SandwichOrder> BuildLocalizedForm() {
//            var culture = Thread.CurrentThread.CurrentUICulture;
//            IForm<SandwichOrder> form;
//            if (!_forms.TryGetValue(culture, out form)) {
//                OnCompletionAsyncDelegate<SandwichOrder> processOrder = async (context, state) =>
//                {
//                    await context.PostAsync(DynamicSandwich.Processing);
//                };
//                // Form builder uses the thread culture to automatically switch framework strings
//                // and also your static strings as well.  Dynamically defined fields must do their own localization.
//                form = new FormBuilder<SandwichOrder>()
//                        .Message("Welcome to the sandwich order bot!")
//                        .Field(nameof(Sandwich))
//                        .Field(nameof(Length))
//                        .Field(nameof(Bread))
//                        .Field(nameof(Cheese))
//                        .Field(nameof(Toppings),
//                            validate: async (state, value) => {
//                                var values = ((List<object>)value).OfType<ToppingOptions>();
//                                var result = new ValidateResult { IsValid = true, Value = values };
//                                if (values != null && values.Contains(ToppingOptions.Everything)) {
//                                    result.Value = (from ToppingOptions topping in Enum.GetValues(typeof(ToppingOptions))
//                                                    where topping != ToppingOptions.Everything && !values.Contains(topping)
//                                                    select topping).ToList();
//                                }
//                                return result;
//                            })
//                        .Message("For sandwich toppings you have selected {Toppings}.")
//                        .Field(nameof(SandwichOrder.Sauces))
//                        .Field(new FieldReflector<SandwichOrder>(nameof(Specials))
//                            .SetType(null)
//                            .SetActive((state) => state.Length == LengthOptions.FootLong)
//                            .SetDefine(async (state, field) =>
//                            {
//                                field
//                                    .AddDescription("cookie", DynamicSandwich.FreeCookie)
//                                    .AddTerms("cookie", Language.GenerateTerms(DynamicSandwich.FreeCookie, 2))
//                                    .AddDescription("drink", DynamicSandwich.FreeDrink)
//                                    .AddTerms("drink", Language.GenerateTerms(DynamicSandwich.FreeDrink, 2));
//                                return true;
//                            }))
//                        .Confirm(async (state) =>
//                        {
//                            var cost = 0.0;
//                            switch (state.Length) {
//                                case LengthOptions.SixInch: cost = 5.0; break;
//                                case LengthOptions.FootLong: cost = 6.50; break;
//                            }
//                            return new PromptAttribute(string.Format(DynamicSandwich.Cost, cost));
//                        })
//                        .Field(nameof(SandwichOrder.DeliveryAddress),
//                            validate: async (state, response) => {
//                                var result = new ValidateResult { IsValid = true, Value = response };
//                                var address = (response as string).Trim();
//                                if (address.Length > 0 && address[0] < '0' || address[0] > '9') {
//                                    result.Feedback = DynamicSandwich.BadAddress;
//                                    result.IsValid = false;
//                                }
//                                return result;
//                            })
//                        .Field(nameof(SandwichOrder.DeliveryTime), "What time do you want your sandwich delivered? {||}")
//                        .Confirm("Do you want to order your {Length} {Sandwich} on {Bread} {&Bread} with {[{Cheese} {Toppings} {Sauces}]} to be sent to {DeliveryAddress} {?at {DeliveryTime:t}}?")
//                        .AddRemainingFields()
//                        .Message("Thanks for ordering a sandwich!")
//                        .OnCompletion(processOrder)
//                        .Build();
//                _forms[culture] = form;
//            }
//            return form;
//        }
//    };
//}




/// NEW CONTROLLER
//using System;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Web.Http;


//using Microsoft.Bot.Connector;



//namespace HomeAdvisorTestBot.Controllers {
//    [BotAuthentication]
//    public class MessagessController : ApiController {
//        public async Task<HttpResponseMessage> Post([FromBody]Activity message) {
//            if (message.Type == ActivityTypes.Message) {
//                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
//                // calculate something for us to return
//                int length = (message.Text ?? string.Empty).Length;

//                Activity replyToConversation = CreateCard(message);

//                await connector.Conversations.SendToConversationAsync(replyToConversation);
//            }
//            else {
//                HandleSystemMessage(message);
//            }
//            var response = Request.CreateResponse(HttpStatusCode.OK);
//            return response;
//        }

//        private Activity CreateCard(Activity message) {
//            Activity replyToConversation = message.CreateReply("Should go to conversation, with a thumbnail card");
//            replyToConversation.Recipient = message.From;
//            replyToConversation.Type = "message";
//            replyToConversation.Attachments = new List<Attachment>();
//            List<CardImage> cardImages = new List<CardImage>();
//            cardImages.Add(new CardImage(url: "https://upload.wikimedia.org/wikipedia/commons/f/ff/Aminah_Cendrakasih%2C_c._1959%2C_by_Tati_Photo_Studio.jpg"));
//            List<CardAction> cardButtons = new List<CardAction>();
//            CardAction plButton = new CardAction() {
//                Value = "https://en.wikipedia.org/wiki/Pig_Latin",
//                Type = "openUrl",
//                Title = "WikiPedia Page"
//            };
//            cardButtons.Add(plButton);
//            ThumbnailCard plCard = new ThumbnailCard() {
//                Title = "I'm a thumbnail card",
//                Subtitle = "Pig Latin Wikipedia Page",
//                Images = cardImages,
//                Buttons = cardButtons
//            };
//            Attachment plAttachment = plCard.ToAttachment();
//            //   plAttachment.ContentType = "application/vnd.microsoft.card.heros";

//            replyToConversation.Attachments.Add(plAttachment);
//            return replyToConversation;
//        }
//        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity) {

//        //    if (activity.Type == ActivityTypes.Message) {
//        //        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
//        //        // calculate something for us to return
//        //        int length = (activity.Text ?? string.Empty).Length;
//        //        // return our reply to the user
//        //        Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
//        //        await connector.Conversations.SendToConversationAsync(reply);
//        //    }
//        //    else {
//        //        HandleSystemMessage(activity);
//        //    }
//        //    var response = Request.CreateResponse(HttpStatusCode.OK);
//        //    return response;
//        //}

//        private Activity HandleSystemMessage(Activity message) {
//            if (message.Type == ActivityTypes.DeleteUserData) {
//                // Implement user deletion here
//                // If we handle user deletion, return a real message
//            }
//            else if (message.Type == ActivityTypes.ConversationUpdate) {
//                // Handle conversation state changes, like members being added and removed
//                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
//                // Not available in all channels
//            }
//            else if (message.Type == ActivityTypes.ContactRelationUpdate) {
//                // Handle add/remove from contact lists
//                // Activity.From + Activity.Action represent what happened
//            }
//            else if (message.Type == ActivityTypes.Typing) {
//                // Handle knowing tha the user is typing
//            }
//            else if (message.Type == ActivityTypes.Ping) {
//            }

//            return null;
//        }
//        //public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity) {
//        //    // check if activity is of type message
//        //    if (activity != null && activity.GetActivityType() == ActivityTypes.Message) {
//        //        await Conversation.SendAsync(activity, () => new EchoDialog());
//        //    }
//        //    //else {
//        //    //    HandleSystemMessage(activity);
//        //    //}
//        //    return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
//        //}
//        //public async Task<HttpResponseMessage> Post([FromBody]Activity message) {
//        //    if (message.Type == ActivityTypes.Message) {
//        //        ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
//        //        Activity replyToConversation;
//        //        if (!await DidSendGreeting(message)) {
//        //            replyToConversation = GetGreetingReply(message);
//        //        }
//        //        else {
//        //            replyToConversation = GetSecondReply(message);
//        //        }


//        //        await connector.Conversations.SendToConversationAsync(replyToConversation);
//        //    }
//        //    else {
//        //       // HandleSystemMessage(message);
//        //    }
//        //    var response = Request.CreateResponse(HttpStatusCode.OK);
//        //    return response;
//        //}


//        private Activity GetGreetingReply(Activity messageFromUser) {
//            Activity toReturn = messageFromUser.CreateReply("Hello there!");
//            SetGreetingSent(messageFromUser);
//            return toReturn;
//        }

//        private Activity GetSecondReply(Activity messageFromUser) {
//            Activity toReturn = messageFromUser.CreateReply("Glad to have you back!");

//            return toReturn;
//        }

//        public async Task<bool> DidSendGreeting(Activity message) {
//            StateClient stateClient = message.GetStateClient();
//            BotData userData = await stateClient.BotState.GetUserDataAsync(message.ChannelId, message.From.Id);
//            bool didSendGreeting = userData.GetProperty<bool?>("SentGreeting") ?? false;
//            return didSendGreeting;
//        }

//        public async void SetGreetingSent(Activity message) {
//            StateClient stateClient = message.GetStateClient();
//            BotData userData = await stateClient.BotState.GetUserDataAsync(message.ChannelId, message.From.Id);
//            userData.SetProperty<bool>("SentGreeting", true);
//            await stateClient.BotState.SetUserDataAsync(message.ChannelId, message.From.Id, userData);

//        }

//    }
//    //[Serializable]
//    //public class EchoDialog : IDialog<object> {
//    //    protected int count = 1;
//    //    public async Task StartAsync(IDialogContext context) {
//    //        context.Wait(MessageReceivedAsync);
//    //    }
//    //    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument) {
//    //        var message = await argument;
//    //        if (message.Text == "reset") {
//    //            PromptDialog.Confirm(
//    //                context,
//    //                AfterResetAsync,
//    //                "Are you sure you want to reset the count?",
//    //                "Didn't get that!",
//    //                promptStyle: PromptStyle.None);
//    //        }
//    //        else {
//    //            await context.PostAsync(string.Format("{0}: You said {1}", this.count++, message.Text));
//    //            context.Wait(MessageReceivedAsync);
//    //        }
//    //    }
//    //    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument) {
//    //        var confirm = await argument;
//    //        if (confirm) {
//    //            this.count = 1;
//    //            await context.PostAsync("Reset count.");
//    //        }
//    //        else {
//    //            await context.PostAsync("Did not reset count.");
//    //        }
//    //        context.Wait(MessageReceivedAsync);
//    //    }
//    //}
// }