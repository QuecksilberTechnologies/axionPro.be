using axionpro.application.Interfaces.IFileStorage;
using axionpro.domain.Entity;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace axionpro.infrastructure.FileStoringService
{
    public class GcsFileService : IFileService
    {
        private readonly StorageClient _storage;
        private readonly string _bucket;

        public GcsFileService(IConfiguration config)
        {
            var credPath = config["GoogleCloud:CredentialsPath"];
            _bucket = config["GoogleCloud:BucketName"];

            var credential = GoogleCredential.FromFile(credPath);
            _storage = StorageClient.Create(credential);
        }

        public async Task<string> UploadAsync(
            IFormFile file,
            string tenantEncoded,
            string employeeEncoded,
            string category)
        {
            var ext = Path.GetExtension(file.FileName);
            var randomName = $"f_{Guid.NewGuid()}{ext}";

            var objectName =
                $"uploads/{tenantEncoded}/{employeeEncoded}/{category}/{randomName}";

            using var stream = file.OpenReadStream();
            await _storage.UploadObjectAsync(
                _bucket,
                objectName,
                file.ContentType,
                stream);

            return objectName; // DB me yahi store karo
        }

        public async Task<(Stream Stream, string ContentType, string FileName)>
            DownloadAsync(string storedPath)
        {
            var memory = new MemoryStream();
            var obj = await _storage.GetObjectAsync(_bucket, storedPath);

            await _storage.DownloadObjectAsync(_bucket, storedPath, memory);
            memory.Position = 0;

            return (memory, obj.ContentType, Path.GetFileName(storedPath));
        }

        public async Task DeleteAsync(string storedPath)
        {
            await _storage.DeleteObjectAsync(_bucket, storedPath);
        }

        public async Task<string> GetSignedUrlAsync(string storedPath, TimeSpan ttl)
        {
            var signer = UrlSigner.FromServiceAccountPath(
                Path.GetFullPath(
                    AppDomain.CurrentDomain.BaseDirectory +
                    "/" +
                    "Secrets/gcs-service-account.json"));

            return await signer.SignAsync(
                _bucket,
                storedPath,
                ttl,
                HttpMethod.Get);
        }

        public Task<FileRecord> UploadAsync(IFormFile file, string tenantEncoded, string employeeEncoded, string category, bool isSensitive)
        {
            throw new NotImplementedException();
        }

        public Task<(Stream Stream, string ContentType, string FileName)> GetFileStreamAsync(Guid fileId, ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSignedUrlAsync(Guid fileId, TimeSpan ttl, ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid fileId)
        {
            throw new NotImplementedException();
        }
    }
}
