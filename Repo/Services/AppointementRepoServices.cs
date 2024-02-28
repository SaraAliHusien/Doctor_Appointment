using Doctor_Appointment.Data;
using Doctor_Appointment.Models;
using Doctor_Appointment.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment.Repo.Services
{
	public class AppointementRepoServices : IAppointmentRepo
	{
		private readonly ApplicationDbContext _context;
		private readonly IDailyAvailbilityRepo _dailyAvailbilityRepo;

		public AppointementRepoServices(ApplicationDbContext context, IDailyAvailbilityRepo dailyAvailbilityRepo)
		{
			_context = context;
			_dailyAvailbilityRepo = dailyAvailbilityRepo;

		}


		public IQueryable<Appointment> GetAllQuerable()
		{
			return _context.Appointments.AsNoTracking().AsQueryable();
		}


		public Appointment GetById(int id)
		{
			return _context.Appointments.Where(d => d.AppointmentID == id)
				.Include(d => d.Doctor)
				.Include(p => p.Patient)
				.Include(da => da.Day).FirstOrDefault();
		}

		public void Insert(AppoinmentVM appointmentVM)
		{
			var appointment = new Appointment()
			{
				DoctorID = appointmentVM.DocId,
				PatientID = appointmentVM.PatienteId.Value,
				DayId = appointmentVM.DayId,
				AppointmentType = appointmentVM.AppointmentType,
				MedicalHistory = appointmentVM.MedicalHistory,

			};
			_context.Appointments.Add(appointment);
			var roweff = _context.SaveChanges();
			if (roweff < 0)
			{
				var day = _dailyAvailbilityRepo.GetById(appointment.DayId);
				day.BookedAppointments = day.BookedAppointments + 1;
				_context.DailyAvailbilities.Update(day);
			}
		}

		public void Update(int DocId, int PatId, Appointment appointment)
		{
			var upd_app = _context.Appointments.Where(d => d.DoctorID == DocId && d.PatientID == PatId).FirstOrDefault();
			upd_app.MedicalHistory = appointment.MedicalHistory;

			_context.Update(upd_app);
			_context.SaveChanges();

		}
		public bool Delete(int appId)
		{
			Appointment appo = GetById(appId);
			if (appo is null)
				return false;

			_context.Appointments.Remove(appo);
			var rowEff = _context.SaveChanges();


			return rowEff > 0;
		}
		public void DeleteRange(IEnumerable<Appointment> appointments)
		{
			if (appointments.Any())
			{
				_context.Appointments.RemoveRange(appointments);
				_context.SaveChanges();
			}
		}

		public bool IsBoocked(AppoinmentVM appVm)
		{
			var appointment = GetAllQuerable().Where(p => p.DayId == appVm.DayId && p.DoctorID == appVm.DocId && p.PatientID == appVm.PatienteId).FirstOrDefault();
			return appointment != null;
		}

	}
}
