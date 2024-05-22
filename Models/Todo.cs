namespace TODOAPP.Models
{
  public class Todo
  {
    public int TodoId{get;set;}
    public int UserId{get;set;}
    public string TodoContent{get;set;}="";
    public bool TodoCompleted{get;set;}
  }
}