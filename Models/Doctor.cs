using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment.Models
{
	public enum Gender
	{
		Female = 1,
		Male
	}

	public enum Spectialist
	{
		[Description("Neurology")]
		Neurology = 1,
		[Description("Dentists")]
		Dentists,
		[Description("Ophthalmology")]
		Ophthalmology,
		[Description("Orthopedics")]
		Orthopedics,
		[Description("Cancer Deparment")]
		Cancer_Department,
		[Description("Internal Medicine")]
		Internal_medicine,
		[Description("ENT")]
		ENT

	}

	public enum MedicalDegree
	{
		specialist = 1,

		Advisor,

		professor
	}

	public class Doctor
	{
		[Key]
		public int DoctorID { get; set; }

		[Required]
		[MinLength(10)]
		public string FullName { get; set; } = default!;

		[Required]
		[EnumDataType(typeof(Gender))]
		public Gender Gender { get; set; }
		[Required]
		public string IdentityId { get; set; }
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
		public virtual ICollection<DailyAvailbility> AvailableDays { get; set; } = new HashSet<DailyAvailbility>();
		public override string ToString()
		{
			return $"{Specialist}";

		}

		public string GetImage()
		{
			if (Gender == Gender.Male)
			{
				int num = (DoctorID % 3) + 1;
				return "/images/doctors/" + num + ".jpg";
			}
			else
			{
				return "/images/doctors/2.jpg";
			}
		}

		public string GetSpecializationDescription()
		{
			DescriptionAttribute[] attributes = (DescriptionAttribute[])Specialist
		   .GetType().GetField(Specialist.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			string description = attributes.Length > 0 ? attributes[0].Description : Specialist.ToString();

			return description;
		}

	}
}
