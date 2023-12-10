using KegMonitor.Core;
using KegMonitor.Core.Entities;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class BeerCommandService : IBeerCommandService
    {
        private readonly KegMonitorDbContext _dbContext;
        private readonly IFileHandler _fileHandler;

        public BeerCommandService(
            KegMonitorDbContext dbContext,
            IFileHandler fileHandler)
        {
            _dbContext = dbContext;
            _fileHandler = fileHandler;
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
            beer.OG = model.OG;
            beer.FG = model.FG;
            beer.ImagePath = model.ImagePath;

            // set Kind to UTC - required by postgres
            beer.TapDate = model.TapDate?.Date.ToUtcKindDate();
            
            beer.LastUpdatedDate = DateTime.UtcNow;

            if (isNew)
                await _dbContext.Beers.AddAsync(beer);

            await _dbContext.SaveChangesAsync();
            
            return beer.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var beer = await _dbContext.Beers.FirstOrDefaultAsync(s => s.Id == id);
            if (beer == null)
                throw new InvalidOperationException("Beer not found.");

            if (!string.IsNullOrWhiteSpace(beer.ImagePath))
                await _fileHandler.DeleteAsync(beer.ImagePath);

            await _dbContext.ScaleWeightChanges.Where(swc => swc.Beer != null && swc.Beer.Id == id)
                                               .ExecuteDeleteAsync();

            await _dbContext.BeerPours.Where(swc => swc.Beer.Id == id)
                                      .ExecuteDeleteAsync();

            _dbContext.Beers.Remove(beer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveImageAsync(int id)
        {
            var beer = await _dbContext.Beers.FirstOrDefaultAsync(s => s.Id == id);
            if (beer == null)
                throw new InvalidOperationException("Beer not found.");

            beer.ImagePath = null;
            beer.LastUpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }
}
