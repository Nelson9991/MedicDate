using MedicDate.Bussines.Services.IServices;
using MedicDate.DataAccess.Data;
using MedicDate.DataAccess.Models;
using MedicDate.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MedicDate.Bussines.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (_context.Roles.Any(x => x.Name == Sd.ROLE_ADMIN))
            {
                return;
            }

            _roleManager.CreateAsync(new AppRole()
            {
                Name = Sd.ROLE_ADMIN,
                Descripcion = "Tiene permisos para todos los m�dulos y funciones de la aplicaci�n"
            }).GetAwaiter().GetResult();

            _roleManager.CreateAsync(new AppRole()
            {
                Name = Sd.ROLE_DOCTOR,
                Descripcion =
                    "Tiene permisos para todos los m�dulos de administraci�n de citas y pacientes. No puede administrar los usuarios del sistema ni a otros doctores"
            }).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "nelsonmarro99@gmail.com",
                Email = "nelsonmarro99@gmail.com",
                Apellidos = "Master",
                Nombre = "Web",

                EmailConfirmed = true
            }, "Admin123*").GetAwaiter().GetResult();

            var user = _context.ApplicationUser.FirstOrDefault(x => x.Email == "nelsonmarro99@gmail.com");
            _userManager.AddToRoleAsync(user, Sd.ROLE_ADMIN).GetAwaiter().GetResult();
        }
    }
}