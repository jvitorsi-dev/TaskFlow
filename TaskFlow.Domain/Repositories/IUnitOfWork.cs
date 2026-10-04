using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken ct = default);
    }
}
