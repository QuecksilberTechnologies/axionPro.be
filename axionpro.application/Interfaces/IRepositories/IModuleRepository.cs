using axionpro.application.DTOs;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using axionpro.application.Features.OperationCmd.Queries;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IModuleRepository
    {
        /// <summary>
        /// Ek module ko fetch karta hai by Id
        /// </summary>
        /// 

        Task<Module?> GetCommonMenuParentAsync();
        Task<List<ModuleDTO>> GetCommonMenuTreeAsync(int? parentModuleId);

        Task<Module?> GetModuleByIdAsync(long moduleId);

        /// <summary>
        /// Sare modules laata hai (optionally filterable)
        /// </summary>
    

        Task<List<ModuleDTO>> GetAllActiveModulesAsync(List<ModuleDTO> modules);

        /// <summary>
        /// Naya module insert karta hai
        /// </summary>
        /// 
        #region Create All module
        Task<List<GetParentModuleResponseDTO>> AddParentModuleAsync(CreateParentModuleRequestDTO Dto);
        Task<List<GetCommonModuleResponseDTO>> AddCommonModuleAsync(CreateCommonModuleRequestDTO Dto);
        Task<List<GetModuleChildInversResponseDTO>> AddSubModuleAsync(CreateSubModuleRequestDTO Dto);     

        #endregion
        #region Get All module

     //   Task<List<GetSubModuleResponseDTO>> GetSubModuleAsync(GetCommonModuleRequestDTO Dto);
        Task<List<GetParentModuleResponseDTO>> GetParentModuleAsync(GetParentModuleRequestDTO Dto);
        Task<List<GetParentModuleResponseDTO>> GetSubParentModuleAsync(GetSubParentModulRequestDTO Dto);
        Task<List<GetCommonModuleResponseDTO>> GetCommonModuleAsync(GetCommonModuleRequestDTO Dto);
        Task<List<GetModuleChildInversResponseDTO>> GetAllOnlyModuleTreeAsync();
        Task<List<GetModuleChildInversResponseDTO>> GetAllModuleTreeAsync();
        #endregion
        Task<Module> AddSubModuleAsync(Module module);

        /// <summary>
        /// Module ko update karta hai
        /// </summary>
        Task<bool> UpdateModuleAsync(Module module);

        /// <summary>
        /// Module ko soft/hard delete karta hai
        /// </summary>
        Task<bool> DeleteModuleAsync(long moduleId);


    }
}
