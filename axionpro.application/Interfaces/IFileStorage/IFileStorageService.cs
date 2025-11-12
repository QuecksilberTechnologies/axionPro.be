using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IFileStorage
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(byte[] fileData, string fileName, string folderPath);
        Task<byte[]> GetFileAsync(string filePath);

        string GetTenantFolderPath(long? tenantId, string subFolder);
        string GenerateFilePath(long? tenantId, string subFolder, string fileName);
        string GenerateFilePath(string? tenantId, string fileName);
        string GetPublicUrl(string relativePath);
        string GetRelativePath(string fullPath);

        // ✅ Add these
        string GetDefaultImagePath();
        string GetDefaultImageUrl();

    }
}
