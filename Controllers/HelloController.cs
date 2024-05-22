using Microsoft.AspNetCore.Mvc;
using TODOAPP.Data;

namespace TODOAPP.Controlers
{
  [ApiController]
  [Route("[controller]")]
  public class HelloController : ControllerBase
  {
    DataContextDapper _dapper;
    public HelloController(DataContextDapper dapper)
    {
      _dapper = dapper;
    }
    [HttpGet]
    public string Hello()
    {
      return "Hello Akro";
    }
  }
}