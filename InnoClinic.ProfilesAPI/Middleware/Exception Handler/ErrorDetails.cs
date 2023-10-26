using System.Text.Json;

namespace InnoClinic.ProfilesAPI.Middleware.Exception_Handler
{
    public class ErrorDetails
    {
        public string? Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}