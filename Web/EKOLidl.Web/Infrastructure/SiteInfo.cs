namespace EKOLidl.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    ///     Fixed site content shared by the layout and pages: contacts, partners and the funding notice.
    /// </summary>
    public static class SiteInfo
    {
        public const string Organisation = "Морски клуб „Приятели на морето“ – Варна";

        public const string OrganisationUrl = "https://seafriends.bg/";

        public const string ContactName = "Дарина Иванова";

        public const string ContactPhone = "+359 889 818 727";

        public const string ContactPhoneHref = "tel:+359889818727";

        public const string ContactEmail = "seaclub2@gmail.com";

        public const string Address = "Варна 9000, ул. „Сава Радулов“ 14";

        public const string FundingProgramme = "„Ти и Lidl за по-добър живот“";

        public const string FundingProgrammeUrl = "https://corporate.lidl.bg/";

        public const string FundingNotice =
            "Настоящият проект се осъществи благодарение на най-голямата социално отговорна инициатива на Лидл България "
            + "„Ти и Lidl за по-добър живот“, в партньорство с Фондация „Работилница за граждански инициативи“ "
            + "и Български дарителски форум. Отговорността за съдържанието е на МК „Приятели на морето“ "
            + "и по никакъв начин не отразява официалните позиции на финансиращите организации.";

        public static readonly IReadOnlyList<Partner> Partners = new[]
        {
            new Partner("Морски клуб „Приятели на морето“ – Варна", OrganisationUrl, "/img/partners/sea-friends.png", 320, 320),
            new Partner("„Ти и Lidl за по-добър живот“ – инициатива на Лидл България", FundingProgrammeUrl, "/img/partners/ti-i-lidl.webp", 480, 485),
            new Partner("Фондация „Работилница за граждански инициативи“", "https://frgi.bg/", "/img/partners/frgi.png", 480, 259),
            new Partner("Български дарителски форум", "https://dfbulgaria.org/", "/img/partners/bdf.webp", 354, 171),
        };

        private static readonly CultureInfo Bulgarian = CultureInfo.GetCultureInfo("bg-BG");

        // IANA id when ICU is available; Windows id as a fallback ("FLE Standard Time" covers Sofia).
        private static readonly TimeZoneInfo Sofia =
            TimeZoneInfo.TryFindSystemTimeZoneById("Europe/Sofia", out TimeZoneInfo iana) ? iana : TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        /// <summary>
        ///     Formats a date as „31 октомври 2022“.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The formatted date.</returns>
        public static string LongDate(DateTime date) => ToSofia(date).ToString("d MMMM yyyy", Bulgarian);

        /// <summary>
        ///     Formats a date as „31 окт 2022“.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The formatted date.</returns>
        public static string ShortDate(DateTime date) => ToSofia(date).ToString("d MMM yyyy", Bulgarian);

        /// <summary>
        ///     Formats a time as „11:00 ч.“.
        /// </summary>
        /// <param name="date">The date and time.</param>
        /// <returns>The formatted time.</returns>
        public static string Time(DateTime date) => ToSofia(date).ToString("HH:mm", Bulgarian) + " ч.";

        /// <summary>
        ///     Formats a duration in hours: „1 час“, „3 часа“.
        /// </summary>
        /// <param name="hours">The duration in hours.</param>
        /// <returns>The formatted duration.</returns>
        public static string Hours(int hours) => hours == 1 ? "1 час" : $"{hours} часа";

        /// <summary>
        ///     Dates entered in the admin have no time zone and are already Sofia time; only UTC values are converted.
        /// </summary>
        /// <param name="date">The stored date.</param>
        /// <returns>The date in Sofia time, without a kind.</returns>
        public static DateTime ToSofiaTime(DateTime date) =>
            DateTime.SpecifyKind(date.Kind == DateTimeKind.Utc ? TimeZoneInfo.ConvertTimeFromUtc(date, Sofia) : date, DateTimeKind.Unspecified);

        private static DateTime ToSofia(DateTime date) => ToSofiaTime(date);

        public sealed record Partner(string Name, string Url, string Logo, int Width, int Height);
    }
}
