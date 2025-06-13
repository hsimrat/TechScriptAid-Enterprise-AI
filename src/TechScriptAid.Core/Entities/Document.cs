using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechScriptAid.Core.Entities
{
    public class Document : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
        public string? AiSummary { get; set; }
        public double? AiConfidenceScore { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        // Navigation properties
        public ICollection<DocumentAnalysis> Analyses { get; set; } = new List<DocumentAnalysis>();
    }

    public enum DocumentStatus
    {
        Draft,
        Published,
        Archived,
        Processing,
        Analyzed
    }
}
