using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using Microsoft.AspNetCore.DataProtection;
using SfmcCustomActivities.Services;
using System.Text;
using JWT.Exceptions;

namespace SfmcCustomActivities.Helpers
{
    public class JwtRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _log;

        public JwtRequestMiddleware(RequestDelegate next, ILogger<JwtRequestMiddleware> log)
        {
            _next = next;
            _log = log;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request.ContentType == "application/jwt" && SmsSettings.Instance.JWTEnabled)
            {
                _log.LogInformation($"Received request of type {context.Request.ContentType}");
                var decodedBody = await JwtVerified(context);
                if(!string.IsNullOrEmpty(decodedBody)) 
                {
                    _log.LogInformation("JwtVerified.");
                    var jsonBody = new StringContent(decodedBody, Encoding.UTF8, "application/json");
                    var stream = jsonBody.ReadAsStream();
                    context.Request.Body = stream;
                    context.Request.ContentType = "application/json";
                    context.Request.ContentLength = stream.Length;
                }
            }

            await _next(context);
        }

        private async Task<string> JwtVerified(HttpContext context)
        {
            string result = "";
            var stream = context.Request.Body;
            if (stream == null)
                return result;

            var reqBody = await new StreamReader(stream).ReadToEndAsync();
            _log.LogDebug($"Verifying received JWT: {reqBody}");

            if (string.IsNullOrEmpty(SmsSettings.Instance.JWTSecret))
                return result;

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                JwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                var byteSecret = Encoding.UTF8.GetBytes(SmsSettings.Instance.JWTSecret);
                byte[][] byteKey = new byte[1][];
                byteKey[0] = byteSecret;

                var token = new JwtParts(reqBody);
                result = decoder.Decode(token, byteKey, true);
            }
            catch (SignatureVerificationException e)
            {
                _log.LogError("Token has invalid signature", e);
            }
            catch (Exception e)
            {
                _log.LogError($"Exception in JwtVerification: {e.Message}", e);
            }
            return result;
        }
    }
}
