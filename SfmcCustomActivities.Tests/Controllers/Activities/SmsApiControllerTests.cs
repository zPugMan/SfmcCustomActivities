using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SfmcCustomActivities.Controllers.Activities;
using SfmcCustomActivities.Models.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfmcCustomActivities.Tests.Controllers.Activities
{
    [TestClass]
    public class SmsApiControllerTests
    {
        private readonly ILogger<SmsApiController> _log = new NullLogger<SmsApiController>();

        [TestMethod]
        [DynamicData(nameof(TestRequests))]
        public async Task Execute_MissingData(SmsExecute data)
        {
            //arrange
            var mockAzure = new Mock<IAzureClientFactory<ServiceBusSender>>();
            var mockConf = new Mock<IConfiguration>();
            var controller = new SmsApiController(_log, mockAzure.Object, mockConf.Object);

            //act
            var response = await controller.Execute(data);

            //assert
            Assert.IsTrue(response.Result!.GetType() == typeof(BadRequestObjectResult), $"Response type mismatch, received {response.Result!.GetType()}");

            BadRequestObjectResult result = (BadRequestObjectResult)response.Result;
            Assert.IsTrue(result.StatusCode == 400, $"Expecting HTTP400 BadReqeust, received: {result.StatusCode}");
        }

        //[TestMethod]
        //public async Task Execute_FailedServiceBus()
        //{
        //    //arrange
        //    var request = FormRequest("5558675309", "test -message");
        //    var mockConf = new Mock<IConfiguration>();

        //    var mockAzure = new Mock<IAzureClientFactory<ServiceBusSender>>();
        //    mockAzure.Setup(x => x.CreateClient(It.IsAny<string>()))
        //        .Returns<ServiceBusSender>( res =>
        //        {
        //            var mockAzureSend = new Mock<ServiceBusSender>();
        //            mockAzureSend
        //                .Setup(y => y.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
        //                .Throws(() => new ServiceBusException("Invalid bus message", ServiceBusFailureReason.GeneralError));

        //            return mockAzureSend.Object;
        //        }
        //        );

        //    var controller = new SmsApiController(_log, mockAzure.Object, mockConf.Object);

        //    //act
        //    var response = await controller.Execute(request);

        //    //assert
        //    Assert.IsTrue(response.Result!.GetType() == typeof(StatusCodeResult), $"Response type mismatch, received {response.Result!.GetType()}");

        //    StatusCodeResult result = (StatusCodeResult)response.Result;
        //    Assert.IsTrue(result.StatusCode == 500, $"Expecting HTTP500, received {result.StatusCode}");

        //}

        private static IEnumerable<object[]> TestRequests
        {
            get
            {
                return new[]
                {
                  new object[] { FormRequest(phone: "5558675309",null)},
                  new object[] { FormRequest(phone: null,message: "hello no phone!")}
                };
            }
        }

        private static SmsExecute FormRequest(string? phone, string? message)
        {
            var req = new SmsExecute()
            {
                ActivityId = Guid.NewGuid().ToString(),
                ActivityObjectId = Guid.NewGuid().ToString(),
                ActivityInstanceId = Guid.NewGuid().ToString(),
                DefinitionInstanceId = Guid.NewGuid().ToString(),
                JourneyId = Guid.NewGuid().ToString(),
                KeyValue = "random@email.com",
                Mode = 0
            };

            req.InArguments = new List<ExecuteInputs>()
            {
                new ExecuteInputs()
                {
                    SmsKeyword = "test-key",
                    SmsPhone = phone,
                    SmsMessage = message
                }
            };

            return req;
        }
    }
}
