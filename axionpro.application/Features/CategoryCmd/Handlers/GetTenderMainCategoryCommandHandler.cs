using AutoMapper;
using axionpro.application.DTOs.Category;
using axionpro.application.Features.CategoryCmd.Command;
using axionpro.application.Interfaces;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.CategoryCmd.Handlers
{

    public class GetTenderMainCategoryRequestHandler : IRequestHandler<GetTenderMainCategoryCommand, ApiResponse<List<TenderCategoryResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTenderMainCategoryRequestHandler> _logger;

        public GetTenderMainCategoryRequestHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetTenderMainCategoryRequestHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        //public GetTenderMainCategoryRequestHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetTenderMainCategoryRequestHandler> logger)
        //{
        //    _mapper = mapper;
        //    _unitOfWork = unitOfWork;
        //    _logger = logger;
        //}

        public Task<ApiResponse<List<TenderCategoryResponseDTO>>> Handle(GetTenderMainCategoryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
