using Dapper;
using Dapper.Contrib.Extensions;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;

namespace DapperDemo.Repository
{
    public class CompanyRepositoryContrib : ICompanyRepository
    {
        private IDbConnection db;

        public CompanyRepositoryContrib(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
           var id =  db.Insert(company);

            company.CompanyId = (int)id;

            return company;

        }

        public Company Find(int id)
        {
            return db.Get<Company>(id);
        }

        public List<Company> GetAll()
        {

            return db.GetAll<Company>().ToList();
        }

        public void Remove(int Id)
        {
            db.Delete(new Company { CompanyId = Id });
        }

        public Company Update(Company company)
        {
            db.Update(company);
            return company;
        }
    }
}
