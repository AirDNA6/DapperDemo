using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DapperDemo.Data;
using DapperDemo.Models;
using DapperDemo.Repository;

namespace DapperDemo.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBonusRespository _bonusRespository;

        [BindProperty]
        public Employee Employee { get; set; }

        public EmployeesController(ICompanyRepository companyRepository, IEmployeeRepository employeeRepository, IBonusRespository bonusRespository)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _bonusRespository = bonusRespository;
        }

        // GET: Companies
        public async Task<IActionResult> Index(int companyId = 0)
        {
            //List<Employee> employees = _employeeRepository.GetAll();

            //foreach (var item in employees)
            //{
            //    item.Company = _companyRepository.Find(item.CompanyId);
            //}

            List<Employee> employees = _bonusRespository.GetEmployeeWithCompany(companyId);

            return View(employees);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _companyRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString()
            });

            ViewBag.CompanyList = companyList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                await _employeeRepository.AddAsync(Employee);
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> companyList = _companyRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CompanyId.ToString()
            });

            ViewBag.CompanyList = companyList;

            var company = _employeeRepository.Find(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != Employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                _employeeRepository.Update(Employee);

                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
             _employeeRepository.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }

    }
}
