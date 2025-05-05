using EnsekMeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Services
{
    /// <summary>
    /// Interface for accessing the underlying database
    /// </summary>
    public interface IMeterReadingRepository
    {
        /// <summary>
        /// Get the accounts by account id
        /// </summary>
        /// <param name="accountId">The id of the account</param>
        /// <returns>The account information if found, null if no account exists</returns>
        Task<Account?> GetAccountByIdAsync(int accountId);

        /// <summary>
        /// Add a new meter reading to the database
        /// </summary>
        /// <param name="reading">The meter reading information.</param>
        /// <returns>The Async task object</returns>
        Task AddMeterReadingAsync(MeterReading reading);

        /// <summary>
        /// Commit the updates to file
        /// </summary>
        /// <returns>The Async task object</returns>
        Task SaveChangesAsync();
    }
}
