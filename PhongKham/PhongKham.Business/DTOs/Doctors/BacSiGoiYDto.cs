namespace PhongKham.Business.DTOs.Doctors;

public class BacSiGoiYDto
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Degree { get; set; }
}
