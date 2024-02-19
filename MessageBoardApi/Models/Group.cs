namespace MessageBoardApi.Models
{
  public class Group
  {
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public List<Message> Messages { get; set; }
  }
}