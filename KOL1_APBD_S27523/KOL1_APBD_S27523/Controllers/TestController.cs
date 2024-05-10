using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using przykl_kol1.DTOs;
using przykl_kol1.Repositories;

namespace przykl_kol1.Controllers;

[Route("api/test")]
[ApiController]
public class TestController(ITestRepository testRepository) : ControllerBase
{
    private ITestRepository _testRepository = testRepository;

    [HttpGet("{testId:int}/test")]
    public async Task<IActionResult> GetById(int testId)
    {
        return Ok("response");
    }

    [HttpPost]
    public async Task<IActionResult> AddNewTest(
        [FromBody] XType testpostdata)
    {
        return StatusCode(201, "response");
    }
}