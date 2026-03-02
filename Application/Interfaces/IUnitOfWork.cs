using Application.Repositories;

namespace Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;

        Task<int> Commit();
        void Rollback();
    }
}