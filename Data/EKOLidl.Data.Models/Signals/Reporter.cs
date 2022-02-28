namespace EKOLidl.Data.Models
{
    using Newtonsoft.Json;

    public class Reporter
    {
        [JsonProperty(PropertyName = "names")]
        public string Names { get; set; }

        [JsonProperty(PropertyName = "gsm")]
        public string Gsm { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}
