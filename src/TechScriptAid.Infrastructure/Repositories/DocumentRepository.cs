using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechScriptAid.Core.Entities;
using TechScriptAid.Core.Interfaces;
using TechScriptAid.Infrastructure.Data;

namespace TechScriptAid.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Document>> GetDocumentsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(d => d.Category.ToLower() == category.ToLower())
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetDocumentsByStatusAsync(DocumentStatus status)
        {
            return await _dbSet
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentWithAnalysesAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.Analyses)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(d => d.Title.ToLower().Contains(lowerSearchTerm) ||
                           d.Content.ToLower().Contains(lowerSearchTerm) ||
                           d.Tags.Any(t => t.ToLower().Contains(lowerSearchTerm)))
                .OrderByDescending(d => d.CreatedAt)
                .Take(50) // Limit results
                .ToListAsync();
        }
    }
}
