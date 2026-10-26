using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Dtos.Audit;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

/// <summary>
/// Service interface for job audit operations, providing methods to create and enqueue audit records
/// for various job types including PTAEO codes and post position processing.
/// </summary>
public interface IJobAuditService
{
    /// <summary>
    /// Enqueues a single audit record for asynchronous processing.
    /// </summary>
    /// <typeparam name="T">The type of audit record</typeparam>
    /// <param name="audit">The audit record to enqueue</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task EnqueueAuditAsync<T>(T audit, CancellationToken cancellationToken) where T : AuditBase;
    
    
    /// <summary>
    /// Creates a waiting post position job audit record.
    /// </summary>
    /// <param name="tenureRow">The waiting post position data</param>
    /// <param name="departmentId">Department identifier</param>
    /// <param name="employeeId">Employee identifier (optional)</param>
    /// <param name="dateOfWork">Date of work</param>
    /// <param name="action">Audit action type</param>
    /// <param name="changedOn">When the change occurred</param>
    /// <param name="correlationId">Correlation identifier for tracking</param>
    /// <param name="processingStatus">Processing status</param>
    /// <param name="failureReason">Reason for failure (if applicable)</param>
    /// <param name="entityIdOverride">Override for entity ID</param>
    /// <returns>Waiting post position job run audit record</returns>
    WaitingPostPositionJobRunAudit CreateWaitingPostPositionJobAudit(
        WaitingPostPositionDto tenureRow,
        Guid departmentId,
        Guid? employeeId,
        DateOnly dateOfWork,
        AuditAction action,
        DateTime changedOn,
        string? correlationId,
        ProcessingStatus processingStatus,
        string? failureReason,
        string? entityIdOverride = null);
        
    /// <summary>
    /// Creates an update released-on to released job audit record.
    /// </summary>
    /// <param name="requestId">Request identifier</param>
    /// <param name="displayName">Display name for the audit</param>
    /// <param name="departmentId">Department identifier (optional)</param>
    /// <param name="processedTime">When the request was processed</param>
    /// <param name="hadOnToReleased">Whether status changed from On to Released</param>
    /// <param name="hadToComplete">Whether status changed from Released to Complete</param>
    /// <returns>Update released-on to released job run audit record</returns>
    UpdateReleasedOnToReleasedJobRunAudit CreateUpdateReleasedOnToReleasedAudit(
        Guid requestId,
        string displayName,
        Guid? departmentId,
        DateTime processedTime,
        bool hadOnToReleased,
        bool hadToComplete);
        
    /// <summary>
    /// Creates an update preferred to managed job audit record.
    /// </summary>
    /// <param name="requestId">Request identifier</param>
    /// <param name="displayName">Display name for the audit</param>
    /// <param name="departmentId">Department identifier (optional)</param>
    /// <param name="processedUtc">When the request was processed (UTC)</param>
    /// <param name="success">Whether the operation was successful</param>
    /// <param name="message">Additional message (optional)</param>
    /// <returns>Update preferred to managed job run audit record</returns>
    UpdatePreferredToManagedJobRunAudit CreateUpdatePreferredToManagedAudit(
        Guid requestId,
        string displayName,
        Guid? departmentId,
        DateTime processedUtc,
        bool success,
        string? message);
        
    /// <summary>
    /// Creates a PTAEO code job audit record.
    /// </summary>
    /// <param name="dto">PTAEO code data</param>
    /// <param name="action">Audit action type</param>
    /// <param name="isDelete">Whether this is a delete operation</param>
    /// <param name="changedOn">When the change occurred</param>
    /// <param name="previousDto">Before image for updates; when set, <c>ChangedFields</c> lists old→new per changed attributed column.</param>
    /// <returns>PTAEO job run audit record</returns>
    PtaeoJobRunAudit CreatePtaeoAudit(
        PtaeoCodeDto dto,
        AuditAction action,
        bool isDelete,
        DateTime changedOn,
        PtaeoCodeDto? previousDto = null);
}
