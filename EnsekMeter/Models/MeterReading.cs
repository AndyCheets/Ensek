using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekMeter.Models;

/// <summary>
/// The Meter reading
/// </summary>
public class MeterReading
{
    /// <summary>
    /// The Meter reading ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The id of the Account this reading is for.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// The date of the reading 
    /// </summary>
    public DateTime ReadingDate { get; set; }

    /// <summary>
    /// The meter reading
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// The account details
    /// </summary>
    public Account Account { get; set; } = null!;
}
