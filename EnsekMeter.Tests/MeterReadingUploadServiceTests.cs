using EnsekMeter.Models;
using EnsekMeter.Services;
using Moq;
using FluentAssertions;
using EnsekMeter.Validation;
using EnsekMeter.DTOs;
using System.Security.Principal;

namespace EnsekMeter.Tests
{
    public class MeterReadingUploadServiceTests
    {
        private readonly Mock<IMeterReadingRepository> _repoMock;
        private readonly MeterReadingUploadService _service;
        private readonly List<Mock<IMeterReadingValidator>> _validatorMocks;

        public MeterReadingUploadServiceTests()
        {
            _repoMock = new Mock<IMeterReadingRepository>();
            _validatorMocks = new List<Mock<IMeterReadingValidator>>
            {
                new Mock<IMeterReadingValidator>(),
                new Mock<IMeterReadingValidator>()
            };

            var validators = _validatorMocks.Select(v => v.Object);
            _service = new MeterReadingUploadService(_repoMock.Object, validators);
        }

        [Fact]
        public async Task ProcessUploadAsync_Should_Add_Readings_For_Valid_Accounts()
        {
            // Arrange
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1001,22/04/2019 09:24,100";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

            _repoMock.Setup(r => r.GetAccountByIdAsync(1001))
                     .ReturnsAsync(new Account { Id = 1001 });

            //All validators to return true
            foreach (var validator in _validatorMocks)
            {
                validator.Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), It.IsAny<Account>()))
                         .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });
            }

            // Act
            var result = await _service.ProcessUploadAsync(stream);

            // Assert
            result.Results.Should().ContainSingle().Which.Should().Contain("Added: Reading for 1001");
            _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessUploadAsync_Extra_Column()
        {
            // Arrange
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1001,01/01/2024 00:00,100,X";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

            _repoMock.Setup(r => r.GetAccountByIdAsync(1001))
                     .ReturnsAsync(new Account { Id = 1001 });
            
            //All validators to return true
            foreach (var validator in _validatorMocks)
            {
                validator.Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), It.IsAny<Account>()))
                         .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });
            }
            
            // Act
            var result = await _service.ProcessUploadAsync(stream);

            // Assert
            result.Results.Should().ContainSingle().Which.Should().Contain("Added: Reading for 1001");
            _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessUploadAsync_Should_Skip_Readings_For_Invalid_Accounts()
        {
            // Arrange
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n-10,01/01/2024 00:00,100";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

            _repoMock.Setup(r => r.GetAccountByIdAsync(-10))
                     .ReturnsAsync((Account?)null);

            //All validators to return true
            foreach (var validator in _validatorMocks)
            {
                validator.Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), It.IsAny<Account>()))
                         .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });
            }

            // Act
            var result = await _service.ProcessUploadAsync(stream);

            // Assert
            result.Results.Should().ContainSingle().Which.Should().Contain("Skipped: No account with ID -10");
            _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Never);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessUploadAsync_Upload_Mix_of_Valid_and_Invalid_Accounts()
        {
            // Arrange
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n-10,01/01/2024 00:00,100\n1001,01/01/2024 00:00,500";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

            _repoMock.Setup(r => r.GetAccountByIdAsync(-10))
                     .ReturnsAsync((Account?)null);

            _repoMock.Setup(r => r.GetAccountByIdAsync(1001))
                 .ReturnsAsync(new Account
                 {
                     Id = 1001,
                     FirstName = "Andy",
                     LastName = "Cheetham",
                     MeterReadings = new List<MeterReading> { new MeterReading { AccountId = 1001, ReadingDate = DateTime.Now, Value = 12345 } }
                 });

            //All validators to return true
            foreach (var validator in _validatorMocks)
            {
                validator.Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), It.IsAny<Account>()))
                         .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });
            }

            // Act
            var result = await _service.ProcessUploadAsync(stream);

            // Assert - Check the results
            result.Results.Should().Contain(r => r.Contains("Added: Reading for 1001"));
            result.Results.Should().Contain(r => r.Contains("Skipped: No account with ID -10"));

            // Check how many times the functions were called
            _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        //[Fact]
        //public async Task ProcessUploadAsync_Upload_Invalid_Reading_Value()
        //{
        //    // Arrange
        //    var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1001,2024-01-01,1000\n1001,2024-01-01,ABCD";
        //    var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        //    var account = new Account { 
        //        Id = 1001,
        //        FirstName = "Andy",
        //        LastName = "Cheetham",
        //        MeterReadings = new List<MeterReading> { new MeterReading { AccountId = 1001, ReadingDate = new DateTime(2025, 05, 05), Value = 12345 } }
        //    };

        //    _repoMock.Setup(r => r.GetAccountByIdAsync(1001))
        //         .ReturnsAsync(new Account
        //         {
        //             Id = 1001,
        //             FirstName = "Andy",
        //             LastName = "Cheetham",
        //             MeterReadings = new List<MeterReading> { new MeterReading { AccountId = 1001, ReadingDate = new DateTime(2025, 05, 05), Value = 12345 } }
        //         });

        //    foreach (var validator in _validatorMocks)
        //    {
        //        validator.Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), account))
        //                 .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });
        //    }

        //    // Act
        //    var result = await _service.ProcessUploadAsync(stream);

        //    // Assert - Check the results
        //    result.Should().Contain(r => r.Contains("Skipped: Invalid Reading Value ABCD for account with ID 1001"));

        //    // Check how many times the functions were called
        //    _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Once);
        //    _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        //}

        [Fact]
        public async Task Should_Skip_If_Any_Validator_Fails()
        {
            // Arrange
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1001,01/01/2024 00:00,100";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
            var account = new Account { Id = 1001};

            _repoMock.Setup(r => r.GetAccountByIdAsync(1001))
                     .ReturnsAsync(account);

            _validatorMocks[0].Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), account))
                             .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });

            _validatorMocks[1].Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), account))
                             .ReturnsAsync(new MeterReadingValidationResult { IsValid = false, ErrorMessage = "Test failure" });

            // Act
            var result = await _service.ProcessUploadAsync(stream);

            // Assert
            result.Results.Should().ContainSingle().Which.Should().Contain("Skipped: Test failure");
            _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Never);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_Add_Reading_If_All_Validators_Pass()
        {
            // Arrange
            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1002,01/01/2024 00:00,200";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
            var account = new Account { Id = 1002 };

            _repoMock.Setup(r => r.GetAccountByIdAsync(1002))
                     .ReturnsAsync(account);

            foreach (var validator in _validatorMocks)
            {
                validator.Setup(v => v.ValidateAsync(It.IsAny<MeterReadingCsvRow>(), account))
                         .ReturnsAsync(new MeterReadingValidationResult { IsValid = true });
            }

            // Act
            var result = await _service.ProcessUploadAsync(stream);

            // Assert
            result.Results.Should().ContainSingle().Which.Should().Contain("Added: Reading for 1002");
            _repoMock.Verify(r => r.AddMeterReadingAsync(It.IsAny<MeterReading>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
