namespace EKOLidl.Data.Common.Models
{
    using System;

    using Newtonsoft.Json;

    public abstract class BaseEntity : IAuditInfo
    {
        public BaseEntity()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedOn = DateTime.UtcNow;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "modified_dt")]
        public DateTime? ModifiedOn { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
