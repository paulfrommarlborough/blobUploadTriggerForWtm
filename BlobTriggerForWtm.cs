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
            _logger.LogInformation($"C# Blob trigger function Processed blob1\n Name: {name} \n Data: {content}");
     
        }        
    

    public static async Task SendEmailWithAttachmentAsync(Stream attachmentStream, string attachmentName)
    {
        string sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        string senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
        string recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");

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

    private static byte[] ReadFully(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    }
}
