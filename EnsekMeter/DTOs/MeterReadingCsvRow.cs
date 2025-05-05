using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.DTOs
{
    /// <summary>
    /// Represents a Row in the CSV file
    /// </summary>
    public class MeterReadingCsvRow
    {
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; } = string.Empty;
    }
}
