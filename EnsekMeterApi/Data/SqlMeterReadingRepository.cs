using EnsekMeter.Models;
using EnsekMeter.Services;
using Microsoft.EntityFrameworkCore;

namespace EnsekMeterApi.Data
{
    /// <summary>
    /// Impenetation of the database repository
    /// </summary>
    public class SqlMeterReadingRepository : IMeterReadingRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly MeterDbContext _context;

        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="context"></param>
        public SqlMeterReadingRepository(MeterDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the account by Id
        /// </summary>
        /// <param name="accountId">The id for the account</param>
        /// <returns>If it exists it return the account information, null otherwise</returns>
        public Task<Account?> GetAccountByIdAsync(int accountId) =>
            _context.Accounts
            .Include(a => a.MeterReadings) // make sure to include the meter readings with the account info
            .FirstOrDefaultAsync(a => a.Id == accountId);

        /// <summary>
        /// Add a new meter reading to the database
        /// </summary>
        /// <param name="reading">The meter reading</param>
        /// <returns>The Async Task</returns>
        public Task AddMeterReadingAsync(MeterReading reading)
        {
            _context.MeterReadings.Add(reading);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Commit the changes to the database
        /// </summary>
        /// <returns>The Async Task</returns>
        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
