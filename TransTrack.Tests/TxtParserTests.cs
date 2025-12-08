using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TransTrack.FileHandling;

namespace TransTrack.Tests
{
    [TestClass]
    public class TxtParserTests
    {
        private TxtParser _parser;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _parser = new TxtParser();
            _testFilePath = "test_shipment.txt";
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public void Parse_ValidTxt_ReturnsCorrectRecordCount()
        {
            // Arrange
            string txtContent = "S-55221|North|Takoradi|2024-11-13|Bulk\nS-55222|East|Tema|2024-11-11|Fragile";
            File.WriteAllText(_testFilePath, txtContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(2, records.Count);
        }

        [TestMethod]
        public void Parse_ValidTxt_ParsesFieldsCorrectly()
        {
            // Arrange
            string txtContent = "S-55221|North|Takoradi|2024-11-13|Bulk";
            File.WriteAllText(_testFilePath, txtContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual("S-55221", records[0].ShipmentId);
            Assert.AreEqual("North", records[0].Region);
            Assert.AreEqual("Takoradi", records[0].Destination);
            Assert.AreEqual("2024-11-13", records[0].Date);
            Assert.AreEqual("Bulk", records[0].LoadType);
        }

        [TestMethod]
        public void Parse_EmptyFile_ReturnsEmptyList()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "");

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(0, records.Count);
        }

        [TestMethod]
        public void Parse_TxtWithWhitespace_TrimsValues()
        {
            // Arrange
            string txtContent = " S-55221 | North | Takoradi | 2024-11-13 | Bulk ";
            File.WriteAllText(_testFilePath, txtContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual("S-55221", records[0].ShipmentId);
            Assert.AreEqual("North", records[0].Region);
            Assert.AreEqual("Takoradi", records[0].Destination);
        }

        [TestMethod]
        public void Parse_TxtWithIncorrectColumns_SkipsInvalidRows()
        {
            // Arrange
            string txtContent = "S-55221|North|Takoradi|2024-11-13|Bulk\nS-55222|East\nS-55223|West|Accra|2024-12-05|Liquid";
            File.WriteAllText(_testFilePath, txtContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(2, records.Count); // Should skip the invalid row
        }

        [TestMethod]
        public void Parse_MultipleRecords_ParsesAllRecords()
        {
            // Arrange
            string txtContent = @"S-55221|North|Takoradi|2024-11-13|Bulk
S-55222|East|Tema|2024-11-11|Fragile
S-55223|West|Accra|2024-12-05|Liquid";
            File.WriteAllText(_testFilePath, txtContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(3, records.Count);
            Assert.AreEqual("S-55221", records[0].ShipmentId);
            Assert.AreEqual("S-55222", records[1].ShipmentId);
            Assert.AreEqual("S-55223", records[2].ShipmentId);
        }
    }
}