using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces;

public interface IPayrollWordDocumentGenerator
{
    /// <summary>
    /// Creates a Word document from payroll data records
    /// </summary>
    /// <param name="records">List of payroll data records</param>
    /// <param name="filePath">Output file path</param>
    /// <returns>Path to the generated document</returns>
    Task<string> CreateDocumentAsync(List<PayrollDataRecordDto> records, string filePath);
}
