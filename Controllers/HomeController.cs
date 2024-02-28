using Doctor_Appointment.Data;
using Doctor_Appointment.Models;
using Doctor_Appointment.Repo;
using Doctor_Appointment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Doctor_Appointment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ApplicationDbContext Context { get; }
        public IDoctorRepo DoctorRepo { get; }


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IDoctorRepo doctorRepo)
        {
            _logger = logger;
            Context = context;
            DoctorRepo = doctorRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public ActionResult SpecialistFilter()
        {
            ViewBag.Spectialist = Context.Doctors.ToList();

            return View(DoctorRepo.GetAll());
        }

        [HttpPost]
        public ActionResult SpecialistFilter(Spectialist spl)
        {
            ViewBag.Spectialist = Context.Doctors.ToList();

            if ((Context.Doctors.Any(s => s.Specialist == spl)))
            {
                List<Doctor> spldoc = DoctorRepo.GetBySpecialist(spl);

                return View(spldoc);

            }
            return View();

        }


    }
}