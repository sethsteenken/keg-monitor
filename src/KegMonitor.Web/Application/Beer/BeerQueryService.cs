using KegMonitor.Core.Entities;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class BeerQueryService : IBeerQueryService
    {
        private readonly KegMonitorDbContext _dbContext;

        public BeerQueryService(KegMonitorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BeerEditModel> BuildEditModelAsync(int? id)
        {
            if (id == null)
                return new BeerEditModel();

            var beer = await _dbContext.Beers.FirstOrDefaultAsync(b => b.Id == id.Value);
            if (beer == null)
                return null;

            return new BeerEditModel()
            {
                Id = beer.Id,
                Name = beer.Name,
                Type = beer.Type,
                ABV = beer.ABV,
                OG = beer.OG,
                FG = beer.FG,
                TapDate = beer.TapDate,
                Description = beer.Description,
                ImagePath = beer.ImagePath,
                BrewDate = beer.BrewDate,
                SecondaryDate = beer.SecondaryDate,
                IBU = beer.IBU,
                Ingredients = beer.Ingredients,
                Recipe = beer.Recipe,
                Url = beer.Url,
                Notes = beer.Notes
            };
        }

        public async Task<IEnumerable<Beer>> GetAllAsync()
        {
            return await _dbContext.Beers.AsNoTracking()
                                         .OrderByDescending(b => b.Id)
                                         .ToListAsync();                                  
        }

        public async Task<IEnumerable<BeerPour>> GetPoursAsync(int beerId)
        {
            return await _dbContext.BeerPours.Include(bp => bp.Beer)
                                             .Include(bp => bp.Scale)
                                             .AsNoTracking()
                                             .Where(bp => bp.Beer.Id == beerId)
                                             .OrderByDescending(bp => bp.TimeStamp)
                                             .ToListAsync();
        }
    }
}
