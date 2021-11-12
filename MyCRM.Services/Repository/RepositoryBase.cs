using System;
using System.Threading.Tasks;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using MyCRM.Persistence.Data;

namespace MyCRM.Services.Repository
{
    public abstract class RepositoryBase
    {
        protected RepositoryBase(ApplicationDbContext context)
        {
            Context = context;
        }

        public ApplicationDbContext Context { get; }

        public async Task<bool> Save()
        {
            try
            {
                return await Context.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                throw new DbUpdateException("Error Updating Data.", e.InnerException);
            }
        }

        public async Task<ResponseBaseModel<T>> SaveDbAndReturnReponse<T>(T model) where T:class
        {
            if (await Save()) return ResponseBaseModel<T>.GetSuccessResponse(model);

            return ResponseBaseModel<T>.GetDbSaveFailedResponse();
        }
    }
}