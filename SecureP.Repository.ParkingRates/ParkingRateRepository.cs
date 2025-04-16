using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.ParkingRates;

public class ParkingRateRepository<TKey> : IParkingRateRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly ILogger<ParkingRateRepository<TKey>> _logger;
    private readonly AppDbContext<TKey> _context;

    public ParkingRateRepository(ILogger<ParkingRateRepository<TKey>> logger, AppDbContext<TKey> context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task AddAsync(ParkingRate<TKey> parkingRate)
    {
        await _context.ParkingRates.AddAsync(parkingRate);
    }


    public Task DeleteAsync(ParkingRate<TKey> parkingRate)
    {
        _context.ParkingRates.Remove(parkingRate);
        return Task.CompletedTask;
    }

    public async Task<ParkingRate<TKey>?> GetByIdAsync(TKey id)
    {
        return await _context.ParkingRates.FindAsync(id);
    }

    private static IQueryable<ParkingRate<TKey>?> GetQueryableAsync(IQueryable<ParkingRate<TKey>> query, int page, int limit, ParkingRateOrderBy sort, bool desc, string? search)
    {
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x => x.HourlyRate.ToString().Contains(search) || x.DailyRate.ToString().Contains(search) || x.MonthlyRate.ToString().Contains(search));
        }

        query = sort switch
        {
            ParkingRateOrderBy.HourlyRate => desc ? query.OrderByDescending(x => x.HourlyRate) : query.OrderBy(x => x.HourlyRate),
            ParkingRateOrderBy.DailyRate => desc ? query.OrderByDescending(x => x.DailyRate) : query.OrderBy(x => x.DailyRate),
            ParkingRateOrderBy.MonthlyRate => desc ? query.OrderByDescending(x => x.MonthlyRate) : query.OrderBy(x => x.MonthlyRate),
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
        };

        query = query.Skip(page * (limit > 0 ? limit : 1));
        if (limit > 0)
        {
            query = query.Take(limit);
        }
        return query;
    }

    public Task<List<ParkingRate<TKey>>> GetAllAsync(int page, int limit, ParkingRateOrderBy sort, bool desc, string? search)
    {
        var query = GetQueryableAsync(_context.ParkingRates.AsQueryable(), page, limit, sort, desc, search);

        return query.Where(x => x != null).Cast<ParkingRate<TKey>>().ToListAsync();
    }

    public Task<int> CountAsync(int page, int limit, ParkingRateOrderBy sort, bool desc, string? search)
    {
        return GetQueryableAsync(
                _context.ParkingRates
                    .AsQueryable(),
                page,
                limit,
                sort,
                desc,
                search)
            .CountAsync();
    }

    public Task UpdateAsync(ParkingRate<TKey> parkingRate)
    {
        _context.ParkingRates.Update(parkingRate);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
