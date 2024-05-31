using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public interface IBonusRespository
    {
        List<Employee> GetEmployeeWithCompany(int companyId);

        Company GetCompanyWithEmployees(int id);

        List<Company> GetAllCompanyWithEmployees();

        void AddTestCompanyWithEmployees(Company company);
        void AddTestCompanyWithEmployeesWithTransaction(Company company);

        void RemoveRange(int[] companyId);

        List<Company> FilterCompanyByName(string name);
    }
}
