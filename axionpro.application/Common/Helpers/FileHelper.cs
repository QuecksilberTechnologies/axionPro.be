using axionpro.application.Interfaces.IFileStorage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Saves an asset image (can handle Base64, URL, or local file path).
        /// </summary>
        public static async Task<string> SaveAssetImageAsync(
            string assetImagePath,
            string destinationPath,
            IFileStorageService fileStorageService,
            ILogger logger)
        {
            try
            {
                byte[] bytes;

                if (string.IsNullOrEmpty(assetImagePath))
                {
                    logger.LogWarning("Asset image path is null or empty.");
                    return fileStorageService.GetDefaultImagePath();
                }

                if (assetImagePath.StartsWith("data:image"))
                {
                    // 📸 Base64 format (from UI)
                    var base64Data = assetImagePath.Substring(assetImagePath.IndexOf(',') + 1);
                    bytes = Convert.FromBase64String(base64Data);
                }
                else if (assetImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // 🌐 From remote URL
                    using var httpClient = new HttpClient();
                    bytes = await httpClient.GetByteArrayAsync(assetImagePath);
                }
                else if (File.Exists(assetImagePath))
                {
                    // 💾 From local system path
                    bytes = await File.ReadAllBytesAsync(assetImagePath);
                }
                else
                {
                    logger.LogWarning("Invalid asset image path: {Path}", assetImagePath);
                    return fileStorageService.GetDefaultImagePath();
                }

                // 🧱 Save the file
                var fileName = Path.GetFileName(destinationPath);
                var folderPath = Path.GetDirectoryName(destinationPath);

                return await fileStorageService.SaveFileAsync(bytes, fileName, folderPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving asset image");
                return fileStorageService.GetDefaultImagePath();
            }
        }
    }

}
