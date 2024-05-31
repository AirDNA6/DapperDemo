using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;

namespace DapperDemo.Repository
{
    public class CompanyRepositorySP : ICompanyRepository
    {
        private IDbConnection db;

        public CompanyRepositorySP(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", 0, DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Name", company.Name);
            parameters.Add("@Address", company.Address);
            parameters.Add("@City", company.City);
            parameters.Add("@State", company.State);
            parameters.Add("@PostalCode", company.PostalCode);

            this.db.Execute("usp_AddCompany", parameters, commandType: CommandType.StoredProcedure);

            company.CompanyId = parameters.Get<int>("CompanyId");

            return company;

        }

        public Company Find(int id)
        {
            return db.Query<Company>("usp_GetCompany", new { CompanyId = id }, commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        public List<Company> GetAll()
        {

            return db.Query<Company>("usp_GetALLCompany", commandType: CommandType.StoredProcedure).ToList();
        }

        public void Remove(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", Id);
            db.Execute("usp_RemoveCompany", parameters, commandType: CommandType.StoredProcedure);
        }

        public Company Update(Company company)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", company.CompanyId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@Name", company.Name);
            parameters.Add("@Address", company.Address);
            parameters.Add("@City", company.City);
            parameters.Add("@State", company.State);
            parameters.Add("@PostalCode", company.PostalCode);

            this.db.Execute("usp_UpdateCompany", parameters, commandType: CommandType.StoredProcedure);

            company.CompanyId = parameters.Get<int>("CompanyId");

            return company;
        }
    }
}
