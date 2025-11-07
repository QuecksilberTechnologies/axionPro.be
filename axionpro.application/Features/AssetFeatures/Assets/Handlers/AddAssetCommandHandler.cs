using AutoMapper;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.AssetFeatures.Assets.Commands;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

public class AddAssetCommandHandler : IRequestHandler<AddAssetCommand, ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddAssetCommandHandler> _logger;

    public AddAssetCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AddAssetCommandHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>> Handle(AddAssetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request?.dto == null)
            {
                return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>.Fail("Invalid request or missing asset creation.");
            }

            await _unitOfWork.BeginTransactionAsync();

            var pagedAssets = await _unitOfWork.AssetRepository.AddAssetAsync(request.dto);

            if (pagedAssets == null || pagedAssets.Items == null || !pagedAssets.Items.Any())
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>.Fail("Failed to add asset or no assets found.");
            }

            await _unitOfWork.CommitTransactionAsync();

            return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>.Success(pagedAssets, "Asset created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the asset creation request.");
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<PagedResponseDTO<GetAssetResponseDTO>>.Fail("An error occurred while processing the request.");
        }
    }
}
