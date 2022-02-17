namespace EKOLidl.Data.Models
{
    using System;

    using EKOLidl.Data.Common.Models;
    using Newtonsoft.Json;

    public class Event : BaseModel
    {
        /// <summary>
        /// Gets or sets title of the event.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets short description of the event.
        /// </summary>
        [JsonProperty(PropertyName = "short_description")]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets long description of the event.
        /// </summary>
        [JsonProperty(PropertyName = "long_description")]
        public string LongDescription { get; set; }

        /// <summary>
        /// Gets or sets event starting date.
        /// </summary>
        [JsonProperty(PropertyName ="event_date")]
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets duration of the event in hours.
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets album of the event's pictures.
        /// </summary>
        [JsonProperty(PropertyName = "album")]
        public Album Album { get; set; }

        /// <summary>
        /// Gets or sets result of the event.
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public Result Result { get; set; }
    }
}
