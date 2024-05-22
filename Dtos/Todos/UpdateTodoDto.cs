namespace TODOAPP.Dtos
{
  public class UpdateTodoDto
  {
    public int TodoId { get; set; }
    public string TodoContent { get; set; } = "";
    public bool TodoCompleted { get; set; }
  }
}