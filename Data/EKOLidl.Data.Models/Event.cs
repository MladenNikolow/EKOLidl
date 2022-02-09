namespace EKOLidl.Data.Models
{
    using EKOLidl.Data.Common.Models;
    using Newtonsoft.Json;

    public class Event : BaseModel
    {
        [JsonProperty(PropertyName = "short_description")]
        public string ShortDescription { get; set; }

        [JsonProperty(PropertyName = "long_description")]
        public string LongDescription { get; set; }

        [JsonProperty(PropertyName = "album")]
        public Album Album { get; set; }

        [JsonProperty(PropertyName = "result")]
        public Result Result { get; set; }

        [JsonProperty(PropertyName = "blog")]
        public string BlogId { get; set; }
    }
}
