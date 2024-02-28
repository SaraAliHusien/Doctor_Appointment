using Doctor_Appointment.Models;

namespace Doctor_Appointment.ViewModels
{
    public class EditDayVM
    {
        public string DoctorName { get; set; }
        public int DoctorId { get; set; }
        public DailyAvailbility Day { get; set; }
    }
}
