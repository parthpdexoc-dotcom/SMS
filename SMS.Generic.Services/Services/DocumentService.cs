using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing document operations using the generic service pattern.
    /// </summary>
    public class DocumentService : GenericService<DocumentDto, Document>, IDocumentService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentService"/> class.
        /// </summary>
        /// <param name="repository">The document repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public DocumentService(
            IDocumentRepository documentRepository,
            IMapper mapper,
            ILogger<DocumentService> logger) : base(documentRepository, mapper, "Id")
        {
        }
    }
}