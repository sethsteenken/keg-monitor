using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace KegMonitor.Web.Application
{
    public class ScaleQueryService : IScaleQueryService
    {
        private readonly IBeerQueryService _beerQueryService;
        private readonly KegMonitorDbContext _dbContext;

        public ScaleQueryService(
            IBeerQueryService beerQueryService,
            KegMonitorDbContext dbContext)
        {
            _beerQueryService = beerQueryService;
            _dbContext = dbContext;
        }

        public async Task<ScaleEditModel> BuildEditModelAsync(int id)
        {
            var scale = await _dbContext.Scales.Include(s => s.Beer).FirstOrDefaultAsync(s => s.Id == id);
            if (scale == null)
                throw new InvalidOperationException("Scale not found.");

            var beerOptions = await _beerQueryService.GetAllAsync();

            return new ScaleEditModel()
            {
                Id = scale.Id,
                BeerId = scale.Beer?.Id,
                Active = scale.Active,
                CurrentWeight = scale.CurrentWeight,
                EmptyWeight = scale.EmptyWeight,
                FullWeight = scale.FullWeight,
                RecordingDifferenceThreshold = scale.RecordingDifferenceThreshold,
                BeerOptions = beerOptions
            };
        }
    }
}
