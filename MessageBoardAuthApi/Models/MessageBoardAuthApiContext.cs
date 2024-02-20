using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessageBoardAuthApi.Models
{
  public class MessageBoardAuthApiContext : IdentityDbContext<ApplicationUser>
  {
    public MessageBoardAuthApiContext(DbContextOptions options) : base(options) { }
  }
}