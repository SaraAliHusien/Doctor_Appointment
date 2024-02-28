using Doctor_Appointment.Data;
using Doctor_Appointment.Models;
using Doctor_Appointment.Repo;
using Doctor_Appointment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment.Controllers
{
	[Authorize(Roles = "Admin,Doctor")]
	public class DailyAvailbilitiesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IDailyAvailbilityRepo _dailyAvailbilityRepo;
		private readonly IDoctorRepo _doctorRepo;
		private readonly IAppointmentRepo _appointmentRepo;

		private readonly SignInManager<IdentityUser> _signInManager;

		public DailyAvailbilitiesController(ApplicationDbContext context, IDailyAvailbilityRepo dailyAvailbilityRepo, IAppointmentRepo appointmentRepo, IDoctorRepo doctorRepo, SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_doctorRepo = doctorRepo;
			_dailyAvailbilityRepo = dailyAvailbilityRepo;
			_appointmentRepo = appointmentRepo;
			_signInManager = signInManager;
		}

		// GET: DailyAvailbilities
		public IActionResult Index(int docid = 0)
		{
			Doctor? doc;
			if (User.IsInRole("Doctor"))
			{
				var userIdentity = User.Claims.First().Value;
				doc = _doctorRepo.GetByIdentity(userIdentity);
				return View(doc);

			}

			if (docid <= 0)
				return BadRequest();
			doc = _doctorRepo.GetByIdWithIncludingDays(docid);
			if (doc is null)
				return NotFound();
			return View(doc);
		}


		public IActionResult DayAppointments(int dayId)
		{
			if (dayId <= 0)
				return BadRequest();
			var day = _dailyAvailbilityRepo.GetById(dayId);
			if (day is null)
				return NotFound();
			var appointments = _appointmentRepo.GetAllQuerable().Where(p => p.DayId == dayId).Include(p => p.Patient).Include(p => p.Day).ToList();
			return View(appointments);
		}

		// GET: DailyAvailbilities/Create
		public IActionResult Create(int docId)
		{
			if (docId <= 0)
				return BadRequest();
			var doctor = _doctorRepo.GetById(docId);
			if (doctor is null)
				return NotFound();
			var availableDays = new DailyAvilblabityVM() { DoctorId = docId, DoctorName = doctor.FullName };

			availableDays.Days.Add(new());
			return View(availableDays);

		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddAvailableDay([Bind("Days")] DailyAvilblabityVM doc)
		{
			if (!ModelState.IsValid)
				return PartialView("DailyAvailbilities", doc);

			doc.Days.Add(new DailyAvailbility());
			return PartialView("DailyAvailbilities", doc);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create([Bind("Days", "DoctorId", "DoctorName")] DailyAvilblabityVM doc)
		{
			if (ModelState.IsValid)
			{
				if (doc.DoctorId <= 0)
					return BadRequest();
				var doctor = _doctorRepo.GetById(doc.DoctorId);
				if (doctor is null)
					return NotFound();
				foreach (var day in doc.Days)
				{
					//check if start time sorter than end time
					var compareRes = day.StartTime.CompareTo(day.EndTime);
					if (compareRes != -1)
						ModelState.AddModelError("", $"Start time in day  [{day.Date.ToString("dd-MM-yyyy")} ] must be shorter than End time ");

					//check if day existed
					var isExisted = _dailyAvailbilityRepo.IsExistedDay(day, doc.DoctorId);
					if (isExisted)
						ModelState.AddModelError("", $"This day [{day.Date.ToString("dd-MM-yyyy")} ] in the same time is already existed in doctor exmination day ");
				}
				//return model state errors if is found
				if (ModelState.ErrorCount > 0)
					return View(doc);
				//add days to doctors availabe days
				doctor.AvailableDays = doc.Days;
				_doctorRepo.Update(doctor);

				return RedirectToAction("Index", new { docId = doc.DoctorId });

			}
			return View(doc);
		}

		public IActionResult Edit(int id)
		{
			if (id <= 0) return BadRequest();

			var day = _dailyAvailbilityRepo.GetByIdWithIncliding(id);
			if (day == null)

				return NotFound();
			var dayVM = new EditDayVM()
			{
				DoctorName = day.Doctor.FullName,
				DoctorId = day.DoctorID,
				Day = new DailyAvailbility()
				{
					Dayid = day.Dayid,
					Date = day.Date,
					StartTime = day.StartTime,
					EndTime = day.EndTime,
					countAppointment = day.countAppointment,
					BookedAppointments = day.BookedAppointments
				},


			};

			return View(dayVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditDayVM dailyAvailbility)
		{

			if (ModelState.IsValid)
			{
				//check is valide ids
				if (dailyAvailbility.Day.Dayid <= 0 || dailyAvailbility.DoctorId <= 0)
					return BadRequest();
				//chech if day is found
				var day = _dailyAvailbilityRepo.GetByIdWithIncliding(dailyAvailbility.Day.Dayid);
				if (day is null)
					return NotFound();
				// check if doctor is found
				var doc = _doctorRepo.GetById(dailyAvailbility.DoctorId);
				if (doc is null)
					return NotFound();
				//check if start time sorter than end time
				var compareRes = dailyAvailbility.Day.StartTime.CompareTo(dailyAvailbility.Day.EndTime);
				if (compareRes != -1)
					ModelState.AddModelError("", $"Start time in day  [{day.Date.ToString("dd-MM-yyyy")} ] must be shorter than End time ");
				//check if day repetted
				var isExisted = _dailyAvailbilityRepo.IsExistedEditDay(day);
				if (isExisted)
				{
					ModelState.AddModelError("", $"This day [{day.Date.ToString("dd-MM-yyyy")} ] in the same time is already existed in doctor exmination day ");
					return View(dailyAvailbility);
				}
				// updated
				_dailyAvailbilityRepo.Update(dailyAvailbility.Day);
				return RedirectToAction("Index", new { DocId = doc.DoctorID });
			}
			return View(dailyAvailbility);
		}

		// GET: DailyAvailbilities/Delete/5
		public IActionResult Delete(int DocId, int DayId)
		{


			if (DocId <= 0 || DayId <= 0)
				return BadRequest();
			var Doc = _doctorRepo.GetById(DocId);
			if (Doc == null)
				return NotFound();
			var day = _dailyAvailbilityRepo.GetById(DayId);
			if (day == null)
				return NotFound();
			if (day.BookedAppointments > 0)
				return BadRequest();
			var result = _dailyAvailbilityRepo.Delete(DayId);
			return result ? Ok() : BadRequest();
		}



		private bool DailyAvailbilityExists(int id)
		{
			return (_context.DailyAvailbilities?.Any(e => e.Dayid == id)).GetValueOrDefault();
		}
	}
}
