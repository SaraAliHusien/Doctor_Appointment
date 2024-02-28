using Doctor_Appointment.Models;

namespace Doctor_Appointment.Repo
{
    public interface IPatientRepo
    {
        public IEnumerable<Patient> GetAll();
        public IQueryable<Patient> GetAllIQuarable();

        public Patient GetById(int id);
        public void Insert(Patient patient);
        public void Update(int id, Patient patient);
        public bool Delete(int id);
    }
}
