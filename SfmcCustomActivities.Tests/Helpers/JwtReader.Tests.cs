using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using JWT;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using SfmcCustomActivities.Models.Activities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SfmcCustomActivities.Tests.Helpers
{
    [TestClass]
    public class JwtReader
    {
        const string encodedJWT = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.ew0KICAiaW5Bcmd1bWVudHMiOiBbDQogICAgew0KICAgICAgInNtc0tleXdvcmQiOiAic21zLW5vZGUiLA0KICAgICAgInNtc1Bob25lIjogIjUwODMzMDk5NjAiLA0KICAgICAgInNtc01lc3NhZ2UiOiAiSGkgTWFjLi4gand0IHRlc3QgaGVyZSINCiAgICB9DQogIF0sDQogICJvdXRBcmd1bWVudHMiOiBbXSwNCiAgImFjdGl2aXR5T2JqZWN0SUQiOiAiNTMyYjBmZjQtMjQyZS00YWJmLTg4YTItNThhY2Q4MjQ2NzA3IiwNCiAgImpvdXJuZXlJZCI6ICIyMzYwZTY2Ni1mODQ2LTQxN2MtOGI4Ni04Njg2ZWU3MTk3Y2IiLA0KICAiYWN0aXZpdHlJZCI6ICI1MzJiMGZmNC0yNDJlLTRhYmYtODhhMi01OGFjZDgyNDY3MDciLA0KICAiZGVmaW5pdGlvbkluc3RhbmNlSWQiOiAiZjY0MTAzM2MtMjMzYS00YzJhLWJlMDItOWY3MmIyY2FhZmM4IiwNCiAgImFjdGl2aXR5SW5zdGFuY2VJZCI6ICI3OTM0NjQzMi1kOTgzLTQ0OWEtYjE3OS1hNzU2YmE1ZmJjMTEiLA0KICAia2V5VmFsdWUiOiAibW9rZWVmZUBzb2x2ZW5uYS5jb20iLA0KICAibW9kZSI6IDANCn0.fV3_C94TrGAM-FcPp31plg5VeJUnpl2WHGk5d2Tblb0";
        //"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.ew0KICAiaW5Bcmd1bWVudHMiOiBbDQogICAgew0KICAgICAgInNtc0tleXdvcmQiOiAic21zLW5vZGUiLA0KICAgICAgInNtc1Bob25lIjogIjUwODUyODUyNTMiLA0KICAgICAgInNtc01lc3NhZ2UiOiAiSGkgTWFjLi4gand0IHRlc3QgaGVyZSINCiAgICB9DQogIF0sDQogICJvdXRBcmd1bWVudHMiOiBbXSwNCiAgImFjdGl2aXR5T2JqZWN0SUQiOiAiNTMyYjBmZjQtMjQyZS00YWJmLTg4YTItNThhY2Q4MjQ2NzA3IiwNCiAgImpvdXJuZXlJZCI6ICIyMzYwZTY2Ni1mODQ2LTQxN2MtOGI4Ni04Njg2ZWU3MTk3Y2IiLA0KICAiYWN0aXZpdHlJZCI6ICI1MzJiMGZmNC0yNDJlLTRhYmYtODhhMi01OGFjZDgyNDY3MDciLA0KICAiZGVmaW5pdGlvbkluc3RhbmNlSWQiOiAiY2U2ZGNlNzgtYzk2Mi00ZjJlLWIxMGItZjlkODA0YjgzMGJiIiwNCiAgImFjdGl2aXR5SW5zdGFuY2VJZCI6ICIyMDllOTE5ZS1mOWYxLTQxMGYtYmQ0Zi05NmJlNzE5NTU4ZWEiLA0KICAia2V5VmFsdWUiOiAibWFjb2tlZWZlMTFAZ21haWwuY29tIiwNCiAgIm1vZGUiOiAwDQp9.wxjM64HEsgj-QigDDSdDB4hkgEJ0bV7s51UogAOV3fU";
        //"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.ew0KICAiaW5Bcmd1bWVudHMiOiBbDQogICAgew0KICAgICAgInNtc0tleXdvcmQiOiAic21zLW5vZGUiLA0KICAgICAgInNtc1Bob25lIjogIjg4NDY1OTc4OTQiLA0KICAgICAgInNtc01lc3NhZ2UiOiAiSGkgVGVzdGVyLi4gand0IHRlc3QgaGVyZSINCiAgICB9DQogIF0sDQogICJvdXRBcmd1bWVudHMiOiBbXSwNCiAgImFjdGl2aXR5T2JqZWN0SUQiOiAiNTMyYjBmZjQtMjQyZS00YWJmLTg4YTItNThhY2Q4MjQ2NzA3IiwNCiAgImpvdXJuZXlJZCI6ICIyMzYwZTY2Ni1mODQ2LTQxN2MtOGI4Ni04Njg2ZWU3MTk3Y2IiLA0KICAiYWN0aXZpdHlJZCI6ICI1MzJiMGZmNC0yNDJlLTRhYmYtODhhMi01OGFjZDgyNDY3MDciLA0KICAiZGVmaW5pdGlvbkluc3RhbmNlSWQiOiAiODAwYzMwZTYtZGVmZC00NTlmLTgxMDAtYzA3Zjk3ZmFhMGQyIiwNCiAgImFjdGl2aXR5SW5zdGFuY2VJZCI6ICIxNDgwNTFhMC0yODVkLTQ1YzMtYjZkMy0yZTJhY2RmNTllZjciLA0KICAia2V5VmFsdWUiOiAidGVzdGVtYWlsMTIzQGdtYWlsLmNvbSIsDQogICJtb2RlIjogMA0KfQ.q9l1LvGqrSdWQNGaG7e0D7f8KUMQ7Pe_gf1FpBeFDsw";
        const string secret = "";

        [TestMethod]
        public void JwtDecodeVerifyTest()
        {
            //try
            //{
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            JwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
            var byteSecret = Encoding.UTF8.GetBytes(secret);
            byte[][] byteKey = new byte[1][];
            byteKey[0] = byteSecret;
                
            var token = new JwtParts(encodedJWT);
            var json = decoder.Decode(token, byteKey, true);
                Console.WriteLine(json);
            //}
            //catch (TokenNotYetValidException)
            //{
            //    Console.WriteLine("Token is not valid yet");
            //}
            //catch (TokenExpiredException)
            //{
            //    Console.WriteLine("Token has expired");
            //}
            //catch (SignatureVerificationException)
            //{
            //    Console.WriteLine("Token has invalid signature");
            //}
        }

        [TestMethod]
        public void JwtDecryptTest()
        {
            var tokeHndl = new JwtSecurityTokenHandler();
            string result = null;
            SecurityToken securityToken= null;

            var validationParms = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = true,
                //ValidateIssuerSigningKey = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                
            };

            var val = tokeHndl.ValidateToken(encodedJWT, validationParms, out securityToken);

            //tokeHndl.ValidateToken(encodedJWT, validationParms, out result);
            var token = tokeHndl.ReadJwtToken(encodedJWT);
            //var tokenHeader = token.Header.Base64UrlEncode();
            //var tokenPayload = token.Payload.Base64UrlEncode();

            ////tokeHndl.ValidateToken(encodedJWT, validationParms, out result);
            //using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            //{
            //    byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{tokenHeader}.{tokenPayload}"));
            //    result = Convert.ToBase64String(hashBytes);
            //}
                //    byte[] messageBytes = Encoding.UTF8.GetBytes($"{token.RawSignature}");
                //    byte[] signature = hmac.ComputeHash(messageBytes);
                //    var signHash = Convert.ToBase64String(signature);

                //    if (signHash == token.Payload.Base64UrlEncode())
                //    {
                //        var r = "yes";
                //    } else
                //    {
                //        var y = "no";
                //    }
                //}
                Assert.IsNotNull(token);
        }

        private string EncodedJwtPayload()
        {

            var json = JsonSerializer.Serialize<SmsExecute>(TestRequest());

            return json;
        }

        private SmsExecute TestRequest()
        {
            var result = new SmsExecute();
            result.ActivityObjectId = "09f2e3b3-224c-431f-b8c2-abd3bd59babd";
            result.JourneyId = "e717911b-9238-444d-af4a-582f9498bece";
            result.ActivityId= "09f2e3b3-224c-431f-b8c2-abd3bd59babd";
            result.DefinitionInstanceId = "69ec7f06-8c46-448b-a7a6-c354b29273ab";
            result.ActivityInstanceId = "ae92ce9f-3a6c-4a5d-8d50-6ca47d98216c";
            result.KeyValue = "testemail123@gmail.com";
            result.Mode = 0;

            result.InArguments = new List<ExecuteInputs>() { 
                new ExecuteInputs(){ SmsKeyword = "TEST", SmsMessage = "Hi there", SmsPhone = "5558675309" }
            };

            return result;
        }
    }
}
