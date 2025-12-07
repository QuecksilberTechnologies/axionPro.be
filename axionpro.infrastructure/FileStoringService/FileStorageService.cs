using axionpro.application.Interfaces.IFileStorage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.infrastructure.FileStoringService
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IConfiguration _config;

        public FileStorageService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<string> SaveFileAsync(byte[] fileData, string fileName, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);
            await File.WriteAllBytesAsync(fullPath, fileData);
            return fullPath;
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            return await File.ReadAllBytesAsync(filePath);
        }
   

        private string RootFolder => _config["FileSettings:RootFolder"] ?? string.Empty;        
        private string TenantFolder => _config["FileSettings:TenantFolder"] ?? string.Empty;
        private string DefaultFolder => _config["FileSettings:DefaultFolder"] ?? string.Empty;
        private string BaseUrl => _config["FileSettings:BaseUrl"] ?? string.Empty;
        private string DefaultImage => _config["FileSettings:DefaultImage"] ?? string.Empty;

        public string GetTenantFolderPath(string tenantId, string subFolder)
        {
            var path = Path.Combine(RootFolder, TenantFolder, tenantId, subFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
 

        public string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return string.Empty;

            var relative = fullPath.Replace(RootFolder, "").TrimStart('\\', '/');
            return $"{relative.Replace("\\", "/")}";
        }

        public string GetPublicUrl(string relativePath)
        {
            return $"{BaseUrl}uploads/{relativePath}".Replace("\\", "/");
        }

        // ✅ Default Image Path Helper
        public string GetDefaultImagePath()
        {
            var fullPath = Path.Combine(RootFolder, DefaultImage);
            if (File.Exists(fullPath))
                return fullPath;

            // fallback in case default file missing
            return Path.Combine(RootFolder, DefaultFolder, "default.png");
        }

        public string GetDefaultImageUrl()
        {
            return $"{BaseUrl}uploads/{DefaultImage}".Replace("\\", "/");
        }

        public string GetTenantFolderPath(long? tenantId, string subFolder)
        {
            if (tenantId == null)
                throw new ArgumentNullException(nameof(tenantId));
            if (string.IsNullOrEmpty(subFolder))
                throw new ArgumentNullException(nameof(subFolder));

            var rootFolder = RootFolder ?? string.Empty;
            var tenantFolder = TenantFolder ?? string.Empty;
            var tenantIdStr = tenantId?.ToString() ?? string.Empty;
            var subFolderStr = subFolder ?? string.Empty;

            var path = Path.Combine(rootFolder, tenantFolder, tenantIdStr, subFolderStr);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Use This function
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="employeeId"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetEmployeeFolderPath(long tenantId, long employeeId, string subFolder)
        {
            if (tenantId <0)
                throw new ArgumentNullException(nameof(tenantId));
            if (employeeId <0)
                throw new ArgumentNullException(nameof(employeeId));
            if (string.IsNullOrEmpty(subFolder))
                throw new ArgumentNullException(nameof(subFolder));

            // 🔹 Config values
            var rootFolder = RootFolder ?? string.Empty;
            var tenantFolder = TenantFolder ?? string.Empty;

            // 🔹 Path structure:
            // RootFolder / tenants / {tenantId} / employees / {employeeId} / {subFolder}
            var path = Path.Combine(
                rootFolder,
                tenantFolder,
                tenantId.ToString(),
                "employees",
                employeeId.ToString(),
                subFolder
            );

            // 🔹 Create folder if it doesn’t exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }



        public string GenerateFilePath(long? tenantId, string subFolder, string fileName)
        {
          //  fileName = null;
            var tenantFolder = GetTenantFolderPath(tenantId, subFolder);
          //  return Path.Combine(tenantFolder, fileName);
            return Path.Combine(tenantFolder);
        }

        public string GenerateFullFilePath(string fullPathFolder, string fileName)
        {
            if (string.IsNullOrEmpty(fullPathFolder))
                throw new ArgumentNullException(nameof(fullPathFolder));

            // 🔹 Folder exist check
            if (!Directory.Exists(fullPathFolder))
                Directory.CreateDirectory(fullPathFolder);

            // 🔹 Combine full folder path + file name
            return Path.Combine(fullPathFolder, fileName);
        }

    }


}







