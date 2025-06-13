using System.ComponentModel.DataAnnotations;
using TechScriptAid.Core.Entities;

namespace TechScriptAid.API.DTOs
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AiSummary { get; set; }
        public double? AiConfidenceScore { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateDocumentDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();
    }

    public class UpdateDocumentDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        public DocumentStatus Status { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }

    public class DocumentAnalysisDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentTitle { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public List<string> KeyPhrases { get; set; } = new List<string>();
        public Dictionary<string, double> Sentiments { get; set; } = new Dictionary<string, double>();
        public List<string> SuggestedTags { get; set; } = new List<string>();
        public DateTime AnalyzedAt { get; set; }
    }


}// This code defines the Data Transfer Objects (DTOs) for the Document entity in the TechScriptAid API.