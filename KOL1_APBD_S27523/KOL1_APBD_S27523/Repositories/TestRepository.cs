using System.Data.SqlClient;
using przykl_kol1.DTOs;

namespace przykl_kol1.Repositories;

public class TestRepository(IConfiguration configuration) : ITestRepository
{
    private IConfiguration _configuration = configuration;


    public async Task<string?> GetById(int testId)
    {
    }

    public async Task<XType> AddNewTest(XTypePost testdata)
    {
    }
}