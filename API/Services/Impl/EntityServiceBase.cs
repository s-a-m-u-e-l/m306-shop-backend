using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Services.Impl
{
  public abstract class EntityServiceBase<T> : IEntityService<T> where T : EntityBase
    {
        protected readonly SamadDbContext dbContext;
        protected readonly ILogger logger;

        private readonly DbSet<T> entityDao;
        private readonly string entityName;
        private IEntityService<T> _entityServiceImplementation;

        /// <summary>
        /// Creates a new instance of <see cref="EntityServiceBase{T}"/>
        /// </summary>
        /// <param name="dbContext">Instance of <see cref="WebSpecDbContext"/></param>
        /// <param name="entityDao">Instance of <see cref="DbSet{T}"/> which serves as DAO</param>
        /// <param name="logger">Instance of <see cref="ILogger"/></param>
        /// <param name="entityName">Name of the managed entity. Will appear in logs</param>
        public EntityServiceBase(SamadDbContext dbContext, DbSet<T> entityDao, ILogger<EntityServiceBase<T>> logger, string entityName)
        {
            this.dbContext = dbContext;
            this.entityDao = entityDao;
            this.logger = logger;
            this.entityName = entityName;
        }

        public async virtual Task AddAsync(T entity)
        {
            logger.LogInformation($"Attempting to add new {entityName}.");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                entityDao.Add(entity);

                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }

            logger.LogInformation($"Successfully added {entityName} with id {entity.Id}.");
        }

        public async virtual Task DeleteAsync(T entity)
        {
            logger.LogDebug($"Attempting to delete {entityName} with id {entity.Id}.");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                entityDao.Remove(entity);

                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }

            logger.LogInformation($"Successfully removed {entityName} with id {entity.Id}.");
        }

        public async Task DeleteAsync(Guid entityId)
        {
            await DeleteAsync(await GetByIdAsync(entityId));
        }

        public async virtual Task<T> GetByIdAsync(Guid entityId)
        {
            logger.LogDebug($"Attempting to get {entityName} with id {entityId}.");

            return await entityDao
                .Where(x => x.Id == entityId)
                .FirstOrDefaultAsync();
        }

        public async virtual Task UpdateAsync(T entity)
        {
            logger.LogDebug($"Attempting to update {entityName} with id {entity.Id}.");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                entityDao.Update(entity);

                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }

            logger.LogInformation($"Successfully updated {entityName} with id {entity.Id}.");
        }
    }
}