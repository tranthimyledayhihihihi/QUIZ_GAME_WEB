# 🎮 QUIZ GAME WEB

<div align="center">

![Banner](image/banner.jpg)

**🏆 Nền tảng game trắc nghiệm trực tuyến hiện đại với tính năng real-time và hệ thống xếp hạng**

</div>

---

## 📋 Giới thiệu

**QUIZ GAME WEB** là một ứng dụng web game trắc nghiệm được xây dựng trên nền tảng ASP.NET Core 8.0, cho phép người dùng tham gia các bài quiz đa dạng, thi đấu trực tuyến với nhau và theo dõi thành tích qua hệ thống xếp hạng.

---

### 🎯 Dành cho Người dùng

| Tính năng | Mô tả |
|-----------|-------|
| 📝 **Quiz hàng ngày** | Tham gia các bài quiz được cập nhật mỗi ngày |
| 🎨 **Quiz tùy chỉnh** | Tạo và chơi quiz theo chủ đề yêu thích |
| 🤝 **Quiz chia sẻ** | Chia sẻ quiz với bạn bè và cộng đồng |
| ⚔️ **Chế độ đối kháng** | Thi đấu trực tuyến real-time với người chơi khác |
| 📊 **Lịch sử chơi** | Xem lại kết quả và phân tích các câu trả lời sai |
| 🏆 **Bảng xếp hạng** | Theo dõi thứ hạng và so sánh với người chơi khác |
| 🎁 **Phần thưởng & Thành tựu** | Nhận phần thưởng và mở khóa thành tựu |

### 👨‍💼 Dành cho Quản trị viên

| Tính năng | Mô tả |
|-----------|-------|
| 👥 **Quản lý người dùng** | Thêm, sửa, xóa và phân quyền người dùng |
| ❓ **Quản lý câu hỏi** | Tạo và quản lý kho câu hỏi đa dạng |
| 📚 **Quản lý chủ đề** | Phân loại câu hỏi theo chủ đề |
| ⚙️ **Quản lý độ khó** | Thiết lập các mức độ khó cho câu hỏi |
| 📅 **Quản lý Quiz ngày** | Lên lịch và quản lý quiz hàng ngày |
| 🏅 **Quản lý bảng xếp hạng** | Giám sát và điều chỉnh hệ thống xếp hạng |
| 🎖️ **Quản lý thành tựu** | Tạo và quản lý các thành tựu |
| 🎁 **Quản lý phần thưởng** | Thiết lập phần thưởng cho người chơi |
| 📈 **Thống kê & Báo cáo** | Xem báo cáo chi tiết hoạt động hệ thống |
| 🔑 **Quản lý vai trò** | Phân quyền và quản lý vai trò người dùng |

---

## 🛠️ Công nghệ sử dụng

### 💻 Backend
| Công nghệ | Mô tả |
|-----------|-------|
| **ASP.NET Core 8.0** | Framework chính |
| **Entity Framework Core 8.0** | ORM cho database |
| **SQL Server** | Cơ sở dữ liệu quan hệ |
| **JWT Bearer** | Xác thực và phân quyền |
| **WebSocket** | Giao tiếp real-time |
| **Swagger/OpenAPI** | Tài liệu API |

### 🎨 Frontend
| Công nghệ | Mô tả |
|-----------|-------|
| **ASP.NET MVC** | Web Framework |
| **HTML/CSS/JavaScript** | Giao diện người dùng |

---

## 📚 API Documentation

### 🔌 WebSocket Endpoint

| Endpoint | Mô tả |
|----------|-------|
| `ws://host/ws/game` | Kết nối WebSocket cho chế độ đối kháng real-time |

> **Note**: Sử dụng query string `?access_token=YOUR_JWT_TOKEN` để xác thực

---

### 🏠 Home API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Home` | Kiểm tra trạng thái API (Smoke Test) |
| GET | `/api/Home/HealthCheck` | Kiểm tra kết nối Database (Health Check) |

---

### 🔐 Account API (Xác thực)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/Account/login` | Đăng nhập và nhận JWT Token |
| POST | `/api/Account/register` | Đăng ký tài khoản mới |
| POST | `/api/Account/change-password` | Đổi mật khẩu (yêu cầu đăng nhập) |
| POST | `/api/Account/logout` | Đăng xuất và hủy phiên đăng nhập |

---

### 🎮 Quiz - Chơi Game API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/Choi/start` | Bắt đầu phiên làm bài quiz |
| POST | `/api/Choi/submit` | Nộp đáp án cho câu hỏi |
| POST | `/api/Choi/end/{attemptId}` | Kết thúc phiên và xem kết quả |
| GET | `/api/Choi/next/{attemptId}` | Lấy câu hỏi tiếp theo |

---

### 📅 Quiz Ngày API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/QuizNgay/today` | Lấy quiz của ngày hôm nay |
| POST | `/api/QuizNgay/start` | Bắt đầu làm Quiz Ngày |
| POST | `/api/QuizNgay/submit` | Nộp đáp án Quiz Ngày |
| POST | `/api/QuizNgay/end/{attemptId}` | Kết thúc Quiz Ngày và xem kết quả |

