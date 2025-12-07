using System;
using System.Collections.Generic;
using System.Linq;
using TransTrack.Common.Models;

namespace NorthProcessor
{
    public class NorthValidator
    {
        public ValidationResult Validate(List<ShipmentRecord> records)
        {
            if (records == null || records.Count == 0)
                return new ValidationResult(false, "No records found");

            foreach (var record in records)
            {
                // Rule 1: ShipmentId - not empty and alphanumeric
                if (string.IsNullOrWhiteSpace(record.ShipmentId) || !IsAlphanumeric(record.ShipmentId))
                    return new ValidationResult(false, $"Invalid ShipmentId: {record.ShipmentId}");

                // Rule 2: Origin - not empty
                if (string.IsNullOrWhiteSpace(record.Origin))
                    return new ValidationResult(false, "Origin cannot be empty");

                // Rule 3: Destination - not empty
                if (string.IsNullOrWhiteSpace(record.Destination))
                    return new ValidationResult(false, "Destination cannot be empty");

                // Rule 4: Date - must parse and not be in future
                if (!DateTime.TryParse(record.Date, out DateTime shipDate))
                    return new ValidationResult(false, $"Invalid date: {record.Date}");

                if (shipDate > DateTime.Now)
                    return new ValidationResult(false, $"Date cannot be in the future: {shipDate}");

                // Rule 5: Weight - decimal > 0
                if (!decimal.TryParse(record.Weight, out decimal weight) || weight <= 0)
                    return new ValidationResult(false, $"Invalid weight: {record.Weight}");
            }

            return new ValidationResult(true);
        }

        private bool IsAlphanumeric(string value)
        {
            return value.All(char.IsLetterOrDigit);
        }
    }
}