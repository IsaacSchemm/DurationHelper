using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using DurationHelper;
using Microsoft.AspNetCore.Mvc;
using DurationHelper.Credentials;

namespace FunctionApp
{
    public static class GetDuration
    {
        [FunctionName("get-duration")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try {
                if (!Uri.TryCreate(req.Query["url"], UriKind.Absolute, out Uri uri)) {
                    return new BadRequestErrorMessageResult("The url parmeter is required.");
                }
                
                return new OkObjectResult((await Duration.GetAsync(
                    uri,
                    youTubeKey: Environment.GetEnvironmentVariable("YouTubeKey"),
                    twitchCredentials: new TwitchClientCredentials(
                        Environment.GetEnvironmentVariable("TwitchClientId"),
                        Environment.GetEnvironmentVariable("TwitchClientSecret"))
                ))?.TotalSeconds);
            } catch (WebException) {
                return new StatusCodeResult(502);
            } catch (Exception) {
                return new InternalServerErrorResult();
            }
        }
    }
}
