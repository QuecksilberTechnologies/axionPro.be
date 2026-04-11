using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Device.Enroll;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class EmployeeEnrollmentAtDeviceRepository
    : IEmployeeEnrollmentAtDeviceRepository
{
    private readonly WorkforceDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeEnrollmentAtDeviceRepository> _logger;

    private readonly IPasswordService _passwordService;
    private readonly IEncryptionService _encryptionService;

    public EmployeeEnrollmentAtDeviceRepository(
        WorkforceDbContext context,
        IMapper mapper,
        ILogger<EmployeeEnrollmentAtDeviceRepository> logger,
        IPasswordService passwordService,
        IEncryptionService encryptionService)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _passwordService = passwordService;
        _encryptionService = encryptionService;
    }

    // 🔥 ENROLL EMPLOYEE
    public async Task<bool> EnrollEmployeeAsync(RegisterEmployeeDTORequest dto)
    {
        try
        {
            var commandJson = new
            {
                cmd = DeviceCommands.SetUserInfo,
                enrollid = dto.EmployeeId,   // 🔥 server generated
                name = dto.Name,
                backupnum = 0,
                admin = 0,
                record = ""
            };

            var queue = new DeviceCommandQueue
            {
                TenantId = dto.TenantId,
                DeviceId = dto.DeviceId,
                DeviceSn = dto.DeviceSn,
                CommandName = DeviceCommands.SetUserInfo,
                CommandJson = JsonConvert.SerializeObject(commandJson),
                Status = 0,
                CreatedDate = DateTime.UtcNow
            };

            await _context.DeviceCommandQueues.AddAsync(queue);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while enrolling employee to device");
            throw;
        }
    }

    // 🔥 DELETE EMPLOYEE
    public async Task<bool> DeleteEmployeeAsync(DeleteEmployeeDTORequest dto)
    {
        try
        {
            var commandJson = new
            {
                cmd = DeviceCommands.DeleteUser,
                enrollid = dto.EmployeeCode
            };

            var queue = new DeviceCommandQueue
            {
                TenantId = dto.TenantId,
                DeviceId = dto.DeviceId,
                DeviceSn = dto.DeviceSn,
                CommandName = DeviceCommands.DeleteUser,
                CommandJson = JsonConvert.SerializeObject(commandJson),
                Status = 0,
                CreatedDate = DateTime.UtcNow
            };

            await _context.DeviceCommandQueues.AddAsync(queue);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting employee from device");
            throw;
        }
    }
}