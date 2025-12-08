using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TransTrack.FileHandling;

namespace TransTrack.Tests
{
    [TestClass]
    public class CsvParserTests
    {
        private CsvParser _parser;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _parser = new CsvParser();
            _testFilePath = "test_shipment.csv";
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
        public void Parse_ValidCsv_ReturnsCorrectRecordCount()
        {
            // Arrange
            string csvContent = "SH1001,Accra,Tema,2024-10-12,150.5\nSH1002,Accra,Kumasi,2024-09-20,90";
            File.WriteAllText(_testFilePath, csvContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(2, records.Count);
        }

        [TestMethod]
        public void Parse_ValidCsv_ParsesFieldsCorrectly()
        {
            // Arrange
            string csvContent = "SH1001,Accra,Tema,2024-10-12,150.5";
            File.WriteAllText(_testFilePath, csvContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual("SH1001", records[0].ShipmentId);
            Assert.AreEqual("Accra", records[0].Origin);
            Assert.AreEqual("Tema", records[0].Destination);
            Assert.AreEqual("2024-10-12", records[0].Date);
            Assert.AreEqual("150.5", records[0].Weight);
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
        public void Parse_CsvWithWhitespace_TrimsValues()
        {
            // Arrange
            string csvContent = " SH1001 , Accra , Tema , 2024-10-12 , 150.5 ";
            File.WriteAllText(_testFilePath, csvContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual("SH1001", records[0].ShipmentId);
            Assert.AreEqual("Accra", records[0].Origin);
            Assert.AreEqual("Tema", records[0].Destination);
        }

        [TestMethod]
        public void Parse_CsvWithIncorrectColumns_SkipsInvalidRows()
        {
            // Arrange
            string csvContent = "SH1001,Accra,Tema,2024-10-12,150.5\nSH1002,Accra\nSH1003,Accra,Tema,2024-10-12,90";
            File.WriteAllText(_testFilePath, csvContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(2, records.Count); // Should skip the invalid row
        }

        [TestMethod]
        public void Parse_MultipleRecords_ParsesAllRecords()
        {
            // Arrange
            string csvContent = @"SH1001,Accra,Tema,2024-10-12,150.5
SH1002,Accra,Kumasi,2024-09-20,90
SH1003,Tema,Takoradi,2024-11-01,200.75";
            File.WriteAllText(_testFilePath, csvContent);

            // Act
            var records = _parser.Parse(_testFilePath);

            // Assert
            Assert.AreEqual(3, records.Count);
            Assert.AreEqual("SH1001", records[0].ShipmentId);
            Assert.AreEqual("SH1002", records[1].ShipmentId);
            Assert.AreEqual("SH1003", records[2].ShipmentId);
        }
    }
}