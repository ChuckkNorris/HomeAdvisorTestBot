using HomeAdvisorTestBot.NaturalLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeAdvisorTestBot.Models {
    public class ServiceRequest {
        public ServiceRequest(HomeAdvisorLuis luisModel) {
            foreach (var entity in luisModel.entities) {
                switch (entity.type) {
                    case "RequestedService":
                        this.Service = entity.entity;
                        break;
                    case "Target":
                        this.Target = entity.entity;
                        break;
                    case "RequestedDay":
                        this.Day = entity.entity;
                        break;
                    case "RequestedTime":
                        this.Time = entity.entity;
                        break;
                    case "RequestedTimeOfDay":
                        this.TimeOfDay = entity.entity;
                        break;
                    default:
                        break;
                }
            }
        }

        public string Service { get; set; }
        public string Target { get; set; }
        public string Time { get; set; }
        public string TimeOfDay { get; set; }
        public string Day { get; set; }
    }
}