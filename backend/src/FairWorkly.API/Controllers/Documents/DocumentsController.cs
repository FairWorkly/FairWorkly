using FairWorkly.Application.Documents.Services;
using Microsoft.AspNetCore.Mvc;

namespace FairWorkly.API.Controllers.Documents;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }
}
