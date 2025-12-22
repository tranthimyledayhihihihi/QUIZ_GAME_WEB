using System.Collections.Generic;

namespace WEBB.Models.Admin
{
    public class AdminSessionResponseDto
    {
        public List<AdminSessionDto> Sessions { get; set; }
        public int TotalCount { get; set; }

        public AdminSessionResponseDto()
        {
            Sessions = new List<AdminSessionDto>();
        }
    }
}
