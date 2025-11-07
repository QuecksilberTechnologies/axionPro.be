using AutoMapper;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using axionpro.application.Features.UserLoginAndDashboardCmd.Commands;
using axionpro.domain.Entity;
using axionpro.application.DTOs.Registration;

namespace axionpro.application.Features.UserLoginAndDashboardCmd.Handlers
{

    public class CandidateRegistrationCommandHandler : IRequestHandler<CandidateRegistrationCommand, ApiResponse<CandidateResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CandidateRegistrationCommandHandler> _logger;

        public CandidateRegistrationCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<CandidateRegistrationCommandHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<CandidateResponseDTO>> Handle(CandidateRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                CandidateResponseDTO candidateResponse = new CandidateResponseDTO();

                // Validate the request
                if (request == null || request.RequestCandidateRegistrationDTO == null)
                {
                    return new ApiResponse<CandidateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Invalid request or missing CandidateRegistrationDTO."
                    };
                }

                CandidateRequestDTO candidateResponseDTO = _mapper.Map<CandidateRequestDTO>(request.RequestCandidateRegistrationDTO);

                // Check for duplicate data
                var isBlacklistedOrDuplicate = await _unitOfWork.CandidatesRegistrationRepository.IsEmailPANAdharPhoneExistsAsync(candidateResponseDTO);
                if (isBlacklistedOrDuplicate)
                {
                    return new ApiResponse<CandidateResponseDTO>
                    {
                        IsSucceeded = false,
                        Message = "Data already exists."
                    };
                }

   

                try
                {
                    List<int> skillSetList = candidateResponseDTO.SkillSet.Split(',').Select(int.Parse).ToList();
                    Candidate cc = _mapper.Map<Candidate>(request.RequestCandidateRegistrationDTO);

                    // 🔹 Step 1: Save Candidate
                    var id = await _unitOfWork.CandidatesRegistrationRepository.AddCandidateAsync(cc);

                    // 🔹 Step 2: Save Candidate Skills
                    List<CandidateCategorySkill> candidateSkillsList = skillSetList.Select(skill => new CandidateCategorySkill
                    {
                        CandidateId = id,
                        CategoryId = skill,
                        AddedDateTime = DateTime.Now,
                        IsActive = true
                    }).ToList();

                    var numberOfRecordInserted = await _unitOfWork.CandidateCategorySkillRepository.AddSkillsAsync(candidateSkillsList);

                    // 🔹 Step 3: Commit Transaction
                    await _unitOfWork.CommitTransactionAsync();

                    return new ApiResponse<CandidateResponseDTO>
                    {
                        IsSucceeded = true,
                        Message = "Candidate registration successful.",
                        Data = new CandidateResponseDTO { Success = true, CandidateId = id }
                    };
                }
                catch (Exception ex)
                {
                    // 🔹 Rollback if any step fails
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, "Transaction rolled back due to an error.");
                    return new ApiResponse<CandidateResponseDTO> { IsSucceeded = false, Message = "Transaction failed." };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the registration request.");
                return new ApiResponse<CandidateResponseDTO> { IsSucceeded = false, Message = "An error occurred while processing the request." };
            }
        }



    }
}
