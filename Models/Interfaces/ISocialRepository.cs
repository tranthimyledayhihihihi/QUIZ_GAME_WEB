using QUIZ_GAME_WEB.Models.SocialRankingModels; // BXH, NguoiDungOnline, Comment
using QUIZ_GAME_WEB.Models.ViewModels; // RankingDto
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    // Keeping IGenericRepository<BXH> for basic CRUD operations on the ranking table itself
    public interface ISocialRepository : IGenericRepository<BXH>
    {
        // ===============================================
        // I. RANKING/LEADERBOARD OPERATIONS
        // ===============================================

        /// <summary>
        /// Retrieves paginated leaderboards (weekly/monthly), mapped to DTO.
        /// (Replaces GetTopRankingAsync for better pagination support)
        /// </summary>
        Task<(IEnumerable<RankingDto> Ranking, int TotalCount)> GetLeaderboardAsync(
            string type, int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves the user's specific ranking entry.
        /// </summary>
        Task<BXH?> GetUserRankingEntryAsync(int userId);

        // ===============================================
        // II. ONLINE STATUS & USER COUNT
        // ===============================================

        /// <summary>
        /// Updates or inserts the user's online status (e.g., in NguoiDungOnline table).
        /// </summary>
        Task UpdateOrInsertOnlineStatusAsync(int userId, string status);

        /// <summary>
        /// Counts the total number of currently online users.
        /// (Corrects method name used in RankingController)
        /// </summary>
        Task<int> CountOnlineUsersAsync();

        // ===============================================
        // III. SOCIAL INTERACTION
        // ===============================================

        /// <summary>
        /// Adds a new comment (e.g., to a blog post, a quiz, or a leaderboard).
        /// </summary>
        Task AddCommentAsync(Comment comment);
    }
}