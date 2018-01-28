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
    public static class GetDuration
    {
        [FunctionName("get-duration")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            if (!Uri.TryCreate(req.Query["url"], UriKind.Absolute, out Uri uri)) {
                return new BadRequestErrorMessageResult("The url parmeter is required.");
            }

            try {
                return new OkObjectResult((await Duration.GetAsync(uri))?.TotalSeconds);
            } catch (YouTubeException ex) {
                return new StatusCodeResult(ex.Reasons.Contains("quotaExceeded")
                    ? 429
                    : 500);
            }
        }
    }
}
