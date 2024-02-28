using Doctor_Appointment.Models;
using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment.ViewModels
{
    public class EditDoctorVM
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        public string FullName { get; set; } = default!;

        [Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }


        [Required]
        [EnumDataType(typeof(Spectialist))]
        public Spectialist Specialist { get; set; }

        [Required]
        [EnumDataType(typeof(MedicalDegree))]
        public MedicalDegree Degree { get; set; }

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Clinic Location")]
        public string Clinic_Location { get; set; } = default!;

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Clinic PhonNum")]
        public int Clinic_PhonNum { get; set; }

        public bool HomeExamination { get; set; }

        [Required]
        public int Price { get; set; }



    }
}
