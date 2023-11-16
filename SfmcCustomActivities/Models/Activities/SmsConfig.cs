﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SfmcCustomActivities.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SfmcCustomActivities.Models.Activities
{
    public static class SmsConfig
    {

        public static JsonValue GetConfigJson(HttpContext httpContext, IWebHostEnvironment env)
        {
            string pathUri = httpContext.Request.Path;
            pathUri = pathUri.Substring(0, pathUri.LastIndexOf('/'));

            string host = string.Empty;
            var fqdnHost = httpContext.Request.Host;
            if (fqdnHost.Port == 443)
                host = fqdnHost.Host;
            else
                host = $"{fqdnHost.Host}:{fqdnHost.Port}";

            var json = JsonValue.Create(
                new
                {
                    workflowApiVersion = "1.1",
                    metaData = new JsonObject()
                    {
                        ["icon"] = "images/twilio-icon.png",
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
                                    ["status"] = String.Empty
                                },
                                new JsonObject()
                                {
                                    ["errorCode"] = null
                                },
                                new JsonObject()
                                {
                                    ["errorMessage"] = String.Empty
                                }
                            }
                        },
                        ["url"] = $"https://{host}{pathUri}/execute",
                        ["timeout"] = 10000,
                        ["retryCount"] = 3,
                        ["retryDelay"] = 1000,
                        ["concurrentRequests"] = SmsSettings.Instance.ConcurrentRequests,
                        ["format"] = "JSON",
                        ["useJwt"] = SmsSettings.Instance.JWTEnabled,
                        ["customerKey"] = Environment.GetEnvironmentVariable("SALT_EXTERNAL_KEY")
                    },
                    configurationArguments = new JsonObject() 
                    {
                        ["publish"] = GetUrl("publish", host, pathUri),
                        ["validate"] = GetUrl("validate", host ,pathUri),
                        ["stop"] = GetUrl("validate", host, pathUri)
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
                            ["inArguments"] = new JsonArray
                            {
                                GetSchemaArg("smsKeyword", "Text"),
                                GetSchemaArg("smsPhone", "Text"),
                                GetSchemaArg("smsMessage", "Text")
                            },
                            ["outArguments"] = new JsonArray
                            {
                                GetSchemaArg("status","Text", direction: "out"),
                                GetSchemaArg("errorCode","Number", direction: "out"),
                                GetSchemaArg("errorMessage","Text", direction: "out")
                            }
                        }
                    }
                    
                }
            ) ;

            return json;

        }

        private static JsonObject GetUrl(string action, string host, string pathUri)
        {
            return new JsonObject()
            {
                [$"{action}"] = new JsonObject()
                {
                    ["url"] = $"https://{host}{pathUri}/publish"
                }
            };
        }

        private static JsonObject GetSchemaArg(string arg, string dataType, string direction = "in", string access = "visible")
        {
            return new JsonObject()
            {
                [$"arg"] = new JsonObject()
                {
                    ["dataType"] = dataType,
                    ["direction"] = direction,
                    ["access"] = access
                }
            };
        }
    }
}