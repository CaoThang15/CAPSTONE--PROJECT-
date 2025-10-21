using System.Text.Json.Serialization;

namespace SMarket.Business.DTOs.Voucher
{
    public class AssignVoucherDto
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("voucher_id")]
        public int VoucherId { get; set; }
    }
}