using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TODOAPP.Data;
using TODOAPP.Dtos;
using TODOAPP.Models;

namespace TODOAPP.Controlers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class TodoController : ControllerBase
  {
    DataContextDapper _dapper;
    public TodoController(DataContextDapper dapper)
    {
      _dapper = dapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
      string? userId = this.User.FindFirstValue("userId");
      IEnumerable<Todo> todos = await _dapper.LoadData<Todo>("SELECT * FROM TodoAppSchema.[Todo] WHERE UserId=@UserId", new DynamicParameters(new { UserId = userId }));
      return Ok(todos);
    }
    [HttpGet("{todoId}")]
    public async Task<IActionResult> GetTodo(int todoId)
    {
      string? userId = this.User.FindFirstValue("userId");
      Todo? todo = await _dapper.LoadSingleData<Todo>("SELECT * FROM TodoAppSchema.[Todo] WHERE [TodoId]=@todoId AND [UserId]=@UserId", new DynamicParameters(new { UserId = userId, TodoId = todoId }));
      if (todo == null) return NotFound();
      return Ok(todo);
    }
    [HttpPost]
    public async Task<IActionResult> AddTodo(AddTodoDto addTodoDto)
    {
      string? userId = this.User.FindFirstValue("userId");
      bool todoCreated = await _dapper.ExecuteSql("INSERT INTO TodoAppSchema.[Todo]([UserId],[TodoContent]) VALUES(@UserId,@TodoContent)", new DynamicParameters(new { UserId = userId, TodoContent = addTodoDto.TodoContent }));
      if (todoCreated)
        return StatusCode(201);
      // else return BadRequest(new Dictionary<string,string>(){{"message","something went wrong"}});
      return BadRequest();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateTodo(UpdateTodoDto todo)
    {
      string? userId = this.User.FindFirstValue("userId");
      DynamicParameters parameters = new DynamicParameters(
        new
        {
          TodoContent = todo.TodoContent,
          TodoCompleted = todo.TodoCompleted,
          TodoId = todo.TodoId,
          UserId = userId
        }
      );
      if (await _dapper.ExecuteSql("UPDATE TodoAppSchema.[Todo] SET [TodoContent]=@TodoContent,[TodoCompleted]=@TodoCompleted WHERE [TodoId]=@TodoId AND [UserId]=@UserId", parameters))
        return Ok();
      return BadRequest();
    }
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> DeleteTodo(int todoId)
    {
      string? userId = this.User.FindFirstValue("userId");

      if (await _dapper.ExecuteSql("DELETE FROM TodoAppSchema.[Todo] WHERE TodoId=@TodoId", new DynamicParameters(new { TodoId = todoId, UserId = userId })))
        return NoContent();

      return NotFound();
    }
  }
}