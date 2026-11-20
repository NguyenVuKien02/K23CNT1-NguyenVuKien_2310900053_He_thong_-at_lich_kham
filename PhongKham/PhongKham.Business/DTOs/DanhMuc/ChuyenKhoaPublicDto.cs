namespace PhongKham.Business.DTOs.DanhMuc;

public class ChuyenKhoaPublicDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SoBacSi { get; set; }
}
