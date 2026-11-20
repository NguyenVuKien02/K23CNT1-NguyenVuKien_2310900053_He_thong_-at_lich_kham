using Microsoft.AspNetCore.Razor.TagHelpers;
using PhongKham.Common.Enums;

namespace PhongKham.Web.TagHelpers;

[HtmlTargetElement("trang-thai-lich-hen")]
public class TrangThaiLichHenBadgeTagHelper : TagHelper
{
    [HtmlAttributeName("trang-thai")]
    public TrangThaiLichHen TrangThai { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var (cssClass, label) = TrangThai switch
        {
            TrangThaiLichHen.ChoXacNhan => ("pk-status-cho-xac-nhan", "Chờ xác nhận"),
            TrangThaiLichHen.DaXacNhan => ("pk-status-da-xac-nhan", "Đã xác nhận"),
            TrangThaiLichHen.DaCheckIn => ("pk-status-da-check-in", "Đã check-in"),
            TrangThaiLichHen.DangKham => ("pk-status-dang-kham", "Đang khám"),
            TrangThaiLichHen.HoanThanh => ("pk-status-hoan-thanh", "Hoàn thành"),
            TrangThaiLichHen.DaHuy => ("pk-status-da-huy", "Đã hủy"),
            TrangThaiLichHen.VangMat => ("pk-status-vang-mat", "Vắng mặt"),
            _ => ("pk-status-vang-mat", TrangThai.ToString())
        };

        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;

        var existingClass = output.Attributes["class"]?.Value?.ToString();
        var mergedClass = string.IsNullOrWhiteSpace(existingClass)
            ? $"badge rounded-pill pk-status-badge {cssClass}"
            : $"badge rounded-pill pk-status-badge {cssClass} {existingClass}";

        output.Attributes.SetAttribute("class", mergedClass);
        output.Content.SetContent(label);
    }
}
