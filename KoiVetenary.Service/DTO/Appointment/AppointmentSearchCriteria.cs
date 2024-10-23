using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiVetenary.Service.DTO.Appointment
{
    public class AppointmentSearchCriteria
    {
        public int AppointmentId { get; set; }
        public int? OwnerId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public string SpecialRequests { get; set; }
        public string Notes { get; set; }
        public int? TotalEstimatedDuration { get; set; }
        public decimal? TotalCost { get; set; }
    }
}
