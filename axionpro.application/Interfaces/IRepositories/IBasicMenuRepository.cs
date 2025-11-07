using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IBasicMenuRepository
    {
        Task<IEnumerable<BasicMenu>> GetBasicMenusByUserAndDeviceAsync(long userId, int deviceType);
        
    }
}
