using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DapperDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBonusRespository _bonusRespository;

        public HomeController(ILogger<HomeController> logger, IBonusRespository bonusRespository)
        {
            _logger = logger;
            _bonusRespository = bonusRespository;
        }

        public IActionResult Index()
        {
            IEnumerable<Company> companies = _bonusRespository.GetAllCompanyWithEmployees();
            return View(companies);
        }

        public IActionResult AddTestRecords()
        {
            Company company = new Company()
            {
                Name = "Test" + Guid.NewGuid().ToString(),
                Address = "test address",
                City = "test city",
                PostalCode = "test postalCode",
                State = "test state",
                Employees = new List<Employee>()
            };

            company.Employees.Add(new Employee()
            {
                Email = "test Email",
                Name = "Test Name " + Guid.NewGuid().ToString(),
                Phone = " test phone",
                Title = "Test Manager"
            });

            company.Employees.Add(new Employee()
            {
                Email = "test Email 2",
                Name = "Test Name 2" + Guid.NewGuid().ToString(),
                Phone = " test phone 2",
                Title = "Test Manager 2"
            });

            _bonusRespository.AddTestCompanyWithEmployeesWithTransaction(company);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveTestRecords()
        {
            int[] companyIdToRemove = _bonusRespository.FilterCompanyByName("Test").Select(i => i.CompanyId).ToArray();
            _bonusRespository.RemoveRange(companyIdToRemove);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
