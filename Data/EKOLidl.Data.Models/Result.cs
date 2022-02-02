namespace EKOLidl.Data.Models
{
    using System;

    using Newtonsoft.Json;

    public class Result
    {
        public Result()
        {
            this.Id = Guid.NewGuid().ToString();
            this.AlbumId = Guid.NewGuid().ToString();
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string AlbumId { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
