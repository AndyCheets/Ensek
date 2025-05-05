using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Models
{
    /// <summary>
    /// The results from processing the provided file
    /// </summary>
    public class UploadMeterReadingResults
    {
        /// <summary>
        /// The total number of records processed
        /// </summary>
        public int TotalRecordsProcessed { get; set; }

        /// <summary>
        /// The number of successful readings added to the database
        /// </summary>
        public int SuccessfulReadings { get; set; }

        /// <summary>
        /// The number of records that failed to be added
        /// </summary>
        public int FailedReadings { get; set; }

        /// <summary>
        /// The status of each record that was uploaded
        /// </summary>
        public List<string> Results { get; } = new List<string>();
    }
}
