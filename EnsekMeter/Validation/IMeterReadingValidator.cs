using EnsekMeter.DTOs;
using EnsekMeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Validation
{
    /// <summary>
    /// Interface for the validators 
    /// </summary>
    public interface IMeterReadingValidator
    {
        Task<MeterReadingValidationResult> ValidateAsync(MeterReadingCsvRow row, Account? account);
    }
}
