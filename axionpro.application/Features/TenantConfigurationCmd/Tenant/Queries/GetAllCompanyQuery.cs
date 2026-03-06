using axionpro.domain.Entity; using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Queries
{
    public class GetAllCompanyQuery : IRequest<string>
    {
    }
    public class GetAllCompanyQueryHandler : IRequestHandler<GetAllCompanyQuery, string>
    {
        public Task<string> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
