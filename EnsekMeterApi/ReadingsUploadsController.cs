using EnsekMeter.Services;
using EnsekMeterApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace EnsekMeterApi
{
    /// <summary>
    /// Controller to handle the uploading of meter readings
    /// </summary>
    [Route("meter-reading-uploads")]
    [ApiController]
    public class ReadingsUploadsController : ControllerBase
    {
        private readonly IMeterReadingUploadService _uploadService;
        private readonly ILogger<ReadingsUploadsController> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="uploadService">The service to process the uploaded information</param>
        /// <param name="logger">The logger to monitor the process</param>
        public ReadingsUploadsController(IMeterReadingUploadService uploadService, ILogger<ReadingsUploadsController> logger)
        {
            _uploadService = uploadService;
            _logger = logger;
        }

        /// <summary>
        /// Upload a CSV file of meter readings.
        /// </summary>
        /// <param name="file">CSV file to upload</param>
        /// <returns>Success/failure per row</returns>
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            // Check we have a file
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No CSV file provided in the upload request.");
                return BadRequest("CSV file is required.");
            }

            // Check the file ends with "csv"
            if (!file.FileName.ToLower().EndsWith("csv"))
            {
                _logger.LogWarning("The file does not appear to be a CSV file.");
                return BadRequest("A CSV file is required.");
            }

            try
            {
                var result = await _uploadService.ProcessUploadAsync(file.OpenReadStream());
                return Ok(new { Results = result });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An exception was raised whilst processing the file.");
                return BadRequest("Error processing the data.");
            }
        }
    }
}
