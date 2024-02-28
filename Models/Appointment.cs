using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Appointment.Models
{
    public enum AppointmentType
    {
        ClinicalExaminiation = 1,
        HomeExamination
    }

    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        public virtual Doctor Doctor { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public virtual Patient Patient { get; set; }

        [ForeignKey("DailyAvailbility")]
        public int DayId { get; set; }
        public virtual DailyAvailbility Day { get; set; }

        [EnumDataType(typeof(AppointmentType))]
        public AppointmentType AppointmentType { get; set; }

        public string? MedicalHistory { get; set; }


    }

}

