using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RandomTestValues;
using Mailosaur.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System;

namespace MailosaurApiDemo.Tests
{
    [TestClass()]
    [TestCategory("Unit")]
    public class MailosaurServiceTests
    {
        private IMailosaurClient? client;
        private MailosaurService? service;
        private ILogger<MailosaurService>? logger;

        [TestInitialize]
        public void SetupTest()
        {
            //var env = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
            //var configBuilder = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .AddJsonFile($"appsettings.{env}.json", optional: true)
            //    .Build();

            //var config = configBuilder.Get<TestSettings>();
            //client = new MailosaurClientWrapper(config.ApiKey);

            client = Substitute.For<IMailosaurClient>();
            var options = Options.Create(RandomValue.Object<MailosaurOptions>());
            logger = Substitute.For<ILogger<MailosaurService>>();

            service = new MailosaurService(options, client, logger);
        }

        [TestMethod()]
        public async Task GetMessageAsyncTest_NoExceptionThrown_ReturnsSuccess()
        {
            // Arrange
            var dateTime = RandomValue.DateTime();
            var messageId = RandomValue.Guid();
            var message = RandomValue.Object<Message>();
            client!
                .GetMessageAsync(Arg.Any<string>(), Arg.Any<SearchCriteria>(), Arg.Any<int>(), Arg.Any<DateTime?>())
                .Returns(Task.FromResult(message));

            // Act
            try
            {
                await service!.GetMessageAsync(dateTime, messageId);
            }
            catch (Exception)
            {
                // Assert
                Assert.Fail();
            }

            logger.Received().LogInformation("Email has been received successfully.");
        }

        [TestMethod()]
        public async Task GetMessageAsyncTest_MailosaurExceptionThrown_ReturnsSuccess()
        {
            // Arrange
            var dateTime = RandomValue.DateTime();
            var messageId = RandomValue.Guid();
            var message = RandomValue.Object<Message>();
            client!
                .GetMessageAsync(Arg.Any<string>(), Arg.Any<SearchCriteria>(), Arg.Any<int>(), Arg.Any<DateTime?>())
                .Returns(Task.FromException<Message>(new MailosaurException()));

            // Act
            try
            {
                await service!.GetMessageAsync(dateTime, messageId);
            }
            catch (Exception)
            {
                // Assert
                Assert.Fail();
            }

            logger.Received().LogError(Arg.Any<MailosaurException>(), "No message received.");
        }

        [TestMethod()]
        public async Task GetMessageAsyncTest_SystemExceptionThrown_ReturnsSuccess()
        {
            // Arrange
            var dateTime = RandomValue.DateTime();
            var messageId = RandomValue.Guid();
            var message = RandomValue.Object<Message>();
            client!
                .GetMessageAsync(Arg.Any<string>(), Arg.Any<SearchCriteria>(), Arg.Any<int>(), Arg.Any<DateTime?>())
                .Returns(Task.FromException<Message>(new Exception()));

            // Act
            try
            {
                await service!.GetMessageAsync(dateTime, messageId);
            }
            catch (Exception)
            {
                // Assert
                Assert.Fail();
            }

            logger.Received().LogError(Arg.Any<Exception>(), "Oops, something went wrong.");
        }

    }
}