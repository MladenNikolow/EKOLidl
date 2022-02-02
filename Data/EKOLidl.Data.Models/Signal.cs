namespace EKOLidl.Data.Models
{
    using System;

    using EKOLidl.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Signal
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public Album Album { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public TrashType TrashType { get; set; }  

        public string OtherTrashType { get; set; }

        public DateTime DateCreate { get; set; }
    }
}
