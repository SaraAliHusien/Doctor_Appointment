using Doctor_Appointment.Models;
using Doctor_Appointment.ViewModels;

namespace Doctor_Appointment.Repo
{
    public interface IDoctorRepo
    {
        public IQueryable<Doctor> GetAll();

        public Doctor GetById(int id);
        public Doctor GetByIdentity(string Identity);

        public Doctor GetByIdWithIncludingDays(int id);

        public List<Doctor> GetBySpecialist(Spectialist spl);
        public int Insert(CreateDoctoreVM doctor);
        public void Update(EditDoctorVM doctor);
        public void Update(Doctor doctor);

        public bool Delete(int id);
    }
}
