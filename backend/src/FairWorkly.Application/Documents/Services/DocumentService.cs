using FairWorkly.Application.Common.Interfaces;
using FairWorkly.Application.Documents.Orchestrators;

// TODO: using FairWorkly.Domain.Documents.Interfaces;

namespace FairWorkly.Application.Documents.Services;

public class DocumentService : IDocumentService
{
    // private readonly IDocumentRepository _repository;
    private readonly IAiClient _aiClient;
    private readonly DocumentAiOrchestrator _orchestrator;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentService(
        // IDocumentRepository repository,
        IAiClient aiClient,
        DocumentAiOrchestrator orchestrator,
        IUnitOfWork unitOfWork
    )
    {
        // _repository = repository;
        _aiClient = aiClient;
        _orchestrator = orchestrator;
        _unitOfWork = unitOfWork;
    }
}
