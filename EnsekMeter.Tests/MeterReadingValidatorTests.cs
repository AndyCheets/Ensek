using EnsekMeter.DTOs;
using EnsekMeter.Models;
using EnsekMeter.Services;
using EnsekMeter.Validation;
using FluentAssertions;
using Xunit;

namespace EnsekMeter.Tests
{
    public class MeterReadingValidatorTests
    {
        [Fact]
        public async Task DuplicateReadingValidator_Should_Fail_For_Duplicate()
        {
            var validator = new DuplicateReadingValidator();
            var row = new MeterReadingCsvRow
            {
                AccountId = 1001,
                MeterReadingDateTime = new DateTime(2024, 1, 1),
                MeterReadValue = "100"
            };

            var account = new Account()
            {
                Id = 1001,
                MeterReadings = new List<MeterReading>
            {
                new MeterReading { ReadingDate = new DateTime(2024, 1, 1), Value = 100 }
            }
            };

            var result = await validator.ValidateAsync(row, account);

            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Duplicate reading found");
        }

        [Fact]
        public async Task NewerThanLatestReadingValidator_Should_Fail_For_Older_Reading()
        {
            var validator = new NewerThanLatestReadingValidator();
            var row = new MeterReadingCsvRow
            {
                AccountId = 1001,
                MeterReadingDateTime = new DateTime(2023, 12, 31),
                MeterReadValue = "123"
            };

            var account = new Account()
            {
                Id = 1001,
                MeterReadings = new List<MeterReading>
            {
                new MeterReading { ReadingDate = new DateTime(2024, 1, 1), Value = 150 }
            }
            };

            var result = await validator.ValidateAsync(row, account);

            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("older than or equal");
        }

        [Fact]
        public async Task NewerThanLatestReadingValidator_Should_Pass_For_Newer_Reading()
        {
            var validator = new NewerThanLatestReadingValidator();
            var row = new MeterReadingCsvRow
            {
                AccountId = 1001,
                MeterReadingDateTime = new DateTime(2024, 1, 2),
                MeterReadValue = "200"
            };

            var account = new Account()
            {
                Id = 1001,
                MeterReadings = new List<MeterReading>
            {
                new MeterReading { ReadingDate = new DateTime(2024, 1, 1), Value = 150 }
            }
            };

            var result = await validator.ValidateAsync(row, account);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValuesValidator_Should_Be_Int()
        {
            var validator = new MeterReadingValueValidator();
            var row = new MeterReadingCsvRow
            {
                AccountId = 1001,
                MeterReadingDateTime = new DateTime(2024, 1, 2),
                MeterReadValue = "ABCD"
            };

            var account = new Account()
            {
                Id = 1001,
                MeterReadings = new List<MeterReading>
            {
                new MeterReading { ReadingDate = new DateTime(2025, 1, 1), Value = 150 }
            }
            };

            var result = await validator.ValidateAsync(row, account);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ValuesValidator_Less_Than_Zero()
        {
            var validator = new MeterReadingValueValidator();
            var row = new MeterReadingCsvRow
            {
                AccountId = 1001,
                MeterReadingDateTime = new DateTime(2024, 1, 2),
                MeterReadValue = "-68"
            };

            var account = new Account()
            {
                Id = 1001,
                MeterReadings = new List<MeterReading>
            {
                new MeterReading { ReadingDate = new DateTime(2023, 1, 1), Value = 150 }
            }
            };

            var result = await validator.ValidateAsync(row, account);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ValuesValidator_Value_Too_High()
        {
            var validator = new MeterReadingValueValidator();
            var row = new MeterReadingCsvRow
            {
                AccountId = 1001,
                MeterReadingDateTime = new DateTime(2024, 1, 2),
                MeterReadValue = "-68"
            };

            var account = new Account()
            {
                Id = 1001,
                MeterReadings = new List<MeterReading>
            {
                new MeterReading { ReadingDate = new DateTime(2023, 1, 1), Value = 150 }
            }
            };

            var result = await validator.ValidateAsync(row, account);

            result.IsValid.Should().BeFalse();
        }
    }
}