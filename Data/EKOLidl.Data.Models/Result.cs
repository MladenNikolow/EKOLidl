namespace EKOLidl.Data.Models
{
    using Newtonsoft.Json;

    public class Result
    {
        [JsonProperty(PropertyName = "album")]
        public Album Album { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
