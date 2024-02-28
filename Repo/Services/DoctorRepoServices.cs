using Doctor_Appointment.Data;
using Doctor_Appointment.Models;
using Doctor_Appointment.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment.Repo.Services
{
	public class DoctorRepoServices : IDoctorRepo
	{
		private readonly ApplicationDbContext _context;
		private readonly IAppointmentRepo _appRepo;
		private readonly IDailyAvailbilityRepo _dailyAvailbilityRepo;

		public DoctorRepoServices(ApplicationDbContext context, IAppointmentRepo appRepo, IDailyAvailbilityRepo dailyAvailbilityRepo)
		{
			_context = context;
			_appRepo = appRepo;
			_dailyAvailbilityRepo = dailyAvailbilityRepo;
		}


		public IQueryable<Doctor> GetAll()
		{
			return _context.Doctors.AsNoTracking().AsQueryable();

		}

		public Doctor GetById(int id)
		{
			return _context.Doctors.FirstOrDefault(d => d.DoctorID == id);
		}
		public Doctor GetByIdentity(string Identity)
		{
			return _context.Doctors.Include(d => d.AvailableDays).FirstOrDefault(d => d.IdentityId == Identity);
		}
		public Doctor GetByIdWithIncludingDays(int id)
		{
			return _context.Doctors.Where(d => d.DoctorID == id).Include(d => d.AvailableDays).FirstOrDefault();
		}

		public int Insert(CreateDoctoreVM doctorVM)
		{
			var doctor = new Doctor()
			{
				FullName = doctorVM.FullName,
				Degree = doctorVM.Degree,
				Gender = doctorVM.Gender,
				Specialist = doctorVM.Specialist,
				Description = doctorVM.Description,
				IdentityId = doctorVM.IdentityId,
				Clinic_Location = doctorVM.Clinic_Location,
				Clinic_PhonNum = doctorVM.Clinic_PhonNum,
				HomeExamination = doctorVM.HomeExamination,
				Price = doctorVM.Price,
			};
			_context.Add(doctor);

			_context.SaveChanges();
			return doctor.DoctorID;
		}

		public void Update(Doctor doctor)
		{
			var del_Doc = _context.Doctors.FirstOrDefault(p => p.DoctorID == doctor.DoctorID);

			del_Doc.FullName = doctor.FullName;
			del_Doc.Degree = doctor.Degree;
			del_Doc.Gender = doctor.Gender;
			del_Doc.Description = doctor.Description;
			del_Doc.Clinic_Location = doctor.Clinic_Location;
			del_Doc.Clinic_PhonNum = doctor.Clinic_PhonNum;
			del_Doc.HomeExamination = doctor.HomeExamination;
			del_Doc.Price = doctor.Price;
			del_Doc.AvailableDays = doctor.AvailableDays;
			_context.Doctors.Update(del_Doc);
			_context.SaveChanges();
		}
		public void Update(EditDoctorVM doctor)
		{
			var del_Doc = _context.Doctors.FirstOrDefault(p => p.DoctorID == doctor.Id);


			del_Doc.FullName = doctor.FullName;
			del_Doc.Degree = doctor.Degree;
			del_Doc.Gender = doctor.Gender;
			del_Doc.Description = doctor.Description;
			del_Doc.Clinic_Location = doctor.Clinic_Location;
			del_Doc.Clinic_PhonNum = doctor.Clinic_PhonNum;
			del_Doc.HomeExamination = doctor.HomeExamination;
			del_Doc.Price = doctor.Price;
			del_Doc.AvailableDays = del_Doc.AvailableDays;
			_context.Doctors.Update(del_Doc);
			_context.SaveChanges();
		}
		public bool Delete(int id)
		{
			var del_Doc = GetById(id);
			if (del_Doc is null)
				return false;
			var appointments = _appRepo.GetAllQuerable().Where(p => p.DoctorID == id).ToList();
			_appRepo.DeleteRange(appointments);
			var availabaleDays = _dailyAvailbilityRepo.GetAll(del_Doc.DoctorID);
			_dailyAvailbilityRepo.DeleteRange(availabaleDays);

			_context.Doctors.Remove(del_Doc);
			var rowEff = _context.SaveChanges();
			return rowEff > 0;
		}


		//specialist filter
		public List<Doctor> GetBySpecialist(Spectialist spl)
		{
			var docSpl = _context.Doctors.Where(s => s.Specialist == spl).ToList();
			return docSpl;
		}
	}
}