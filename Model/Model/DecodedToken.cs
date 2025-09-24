using System;

namespace Domain.Model
{
  public class DecodedToken
  {
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string email { get; set; }
    public string name { get; set; }
  }
}
