using AutoMapper;
using BackTareas.Data;
using BackTareas.Models;
using BackTareas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackTareas.Repository
{
    public class WorkRepository : Repository<Work>, IWorkRepository 
    {
        private readonly Context context;
        public WorkRepository( Context context) : base (context)
        {
            this.context = context;
        }

        public async Task<Work> Create(Work model)
        {
            context.Add(model);
            await Save();
            return model;
        }

        public async Task<Work> GetById(int id)
        {
            var model = await context.Works.Include(u => u.User).FirstOrDefaultAsync(i => i.Id == id);

            return model;
        }

        public async Task Update(Work work)
        {
            context.Update(work);
            await Save();

        }
    }
}
