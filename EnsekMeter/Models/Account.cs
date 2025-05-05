using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Models
{
    /// <summary>
    /// The customer account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Account Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Account Holders First Name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Account Holders Last Name
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Meter Readings for this account
        /// </summary>
        public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    }
}
