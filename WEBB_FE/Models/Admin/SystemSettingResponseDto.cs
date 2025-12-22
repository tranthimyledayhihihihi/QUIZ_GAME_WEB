using Newtonsoft.Json;
using System.Collections.Generic;

namespace WEBB.Models.System
{
    public class SystemSettingResponseDto
    {
        [JsonProperty("data")]
        public List<SystemSettingDto> Data { get; set; }
    }
}
