using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment.Models
{
    public enum PGender
    {
        Female = 1,
        Male
    }
    public class Patient
    {
        [Key]
        public int PatientID { get; set; }

        [MaxLength(450)]
        public required string IdentityId { get; set; } = "";

        [Required]
        [MinLength(10)]
        public string FullName { get; set; }

        [Required]
        [EnumDataType(typeof(PGender))]
        public PGender Gender { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public int PhonNum { get; set; }

        public string Address { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();


    }
}

