using axionpro.application.DTOS.Billing;

namespace axionpro.application.Interfaces.IRepositories;

public interface IHostBillingConfigurationRepository
{
    Task<HostBillingConfigurationResponseDTO?> GetAsync(CancellationToken cancellationToken);
    Task<HostBillingConfigurationResponseDTO> UpdateAsync(
        HostBillingConfigurationRequestDTO request,
        long hostUserId,
        CancellationToken cancellationToken);
}