---

### 🎨 Quiz Tùy Chỉnh API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/QuizTuyChinh/my-submissions` | Lấy danh sách đề xuất quiz của tôi |
| GET | `/api/QuizTuyChinh/{quizId}` | Xem chi tiết đề xuất quiz |
| POST | `/api/QuizTuyChinh/submit` | Gửi đề xuất quiz mới |
| DELETE | `/api/QuizTuyChinh/{quizId}` | Xóa đề xuất quiz |

---

### 🤝 Quiz Chia Sẻ API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/QuizChiaSe/share` | Chia sẻ quiz cho người khác |
| GET | `/api/QuizChiaSe/sent` | Danh sách quiz đã gửi |
| GET | `/api/QuizChiaSe/received` | Danh sách quiz nhận được |
| GET | `/api/QuizChiaSe/{id}` | Chi tiết một lần chia sẻ |

---

### ❓ Câu Hỏi API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/CauHoi/incorrect-review` | Lấy câu hỏi sai để ôn tập |
| GET | `/api/CauHoi/total-count` | Tổng số câu hỏi trong hệ thống |
| GET | `/api/CauHoi/statistics` | Thống kê câu hỏi theo chủ đề/độ khó |

---

### 📊 Lịch Sử Chơi API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/LichSuChoi/my` | Danh sách kết quả của tôi (phân trang) |
| GET | `/api/LichSuChoi/{attemptId}` | Chi tiết một lần làm bài |
| GET | `/api/LichSuChoi/streak` | Chuỗi ngày chơi liên tiếp |
| GET | `/api/LichSuChoi/achievements` | Danh sách thành tựu đã đạt |

---

### ❌ Câu Sai API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/quiz/CauSai/recent` | Danh sách câu sai gần đây |
| GET | `/api/quiz/CauSai/count/{attemptId}` | Đếm số câu sai trong 1 lần làm bài |

---

### 👤 Profile API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/user/Profile/me` | Lấy thông tin hồ sơ cá nhân |
| PUT | `/api/user/Profile/me` | Cập nhật thông tin hồ sơ |
| PUT | `/api/user/Profile/settings` | Cập nhật cài đặt người dùng |

---

### 🏆 Achievement API (Thành tựu)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/user/Achievement/me` | Lấy thành tựu của tôi |
| GET | `/api/user/Achievement/streak` | Lấy chuỗi ngày chơi |
| POST | `/api/user/Achievement/daily-reward` | Nhận thưởng hằng ngày |
| GET | `/api/user/Achievement/my-rewards` | Danh sách quà tặng của tôi |

---

### ⚔️ Trận Đấu Online API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/trandau/create` | Tạo phòng đối kháng mới |
| POST | `/api/trandau/join/{matchCode}` | Tham gia phòng đối kháng |
| GET | `/api/trandau/status/{matchCode}` | Kiểm tra trạng thái trận đấu |
| GET | `/api/trandau/history` | Lịch sử các trận đấu |
| GET | `/api/trandau/detail/{matchCode}` | Chi tiết trận đấu |
| GET | `/api/trandau/online-count` | Số người đang online |
| DELETE | `/api/trandau/cancel/{matchCode}` | Hủy phòng đối kháng |

---

### 🏅 Ranking API (Bảng xếp hạng)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/Ranking/leaderboard` | BXH tuần/tháng (phân trang) |
| GET | `/api/Ranking/achievements/my` | Thành tựu của tôi |
| GET | `/api/Ranking/online-count` | Tổng số người đang online |

---

### 💬 Comment API (Bình luận)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/social/Comment/{entityType}/{entityId}` | Lấy danh sách bình luận |
| POST | `/api/social/Comment` | Thêm bình luận mới |
| PUT | `/api/social/Comment/{id}` | Chỉnh sửa bình luận |
| DELETE | `/api/social/Comment/{id}` | Xóa bình luận |

---

### 📈 Thống Kê API

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/ThongKe/User/{userId}/Daily` | Thống kê chơi game hằng ngày (30 ngày) |
| GET | `/api/ThongKe/User/{userId}/Streak` | Chuỗi ngày chơi liên tiếp |

---

## 🔧 ADMIN APIs

> ⚠️ **Yêu cầu quyền**: SuperAdmin hoặc Moderator

---

### 👥 Quản Lý Người Dùng

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/nguoidung` | Danh sách tài khoản (phân trang, lọc) |
| GET | `/api/admin/nguoidung/{userId}` | Chi tiết tài khoản |
| POST | `/api/admin/nguoidung/khoa/{userId}` | Khóa tài khoản |
| POST | `/api/admin/nguoidung/mo-khoa/{userId}` | Mở khóa tài khoản |
| POST | `/api/admin/nguoidung/phan-quyen/{userId}/{roleId}` | Phân quyền người dùng (SuperAdmin) |

---

