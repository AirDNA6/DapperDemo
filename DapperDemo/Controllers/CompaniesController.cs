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
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBonusRespository _bonusRespository;
        private readonly IDapperSPRepository _dapperSPRepository;

        public CompaniesController(ICompanyRepository companyRepository, IEmployeeRepository employeeRepository, IBonusRespository bonusRespository, IDapperSPRepository dapperSPRepository)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _bonusRespository = bonusRespository;
            _dapperSPRepository = dapperSPRepository;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            //return View(_companyRepository.GetAll());
            return View(_dapperSPRepository.List<Company>("usp_GetALLCompany"));
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _bonusRespository.GetCompanyWithEmployees(id.GetValueOrDefault());

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (ModelState.IsValid)
            {
                _companyRepository.Add(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var company = _companyRepository.Find(id.GetValueOrDefault());
            var company = _dapperSPRepository.Single<Company>("usp_GetCompany", new { CompanyId = id.GetValueOrDefault() });
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _companyRepository.Update(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _companyRepository.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }

    }
}
