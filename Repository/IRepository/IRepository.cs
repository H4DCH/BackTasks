namespace BackTareas.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> Getall();
        Task Delete(int id);
    }
}
