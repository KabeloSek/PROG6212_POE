namespace PROG6212_POE.Models
{
    public class InvoiceViewModel
    {
    
        public LecturerDTO Lecturer { get; set; }
        public List<ClaimDTO> ApprovedClaims { get; set; }

        public decimal TotalAmount
        {
            get
            {
                if (ApprovedClaims == null || !ApprovedClaims.Any())
                    return 0;

                return ApprovedClaims.Sum(c => c.Amount);
            }
        }
    }
}