using Doctor_Appointment.Data;
using Doctor_Appointment.Models;

namespace Doctor_Appointment.Repo.Services
{
	public class PatientRepoServices : IPatientRepo
	{
		public readonly ApplicationDbContext _context;
		public readonly IAppointmentRepo _appointmentRepo;


		public PatientRepoServices(ApplicationDbContext context, IAppointmentRepo appointmentRepo)
		{
			_context = context;
			_appointmentRepo = appointmentRepo;

		}
		public IEnumerable<Patient> GetAll()
		{
			return _context.Patients.ToList();
		}
		public IQueryable<Patient> GetAllIQuarable()
		{
			return _context.Patients.AsQueryable();
		}
		public Patient GetById(int id)
		{
			return _context.Patients.FirstOrDefault(p => p.PatientID == id);
		}
		public void Insert(Patient patient)
		{
			if (patient is not null)
			{
				_context.Add(patient);
				_context.SaveChanges();
			}

		}

		public void Update(int id, Patient patient)
		{
			var pat = _context.Patients.FirstOrDefault(p => p.PatientID == id);

			pat.Address = patient.Address;
			pat.Age = patient.Age;
			pat.FullName = patient.FullName;
			pat.PhonNum = patient.PhonNum;
			_context.Update(pat);
			_context.SaveChanges();
		}
		public bool Delete(int id)
		{
			if (id <= 0)
				return false;
			var p = GetById(id);
			if (p is null)
				return false;
			var appointments = _appointmentRepo.GetAllQuerable().Where(p => p.PatientID == id).ToList();
			_appointmentRepo.DeleteRange(appointments);
			_context.Remove(p);
			var rowEff = _context.SaveChanges();
			return rowEff > 0; ;
		}
	}
}
