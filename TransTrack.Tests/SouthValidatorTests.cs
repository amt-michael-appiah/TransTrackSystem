using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TransTrack.Common.Models;
using SouthProcessor;

namespace TransTrack.Tests
{
    [TestClass]
    public class SouthValidatorTests
    {
        private SouthValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new SouthValidator();
        }

        [TestMethod]
        public void Validate_ValidRecord_ReturnsTrue()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-13",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShipmentIdWithoutPrefix_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-13",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("ShipmentId"));
        }

        [TestMethod]
        public void Validate_EmptyShipmentId_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-13",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("ShipmentId"));
        }

        [TestMethod]
        public void Validate_InvalidRegion_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "Central",
                    Destination = "Takoradi",
                    Date = "2024-11-13",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("region"));
        }

        [TestMethod]
        public void Validate_AllValidRegions_ReturnsTrue()
        {
            // Test all valid regions: North, South, East, West
            string[] validRegions = { "North", "South", "East", "West" };

            foreach (var region in validRegions)
            {
                // Arrange
                var records = new List<ShipmentRecord>
                {
                    new ShipmentRecord
                    {
                        ShipmentId = "S-55221",
                        Region = region,
                        Destination = "Takoradi",
                        Date = "2024-11-13",
                        LoadType = "Bulk"
                    }
                };

                // Act
                var result = _validator.Validate(records);

                // Assert
                Assert.IsTrue(result.IsValid, $"Region '{region}' should be valid");
            }
        }

        [TestMethod]
        public void Validate_EmptyDestination_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "",
                    Date = "2024-11-13",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("Destination"));
        }

        [TestMethod]
        public void Validate_WeekendDate_Saturday_ReturnsFalse()
        {
            // Arrange - 2024-11-16 is a Saturday
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-16",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("weekend"));
        }

        [TestMethod]
        public void Validate_WeekendDate_Sunday_ReturnsFalse()
        {
            // Arrange - 2024-11-17 is a Sunday
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-17",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("weekend"));
        }

        [TestMethod]
        public void Validate_InvalidDate_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "invalid-date",
                    LoadType = "Bulk"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("date"));
        }

        [TestMethod]
        public void Validate_InvalidLoadType_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-13",
                    LoadType = "Perishable"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("load type"));
        }

        [TestMethod]
        public void Validate_AllValidLoadTypes_ReturnsTrue()
        {
            // Test all valid load types: Fragile, Bulk, Liquid
            string[] validLoadTypes = { "Fragile", "Bulk", "Liquid" };

            foreach (var loadType in validLoadTypes)
            {
                // Arrange
                var records = new List<ShipmentRecord>
                {
                    new ShipmentRecord
                    {
                        ShipmentId = "S-55221",
                        Region = "North",
                        Destination = "Takoradi",
                        Date = "2024-11-13",
                        LoadType = loadType
                    }
                };

                // Act
                var result = _validator.Validate(records);

                // Assert
                Assert.IsTrue(result.IsValid, $"LoadType '{loadType}' should be valid");
            }
        }

        [TestMethod]
        public void Validate_EmptyRecordList_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>();

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("No records"));
        }

        [TestMethod]
        public void Validate_MultipleValidRecords_ReturnsTrue()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "S-55221",
                    Region = "North",
                    Destination = "Takoradi",
                    Date = "2024-11-13",
                    LoadType = "Bulk"
                },
                new ShipmentRecord
                {
                    ShipmentId = "S-55222",
                    Region = "East",
                    Destination = "Tema",
                    Date = "2024-11-11",
                    LoadType = "Fragile"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}