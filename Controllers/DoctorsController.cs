using Doctor_Appointment.Models;
using Doctor_Appointment.Repo;
using Doctor_Appointment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text;

namespace Doctor_Appointment.Controllers
{
	[Authorize(Roles = "Admin")]
	public class DoctorsController : Controller
	{
		//public ApplicationDbContext Context { get; }
		private readonly IDoctorRepo _doctor;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		public DoctorsController(IDoctorRepo doctor, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{

			_doctor = doctor;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[AllowAnonymous]
		// GET: DoctorsController
		public ActionResult Index()
		{
			return View(_doctor.GetAll());
		}

		// GET: DoctorsController/Details/5
		[AllowAnonymous]
		public ActionResult Details(int id)
		{
			if (id <= 0)
				return BadRequest();
			var doctor = _doctor.GetByIdWithIncludingDays(id);

			if (doctor is null)
				return NotFound();
			return View(doctor);
		}


		// GET: DoctorsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: DoctorsController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateDoctoreVM doc)
		{
			if (!ModelState.IsValid)
			{
				return View(doc);
			}
			var DoctorName = _doctor.GetAll().Where(d => d.FullName == doc.FullName && d.Specialist == doc.Specialist).FirstOrDefault();
			if (DoctorName != null)
			{
				ModelState.AddModelError("FullName", "This name already exist");
				return View(doc);
			}

			var registResult = await RegiterForDoctor(new DoctorRegistrationVM()
			{
				Email = doc.Email,
				Password = doc.Password,
			});
			if (registResult.IsRegister is false)
			{
				ModelState.AddModelError("", registResult.Result);
				return View(doc);
			}
			doc.IdentityId = registResult.Result;
			var DoctorID = _doctor.Insert(doc);


			return RedirectToAction("Create", "DailyAvailbilities", new { docId = DoctorID });

		}

		// GET: DoctorsController/Edit/5
		public ActionResult Edit(int id)
		{
			if (id <= 0)
				return BadRequest();
			var doctor = _doctor.GetById(id);
			if (doctor is null)
				return NotFound();

			var doctorVM = new EditDoctorVM()
			{
				Id = doctor.DoctorID,
				FullName = doctor.FullName,
				Gender = doctor.Gender,
				Specialist = doctor.Specialist,
				Degree = doctor.Degree,
				Description = doctor.Description,
				Clinic_Location = doctor.Clinic_Location,
				Clinic_PhonNum = doctor.Clinic_PhonNum,
				Price = doctor.Price,
				HomeExamination = doctor.HomeExamination,


			};

			return View(doctorVM);

		}

		// POST: DoctorsController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditDoctorVM doctorVM)
		{
			if (!ModelState.IsValid)
			{
				return View(doctorVM);
			}
			var DoctorName = _doctor.GetAll().Where(d => d.FullName == doctorVM.FullName && d.Specialist == doctorVM.Specialist && d.DoctorID != doctorVM.Id).FirstOrDefault();
			if (DoctorName != null)
			{
				ModelState.AddModelError("FullName", "This name already exist");
				return View(doctorVM);
			}

			_doctor.Update(doctorVM);
			return RedirectToAction("Details", new { id = doctorVM.Id });

		}

		//GET: DoctorsController/Delete/5
		public async Task<IActionResult> DeleteAsync(int id)
		{
			if (id <= 0)
				return BadRequest();
			var doc = _doctor.GetById(id);
			if (doc is null)
				return NotFound();
			var result = _doctor.Delete(id);
			if (result)
			{
				var user = await _signInManager.UserManager.FindByIdAsync(doc.IdentityId);
				if (user is not null)
					await _signInManager.UserManager.DeleteAsync(user);
				return RedirectToAction(nameof(Index));

			}
			return BadRequest();
		}
		//Register doctor In user table with doctor role
		private async Task<ReturnRegisterDocVm> RegiterForDoctor(DoctorRegistrationVM newUser)
		{

			ApplicationUser userModel = new();
			userModel.UserName = new MailAddress(newUser.Email).User;


			userModel.Email = newUser.Email;
			userModel.PasswordHash = newUser.Password;
			StringBuilder stringBuilder = new StringBuilder();


			IdentityResult result = await _userManager.CreateAsync(userModel, userModel.PasswordHash);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(userModel, "Doctor");

				stringBuilder.Append(userModel.Id);
				return new ReturnRegisterDocVm() { IsRegister = true, Result = stringBuilder.ToString() };

			}
			else
			{
				foreach (var error in result.Errors)
				{
					stringBuilder.AppendLine(error.Description);
				}
			}

			return new ReturnRegisterDocVm() { IsRegister = false, Result = stringBuilder.ToString() };

		}


	}
}
