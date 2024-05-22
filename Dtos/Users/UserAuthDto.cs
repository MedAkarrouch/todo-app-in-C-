namespace TODOAPP.Dtos
{
  public class UserAuthDto
  {
    public byte[] PasswordSalt{get;set;}=new byte[0];
    public byte[] PasswordHash{get;set;}=new byte[0];
  }
}