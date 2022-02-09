namespace EKOLidl.Data.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class Album
    {
        [JsonProperty(PropertyName = "links")]
        public List<string> Links { get; set; }
    }
}
