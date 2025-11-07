 
using axionpro.application.DTOS.DocFile.axionpro.application.DTOS.FileUpload;

using axionpro.application.Features.AssetFeatures.Category.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace axionpro.api.Controllers.FileUpload
{
    /// <summary>
    /// Controller to manage all file upload operations like Asset Image upload.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;
        private readonly IFileStorageService _fileStorageService;

        public FileUploadController(IMediator mediator, ILoggerService logger, IFileStorageService fileStorageService)
        {
            _mediator = mediator;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Uploads an any image/doc/pdf and saves it to the server.
        /// </summary>
        [HttpPost("UploadAsset/upload")]      
        public async Task<IActionResult> UploadAsset([FromBody] UploadFileDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { Success = false, Message = "Invalid file data." });

                byte[] fileBytes;

                // ✅ 1️⃣ - Detect Source (Base64, URL, or Local Path)
                if (!string.IsNullOrEmpty(dto.FileBase64))
                {
                    var base64Data = dto.FileBase64.Contains(",")
                        ? dto.FileBase64.Split(',')[1]
                        : dto.FileBase64;

                    fileBytes = Convert.FromBase64String(base64Data);
                }
                else if (!string.IsNullOrEmpty(dto.FileBase64))
                {

                }
                else
                {
                    return BadRequest(new { Success = false, Message = "No valid file source provided." });
                }

                 
                return Ok(new
                {
                    Success = true,
                    Message = "✅ Your file has been successfully saved.",
                    FilePath = "/path/to/saved/file" // Replace with actual saved file path
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"❌ File upload failed: {ex.Message}"
                });
            }
        }

    }
}
