namespace EKOLidl.Data.Models
{
    using Newtonsoft.Json;

    public class Result
    {
        /// <summary>
        /// Placeholder description stored until a moderator adds a real result.
        /// </summary>
        public const string NoResultsYet = "Още няма добавени резултати!";

        [JsonProperty(PropertyName = "album")]
        public Album Album { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets a value indicating whether a real result (text or photos) has been added.
        /// </summary>
        [JsonIgnore]
        public bool HasContent =>
            (this.Album?.Links?.Count ?? 0) > 0
            || (!string.IsNullOrWhiteSpace(this.Description) && this.Description.Trim() != NoResultsYet);
    }
}
