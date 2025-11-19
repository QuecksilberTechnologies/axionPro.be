using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class EmployeeCompletionRepository : IEmployeeCompletionRepository
    {
        //private readonly IBankCompletionService _bankService;
        //private readonly IContactCompletionService _contactService;
        //private readonly IImageCompletionService _imageService;
        //private readonly IExperienceCompletionService _experienceService;

        //public EmployeeCompletionService(
        //    IBankCompletionService bankService,
        //    IContactCompletionService contactService,
        //    IImageCompletionService imageService,
        //    IExperienceCompletionService experienceService
        //)
        //{
        //    _bankService = bankService;
        //    _contactService = contactService;
        //    _imageService = imageService;
        //    _experienceService = experienceService;
        //}

        //public async Task<EmployeeCompletionResponseDTO> GetEmployeeCompletionAsync(long employeeId)
        //{
        //    var result = new EmployeeCompletionResponseDTO
        //    {
        //        EmployeeId = employeeId
        //    };

        //    result.Sections.Add(await _bankService.GetBankCompletionAsync(employeeId));
        //    result.Sections.Add(await _contactService.GetContactCompletionAsync(employeeId));
        //    result.Sections.Add(await _imageService.GetImageCompletionAsync(employeeId));
        //    result.Sections.Add(await _experienceService.GetExperienceCompletionAsync(employeeId));

        //    return result;
        //}
        public Task<EmployeeCompletionResponseDTO> GetEmployeeCompletionAsync(long employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
