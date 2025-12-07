using System;
using System.Collections.Generic;
using TransTrack.Common.Models;

namespace SouthProcessor
{
    public class SouthValidator
    {
        private readonly string[] _validRegions = { "North", "South", "East", "West" };
        private readonly string[] _validLoadTypes = { "Fragile", "Bulk", "Liquid" };

        public ValidationResult Validate(List<ShipmentRecord> records)
        {
            if (records == null || records.Count == 0)
                return new ValidationResult(false, "No records found");

            foreach (var record in records)
            {
                // Rule 1: ShipmentId - must start with S-
                if (string.IsNullOrWhiteSpace(record.ShipmentId) || !record.ShipmentId.StartsWith("S-"))
                    return new ValidationResult(false, $"Invalid ShipmentId: {record.ShipmentId}");

                // Rule 2: Region - must be one of valid regions
                if (!Array.Exists(_validRegions, r => r.Equals(record.Region, StringComparison.OrdinalIgnoreCase)))
                    return new ValidationResult(false, $"Invalid region: {record.Region}");

                // Rule 3: Destination - not empty
                if (string.IsNullOrWhiteSpace(record.Destination))
                    return new ValidationResult(false, "Destination cannot be empty");

                // Rule 4: Date - must parse and not be weekend
                if (!DateTime.TryParse(record.Date, out DateTime shipDate))
                    return new ValidationResult(false, $"Invalid date: {record.Date}");

                if (shipDate.DayOfWeek == DayOfWeek.Saturday || shipDate.DayOfWeek == DayOfWeek.Sunday)
                    return new ValidationResult(false, $"Date cannot be a weekend: {shipDate:yyyy-MM-dd}");

                // Rule 5: LoadType - must be valid
                if (!Array.Exists(_validLoadTypes, l => l.Equals(record.LoadType, StringComparison.OrdinalIgnoreCase)))
                    return new ValidationResult(false, $"Invalid load type: {record.LoadType}");
            }

            return new ValidationResult(true);
        }
    }
}