using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using DurationHelper;

namespace FunctionApp
{
    public static class GetDuration
    {
        [FunctionName("get-duration")]
        public static async Task<double?> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            if (!Uri.TryCreate(req.Query["url"], UriKind.Absolute, out Uri uri)) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return (await Duration.GetAsync(uri))?.TotalSeconds;
        }
    }
}
