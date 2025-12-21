// Models/Interfaces/IClientKeyRepository.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IClientKeyRepository : IGenericRepository<ClientKey>
    {
        // ❌ ĐÃ BỎ HÀM GetByKeyValueAsync() GÂY LỖI 'void'

        /// <summary>
        /// Lấy Key theo giá trị (key value).
        /// </summary>
        Task<ClientKey?> GetKeyByValueAsync(string keyValue);
    }
}