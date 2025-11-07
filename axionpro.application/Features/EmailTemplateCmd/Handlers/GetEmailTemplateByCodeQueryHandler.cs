using AutoMapper;
using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.Features.EmailTemplateCmd.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmailTemplateCmd.Handlers
{
    public class GetEmailTemplateByCodeQueryHandler : IRequestHandler<GetEmailTemplateByCodeQuery, ApiResponse<EmailTemplateDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmailTemplateByCodeQueryHandler> _logger;

        public GetEmailTemplateByCodeQueryHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetEmailTemplateByCodeQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<EmailTemplateDTO>> Handle(GetEmailTemplateByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                EmailTemplate templates = await _unitOfWork.EmailTemplateRepository.GetTemplateByCodeAsync(request.Code);

                if (templates == null)
                {
                    _logger.LogWarning("No email templates found for code: {Code}", request.Code);

                    return new ApiResponse<EmailTemplateDTO>
                    {
                        IsSucceeded = false,
                        Message = $"No templates found for code: {request.Code}",
                        Data = null
                    };
                }

                var templateDTOs = _mapper.Map<EmailTemplateDTO>(templates);

                _logger.LogInformation("Successfully retrieved {Count} email templates for code: {Code}", templateDTOs, request.Code);

                return new ApiResponse<EmailTemplateDTO>
                {
                    IsSucceeded = true,
                    Message = "Email templates fetched successfully.",
                    Data = templateDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching email templates for code: {Code}", request.Code);

                return new ApiResponse<EmailTemplateDTO>
                {
                    IsSucceeded = false,
                    Message = "An error occurred while fetching email templates.",
                    Data = null
                };
            }
        }
    }
}
 