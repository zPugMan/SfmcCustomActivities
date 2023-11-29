using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JWT.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using Moq.Contrib.HttpClient;
using SfmcCustomActivities.Helpers;
using SfmcCustomActivities.Models.Activities;
using SfmcCustomActivities.Services;


namespace SfmcCustomActivities.Tests.Helpers
{
    [TestClass]
    public class JwtRequestMiddlewareTests
    {
        // generate new jwtString values via: https://jwt.io/

        const string jwtString = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpbkFyZ3VtZW50cyI6W3sic21zS2V5d29yZCI6IkhpdFVQISIsInNtc1Bob25lIjoiODg0NjU5Nzg5NCIsInNtc01lc3NhZ2UiOiJIaSB0aGVyZSBUZXN0ZXIhIFRoaXMgaXMgYSB0ZXN0LiJ9XSwib3V0QXJndW1lbnRzIjpbXSwiYWN0aXZpdHlPYmplY3RJRCI6IjA5ZjJlM2IzLTIyNGMtNDMxZi1iOGMyLWFiZDNiZDU5YmFiZCIsImpvdXJuZXlJZCI6ImU3MTc5MTFiLTkyMzgtNDQ0ZC1hZjRhLTU4MmY5NDk4YmVjZSIsImFjdGl2aXR5SWQiOiIwOWYyZTNiMy0yMjRjLTQzMWYtYjhjMi1hYmQzYmQ1OWJhYmQiLCJkZWZpbml0aW9uSW5zdGFuY2VJZCI6IjY5ZWM3ZjA2LThjNDYtNDQ4Yi1hN2E2LWMzNTRiMjkyNzNhYiIsImFjdGl2aXR5SW5zdGFuY2VJZCI6ImFlOTJjZTlmLTNhNmMtNGE1ZC04ZDUwLTZjYTQ3ZDk4MjE2YyIsImtleVZhbHVlIjoidGVzdGVtYWlsMTIzQGdtYWlsLmNvbSIsIm1vZGUiOjB9.0UMF38b70N0VhJmVugjTd9f_dso7jvf-gtqF6iXKWFw";
        private readonly ILogger<JwtRequestMiddleware> _log = new NullLogger<JwtRequestMiddleware>();
        const string jwtSecret = "super-test-secret";

        [TestMethod]
        public async Task InvokeAsyncWithJwt()
        {
            //arrange
            var ctx = MockedHttpContext();

            RequestDelegate reqDelegate = async (HttpContext hc) => await Task.CompletedTask;
            var jwtMiddleware = new JwtRequestMiddleware(reqDelegate, _log);
            SmsSettings.Instance.JWTEnabled = true;
            SmsSettings.Instance.JWTSecret = jwtSecret;

            //act
            await jwtMiddleware.InvokeAsync(ctx);
            var reqBody = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
            var jsonReq = JsonSerializer.Deserialize<SmsExecute>(reqBody);

            //assert
            Assert.IsTrue(ctx.Request.Headers.ContentLength > 0, "Request content length should be greater than 0");
            Assert.IsTrue(ctx.Request.Headers.ContentType == "application/json", $"Expecting content-type 'application/json'. Received ${ctx.Request.Headers.ContentType}");
            Assert.IsNotNull(jsonReq, $"JWT -> JSON body could not be deserialized to expected type {nameof(SmsExecute)}");
            Assert.IsTrue(jsonReq.InArguments!.First().SmsPhone == "8846597894", $"Decoded JSON {nameof(ExecuteInputs.SmsPhone)} does not match expected value");
        }

        [TestMethod]
        public async Task InvokeAsyncWithJwt_Invalid()
        {
            //arrange
            var ctx = MockedHttpContext();
            RequestDelegate reqDelegate = async (HttpContext hc) => await Task.CompletedTask;
            var jwtMiddleware = new JwtRequestMiddleware(reqDelegate, _log);
            SmsSettings.Instance.JWTEnabled = true;
            SmsSettings.Instance.JWTSecret = "invalid-secret";

            //act

            //assert
            await Assert.ThrowsExceptionAsync<SignatureVerificationException>(() => jwtMiddleware.InvokeAsync(ctx), "SignatureVerificationException expected");

        }

        private HttpContext MockedHttpContext()
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.ContentType = "application/jwt";
            ctx.Request.Method = "POST";
            var bodyContent = new StringContent(jwtString);
            ctx.Request.Body = bodyContent.ReadAsStream();
            ctx.Request.Path = "/Activities/api/SmsApi/execute";
            ctx.Request.Scheme = "https";

            return ctx;
        }
    }
}
