using axionpro.application.DTOS.Common;
using axionpro.application.Interfaces.IFileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers
{
    public class FileUploadHelper
    {
        private readonly IFileStorageService _fileStorageService;

        public FileUploadHelper(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<UploadDocResponseDTO> UploadDocAsync(UploadDocRequestDTO req)
        {
            var result = new UploadDocResponseDTO();

            if (req.File == null || req.File.Length == 0)
                return result;

            // ---------------------------------------
            // Determine Extension
            // ---------------------------------------
            string extension = req.FileType switch
            {
                1 => ".pdf",
                2 => ".png",
                _ => Path.GetExtension(req.File.FileName)
            };

            // ---------------------------------------
            // Generated File Name
            // PREFIX-EMPID-yyyyMMddHHmmssfff.pdf
            // ---------------------------------------
            string finalName =
                $"{req.FilePrefix}-{req.EmployeeId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";

            // ---------------------------------------
            // Convert File to Bytes
            // ---------------------------------------
            using var ms = new MemoryStream();
            await req.File.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // ---------------------------------------
            // Build Folder Path
            // ---------------------------------------
            string fullFolderPath = _fileStorageService
                .GetEmployeeFolderPath(req.TenantId, req.EmployeeId, req.SubFolderName);

            // ---------------------------------------
            // Save Physical File
            // ---------------------------------------
            string savedFullPath = await _fileStorageService
                .SaveFileAsync(bytes, finalName, fullFolderPath);

            if (string.IsNullOrEmpty(savedFullPath))
                return result;

            // ---------------------------------------
            // Success Result
            // ---------------------------------------
            result.IsUploaded = true;
            result.FileName = finalName;
            result.FilePath = _fileStorageService.GetRelativePath(savedFullPath);

            return result;
        }
    }


}
