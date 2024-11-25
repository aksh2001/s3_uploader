using Amazon.S3.Model;
using AwsS3.Models;
using AwsS3.Services;
using Microsoft.AspNetCore.Mvc;


namespace S3FileUpload.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;

        private readonly IStorageService _service;

        private readonly IConfiguration _configuration;

        public UploadController(ILogger<UploadController> logger, IStorageService service, IConfiguration configuration)
        {
            _logger = logger;
            _service = service;
            _configuration = configuration;
        }

        [HttpPost(Name = "Uploadfile")]

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileExt = Path.GetExtension(file.Name);
            var objname = file.FileName;

            var s3Obj = new AwsS3.Models.S3Object()
            {
                BucketName = "demo-webapi-s3upload",
                InputStream = memoryStream,
                Name = objname
            };

            var cred = new AwsCredentials()
            {
                AwsKey = _configuration["AwsConfiguration:AWSAcessKey"],

                AwsSecretKey = _configuration["AwsConfiguration:AwsSecretKey"]

            };

            var result = await _service.UploadFileAsync(s3Obj, cred);

            return Ok(result);
        }

    }
}
