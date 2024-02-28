using Doctor_Appointment.Models;

namespace Doctor_Appointment.ViewModels
{
    public class DoctorViewModel
    {
        public Doctor doctor { get; set; }

        public List<DailyAvailbility> dailyAvailbilities { get; set; }

        public Appointment appointment { get; set; }

        public Patient patient { get; set; }

    }
}
