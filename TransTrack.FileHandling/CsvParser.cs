using System.Collections.Generic;
using System.IO;
using TransTrack.Common.Models;

namespace TransTrack.FileHandling
{
    public class CsvParser : IFileParser
    {
        public List<ShipmentRecord> Parse(string filePath)
        {
            var records = new List<ShipmentRecord>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length != 5) continue;

                records.Add(new ShipmentRecord
                {
                    ShipmentId = parts[0].Trim(),
                    Origin = parts[1].Trim(),
                    Destination = parts[2].Trim(),
                    Date = parts[3].Trim(),
                    Weight = parts[4].Trim()
                });
            }

            return records;
        }
    }
}