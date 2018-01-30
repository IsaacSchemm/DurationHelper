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
using System.Linq;

namespace FunctionApp
{
    public static class GetDurationById
    {
        [FunctionName("get-duration-by-id")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try {
                if (string.IsNullOrEmpty(req.Query["provider"])) {
                    return new BadRequestErrorMessageResult("The provider parmeter is required.");
                }
                if (string.IsNullOrEmpty(req.Query["id"])) {
                    return new BadRequestErrorMessageResult("The id parmeter is required.");
                }
                
                return new OkObjectResult((await Duration.GetAsync(
                    req.Query["provider"],
                    req.Query["id"],
                    youTubeKey: Environment.GetEnvironmentVariable("YouTubeKey")
                ))?.TotalSeconds);
            } catch (YouTubeException ex) {
                return new StatusCodeResult(ex.Reasons.Contains("quotaExceeded")
                    ? 429
                    : 502);
            } catch (WebException) {
                return new StatusCodeResult(502);
            } catch (Exception) {
                return new InternalServerErrorResult();
            }
        }
    }
}
