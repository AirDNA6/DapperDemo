﻿using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;

namespace DapperDemo.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private IDbConnection db;

        public EmployeeRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Employee Add(Employee employee)
        {
            var sql = @"INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);
                        SELECT CAST(SCOPE_IDENTITY() as int);    
                        ";

            var id = db.Query<int>(sql, employee).Single();

            employee.EmployeeId = id;
            return employee;
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            var sql = @"INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);
                        SELECT CAST(SCOPE_IDENTITY() as int);    
                        ";

            var id = await db.QueryAsync<int>(sql, employee);

            employee.EmployeeId = id.Single();
            return employee;
        }

        public Employee Find(int id)
        {
            var sql = "SELECT * FROM Employees WHERE EmployeeId = @Id";
            return db.Query<Employee>(sql, new { Id = id }).Single();
        }

        public List<Employee> GetAll()
        {
            var sql = "SELECT * FROM Employees";

            return db.Query<Employee>(sql).ToList();
        }

        public void Remove(int Id)
        {
            var sql = "DELETE FROM Employees WHERE Employees = @Id";
            db.Execute(sql, new { Id });
        }

        public Employee Update(Employee employee)
        {
            var sql = "UPDATE Employees SET Name = @Name, Title = @Title, Email = @Email, Phone = @Phone, CompanyId = @CompanyId WHERE EmployeeId = @EmployeeId";

            db.Execute(sql, employee);
            return employee;
        }
    }
}
