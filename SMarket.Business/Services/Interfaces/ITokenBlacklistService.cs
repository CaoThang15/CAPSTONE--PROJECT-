namespace SMarket.Business.Services.Interfaces
{
    public interface ITokenBlacklistService
    {
        void Blacklist(string token, DateTime expiry);
        bool IsBlacklisted(string token);
        Task CleanupExpiredTokensAsync();
    }
}
