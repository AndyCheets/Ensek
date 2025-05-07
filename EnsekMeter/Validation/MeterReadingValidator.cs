using EnsekMeter.DTOs;
using EnsekMeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Validation
{
    public interface IMeterReadingValidation
    {
        Task<MeterReadingValidationResult> IsValidAsync(MeterReadingCsvRow row, Account? account);
    }

    public class MeterReadingValidation: IMeterReadingValidation
    {
        /// <summary>
        /// The validators used to check the meter reading data
        /// </summary>
        private readonly IEnumerable<IMeterReadingValidator> _validators;


        public MeterReadingValidation(IEnumerable<IMeterReadingValidator> validators)
        {
            _validators = validators;
        }

        public async Task<MeterReadingValidationResult> IsValidAsync(MeterReadingCsvRow row, Account? account)
        {
            // Validate the data in the row
            foreach (var validator in _validators)
            {
                var validationResult = await validator.ValidateAsync(row, account);
                if (!validationResult.IsValid)
                {
                    // Update the return value with the reason for the failure
                    return validationResult;
                }
            }

            return new MeterReadingValidationResult();
        }
    }
}
