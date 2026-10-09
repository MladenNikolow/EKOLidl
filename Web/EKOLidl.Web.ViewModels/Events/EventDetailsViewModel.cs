namespace EKOLidl.Web.ViewModels.Events
{
    using System;

    using EKOLidl.Data.Models;

    public class EventDetailsViewModel
    {
        /// <summary>
        /// Gets or sets title of the event.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets short description of the event.
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets long description of the event.
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// Gets or sets event starting date.
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets duration of the event in hours.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets album of the event's pictures.
        /// </summary>
        public Album Album { get; set; }

        /// <summary>
        /// Gets or sets result of the event.
        /// </summary>
        public Result Result { get; set; }

        public static EventDetailsViewModel FromEntity(Event @event)
        {
            return new EventDetailsViewModel
            {
                Title = @event.Title,
                ShortDescription = @event.ShortDescription,
                LongDescription = @event.LongDescription,
                EventDate = @event.EventDate,
                Duration = @event.Duration,
                Album = @event.Album,
                Result = @event.Result,
            };
        }
    }
}
