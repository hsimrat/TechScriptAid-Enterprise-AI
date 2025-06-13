using TechScriptAid.API.DTOs;
using TechScriptAid.Core.Entities;
using TechScriptAid.Core.Interfaces;

namespace TechScriptAid.API.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IUnitOfWork unitOfWork, ILogger<DocumentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentsAsync(string? category = null, DocumentStatus? status = null)
        {
            IEnumerable<Document> documents;

            if (!string.IsNullOrEmpty(category))
            {
                documents = await _unitOfWork.Documents.GetDocumentsByCategoryAsync(category);
            }
            else if (status.HasValue)
            {
                documents = await _unitOfWork.Documents.GetDocumentsByStatusAsync(status.Value);
            }
            else
            {
                documents = await _unitOfWork.Documents.GetAllAsync();
            }

            return documents.Select(MapToDto);
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(Guid id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            return document != null ? MapToDto(document) : null;
        }

        public async Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto createDto)
        {
            var document = new Document
            {
                Title = createDto.Title,
                Content = createDto.Content,
                Category = createDto.Category,
                Tags = createDto.Tags,
                Status = DocumentStatus.Draft
            };

            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document created with ID: {DocumentId}", document.Id);

            return MapToDto(document);
        }

        public async Task<bool> UpdateDocumentAsync(Guid id, UpdateDocumentDto updateDto)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            if (document == null)
            {
                return false;
            }

            document.Title = updateDto.Title;
            document.Content = updateDto.Content;
            document.Category = updateDto.Category;
            document.Status = updateDto.Status;
            document.Tags = updateDto.Tags;

            await _unitOfWork.Documents.UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document updated: {DocumentId}", id);

            return true;
        }

        public async Task<bool> DeleteDocumentAsync(Guid id)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(id);
            if (document == null)
            {
                return false;
            }

            await _unitOfWork.Documents.DeleteAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DocumentDto>> SearchDocumentsAsync(string searchTerm)
        {
            var documents = await _unitOfWork.Documents.SearchDocumentsAsync(searchTerm);
            return documents.Select(MapToDto);
        }

        public async Task<DocumentAnalysisDto?> AnalyzeDocumentAsync(Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null)
            {
                return null;
            }

            // TODO: In Episode 3, we'll integrate Azure OpenAI here
            // For now, return mock analysis
            var analysis = new DocumentAnalysisDto
            {
                DocumentId = document.Id,
                DocumentTitle = document.Title,
                Summary = $"This is a mock summary for {document.Title}. In the next episode, we'll integrate Azure OpenAI to generate real summaries.",
                ConfidenceScore = 0.85,
                KeyPhrases = new List<string> { "enterprise", "AI", "integration", ".NET" },
                Sentiments = new Dictionary<string, double>
                {
                    { "positive", 0.7 },
                    { "neutral", 0.2 },
                    { "negative", 0.1 }
                },
                SuggestedTags = new List<string> { "technology", "tutorial" },
                AnalyzedAt = DateTime.UtcNow
            };

            // Update document with analysis results
            document.AiSummary = analysis.Summary;
            document.AiConfidenceScore = analysis.ConfidenceScore;
            document.Status = DocumentStatus.Analyzed;

            await _unitOfWork.Documents.UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return analysis;
        }

        private static DocumentDto MapToDto(Document document)
        {
            return new DocumentDto
            {
                Id = document.Id,
                Title = document.Title,
                Content = document.Content,
                Category = document.Category,
                Status = document.Status.ToString(),
                AiSummary = document.AiSummary,
                AiConfidenceScore = document.AiConfidenceScore,
                Tags = document.Tags,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt
            };
        }
    }
}
