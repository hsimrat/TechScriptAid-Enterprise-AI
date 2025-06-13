using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechScriptAid.Core.Entities;

namespace TechScriptAid.Core.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> GetDocumentsByCategoryAsync(string category);
        Task<IEnumerable<Document>> GetDocumentsByStatusAsync(DocumentStatus status);
        Task<Document?> GetDocumentWithAnalysesAsync(Guid id);
        Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm);
    }
}
