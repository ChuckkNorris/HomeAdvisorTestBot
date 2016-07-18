using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeAdvisorTestBot.Models {
    public class Professional {
        public int Id { get; set; }
        public string FullName { get; set; }
        public decimal Rating { get; set; }
        public string ImageUrl { get; set; }
        
        public List<string> AvailableTimes { get; set; }

        public Attachment ToAttachment() {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: this.ImageUrl));
            List<CardAction> cardButtons = new List<CardAction>();
            foreach (var time in AvailableTimes) {
                CardAction plButton = new CardAction() {
                    Value = $"{FullName} at {time}",
                    Type = "imBack",
                    Title = time
                };
                cardButtons.Add(plButton);
            }
            HeroCard plCard = new HeroCard() {
                Title = this.FullName,
                Subtitle = $"Rating: {this.Rating.ToString("#.##")}",
                Images = cardImages,
                Buttons = cardButtons
            };
            return plCard.ToAttachment();
        }

        public Attachment ToAppointmentDetailsAttachment(ServiceRequest request, string selectedTime) {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url: this.ImageUrl));
           
            HeroCard plCard = new HeroCard() {
                Title = this.FullName,
                Subtitle = $"{request.Service}",
                Images = cardImages
            };
            return plCard.ToAttachment();
        }

        public static List<Professional> GetProfessionalMockData() {
            var professionals = new List<Professional>();
            professionals.Add(
                new Professional() {
                    Id = 1,
                    FullName = "Pinnacle Roofing Associates, LLC",
                    Rating = 4.79M,
                    ImageUrl = "https://imagescdn.staticp.com/api/image/display/v2/a227x227/a3fc4bb3-9675-4ea9-9e2b-335c8e1a7ada.jpg",
                    AvailableTimes = new List<string>() { "9:30 AM", "11:00 AM", "3:30 PM" }
                }
            );
            professionals.Add(
                new Professional() {
                    Id = 2,
                    FullName = "Mountain States Home Improvement, Inc.",
                    Rating = 5M,
                    ImageUrl = "https://lh3.googleusercontent.com/-6-Oie3WAV_I/UyrnaVGToSI/AAAAAAAAABs/Rqm17GFLv7k/s426/roofing-the-rockies.png",
                    AvailableTimes = new List<string>() { "10:00 AM", "12:00 PM", "1:00 PM" }
                }
            );
            professionals.Add(
                new Professional() {
                    Id = 2,
                    FullName = "Lakota Roofing & Construction, Inc.",
                    Rating = 4.05M,
                    ImageUrl = "http://lakotaconstructioninc.com/wp-content/uploads/2016/03/window-installation-333x200.jpg",
                    AvailableTimes = new List<string>() { "4:00 PM" }
                }
            );
            return professionals;
        }

    }
}