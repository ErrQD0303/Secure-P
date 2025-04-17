using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.ParkingZones;

public class ParkingZoneRepository<TKey> : IParkingZoneRepository<TKey> where TKey : IEquatable<TKey>
{
    private ILogger<ParkingZoneRepository<TKey>> Logger { get; }
    private AppDbContext<TKey> Context { get; }

    public ParkingZoneRepository(ILogger<ParkingZoneRepository<TKey>> logger, AppDbContext<TKey> context)
    {
        Logger = logger;
        Context = context;
    }

    public async Task AddAsync(ParkingZone<TKey> parkingZone)
    {
        await Context.ParkingZones.AddAsync(parkingZone);
    }

    public Task<int> CountAsync(int page = 0, int limit = -1, ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, bool desc = false, string? search = null)
    {
        return GetQueryableAsync(Context.ParkingZones, page, limit, sort, desc, search).CountAsync();
    }

    public Task DeleteAsync(ParkingZone<TKey> parkingZone)
    {
        Context.ParkingZones.Remove(parkingZone);
        return Task.CompletedTask;
    }

    public Task<List<ParkingZone<TKey>>> GetAllAsync(int page = 0, int limit = -1, ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, bool desc = false, string? search = null)
    {
        var query = GetQueryableAsync(Context.ParkingZones, page, limit, sort, desc, search);

        return query.Where(x => x != null).Cast<ParkingZone<TKey>>().ToListAsync();
    }

    public async Task<ParkingZone<TKey>?> GetByIdAsync(TKey id)
    {
        return await Context.ParkingZones.FindAsync(id);
    }

    public Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }

    public Task UpdateAsync(ParkingZone<TKey> parkingZone)
    {
        Context.ParkingZones.Update(parkingZone);
        return Task.CompletedTask;
    }

    private static IQueryable<ParkingZone<TKey>?> GetQueryableAsync(IQueryable<ParkingZone<TKey>> query, int page, int limit, ParkingZoneOrderBy sort, bool desc, string? search)
    {
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x => x.Name.ToString().Contains(search));
        }

        query = sort switch
        {
            ParkingZoneOrderBy.Name => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            ParkingZoneOrderBy.Capacity => desc ? query.OrderByDescending(x => x.Capacity) : query.OrderBy(x => x.Capacity),
            ParkingZoneOrderBy.AvailableSpaces => desc ? query.OrderByDescending(x => x.AvailableSpaces) : query.OrderBy(x => x.AvailableSpaces),
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
        };

        query = query.Skip(page * (limit > 0 ? limit : 1));
        if (limit > 0)
        {
            query = query.Take(limit);
        }
        return query;
    }
}
