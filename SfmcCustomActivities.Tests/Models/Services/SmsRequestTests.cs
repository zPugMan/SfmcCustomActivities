using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using SfmcCustomActivities.Helpers;
using SfmcCustomActivities.Models.Activities;
using SfmcCustomActivities.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;

namespace SfmcCustomActivities.Tests.Models.Services
{
    [TestClass]
    public class SmsRequestTests
    {
        private readonly ILogger _log = new NullLogger<SmsRequestTests>();

        [TestMethod]
        public void SmsRequestIsValid()
        {
            //arrange
            var execReq = TestSmsExecute();
            var mockConf = new Mock<IConfiguration>();

            //act
            var req = new SmsRequest(mockConf.Object, _log, execReq);

            //assert
            Assert.IsTrue(req.IsValid(), $"SmsRequest constructor using {nameof(SmsExecute)} should be valid");
        }

        [TestMethod]
        public void SmsRequest_ConfirmMapping()
        {
            //arrange
            var execReq = TestSmsExecute();
            var mockConf = new Mock<IConfiguration>();

            //act
            var req = new SmsRequest(mockConf.Object, _log, execReq);

            //assert
            Assert.IsTrue(string.Equals(execReq.InArguments![0].SmsPhone,req.ToPhone), $"Phone requested does not match outbound phone.");
            Assert.IsTrue(string.Equals(execReq.InArguments![0].SmsMessage, req.Message), $"Message provided does not match");
            Assert.IsTrue(string.Equals(req.TrackingID, execReq.ActivityInstanceId), "TrackingID does match provided ActivityInstanceIdd");
        }

        [TestMethod]
        public void SmsRequestIsValid_False()
        {
            //arrange
            var execReq = TestSmsExecute();
            var args = new ExecuteInputs()
            {
                SmsKeyword = "test",
                SmsPhone = "8675409123",
                SmsMessage = string.Empty
            };

            execReq.InArguments!.Clear();
            execReq.InArguments.Add(args);

            var mockConf = new Mock<IConfiguration>();

            //act
            var req = new SmsRequest(mockConf.Object, _log, execReq);

            //assert
            Assert.IsFalse(req.IsValid(), "SmsRequest should be invalid with no SmsMessage value provided");
        }

        [TestMethod]
        public void SmsRequestIsValid_False_InvalidPhoneLength()
        {
            //arrange
            var execReq = TestSmsExecute();
            var args = new ExecuteInputs()
            {
                SmsKeyword = "test",
                SmsPhone = "86754091",
                SmsMessage = "test message"
            };

            execReq.InArguments!.Clear();
            execReq.InArguments.Add(args);

            var mockConf = new Mock<IConfiguration>();

            //act
            var req = new SmsRequest(mockConf.Object, _log, execReq);

            //assert
            Assert.IsFalse(req.IsValid(), $"SmsRequest should be invalid provided phone length {args.SmsPhone.Length}");
        }

        private SmsExecute TestSmsExecute()
        {
            return new SmsExecute()
            {
                ActivityId = Guid.NewGuid().ToString(),
                ActivityObjectId = Guid.NewGuid().ToString(),
                ActivityInstanceId = Guid.NewGuid().ToString(),
                DefinitionInstanceId = Guid.NewGuid().ToString(),
                JourneyId = Guid.NewGuid().ToString(),
                KeyValue = "random@email.com",
                Mode = 0,
                InArguments = new List<ExecuteInputs>()
                {
                    new ExecuteInputs()
                    {
                        SmsKeyword = "SMSKeyWord",
                        SmsPhone = "5558675309",
                        SmsMessage = "some rando message"
                    }
                }
            };
        }
    }
}
