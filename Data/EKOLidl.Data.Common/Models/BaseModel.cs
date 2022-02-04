namespace EKOLidl.Data.Common.Models
{
    using System;

    using Newtonsoft.Json;

    public abstract class BaseModel<TKey> : IAuditInfo
    {
        [JsonProperty(PropertyName = "id")]
        public TKey Id { get; set; }

        [JsonProperty(PropertyName = "_ts")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "modified_dt")]
        public DateTime? ModifiedOn { get; set; }
    }
}
