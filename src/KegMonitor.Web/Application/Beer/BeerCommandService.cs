using KegMonitor.Core.Entities;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class BeerCommandService : IBeerCommandService
    {
        private readonly KegMonitorDbContext _dbContext;

        public BeerCommandService(
            KegMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveAsync(BeerEditModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Beer beer;
            bool isNew = model.Id == null;
            if (isNew)
            {
                beer = new Beer(model.Name, model.Type);
            }
            else
            {
                beer = await _dbContext.Beers.FirstOrDefaultAsync(b => b.Id == model.Id.Value);
                if (beer == null)
                    throw new InvalidOperationException("Beer not found");

                beer.Name = model.Name;
                beer.Type = model.Type;
            }

            beer.Description = model.Description;
            beer.ABV = model.ABV;
            beer.LastUpdatedDate = DateTime.UtcNow;

            if (isNew)
                await _dbContext.Beers.AddAsync(beer);

            await _dbContext.SaveChangesAsync();

            return beer.Id;
        }
    }
}
