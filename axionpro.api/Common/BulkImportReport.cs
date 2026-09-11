using System.Text;
using System.Text.Json;
using axionpro.application.DTOS.Common;

namespace axionpro.api.Common;

/// <summary>Exports correction-friendly row results without exposing queue internals or tenant identifiers.</summary>
public static class BulkImportReport
{
    public static byte[] Create(BulkImportJobResponseDTO job)
    {
        var employee = job.Master == axionpro.application.Common.Enums.BulkImportMaster.Employee;
        var csv = new StringBuilder(employee
            ? "RowNumber,Status,RecordId,Values,Errors,ProposedEmployeeCode,InvitationStatus,InvitationError\r\n"
            : "RowNumber,Status,RecordId,Values,Errors\r\n");
        foreach (var row in job.Preview?.Rows ?? new())
        {
            csv.Append(row.RowNumber).Append(',').Append(row.Status).Append(',')
                .Append(row.ExistingId).Append(',').Append(Cell(JsonSerializer.Serialize(row.Values)))
                .Append(',').Append(Cell(string.Join(" | ", row.Errors)));
            if (employee)
            {
                csv.Append(',').Append(Cell(row.ProposedEmployeeCode ?? string.Empty));
                csv.Append(',').Append(row.InvitationStatus).Append(',').Append(Cell(row.InvitationError ?? string.Empty));
            }
            csv.Append("\r\n");
        }
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
    }

    private static string Cell(string value)
    {
        if (value.Length > 0 && "=+-@\t\r\n".Contains(value[0]))
        {
            value = "'" + value;
        }
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
