namespace Domain.Model
{
  public class EmailSettings
  {
    public string PrimaryDomain { get; set; }
    public string PrimaryPort { get; set; }
    public string UsernameEmail { get; set; }
    public string UsernamePassword { get; set; }
    public string FromEmail { get; set; }
    public string ToEmail { get; set; }
    public string CcEmail { get; set; }
    public string EnableSsl { get; set; }
    public string UseDefaultCredentials { get; set; }
  }
}
