namespace EKOLidl.Data.Models
{
    using Newtonsoft.Json;

    public class Event
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public Album? Album { get; set; }

        public Result? Result { get; set; }

        public string BlogId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
