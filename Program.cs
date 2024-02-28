using Doctor_Appointment.Clients;
using Doctor_Appointment.Data;
using Doctor_Appointment.Repo;
using Doctor_Appointment.Repo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Doctor_Appointment
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();



			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddSingleton(x => new PaypalClient(
			builder.Configuration["Paypal:ClientId"],
			builder.Configuration["Paypal:ClientSecret"],
			builder.Configuration["Paypal:Mode"]
			 )
		);
			//our own services

			builder.Services.AddScoped<IDoctorRepo, DoctorRepoServices>();
			builder.Services.AddScoped<IPatientRepo, PatientRepoServices>();
			builder.Services.AddScoped<IAppointmentRepo, AppointementRepoServices>();
			builder.Services.AddScoped<IDailyAvailbilityRepo, DailyAvailbiltyRepoServices>();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;


				options.LoginPath = "/Identity/Account/Login";
				//options.AccessDeniedPath = "/Identity/Account/AccessDenied";
				options.SlidingExpiration = true;
			});

			builder.Services.AddControllersWithViews();


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			//Create Roles
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var roles = new[] { "Doctor", "Patient", "Admin" };
				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
						await roleManager.CreateAsync(new IdentityRole(role));
				}
			}
			//Craete User
			using (var scope = app.Services.CreateScope())
			{
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
				string email = "Admin45@gmail.com";
				string password = "Admin@12345";
				if (await userManager.FindByEmailAsync(email) == null)
				{
					var user = new IdentityUser();
					user.UserName = new MailAddress(email).User;
					user.Email = email;
					await userManager.CreateAsync(user, password);
					await userManager.AddToRoleAsync(user, "Admin");
				}
			}



			app.Run();
		}
	}
}