### ❓ Quản Lý Câu Hỏi

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/QLCauHoi` | Danh sách câu hỏi (phân trang, lọc) |
| GET | `/api/admin/QLCauHoi/{id}` | Chi tiết câu hỏi |
| POST | `/api/admin/QLCauHoi` | Tạo câu hỏi mới |
| PUT | `/api/admin/QLCauHoi/{id}` | Cập nhật câu hỏi |
| DELETE | `/api/admin/QLCauHoi/{id}` | Xóa câu hỏi (SuperAdmin) |

---

### 📚 Quản Lý Chủ Đề

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/QLChuDe` | Danh sách chủ đề |
| POST | `/api/admin/QLChuDe` | Thêm chủ đề mới |
| PUT | `/api/admin/QLChuDe/{id}` | Cập nhật chủ đề |
| DELETE | `/api/admin/QLChuDe/{id}` | Xóa chủ đề |

---

### ⚙️ Quản Lý Độ Khó

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/dokho` | Danh sách độ khó |
| GET | `/api/admin/dokho/{doKhoId}` | Chi tiết độ khó |
| POST | `/api/admin/dokho` | Thêm độ khó mới |
| PUT | `/api/admin/dokho/{doKhoId}` | Cập nhật độ khó |
| DELETE | `/api/admin/dokho/{doKhoId}` | Xóa độ khó (SuperAdmin) |

---

### 📅 Quản Lý Quiz Ngày

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/quiz-ngay/lich-trinh` | Lịch trình Quiz Ngày theo tháng/năm |
| POST | `/api/admin/quiz-ngay/set-daily` | Thiết lập câu hỏi cho ngày cụ thể |

---

### 🎨 Quản Lý Quiz Tùy Chỉnh

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/quiz-tuy-chinh` | Danh sách quiz tùy chỉnh |
| GET | `/api/admin/quiz-tuy-chinh/{id}` | Chi tiết quiz + danh sách câu hỏi |
| POST | `/api/admin/quiz-tuy-chinh/{id}/phe-duyet` | Phê duyệt quiz |
| POST | `/api/admin/quiz-tuy-chinh/{id}/tu-choi` | Từ chối quiz |
| DELETE | `/api/admin/quiz-tuy-chinh/{id}` | Xóa quiz (SuperAdmin) |

---

### 🏆 Quản Lý Bảng Xếp Hạng

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/bxh/top-global` | Top người chơi toàn server |
| GET | `/api/admin/bxh/today-stats` | Thống kê hoạt động hôm nay |

---

### 🎖️ Quản Lý Thành Tựu

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/thanhtuu` | Danh sách định nghĩa thành tựu |
| GET | `/api/admin/thanhtuu/{thanhTuuId}` | Chi tiết thành tựu |
| POST | `/api/admin/thanhtuu` | Tạo thành tựu mới |
| PUT | `/api/admin/thanhtuu/{thanhTuuId}` | Cập nhật thành tựu |
| DELETE | `/api/admin/thanhtuu/{thanhTuuId}` | Xóa thành tựu (SuperAdmin) |

---

### 🎁 Quản Lý Phần Thưởng

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/phan-thuong/lich-su` | Lịch sử nhận thưởng (phân trang) |
| POST | `/api/admin/phan-thuong/tang-qua` | Tặng quà cho người dùng |
| DELETE | `/api/admin/phan-thuong/{id}` | Xóa bản ghi thưởng |

---

### 🔑 Quản Lý Vai Trò (SuperAdmin)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/vaitro` | Danh sách vai trò |
| GET | `/api/admin/vaitro/{id}` | Chi tiết vai trò và quyền đi kèm |
| POST | `/api/admin/vaitro` | Tạo vai trò mới |
| POST | `/api/admin/vaitro/{roleId}/gan-quyen` | Gán quyền cho vai trò |
| DELETE | `/api/admin/vaitro/{id}` | Xóa vai trò |

---

### 📱 Quản Lý Phiên Đăng Nhập

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/admin/phiendangnhap` | Danh sách phiên (phân trang, lọc) |
| GET | `/api/admin/phiendangnhap/{sessionId}` | Chi tiết phiên đăng nhập |
| POST | `/api/admin/phiendangnhap/buoc-dang-xuat/{sessionId}` | Buộc đăng xuất (SuperAdmin) |

---

## 🎯 Kiến trúc hệ thống

```mermaid
graph TB
    subgraph Client
        A[🌐 Web Browser]
        B[📱 Mobile App]
    end
    
    subgraph Backend
        C[⚙️ ASP.NET Core API]
        D[🔌 WebSocket Server]
    end
    
    subgraph Database
        E[(🗄️ SQL Server)]
    end
    
    A --> C
    B --> C
    A <-.->|Real-time| D
    B <-.->|Real-time| D
    C --> E
    D --> E
```

---

## 📄 License

Dự án được phát triển cho mục đích học tập.

---

<div align="center">

**⭐ Nếu thấy hữu ích, hãy cho dự án một star nhé! ⭐**

Made with ❤️

</div>