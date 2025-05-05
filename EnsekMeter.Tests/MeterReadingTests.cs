using Xunit;
using EnsekMeter.Models;
using EnsekMeterApi.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace EnsekMeter.Tests;

public class MeterReadingTests
{
    [Fact]
    public void Should_Save_And_Retrieve_Reading()
    {
        var options = new DbContextOptionsBuilder<MeterDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        using var context = new MeterDbContext(options);
        context.MeterReadings.Add(new MeterReading
        {
            AccountId = 1001,
            ReadingDate = DateTime.UtcNow,
            Value = 123
        });
        context.SaveChanges();

        context.MeterReadings.Count().Should().Be(1);
    }
}