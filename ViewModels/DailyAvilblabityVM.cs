using Doctor_Appointment.Models;

namespace Doctor_Appointment.ViewModels
{
    public class DailyAvilblabityVM
    {
        public string? DoctorName { get; set; }
        public int DoctorId { get; set; }
        public ICollection<DailyAvailbility> Days { get; set; } = new List<DailyAvailbility>();

    }
}
