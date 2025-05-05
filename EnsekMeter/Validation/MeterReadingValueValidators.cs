using EnsekMeter.DTOs;
using EnsekMeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Validation
{
    // Ensure we have an account
    public class MeterReadingAccountValidator : IMeterReadingValidator
    {
        public Task<MeterReadingValidationResult> ValidateAsync(MeterReadingCsvRow row, Account? account)
        {
            if (account == null)
            {
                return Task.FromResult(new MeterReadingValidationResult { IsValid = false, ErrorMessage = $"No account with ID {row.AccountId}" });
            }

            return Task.FromResult(new MeterReadingValidationResult { IsValid = true });
        }
    }

    // Ensure meter value is a number (basic, optional if parsing handled earlier)
    public class MeterReadingValueValidator : IMeterReadingValidator
    {
        public Task<MeterReadingValidationResult> ValidateAsync(MeterReadingCsvRow row, Account? account)
        {
            if(!int.TryParse(row.MeterReadValue, out int value))
            {
                return Task.FromResult(new MeterReadingValidationResult { IsValid = false, ErrorMessage = $"Invalid meter reading value {row.MeterReadValue} for account {row.AccountId}" });
            }

            //Value is less than 0 - unless they are giving energy back to the grid I assume this is invalid
            if (value <= 0)
            {
                return Task.FromResult(new MeterReadingValidationResult { IsValid = false, ErrorMessage = $"Invalid meter reading value {row.MeterReadValue} for account {row.AccountId}" });
            }

            //Value is too high - Spec states that the readings should be in the formatt NNNNN
            if (value > 99999)
            {
                return Task.FromResult(new MeterReadingValidationResult { IsValid = false, ErrorMessage = $"Invalid meter reading value, value is too high: {row.MeterReadValue} for account {row.AccountId}" });
            }

            return Task.FromResult(new MeterReadingValidationResult { IsValid = true });
        }
    }

    // Prevent older readings than already exist
    public class NewerThanLatestReadingValidator : IMeterReadingValidator
    {
        public Task<MeterReadingValidationResult> ValidateAsync(MeterReadingCsvRow row, Account? account)
        {
            var latest = account?.MeterReadings.OrderByDescending(r => r.ReadingDate).FirstOrDefault();
            if (latest != null && row.MeterReadingDateTime <= latest.ReadingDate)
            {
                return Task.FromResult(new MeterReadingValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Reading dated {row.MeterReadingDateTime} is older than or equal to the latest existing reading ({latest.ReadingDate}) for account {row.AccountId}"
                });
            }

            return Task.FromResult(new MeterReadingValidationResult { IsValid = true });
        }
    }

    // Validator: Prevent duplicate readings (same date and value)
    public class DuplicateReadingValidator : IMeterReadingValidator
    {
        public Task<MeterReadingValidationResult> ValidateAsync(MeterReadingCsvRow row, Account? account)
        {
            var exists = account != null && account.MeterReadings.Any(r => r.ReadingDate == row.MeterReadingDateTime && r.Value.ToString() == row.MeterReadValue);
            if (exists)
            {
                return Task.FromResult(new MeterReadingValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Duplicate reading found for date {row.MeterReadingDateTime} and value {row.MeterReadValue} for account {row.AccountId}"
                });
            }

            return Task.FromResult(new MeterReadingValidationResult { IsValid = true });
        }
    }
}
