using Doctor_Appointment.Data;
using Doctor_Appointment.Models;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment.Repo.Services
{
	public class DailyAvailbiltyRepoServices : IDailyAvailbilityRepo
	{
		private readonly ApplicationDbContext _context;
		public DailyAvailbiltyRepoServices(ApplicationDbContext context)
		{
			_context = context;
		}

		public IEnumerable<DailyAvailbility> GetAll(int docid)
		{
			return _context.DailyAvailbilities.Where(d => d.DoctorID == docid).ToList();
		}

		public DailyAvailbility GetById(int id)
		{
			return _context.DailyAvailbilities.FirstOrDefault(d => d.Dayid == id);

		}
		public DailyAvailbility GetByIdWithIncliding(int id)
		{
			return _context.DailyAvailbilities.Include(d => d.Doctor).Where(d => d.Dayid == id).FirstOrDefault();

		}
		public void Insert(DailyAvailbility dailyAvailbility)
		{
			_context.DailyAvailbilities.Add(dailyAvailbility);
			_context.SaveChanges();
		}


		public void Update(DailyAvailbility dailyAvailbility)
		{
			var upd_daily = GetById(dailyAvailbility.Dayid);

			upd_daily.DoctorID = upd_daily.DoctorID;
			upd_daily.Date = dailyAvailbility.Date;
			upd_daily.StartTime = dailyAvailbility.StartTime;
			upd_daily.EndTime = dailyAvailbility.EndTime;
			upd_daily.countAppointment = dailyAvailbility.countAppointment;
			upd_daily.BookedAppointments = dailyAvailbility.BookedAppointments;

			_context.DailyAvailbilities.Update(upd_daily);
			_context.SaveChanges();
		}
		public bool Delete(int Id)
		{
			DailyAvailbility day = GetById(Id);
			if (day is null)
				return false;

			_context.DailyAvailbilities.Remove(day);
			var rowEff = _context.SaveChanges();


			return rowEff > 0;
		}
		public void DeleteRange(IEnumerable<DailyAvailbility> Days)
		{
			if (Days.Any())
			{
				_context.RemoveRange(Days);
				_context.SaveChanges();
			}

		}
		public bool IsExistedDay(DailyAvailbility day, int docId)
		{
			var existedDay = _context.DailyAvailbilities.Where(d => d.Date == day.Date && d.DoctorID == docId && d.StartTime == day.StartTime).FirstOrDefault();

			return existedDay != null;
		}

		public bool IsExistedEditDay(DailyAvailbility day)
		{
			var existedDay = _context.DailyAvailbilities.Where(d => d.Date == day.Date && d.DoctorID == day.DoctorID && d.StartTime == day.StartTime && d.Dayid != day.Dayid).FirstOrDefault();

			return existedDay != null;
		}
	}
}