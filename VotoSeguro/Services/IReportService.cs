



using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public interface IReportService
    {
        Task<VoteStatistics> GetVoteStatistics();
        Task<List<Vote>> GetVoteAuditLog();
    }
}