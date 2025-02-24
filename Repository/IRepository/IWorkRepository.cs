using BackTareas.Models;

namespace BackTareas.Repository.IRepository
{
    public interface IWorkRepository : IRepository<Work>
    {
        Task<Work> GetById(int id);
        Task Update(Work work);
        Task<Work>Create(Work model);
    }
}
