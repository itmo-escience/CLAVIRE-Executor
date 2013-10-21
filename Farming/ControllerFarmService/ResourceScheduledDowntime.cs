using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.Serialization;
using ControllerFarmService.ResourceBaseService;

namespace MITP
{
    public static class ResourceScheduledDowntimeExt
    {
        public static bool IsInScheduledDowntime(this Resource resource)
        {
            DateTime now = DateTime.Now;

            bool isInScheduledMaintenance = false;
            foreach (var datesInterval in resource.ScheduledDowntime)
            {
                try
                {
                    DateTime downtimeStart  = ParseScheduledDate(datesInterval.First());
                    DateTime downtimeFinish = ParseScheduledDate(datesInterval.Last());

                    if (downtimeFinish < downtimeStart)
                        Log.Warn(String.Format(
                            "ScheduledDowntime Finish is less then Start ({1} < {0}) for resource {2}",
                            downtimeStart, downtimeFinish,
                            resource.ResourceName
                        ));
                    else
                    {
                        if (downtimeStart <= now && now <= downtimeFinish)
                        {
                            isInScheduledMaintenance = true;

                            Log.Info(String.Format(
                                "Resource {4} is in scheduled maintenance: '{0}' ({1}) -- '{2}' ({3})",
                                datesInterval.First(), downtimeStart,
                                datesInterval.Last(), downtimeFinish,
                                resource.ResourceName
                            ));
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(String.Format(
                        "Exception while processing ScheduledDowntime ('{0}' -- '{1}') for resource {2}: {3}", 
                        datesInterval.FirstOrDefault() ?? "", datesInterval.LastOrDefault() ?? "",
                        resource.ResourceName,
                        e
                    ));
                }
            }

            return isInScheduledMaintenance;
        }

        private static DateTime ParseScheduledDate(string dateAsString)
        {
            DateTime parsedDate = new DateTime();
            bool parsedSuccessfully = false;

            if (!parsedSuccessfully)
                parsedSuccessfully = TryParseExactScheduledDate(dateAsString, out parsedDate);

            if (!parsedSuccessfully)
                parsedSuccessfully = TryParseDailyScheduledDate(dateAsString, out parsedDate);

            if (!parsedSuccessfully)
                parsedSuccessfully = TryParseWeeklyScheduledDate(dateAsString, out parsedDate);

            if (!parsedSuccessfully)
                throw new ArgumentException("Scheduled Date is in wrong format");
            return parsedDate;
        }

        private static bool TryParseExactScheduledDate(string dateAsString, out DateTime parsedDate)
        {
            string[] exactTimeFormats = new[]
            {
                 "d.MM HH:mm",
                 "d.MM HH:mm:ss",
                "dd.MM HH:mm",
                "dd.MM HH:mm:ss",

                 "d.MM.yy HH:mm",
                 "d.MM.yy HH:mm:ss",
                "dd.MM.yy HH:mm",
                "dd.MM.yy HH:mm:ss",

                 "d.MM.yyyy HH:mm",
                 "d.MM.yyyy HH:mm:ss",
                "dd.MM.yyyy HH:mm",
                "dd.MM.yyyy HH:mm:ss",

                "HH:mm     d.MM",
                "HH:mm:ss  d.MM",
                "HH:mm    dd.MM",
                "HH:mm:ss dd.MM",

                "HH:mm     d.MM.yy",
                "HH:mm:ss  d.MM.yy",
                "HH:mm    dd.MM.yy",
                "HH:mm:ss dd.MM.yy",

                "HH:mm     d.MM.yyyy",
                "HH:mm:ss  d.MM.yyyy",
                "HH:mm    dd.MM.yyyy",
                "HH:mm:ss dd.MM.yyyy",
            };

            bool isExactTime = DateTime.TryParseExact(
                dateAsString, exactTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out parsedDate
            );

            return isExactTime;
        }

        private static bool TryParseDailyScheduledDate(string dateAsString, out DateTime parsedDate)
        {
            string[] dailyFormats = new[]
            {
                "H:mm",
                "HH:mm",
                "HH:mm:ss",
                "H:mm:ss",
            };

            bool isDailyTime = DateTime.TryParseExact(
                dateAsString, dailyFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out parsedDate
            );

            return isDailyTime;
        }

        private static bool TryParseWeeklyScheduledDate(string dateAsString, out DateTime parsedDate)
        {
            DateTime now = DateTime.Now;

            var nearestDays = Enumerable.Range(-10, 20)
                .Select(n => now.AddDays(n))
                .Select(day => new
                {
                    date = day,

                    dayNames = new[] 
                    {
                        DateTimeFormatInfo.InvariantInfo.GetDayName(day.DayOfWeek),
                        DateTimeFormatInfo.InvariantInfo.GetAbbreviatedDayName(day.DayOfWeek),

                        DateTimeFormatInfo.CurrentInfo.GetDayName(day.DayOfWeek),
                        DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(day.DayOfWeek),
                    }
                });

            var sameDaysOfWeek = nearestDays.Where(d => 
                   d.dayNames.Any(name => dateAsString.StartsWith(name, ignoreCase: true, culture: CultureInfo.InvariantCulture)) 
                || d.dayNames.Any(name => dateAsString.StartsWith(name, ignoreCase: true, culture: CultureInfo.CurrentCulture  )) 
                || d.dayNames.Any(name => dateAsString.EndsWith  (name, ignoreCase: true, culture: CultureInfo.InvariantCulture)) 
                || d.dayNames.Any(name => dateAsString.EndsWith  (name, ignoreCase: true, culture: CultureInfo.CurrentCulture  ))
            ).ToArray();

            if (sameDaysOfWeek.Any())
            {
                var nearestSameDayOfWeek = sameDaysOfWeek.OrderBy(d => Math.Abs((d.date - now).TotalSeconds)).First();

                var hoursRegex = new Regex(@"(\d+):");
                string hoursStr = hoursRegex.Match(dateAsString).Groups[1].Value;

                var minutesRegex = new Regex(@":(\d+)");
                string minutesStr = minutesRegex.Match(dateAsString).Groups[1].Value;

                parsedDate = new DateTime(
                    nearestSameDayOfWeek.date.Year, nearestSameDayOfWeek.date.Month, nearestSameDayOfWeek.date.Day,
                    int.Parse(hoursStr), int.Parse(minutesStr), 00
                );

                return true;
            }

            parsedDate = new DateTime();
            return false;
        }
    }
}
