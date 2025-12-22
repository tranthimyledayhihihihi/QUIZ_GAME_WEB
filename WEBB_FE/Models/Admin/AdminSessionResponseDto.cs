using System.Collections.Generic;
using WEBB.Models.Admin;

public class AdminSessionResponseDto
{
    public List<AdminSessionDto> Sessions { get; set; }

    public int TotalCount { get; set; }

    // ✅ THÊM
    public int TodayCount { get; set; }

    public AdminSessionResponseDto()
    {
        Sessions = new List<AdminSessionDto>();
    }
}

