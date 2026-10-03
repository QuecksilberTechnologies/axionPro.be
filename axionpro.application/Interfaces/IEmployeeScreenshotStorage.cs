namespace axionpro.application.Interfaces;

public interface IEmployeeScreenshotStorage
{
    Task<string> SaveAsync(
        long tenantId,
        long employeeId,
        Guid captureId,
        string contentType,
        Stream content,
        CancellationToken cancellationToken);

    Task DeleteAsync(string objectKey, CancellationToken cancellationToken);
}
