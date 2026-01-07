# [Bài Test Intern Unity Developer  / Nguyễn Văn Tuấn]

> Unity Version: [ 2020.3.38f1]
> Thời gian hoàn thành: [ 4 giờ]

## ✅ Các phần đã hoàn thành (Work Done)
Dựa theo yêu cầu đề bài, mình đã thực hiện các phần sau:

### Task1. Re-skin All items into Fish
-✅ Thay đổi tất cả các assets trong project thành hình ảnh con cá (fish).
b1: Truy cap vao folder Scripts/Utility/Constants.cs
b2: Thay doi gia tri trong bien public static string FISH_TAG = "Fish_1,...";

### Task2. change the game mechanics ...
-✅ Thay đổi cơ chế trò chơi để người chơi phải thu thập tất cả các con cá trong một khoảng thời gian nhất định để giành chiến thắng.
+ Logic của Task 2 được tách biệt hoàn toàn khỏi chế độ cũ để tránh xung đột
- Tạo ThreeMatchingControl.cs để quản lý chế độ chơi mới
	+ Controller chính, chịu trách nhiệm khởi tạo game và xử lý Input (Raycast click).
	+ Điều phối luồng game giữa Bàn cờ và Khay chứa.
- Tạo BottomBarControler.cs
	+ Tự động sinh ra 7 ô nền màu xám (CreateVisualSlots) bằng code khi bắt đầu game, không cần sắp xếp thủ công trên Scene.
	+ Xử lý logic di chuyển con cá từ bàn cờ vào khay chứa khi người chơi click.
	+ Sử dụng DOTween để xử lý hiệu ứng item bay từ bàn cờ xuống khay.
	+ Xử lý logic CheckMatch(): Tìm và xóa bộ 3 item.
- Tạo TileBoard.cs 
	+ Quản lý dữ liệu bàn cờ.
	+ Smart Generation: Thuật toán sinh item theo bộ 3 (FillTriplets) sau đó trộn vị trí (Shuffle), đảm bảo game luôn có thể giải được (Solvable), không bị trường hợp "chết bàn".