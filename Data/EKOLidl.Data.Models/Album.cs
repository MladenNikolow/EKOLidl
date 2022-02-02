namespace EKOLidl.Data.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Album
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public List<string> Links { get; set; }
    }
}
