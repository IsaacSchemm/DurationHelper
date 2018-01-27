
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;

namespace FunctionApp
{
    public static class Function1
    {
        [FunctionName("get-duration")]
        public static async Task<double?> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            if (!Uri.TryCreate(req.Query["url"], UriKind.Absolute, out Uri uri)) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return (await DurationHelper.MP4.GetDurationAsync(uri))?.TotalSeconds;
        }
    }
}
