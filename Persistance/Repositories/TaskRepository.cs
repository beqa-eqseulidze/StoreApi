
using Application.Common.Interfaces;
using Domain.Entities;
using Persistance.Common;

namespace Persistance.Repositories
{
    public class TaskRepository(AppDbContext dbContext) : BaseRepository<TaskItem>(dbContext), ITaskRepository
    {
    }
}
