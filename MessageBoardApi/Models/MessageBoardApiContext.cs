using Microsoft.EntityFrameworkCore;

namespace MessageBoardApi.Models
{
  public class MessageBoardApiContext : DbContext
  {
    public DbSet<Message> Messages { get; set; }

    public MessageBoardApiContext(DbContextOptions<MessageBoardApiContext> options) : base(options)
    {
      
    }

  }
}