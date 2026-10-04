using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskFlowDbContext _context;
        public UnitOfWork(TaskFlowDbContext context) => _context = context;
        public Task<int> CommitAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    }
}
