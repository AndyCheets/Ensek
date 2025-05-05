using EnsekMeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Services
{
    /// <summary>
    /// The interface for the upload service
    /// </summary>
    public interface IMeterReadingUploadService
    {
        /// <summary>
        /// Process the information
        /// </summary>
        /// <param name="csvStream">The CSV data</param>
        /// <returns>The results of the upload process</returns>
        Task<UploadMeterReadingResults> ProcessUploadAsync(Stream csvStream);
    }
}
