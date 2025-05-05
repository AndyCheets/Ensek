using EnsekMeter.DTOs;
using EnsekMeter.Models;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using EnsekMeter.Validation;

namespace EnsekMeter.Services
{
    /// <summary>
    /// Impementation of the uploas service
    /// </summary>
    public class MeterReadingUploadService : IMeterReadingUploadService
    {
        /// <summary>
        /// The database repository
        /// </summary>
        private readonly IMeterReadingRepository _repo;

        /// <summary>
        /// The validators used to check the meter reading data
        /// </summary>
        private readonly IEnumerable<IMeterReadingValidator> _validators;

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="repo">The instance of the database repo</param>
        /// <param name="validators">A collection of validators</param>
        public MeterReadingUploadService(IMeterReadingRepository repo,
        IEnumerable<IMeterReadingValidator> validators)
        {
            _repo = repo;
            _validators = validators;
        }

        /// <summary>
        /// Process the data, check if its valid and adding it to the database if possible
        /// </summary>
        /// <param name="csvStream">The data to process</param>
        /// <returns>Infomration about the results of processing the data</returns>
        public async Task<UploadMeterReadingResults> ProcessUploadAsync(Stream csvStream)
        {
            // Create the results object
            var result = new UploadMeterReadingResults();

            // Setup to read the contents of the stream
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Register custom converter so we can read the date format in the file
            csv.Context.TypeConverterCache.AddConverter<DateTime>(
                new Utils.DateTimeConverterWithFormat()
            );

            // Read the contents into memory.
            // (this loads the entire file, so if we have very large files we may wish to revise this)
            var rows = csv.GetRecords<MeterReadingCsvRow>().ToList();
            
            // Process the data
            foreach (var row in rows)
            {
                // Increment the total record count
                result.TotalRecordsProcessed++;

                // Get the account
                var account = await _repo.GetAccountByIdAsync(row.AccountId);
                if (account == null)
                {
                    // Update the return value with the reason for the failure
                    result.Results.Add($"Skipped: No account with ID {row.AccountId} found.");
                    result.FailedReadings++;
                    continue;
                }
                    
                // Validate the data in the row
                bool isValid = true;
                foreach (var validator in _validators)
                {
                    var validation = await validator.ValidateAsync(row, account);
                    if (!validation.IsValid)
                    {
                        // Update the return value with the reason for the failure
                        result.Results.Add($"Skipped: {validation.ErrorMessage}");
                        result.FailedReadings++;
                        isValid = false;
                        break;
                    }
                }

                // If the record is not valid move on to next row
                if (!isValid)
                    continue;

                // Create new meter reading to be added to database
                var newReading = new MeterReading
                {
                    AccountId = account.Id,
                    ReadingDate = row.MeterReadingDateTime,
                    Value = int.Parse(row.MeterReadValue),
                };

                // Add the record to the database
                await _repo.AddMeterReadingAsync(newReading);

                // Record the successful read 
                result.SuccessfulReadings++;
                result.Results.Add($"Added: Reading for {row.AccountId}");
            }

            await _repo.SaveChangesAsync();
            return result;
        }
    }
}
