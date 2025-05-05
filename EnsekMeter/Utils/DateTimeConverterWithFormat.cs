using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace EnsekMeter.Utils
{
    /// <summary>
    /// Custom data converter, reads the date format used in the CSV file
    /// </summary>
    public class DateTimeConverterWithFormat : DateTimeConverter
    {
        private readonly string _format;

        public DateTimeConverterWithFormat()
        {
            _format = "dd/MM/yyyy HH:mm";
        }

        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text?.Trim(), _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            throw new TypeConverterException(this, memberMapData, text, row.Context, $"Date '{text}' is not in expected format: {_format}");
        }
    }

}
