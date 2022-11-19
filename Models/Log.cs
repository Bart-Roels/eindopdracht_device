using System;
using Newtonsoft.Json;

namespace eindopdracht_device.Models
{
    public class Log
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "time")]
        public TimeSpan Time { get; set; }

        [JsonProperty(PropertyName = "distance")]
        public double Distance { get; set; }

        [JsonProperty(PropertyName = "speed")]
        public double Speed { get; set; }

        [JsonProperty(PropertyName = "balloon")]
        public string HotAirBalloon { get; set; }

        [JsonProperty(PropertyName = "takeOffLocation")]
        public string TakeOffLocation { get; set; }

        [JsonProperty(PropertyName = "landingLocation")]
        public string LandingLocation { get; set; }

        
                      
    }
}
