
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using axionpro.application.Interfaces.IFileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace axionpro.infrastructure.FileStoringService;

public class AWSFileService : IFileServiceAWS
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public AWSFileService(IConfiguration configuration)
    {
        var accessKey = configuration["AWSUploads:AccesskeyID"];
        var secretKey = configuration["AWSUploads:Secretaccesskey"];
        var region = configuration["AWSUploads:Region"];
        _bucketName = configuration["AWSUploads:BucketName"]!;


        _s3Client = new AmazonS3Client(
            accessKey,
            secretKey,
            RegionEndpoint.GetBySystemName(region)
        );

    }
    public async Task<string> UploadFileAsync(IFormFile file, string folderPath,string fileName)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is empty.");

        var fileExtension = Path.GetExtension(file.FileName);
       // var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var key = $"{folderPath}/{fileName}{fileExtension}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(request);

        return key;
    }
    public string GetEmployeeFolderPath(long tenantId, long employeeId, string subFolder)
    {
        if (tenantId <= 0)
            throw new ArgumentException("Invalid tenantId");

        if (employeeId <= 0)
            throw new ArgumentException("Invalid employeeId");

        if (string.IsNullOrWhiteSpace(subFolder))
            throw new ArgumentException("Invalid subFolder");

        return $"tenant-{tenantId}/employees/{employeeId}/{subFolder}";
    }

    public async Task<bool> DeleteFileAsync(string fileKey)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileKey
        };

        var response = await _s3Client.DeleteObjectAsync(request);
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent ||
               response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}
