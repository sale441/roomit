using System.Text.Json.Serialization;

namespace roomit2.Models
{
    public class ReCaptchaResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
