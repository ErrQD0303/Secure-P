namespace Secure_P_Backend.CORS;

public class CORSConfigures
{
    public List<CORSOrigin>? Origins { get; set; }

    public class CORSOrigin
    {
        public string? Name { get; set; }
        public string? Origins { get; set; }
    }
}
