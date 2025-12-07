using System.Collections.Generic;
using TransTrack.Common.Models;

namespace TransTrack.FileHandling
{
    public interface IFileParser
    {
        List<ShipmentRecord> Parse(string filePath);
    }
}
