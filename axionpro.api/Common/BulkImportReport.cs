using System.Text;
using System.Text.Json;
using axionpro.application.DTOS.Common;

namespace axionpro.api.Common;

/// <summary>Exports correction-friendly row results without exposing queue internals or tenant identifiers.</summary>
public static class BulkImportReport
{
    public static byte[] Create(BulkImportJobResponseDTO job)
    {
        var csv = new StringBuilder("RowNumber,Status,RecordId,Values,Errors\r\n");
        foreach (var row in job.Preview?.Rows ?? new())
        {
            csv.Append(row.RowNumber).Append(',').Append(row.Status).Append(',')
                .Append(row.ExistingId).Append(',').Append(Cell(JsonSerializer.Serialize(row.Values)))
                .Append(',').Append(Cell(string.Join(" | ", row.Errors))).Append("\r\n");
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
