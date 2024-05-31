using Dapper;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRespository
    {
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company company)
        {
            var sql = @"INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);
                        SELECT CAST(SCOPE_IDENTITY() as int);    
                        ";

            var id = db.Query<int>(sql, company).Single();

            company.CompanyId = id;

            //foreach(var emp in company.Employees)
            //{
            //    emp.CompanyId = company.CompanyId;
            //    var sql1 = @"INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);
            //            SELECT CAST(SCOPE_IDENTITY() as int);    
            //            ";

            //    db.Query<int>(sql1, emp).Single();
            //}

            company.Employees.Select(c => { c.CompanyId = id; return c; }).ToList();

            var sqlExmp = @"INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);
                        SELECT CAST(SCOPE_IDENTITY() as int);    
                        ";

            db.Execute(sqlExmp, company.Employees);
        }

        public void AddTestCompanyWithEmployeesWithTransaction(Company company)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var sql = @"INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);
                        SELECT CAST(SCOPE_IDENTITY() as int);    
                        ";

                    var id = db.Query<int>(sql, company).Single();

                    company.CompanyId = id;


                    company.Employees.Select(c => { c.CompanyId = id; return c; }).ToList();

                    var sqlExmp = @"INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);
                        SELECT CAST(SCOPE_IDENTITY() as int);    
                        ";

                    db.Execute(sqlExmp, company.Employees);
                    transaction.Complete();
                }
                catch(Exception ex)
                {

                }

            }



            
        }


        public List<Company> GetAllCompanyWithEmployees()
        {
            var sql = @"
                    SELECT C.*, E.*
                    FROM Employees E
                    INNER JOIN Companies C on E.CompanyId = C.CompanyId
                       ";

            var companyDict = new Dictionary<int, Company>();

            var company = db.Query<Company, Employee, Company>(sql, (c, e) =>
            {
                if(!companyDict.TryGetValue(c.CompanyId, out var currentCompany))
                {
                    currentCompany = c;
                    companyDict.Add(currentCompany.CompanyId, currentCompany);
                }

                currentCompany.Employees.Add(e);

                return currentCompany;
            }, splitOn: "EmployeeId");

            return company.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new
            {
                CompanyId = id
            };

            var sql = @"
                SELECT * FROM Companies WHERE CompanyId = @CompanyId;

                SELECT * FROM Employees WHERE CompanyId = @CompanyId;
                ";

            Company company;

            using (var lists = db.QueryMultiple(sql, p))
            {
                company = lists.Read<Company>().ToList().FirstOrDefault();
                company.Employees = lists.Read<Employee>().ToList();
            }

            return company;
        }

        public List<Employee> GetEmployeeWithCompany(int companyId)
        {
            // One to One mapping
            var sql = @"
                    SELECT E.*, C.*
                    FROM Employees E
                    INNER JOIN Companies C on E.CompanyId = C.CompanyId
                       ";

            if ( companyId != 0)
            {
                sql += " WHERE E.CompanyId = @companyId ";
            }

            var employee = db.Query<Employee, Company, Employee>(sql, (e, c) =>
            {
                e.Company = c;

                return e;
            }, new { companyId }, splitOn: "CompanyId");


            return employee.ToList();
        }

        public void RemoveRange(int[] companyId)
        {
            db.Query("DELETE FROM Companies WHERE CompanyId IN @companyId", new { companyId });
        }


        public List<Company> FilterCompanyByName(string name)
        {
            return db.Query<Company>("SELECT * FROM Companies WHERE Name LIKE '%' + @name + '%'", new { name }).ToList();
        }
    }
}
