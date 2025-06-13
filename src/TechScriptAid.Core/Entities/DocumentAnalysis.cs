using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace TechScriptAid.Core.Entities
{
    public class DocumentAnalysis : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public string Results { get; set; } = string.Empty; // JSON serialized results
        public double ConfidenceScore { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime AnalyzedAt { get; set; }

        // Navigation property
        public Document Document { get; set; } = null!;

        // Helper methods for Results serialization
        public T? GetResults<T>() where T : class
        {
            if (string.IsNullOrEmpty(Results))
                return null;

            return JsonSerializer.Deserialize<T>(Results);
        }

        public void SetResults<T>(T results) where T : class
        {
            Results = JsonSerializer.Serialize(results);
        }
    }

    public enum AnalysisType
    {
        Summary,
        Sentiment,
        KeyPhraseExtraction,
        Classification,
        Translation
    }
}
