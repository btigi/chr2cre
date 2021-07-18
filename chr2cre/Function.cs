using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace chr2cre
{
    public static class Function
    {
        [FunctionName("Index")]
        public static async Task<HttpResponseMessage> Index(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"{nameof(Function.Index)} called");

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var fileLocation = Path.Combine(context.FunctionAppDirectory, "index.html");
            if (File.Exists(fileLocation))
            {
                var content = await File.ReadAllTextAsync(fileLocation);
                response.Content = new StringContent(content);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }

            log.LogInformation($"Index file does not exist");
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Content = new StringContent("Project dependencies not found");
            return response;
        }

        [FunctionName("FileProcessFunction")]
        public static async Task<IActionResult> FileProcessFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            const int MaxFileSize = 1024 * 10;
            const int BufferReadSize = 1024;
            const int CHRHeaderSize = 0x64;

            log.LogInformation($"{nameof(Function.FileProcessFunction)} called");

            var message = String.Empty;
            try
            {
                var formdata = await req.ReadFormAsync();
                var file = req.Form.Files["file"];

                message = "No file selected";
                if (file != null)
                {
                    var filesize = file.Length;
                    var creFilesize = filesize - CHRHeaderSize;

                    // CHR files should only be a few Kb, so we'll put an explicit check to avoid someone upload wanting us to process a 10gb file
                    message = $"Filesize above allowed maximum of {MaxFileSize} bytes";
                    if (filesize <= MaxFileSize)
                    {
                        using Stream s = file.OpenReadStream();
                        s.Position = 0x64;

                        var bytes = new byte[creFilesize];
                        var bytesRead = 0;
                        while (bytesRead < creFilesize)
                        {
                            bytesRead += await s.ReadAsync(bytes, bytesRead, BufferReadSize);
                        }

                        message = $"Read {bytesRead} bytes from file expected {creFilesize} bytes";
                        if (bytesRead == creFilesize)
                        {
                            message = "Conversion successful";
                            var encodedString = Convert.ToBase64String(bytes);
                            log.LogInformation($"Successful conversion");
                            return new OkObjectResult(new Data() { File = bytes, Filename = Path.GetFileNameWithoutExtension(file.FileName) + ".cre", Message = message, Result = true });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"Conversion Error {ex.Message}");
                return new BadRequestObjectResult(ex);
            }

            log.LogInformation("Unsuccessful conversion");
            return new OkObjectResult(new Data() { File = null, Filename = String.Empty, Message = message, Result = false });
        }

        public class Data
        {
            public bool Result { get; set; }
            public string Message { get; set; }
            public string Filename { get; set; }
            public byte[] File { get; set; }
        }
    };
}