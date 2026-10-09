namespace EKOLidl.Web.ViewModels.Signals
{
    using System.Collections.Generic;
    using System.Linq;

    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;

    public class SignalDetailsViewModel
    {
        /// <summary>
        /// Gets or sets title of the signal.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets description of the signal.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets list of trash types.
        /// </summary>
        public List<TrashType> TrashType { get; set; }

        /// <summary>
        /// Gets or sets list of trash types translations.
        /// </summary>
        public List<string> TrashTypeDescriptions { get; set; }

        /// <summary>
        /// Gets or sets string, describing the trash type.
        /// </summary>
        public string OtherTrashType { get; set; }

        /// <summary>
        /// Gets or sets location of the signal.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets album of the signal's pictures.
        /// </summary>
        public Album Album { get; set; }

        /// <summary>
        /// Gets or sets result of the signal.
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// Gets or sets when the signal was submitted.
        /// </summary>
        public System.DateTime CreatedOn { get; set; }

        public static SignalDetailsViewModel FromEntity(Signal signal)
        {
            List<TrashType> trashTypes = signal.TrashType ?? new List<TrashType>();

            return new SignalDetailsViewModel
            {
                Title = signal.Title,
                Description = signal.Description,
                TrashType = trashTypes,
                TrashTypeDescriptions = trashTypes.Select(x => x.GetDescription()).ToList(),
                OtherTrashType = signal.OtherTrashType,
                Location = signal.Location,
                Album = signal.Album,
                Result = signal.Result,
                CreatedOn = signal.CreatedOn,
            };
        }
    }
}
