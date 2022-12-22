using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SendGrid;
using NSubstitute;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RandomTestValues;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Http;

namespace MailosaurApiDemo.Tests
{
    [TestClass()]
    [TestCategory("Unit")]
    public class SendgridServiceTests
    {
        private ISendGridClient? client;
        private ISendgridService? service;
        private SendgridOptions? options;
        private ILogger<SendgridService>? logger;

        [TestInitialize]
        public void TestSetup()
        {
            options = RandomValue.Object<SendgridOptions>();
            client = Substitute.For<ISendGridClient>();
            logger = Substitute.For<ILogger<SendgridService>>();
            service = new SendgridService(Options.Create(options), client, logger);
        }

        [TestMethod()]
        public async Task SendMessageAsync_SendSucceeded_ReturnsSuccess()
        {
            // Arrange
            var dateTime = RandomValue.DateTime();
            var messageId = RandomValue.Guid();
            var headers = new HttpResponseMessage().Headers;
            var response = new Response(HttpStatusCode.OK, new StringContent(""), headers);

            client!
                .SendEmailAsync(Arg.Any<SendGridMessage>())
                .Returns(Task.FromResult(response));

            // Act
            try
            {
                await service!.SendMessageAsync(dateTime, messageId);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            logger.Received().LogInformation("Email has been sent successfully.");
        }

        [TestMethod()]
        public async Task SendMessageAsync_SendFailed_ReturnsSuccess()
        {
            // Arrange
            var dateTime = RandomValue.DateTime();
            var messageId = RandomValue.Guid();
            var headers = new HttpResponseMessage().Headers;
            var response = new Response(HttpStatusCode.NotFound, new StringContent(""), headers);

            client!
                .SendEmailAsync(Arg.Any<SendGridMessage>())
                .Returns(Task.FromResult(response));

            // Act
            try
            {
                await service!.SendMessageAsync(dateTime, messageId);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            logger.Received().LogError("Failed to send email.");
        }

    }
}