using Doctor_Appointment.Models;
using Doctor_Appointment.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doctor_Appointment.Controllers
{
    [Authorize]

    public class PatientsController : Controller
    {
        private readonly IPatientRepo _patientRepo;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _user;

        public PatientsController(IPatientRepo patientRepo, SignInManager<IdentityUser> manager, UserManager<IdentityUser> user)
        {
            _patientRepo = patientRepo;
            _signInManager = manager;
            _user = user;
        }

        // GET: Patients
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_patientRepo.GetAll());
        }



        [Authorize(Roles = "Patient")]

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var userIdentity = User.Claims.First().Value;
                    patient.IdentityId = userIdentity;
                }
                else
                {
                    return BadRequest();
                }
                var DoctorName = _patientRepo.GetAllIQuarable().Where(p => p.FullName == patient.FullName).FirstOrDefault();
                if (DoctorName != null)
                {
                    ModelState.AddModelError("FullName", "This name already exist");
                    return View(patient);
                }
                _patientRepo.Insert(patient);

                return RedirectToAction("Index", "Home");
            }
            return View(patient);
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UncomplatedRegister()
        {

            if (_signInManager.IsSignedIn(User))
            {
                var userIdentity = User.Claims.First().Value;
                var user = await _signInManager.UserManager.FindByIdAsync(userIdentity);
                if (user is not null)
                    await _signInManager.UserManager.DeleteAsync(user);
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");

        }


        // GET: Patients/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0) return BadRequest();

            var patient = _patientRepo.GetById(id);

            if (patient is null)
                return NotFound();
            var result = _patientRepo.Delete(id);
            if (result)
            {
                var user = await _signInManager.UserManager.FindByIdAsync(patient.IdentityId);
                if (user is not null)
                    await _signInManager.UserManager.DeleteAsync(user);
                return Ok();
            }
            return BadRequest();
        }



    }
}
