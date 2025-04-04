namespace Secure_P_Backend.Cors;

public class CorsConfigures
{
    public List<CorsOrigin>? Origins { get; set; }

    public class CorsOrigin
    {
        public string? Name { get; set; }
        public string? Origins { get; set; }
    }
}
