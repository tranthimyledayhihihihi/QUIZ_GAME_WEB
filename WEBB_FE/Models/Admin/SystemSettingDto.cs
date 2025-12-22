using Newtonsoft.Json;

namespace WEBB.Models.System
{
    public class SystemSettingDto
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("moTa")]
        public string MoTa { get; set; }
    }
}
