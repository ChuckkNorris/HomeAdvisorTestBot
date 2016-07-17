using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace HomeAdvisorTestBot.NaturalLanguage {
    
    public class LuisClient {
        public static async Task<HomeAdvisorLuis> ParseUserInput(string input) {
            string escaped = Uri.EscapeDataString(input);
            using (var client = new HttpClient()) {
                string uri = "https://api.projectoxford.ai/luis/v1/application?id=dfef6a69-8742-4e29-81ef-ca3c002cd6a9&subscription-key=ea42161999e4452fa1c0d0598b264f5c&q=" + escaped;
                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode) {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<HomeAdvisorLuis>(jsonResponse);
                    return data;
                }
            }
            return null;
        }
    }
    

    public class HomeAdvisorLuis {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class Intent {
        public string intent { get; set; }
        public float score { get; set; }
        public Action[] actions { get; set; }
    }

    public class Action {
        public bool triggered { get; set; }
        public string name { get; set; }
        public Parameter[] parameters { get; set; }
    }

    public class Parameter {
        public string name { get; set; }
        public bool required { get; set; }
        public Value[] value { get; set; }
    }

    public class Value {
        public string entity { get; set; }
        public string type { get; set; }
        public float score { get; set; }
    }

    public class Entity {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }

}