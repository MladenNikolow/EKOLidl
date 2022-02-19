namespace EKOLidl.Web.ViewModels.Signals
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Mapping;

    public class SignalDetailsViewModel : IMapFrom<Signal>, IHaveCustomMappings
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
        /// Gets or sets location of the event in hours.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets album of the event's pictures.
        /// </summary>
        public Album Album { get; set; }

        /// <summary>
        /// Gets or sets result of the event.
        /// </summary>
        public Result Result { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Signal, SignalDetailsViewModel>()
                .ForMember(dest => dest.TrashTypeDescriptions, opt => opt.MapFrom((src, dest) =>
                    {
                        List<string> descriptions = new List<string>();

                        foreach (var type in src.TrashType)
                        {
                            System.Reflection.FieldInfo fieldInfo = type.GetType().GetField(type.ToString());
                            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                            descriptions.Add(attributes[0].Description);
                        }

                        return descriptions;
                    }));
        }
    }
}
