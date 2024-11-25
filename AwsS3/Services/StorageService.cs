using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using AwsS3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3.Services
{
    public class StorageService : IStorageService
    {
        public async Task<S3ResponseDto> UploadFileAsync(S3Object s3obj, AwsCredentials awsCredentials)
        {
            var credentials = new BasicAWSCredentials(awsCredentials.AwsKey, awsCredentials.AwsSecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSouth1
            };

            var response = new S3ResponseDto();

            try
            {
                //Create the upload request
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = s3obj.InputStream,
                    Key = s3obj.Name,
                    BucketName = s3obj.BucketName,
                    CannedACL = S3CannedACL.NoACL,

                };

                // Created an S3 client
                using var client = new AmazonS3Client(credentials, config);

                //Upload utility to S3
                var transferUtility =  new TransferUtility(client);

                //uploading the file to s3
                await transferUtility.UploadAsync(uploadRequest);

                //create a succesfull response
                response.StatusCode = 200;
                response.Message = $"{s3obj.Name}: has been uploaded succesfully";
            }
            catch (AmazonS3Exception ex) 
            {
                response.StatusCode = (int)ex.StatusCode;
                response.Message = ex.Message;
                
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
