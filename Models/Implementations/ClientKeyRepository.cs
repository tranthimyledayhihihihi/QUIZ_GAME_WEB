// Models/Implementations/ClientKeyRepository.cs
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Threading.Tasks; // Cần thiết

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class ClientKeyRepository : GenericRepository<ClientKey>, IClientKeyRepository
    {
        private readonly QuizGameContext _context;

        public ClientKeyRepository(QuizGameContext context) : base(context)
        {
            _context = context;
        }

        // Triển khai hàm từ Interface
        public async Task<ClientKey?> GetKeyByValueAsync(string keyValue)
        {
            return await _context.ClientKeys.FirstOrDefaultAsync(k => k.KeyValue == keyValue);
        }
    }
}