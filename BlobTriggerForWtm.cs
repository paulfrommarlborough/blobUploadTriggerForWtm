using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

// isolated worker

namespace Company.Function
{
    public class BlobTriggerForWtm
    {
        private readonly ILogger<BlobTriggerForWtm> _logger;

        public BlobTriggerForWtm(ILogger<BlobTriggerForWtm> logger)
        {
            _logger = logger;
        }

        [Function(nameof(BlobTriggerForWtm))]
        public async Task Run([BlobTrigger("index-uploads/{name}", Connection = "wtmstorageaccount0_STORAGE")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob add sendgrid\n Name: {name} \n Data: {content}");
     
        }        

        public static async Task SendEmailWithAttachmentAsync(Stream attachmentStream, string attachmentName)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var client = new SendGridClient(sendGridApiKey);
            var message = new SendGridMessage();
            message.SetFrom(new EmailAddress(senderEmail));
            message.AddTo(new EmailAddress(recipientEmail));
            message.SetSubject("Blob Uploaded");
            message.AddAttachment(attachmentName, Convert.ToBase64String(ReadFully(attachmentStream)), "application/octet-stream");

            var response = await client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
