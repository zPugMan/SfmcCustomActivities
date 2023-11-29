using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SfmcCustomActivities.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SfmcCustomActivities.Models.Activities
{
    /// <summary>
    /// Forms the config.json as dictated by SFMC
    /// <br></br>Reference: <see href="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/custom-activity-config.html"/>
    /// </summary>
    public static class SmsConfig
    {

        public static JsonValue? GetConfigJson(HttpContext httpContext, IWebHostEnvironment env)
        {
            string pathUri = httpContext.Request.Path;
            pathUri = pathUri.Substring(0, pathUri.LastIndexOf('/'));

            string host = string.Empty;

            var fqdnHost = httpContext.Request.Host;
            if (env.IsProduction())
                host = fqdnHost.Host;
            else
                host = $"{fqdnHost.Host}:{fqdnHost.Port}";


            var json = JsonValue.Create(
                new
                {
                    workflowApiVersion = "1.1",
                    metaData = new JsonObject()
                    {
                        ["icon"] = "../../images/twilio-icon.png",
                        ["category"] = "message"
                    },
                    type = "REST",
                    lang = new JsonObject()
                    {
                        ["en-US"] = new JsonObject()
                        {
                            ["name"] = "Twilio",
                            ["description"] = "Sends SMS to Twilio via Azure Service Queue"
                        }
                    },
                    arguments = new JsonObject()
                    {
                        ["execute"] = new JsonObject()
                        {
                            ["inArguments"] = new JsonArray()
                            {
                                new JsonObject()
                                {
                                    ["smsKeyword"] = String.Empty
                                },
                                new JsonObject()
                                {
                                    ["smsPhone"] = String.Empty
                                },
                                new JsonObject()
                                {
                                    ["smsMessage"] = String.Empty
                                }
                            },
                            ["outArguments"] = new JsonArray()
                            {
                               new JsonObject()
                               {
                                   ["status"] = string.Empty
                               },
                               new JsonObject()
                               {
                                   ["errorCode"] = 0
                               },
                               new JsonObject()
                               {
                                   ["errorMessage"] = string.Empty
                               }
                            },
                            ["url"] = $"https://{host}/Activities/api/SmsApi/execute",
                            ["timeout"] = 10000,
                            ["retryCount"] = 3,
                            ["retryDelay"] = 1000,
                            ["concurrentRequests"] = SmsSettings.Instance.ConcurrentRequests,
                            ["format"] = "JSON",
                            ["useJwt"] = SmsSettings.Instance.JWTEnabled,
                            ["customerKey"] = SmsSettings.Instance.JWTCustomerKey
                        }
                    },
                    configurationArguments = new JsonObject() 
                    {
                        ["publish"] = GetUrl("publish", host),
                        ["validate"] = GetUrl("validate", host),
                        ["stop"] = GetUrl("validate", host)
                    },
                    userInterfaces = new JsonObject()
                    {
                        ["configurationSupportsReadOnlyMode"] = true,
                        ["configInspector"] = new JsonObject()
                        {
                            ["size"] = "scm-lg",
                            ["emptyIframe"] = true
                        }
                    },
                    schema = new JsonObject()
                    {
                        ["arguments"] = new JsonObject()
                        {
                            ["execute"] = new JsonObject()
                            {
                                ["inArguments"] = new JsonArray
                                {
                                    new JsonObject()
                                    {
                                        GetSchemaArg("smsKeyword", "Text"),
                                        GetSchemaArg("smsPhone", "Text"),
                                        GetSchemaArg("smsMessage", "Text")
                                    }
                                },
                                ["outArguments"] = new JsonArray
                                {
                                    new JsonObject()
                                    {
                                        GetSchemaArg("status","Text", direction: "out"),
                                        GetSchemaArg("errorCode","Number", direction: "out"),
                                        GetSchemaArg("errorMessage","Text", direction: "out")
                                    }
                                }
                            }
                        }
                    }
                    
                }
            ) ;

            return json;

        }

        private static JsonObject GetUrl(string action, string host)
        {
            return new JsonObject()
            {
                ["url"] = $"https://{host}/Activities/api/SmsApi/{action}"
            };
        }

        private static KeyValuePair<string, JsonNode?> GetSchemaArg(string arg, string dataType, string direction = "in", string access = "visible")
        {
            var snippet = new JsonObject()
            {
                ["dataType"] = dataType,
                ["direction"] = direction,
                ["access"] = access
            };

            return new KeyValuePair<string, JsonNode?>(arg, snippet);
        }
    }
}