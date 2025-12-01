using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Models.Interfaces;
using QUIZ_GAME_WEB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauHoiController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;

        public CauHoiController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        // ----------------------------------------------------
        // 1. Lấy câu hỏi ngẫu nhiên (GET /api/cauhoi/random)
        // ----------------------------------------------------
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomQuestions(
            [FromQuery] int? chuDeId = null,
            [FromQuery] int? doKhoId = null,
            [FromQuery] int soLuong = 10)
        {
            try
            {
                // Gọi hàm Repository thực hiện Random và Filter trong DB, trả về DTO
                var randomQuestions = await _quizRepository.GetRandomQuestionsWithDetailsAsync(
                    soLuong, chuDeId, doKhoId);

                if (!randomQuestions.Any())
                {
                    return NotFound(new { message = "Không tìm thấy câu hỏi phù hợp." });
                }

                return Ok(randomQuestions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------------------------------
        // 2. Lấy câu hỏi theo chủ đề (GET /api/cauhoi/by-chude/{chuDeId})
        // ----------------------------------------------------
        [HttpGet("by-chude/{chuDeId}")]
        public async Task<IActionResult> GetQuestionsByTopic(
            int chuDeId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Gọi hàm Repository thực hiện Lọc và Phân trang trong DB
                var (questions, totalCount) = await _quizRepository.GetQuestionsFilteredAsync(
                    pageNumber, pageSize, chuDeId: chuDeId);

                if (!questions.Any())
                {
                    // Trả về 200 OK với danh sách trống nếu totalCount = 0 (tùy chọn) hoặc NotFound
                    return NotFound(new { message = $"Không tìm thấy câu hỏi cho chủ đề ID {chuDeId}." });
                }

                return Ok(new
                {
                    tongSoCauHoi = totalCount,
                    trangHienTai = pageNumber,
                    kichThuocTrang = pageSize,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = questions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------------------------------
        // 3. Lấy câu hỏi theo độ khó (GET /api/cauhoi/by-dokho/{doKhoId})
        // ----------------------------------------------------
        [HttpGet("by-dokho/{doKhoId}")]
        public async Task<IActionResult> GetQuestionsByDifficulty(
            int doKhoId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Gọi hàm Repository thực hiện Lọc và Phân trang trong DB
                var (questions, totalCount) = await _quizRepository.GetQuestionsFilteredAsync(
                    pageNumber, pageSize, doKhoId: doKhoId);

                if (!questions.Any())
                {
                    return NotFound(new { message = $"Không tìm thấy câu hỏi cho độ khó ID {doKhoId}." });
                }

                return Ok(new
                {
                    tongSoCauHoi = totalCount,
                    trangHienTai = pageNumber,
                    kichThuocTrang = pageSize,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = questions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------------------------------
        // 4. Tìm kiếm câu hỏi (GET /api/cauhoi/search)
        // ----------------------------------------------------
        [HttpGet("search")]
        public async Task<IActionResult> SearchQuestions(
            [FromQuery] string? keyword = null,
            [FromQuery] int? chuDeId = null,
            [FromQuery] int? doKhoId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                // Gọi hàm Repository thực hiện TÌM KIẾM, LỌC và Phân trang trong DB
                var (questions, totalCount) = await _quizRepository.GetQuestionsFilteredAsync(
                    pageNumber, pageSize, keyword, chuDeId, doKhoId);

                if (totalCount == 0) // Kiểm tra totalCount cho kết quả tìm kiếm
                {
                    return NotFound(new { message = "Không tìm thấy câu hỏi phù hợp." });
                }

                return Ok(new
                {
                    tongSoCauHoi = totalCount,
                    trangHienTai = pageNumber,
                    kichThuocTrang = pageSize,
                    tongSoTrang = (int)Math.Ceiling(totalCount / (double)pageSize),
                    danhSach = questions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------------------------------
        // 5. Tổng số câu hỏi (GET /api/cauhoi/total-count)
        // ----------------------------------------------------
        [HttpGet("total-count")]
        public async Task<IActionResult> GetTotalQuestionsCount()
        {
            try
            {
                // Gọi hàm đếm trực tiếp trong DB
                var totalCount = await _quizRepository.CountAllCauHoisAsync();
                return Ok(new { tongSoCauHoi = totalCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ----------------------------------------------------
        // 6. Thống kê câu hỏi (GET /api/cauhoi/statistics)
        // ----------------------------------------------------
        [HttpGet("statistics")]
        public async Task<IActionResult> GetQuestionStatistics()
        {
            try
            {
                // Tải toàn bộ Entity kèm chi tiết (Include ChuDe và DoKho)
                var allQuestions = await _quizRepository.GetAllCauHoisWithDetailsAsync();

                var statsByChuDe = allQuestions
                    // GroupBy sử dụng TenChuDe (Navigation Property)
                    .GroupBy(q => q.ChuDe?.TenChuDe ?? "Chưa phân loại")
                    .Select(g => new { tenChuDe = g.Key, soLuong = g.Count() })
                    .ToList();

                var statsByDoKho = allQuestions
                    // GroupBy sử dụng TenDoKho (Navigation Property)
                    .GroupBy(q => q.DoKho?.TenDoKho ?? "Chưa phân loại")
                    .Select(g => new { tenDoKho = g.Key, soLuong = g.Count() })
                    .ToList();

                return Ok(new
                {
                    tongSoCauHoi = allQuestions.Count(),
                    theoGiaDe = statsByChuDe,
                    theoDoKho = statsByDoKho
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}