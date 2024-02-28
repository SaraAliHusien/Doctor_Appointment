using Doctor_Appointment.Models;
using Doctor_Appointment.Repo;
using Doctor_Appointment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly IDoctorRepo _doctorRepo;
        private readonly IPatientRepo _petientRepo;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _user;



        public AppointmentsController(IAppointmentRepo appointmentRepo, SignInManager<IdentityUser> manager,
            UserManager<IdentityUser> user, IDoctorRepo doctorRepo, IPatientRepo petientRepo)
        {
            _appointmentRepo = appointmentRepo;
            _doctorRepo = doctorRepo;
            _signInManager = manager;
            _user = user;
            _petientRepo = petientRepo;

        }

        // GET: Appointments

        [Authorize(Roles = "Patient")]
        public IActionResult Index()
        {
            int patId;
            if (_signInManager.IsSignedIn(User))
            {
                var userIdentity = User.Claims.First().Value;
                var patient = _petientRepo.GetAllIQuarable().Where(p => p.IdentityId == userIdentity).FirstOrDefault();
                if (patient == null)
                    return NotFound();
                patId = patient.PatientID;
            }
            else
                return BadRequest();
            var appointments = _appointmentRepo.GetAllQuerable().Where(p => p.PatientID == patId).Include(p => p.Doctor).Include(p => p.Day).ToList();

            return View(appointments);

        }


        [Authorize(Roles = "Doctor")]
        public IActionResult DoctorAppointmentList()
        {
            int DocId;
            if (_signInManager.IsSignedIn(User))
            {
                var userIdentity = User.Claims.First().Value;
                var Doctor = _doctorRepo.GetAll().Where(p => p.IdentityId == userIdentity).FirstOrDefault();
                if (Doctor == null)
                    return NotFound();
                DocId = Doctor.DoctorID;
            }
            else
                return BadRequest();
            var appointments = _appointmentRepo.GetAllQuerable().Where(p => p.DoctorID == DocId).Include(p => p.Patient).Include(p => p.Day).ToList();

            return View(appointments);

        }

        [Authorize(Roles = "Patient")]
        public IActionResult SelectDoctor()
        {

            return View("SelectDoctor", _doctorRepo.GetAll());
        }
        [Authorize(Roles = "Patient")]

        public IActionResult Create(int Id)
        {
            if (Id <= 0)
                return BadRequest();
            var Doc = _doctorRepo.GetByIdWithIncludingDays(Id);
            if (Doc is null)
                return NotFound();

            var availableDays = Doc.AvailableDays.Where(d => d.Date >= DateTime.Now && d.BookedAppointments < d.countAppointment);
            var appointment = new AppoinmentVM();
            appointment.DocId = Id;
            appointment.Price = Doc.Price;
            appointment.DocName = Doc.FullName;
            appointment.HomeExaminiation = Doc.HomeExamination;
            foreach (var d in availableDays)
            {
                appointment.AvailableDays.Add(d.Dayid, d.ToString());
            }
            return View(appointment);
        }
        [Authorize(Roles = "Patient")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppoinmentVM appointment)
        {
            if (!ModelState.IsValid)
            {
                var Doc = _doctorRepo.GetByIdWithIncludingDays(appointment.DocId);
                if (Doc is null)
                    return NotFound();

                var availableDays = Doc.AvailableDays;
                foreach (var d in availableDays)
                {
                    appointment.AvailableDays.Add(d.Dayid, d.ToString());
                }
                return View(appointment);

            }
            //get patient id using Identity
            if (_signInManager.IsSignedIn(User))
            {
                var userIdentity = User.Claims.First().Value;
                var patient = _petientRepo.GetAllIQuarable().Where(p => p.IdentityId == userIdentity).FirstOrDefault();
                if (patient == null)
                    return NotFound();
                appointment.PatienteId = patient.PatientID;
            }
            else
                return BadRequest();
            //check if docter is exist
            var doctor = _doctorRepo.GetById(appointment.DocId);
            if (doctor == null)
                return BadRequest();
            //if this appointment is already boocked
            var isBoocked = _appointmentRepo.IsBoocked(appointment);
            if (isBoocked == true)
            {
                ModelState.AddModelError("", "You already blocked in this day ");
                //get available days and to appointmenViewModel
                var Doc = _doctorRepo.GetByIdWithIncludingDays(appointment.DocId);
                var availableDays = Doc.AvailableDays.Where(d => d.BookedAppointments < d.countAppointment);
                foreach (var d in availableDays)
                {
                    appointment.AvailableDays.Add(d.Dayid, d.ToString());
                }
                return View(appointment);
            }

            // check if Doctor available to Home Exmamination whene user choose  HomeExmamination
            if (appointment.AppointmentType.Equals(AppointmentType.HomeExamination))
            {
                if (!doctor.HomeExamination)
                {
                    ModelState.AddModelError("AppointmentType", "Ooh!Home Examination Not available of this doctor");
                    //get available days and to appointmenViewModel
                    var Doc = _doctorRepo.GetByIdWithIncludingDays(appointment.DocId);
                    var availableDays = Doc.AvailableDays.Where(d => d.BookedAppointments < d.countAppointment);
                    foreach (var d in availableDays)
                    {
                        appointment.AvailableDays.Add(d.Dayid, d.ToString());
                    }
                    return View(appointment);
                }
            }

            _appointmentRepo.Insert(appointment);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Patient")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();
            var appo = _appointmentRepo.GetById(id);
            if (appo == null)
                return NotFound();
            var result = _appointmentRepo.Delete(id);
            return result ? Ok() : BadRequest();
        }


    }
}
