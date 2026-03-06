using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPermissionRepository
    {
        Task<List<string>> GetPermissionsByRoleAsync(int roleId);

        Task InvalidateRoleCacheAsync(int roleId);
    }
}
