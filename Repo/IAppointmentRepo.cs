using Doctor_Appointment.Models;
using Doctor_Appointment.ViewModels;

namespace Doctor_Appointment.Repo
{
	public interface IAppointmentRepo
	{

		public IQueryable<Appointment> GetAllQuerable();
		public Appointment GetById(int Id);
		public void Insert(AppoinmentVM appointment);
		public void Update(int DocId, int PatId, Appointment appointment);
		public bool Delete(int appId);
		public void DeleteRange(IEnumerable<Appointment> appointments);
		public bool IsBoocked(AppoinmentVM appVm);



	}
}
