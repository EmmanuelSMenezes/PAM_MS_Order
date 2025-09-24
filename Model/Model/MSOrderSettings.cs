namespace Domain.Model
{
  public class MSOrderSettings
  {
    public string ConnectionString { get; set; }

    public string PrivateSecretKey { get; set; }

    public string TokenValidationMinutes { get; set; }
  }
}