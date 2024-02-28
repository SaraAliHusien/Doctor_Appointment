using Doctor_Appointment.Models;

namespace Doctor_Appointment.Repo
{
	public interface IDailyAvailbilityRepo
	{
		public IEnumerable<DailyAvailbility> GetAll(int docid);
		public DailyAvailbility GetById(int id);
		public DailyAvailbility GetByIdWithIncliding(int id);

		public void Insert(DailyAvailbility dailyAvailbility);
		public void Update(DailyAvailbility dailyAvailbility);
		public bool Delete(int Id);
		public void DeleteRange(IEnumerable<DailyAvailbility> Days);
		public bool IsExistedDay(DailyAvailbility day, int docId);
		public bool IsExistedEditDay(DailyAvailbility day);






	}
}
