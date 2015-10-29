using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DsbForsinket.Common.DsbLabs;

namespace DsbForsinket.AndroidResourceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string pathToOut = @"..\..\..\..\";
            string timesFileName = "times.txt";
            string timesValuesFileName = "times-values.txt";
            string stationsIdfileName = "stations-id.txt";
            string stationsNamesFileName = "stations-name.txt";

            int hoursFrom = 6;
            int hoursTo = 13;

            var hours = Enumerable.Range(hoursFrom, hoursTo - hoursFrom);
            var minutes = Enumerable.Range(0, 4).Select(i => i * 15);

            var times = from h in hours
                        from m in minutes
                        let h2 = m + 15 < 60 ? h : h + 1
                        let m2 = (m + 15) % 60
                        select new { fromMin = m, fromHour = h, toMin = m2, toHour = h2 };

            File.WriteAllLines(
                Path.Combine(pathToOut, timesFileName),
                times.Select(t => $"<item>{t.fromHour:D2}:{t.fromMin:D2} - {t.toHour:D2}:{t.toMin:D2}</item>"));

            File.WriteAllLines(
                Path.Combine(pathToOut, timesValuesFileName),
                times.Select(t => $"<item>{t.fromHour:D2}:{t.fromMin:D2}</item>"));

            var service = new DSBLabsStationService(new Uri("http://traindata.dsb.dk/stationdeparture/opendataprotocol.svc"));

            var stationsList = service.Station.ToList();

            var stationsOrdered =
                stationsList
                    .OrderBy(s => s.Name, StringComparer.Create(new CultureInfo("da-DK"), true))
                    .ToList();

            File.WriteAllLines(Path.Combine(pathToOut, stationsIdfileName), stationsOrdered.Select(st => $"<item>{st.UIC}</item>"));
            File.WriteAllLines(Path.Combine(pathToOut, stationsNamesFileName), stationsOrdered.Select(st => $"<item>{st.Name}</item>"));
        }
    }
}
