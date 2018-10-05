namespace JsonHelper.Tests
{
    using System;

    using JsonHelper.Model;
    using Xunit;

    public class BaseTests
    {
        [Fact]
        public void IsLogSerializationInvalid()
        {
            var serialization = System.IO.File.ReadAllText(@"./TestFiles/InvalidLog.json");
            var deserialization = new LogEntry().Deserialize(serialization);
            Assert.True(
                deserialization == null,
                "The deserialization result should be null since the input file has an invalid log entry (missing the MachineName field).");

            var log = new LogEntry();
            log.NativeProcessId = "Invalid Number";
            serialization = log.Serialize();
            deserialization = new LogEntry().Deserialize(serialization);
            Assert.True(
                deserialization == null,
                "The deserialization result should be null since a numeric field has an invalid string format.");
        }

        [Fact]
        public void IsLogSerializationValid()
        {
            var log = new LogEntry();
            var serialization = log.Serialize();
            Assert.True(
                !string.IsNullOrEmpty(serialization),
                "Failed to serialize log entry.");

            var deserialization = new LogEntry().Deserialize(serialization);
            Assert.True(
                deserialization != null,
                "Failed to deserialize JSON payload.");
        }

        [Fact]
        public void IsLogListSerializationInvalid()
        {
            var serialization = System.IO.File.ReadAllText(@"./TestFiles/InvalidLogList.json");
            var deserialization = new LogEntryList().Deserialize(serialization);
            Assert.True(
                deserialization == null,
                "The deserialization result should be null since the input file has an invalid log list (missing the MachineName field).");
        }

        [Fact]
        public void IsLogListSerializationValid()
        {
            var logList = new[] { new LogEntry(), new LogEntry(), new LogEntry() };
            var logs = new LogEntryList
            {
                LogEntries = logList
            };

            var serialization = logs.Serialize();
            Assert.True(
                !string.IsNullOrEmpty(serialization),
                "Failed to serialize log entries.");

            var deserialization = new LogEntryList().Deserialize(serialization);
            Assert.True(
                deserialization != null,
                "Failed to deserialize JSON payload.");
        }
    }
}
