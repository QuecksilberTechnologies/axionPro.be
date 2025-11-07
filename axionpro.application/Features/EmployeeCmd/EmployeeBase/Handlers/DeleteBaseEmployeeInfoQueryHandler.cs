using AutoMapper;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Queries;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class DeleteBaseEmployeeInfoQueryHandler : IRequestHandler<DeleteEmployeeQuery, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBaseEmployeeInfoQueryHandler> _logger;

        public DeleteBaseEmployeeInfoQueryHandler(
            IBaseEmployeeRepository employeeRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            ILogger<DeleteBaseEmployeeInfoQueryHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ApiResponse<bool>> Handle(DeleteEmployeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return ApiResponse<bool>.Fail("Unauthorized: User ID not found in token.");

                var loginId = userIdClaim.Value;
                //      loginId = "embedded.deepesh@gmail.com";



                bool Issuccess = true;
                    
                    //await _unitOfWork.Employees.DeleteAsync(request.Id);
                if (!Issuccess)
                    return ApiResponse<bool>.Fail("Employee not found for current user.");

                return new ApiResponse<bool>
                {

                    IsSucceeded = true,
                    Message = $"successfully deleted" ,
                    Data = Issuccess


                };            

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching self employee info.");
                return ApiResponse<bool>.Fail("Something went wrong.", new List<string> { ex.Message });
            }
        }


    }



}


