using BackTareas.Data;
using BackTareas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BackTareas.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Context context;
        public Repository(Context context)
        {
            this.context = context;
        }

        protected DbSet<T> EntitySet
        {
            get { return context.Set<T>(); }
        }

        public async Task Delete(int id)
        {
            var model = await EntitySet.FindAsync(id);
            if (model != null)
            {
                EntitySet.Remove(model);
                await Save();
            }
        }

        public async Task<List<T>> Getall()
        {
            List<T> list = await EntitySet.ToListAsync();

            return list;
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }

    }
}
