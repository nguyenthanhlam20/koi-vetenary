namespace KoiVetenary.Service.DTO.VNPAY
{
    public class VnPayRequestModel
    {
        public int OrderId { get; set; }
        public string FullName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
