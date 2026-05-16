using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly OrderFlowDbContext _context;
    public RefreshTokenRepository(OrderFlowDbContext context)
    {
        _context = context;
    }
    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jwtId);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}
