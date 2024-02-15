using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
