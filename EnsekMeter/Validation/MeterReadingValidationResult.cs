using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Validation
{
    /// <summary>
    /// Validation result
    /// </summary>
    public class MeterReadingValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
