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
    public static class ParseYouTubeUrl
    {
        [FunctionName("parse-youtube-url")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try {
                if (!Uri.TryCreate(req.Query["url"], UriKind.Absolute, out Uri uri)) {
                    return new BadRequestErrorMessageResult("The url parmeter is required.");
                }

                return new OkObjectResult(YouTube.ParseUrl(uri));
            } catch (YouTubeURLException ex) {
                return new BadRequestErrorMessageResult(ex.Message);
            } catch (Exception) {
                return new InternalServerErrorResult();
            }
        }
    }
}
