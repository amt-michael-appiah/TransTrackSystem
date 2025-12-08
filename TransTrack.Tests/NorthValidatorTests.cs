using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransTrack.Common.Models;
using NorthProcessor;

namespace TransTrack.Tests
{
    [TestClass]
    public class NorthValidatorTests
    {
        private NorthValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new NorthValidator();
        }

        [TestMethod]
        public void Validate_ValidRecord_ReturnsTrue()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsTrue(result.IsValid);
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
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("ShipmentId"));
        }

        [TestMethod]
        public void Validate_NonAlphanumericShipmentId_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH-1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("ShipmentId"));
        }

        [TestMethod]
        public void Validate_EmptyOrigin_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("Origin"));
        }

        [TestMethod]
        public void Validate_EmptyDestination_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "",
                    Date = "2024-10-12",
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("Destination"));
        }

        [TestMethod]
        public void Validate_InvalidDate_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "invalid-date",
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("date"));
        }

        [TestMethod]
        public void Validate_FutureDate_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                    Weight = "150.5"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("future"));
        }

        [TestMethod]
        public void Validate_ZeroWeight_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "0"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("weight"));
        }

        [TestMethod]
        public void Validate_NegativeWeight_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "-50"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("weight"));
        }

        [TestMethod]
        public void Validate_InvalidWeight_ReturnsFalse()
        {
            // Arrange
            var records = new List<ShipmentRecord>
            {
                new ShipmentRecord
                {
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "ABC"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ErrorMessage.Contains("weight"));
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
                    ShipmentId = "SH1001",
                    Origin = "Accra",
                    Destination = "Tema",
                    Date = "2024-10-12",
                    Weight = "150.5"
                },
                new ShipmentRecord
                {
                    ShipmentId = "SH1002",
                    Origin = "Kumasi",
                    Destination = "Accra",
                    Date = "2024-10-15",
                    Weight = "200"
                }
            };

            // Act
            var result = _validator.Validate(records);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}