using przykl_kol1.DTOs;

namespace przykl_kol1.Repositories;

public interface ITestRepository
{
    Task<string?> GetById(int testId);

    Task<XType> AddNewTest(XTypePost testdata);
}