namespace EKOLidl.Data.Models
{
    using EKOLidl.Data.Common.Models;
    using EKOLidl.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Signal : BaseModel
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "album")]
        public Album Album { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "trash_types")]
        public List<TrashType> TrashType { get; set; }

        [JsonProperty(PropertyName = "other_trash_type")]
        public string OtherTrashType { get; set; }

        [JsonProperty(PropertyName = "result")]
        public Result Result { get; set; }
    }
}
