using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechScriptAid.Core.Entities;

namespace TechScriptAid.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDocumentRepository Documents { get; }
        IGenericRepository<DocumentAnalysis> DocumentAnalyses { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
