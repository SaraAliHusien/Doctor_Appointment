using Doctor_Appointment.Models;
using System.ComponentModel;

namespace Doctor_Appointment.ViewModels
{
    public class AppoinmentVM
    {
        [DisplayName("Examiniation Type")]
        public AppointmentType AppointmentType { get; set; }
        public bool? HomeExaminiation { get; set; }
        public int DocId { get; set; }
        public int? PatienteId { get; set; }
        public string DocName { get; set; }
        [DisplayName("Medical History ")]
        public string? MedicalHistory { get; set; }
        [DisplayName("Day ")]
        public int DayId { get; set; }

        public int Price { get; set; }
        public Dictionary<int, string> AvailableDays { get; set; } = new Dictionary<int, string>();

    }
}
