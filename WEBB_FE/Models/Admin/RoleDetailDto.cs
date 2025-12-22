using Newtonsoft.Json;
using System.Collections.Generic;

namespace WEBB.Models.Admin
{
    public class RoleDetailDto
    {
        [JsonProperty("vaiTroID")]
        public int VaiTroID { get; set; }

        [JsonProperty("tenVaiTro")]
        public string TenVaiTro { get; set; }

        [JsonProperty("quyenHienTai")]
        public List<PermissionDto> QuyenHienTai { get; set; }

        public RoleDetailDto()
        {
            QuyenHienTai = new List<PermissionDto>();
        }
    }
}
