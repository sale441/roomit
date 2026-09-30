namespace roomit2.Models
{
    public class FriendlyCaptchaResponse
    {
        public bool success { get; set; }
        public Data? data { get; set; }
    }
    public class Data
    {
        public Challenge? challenge { get; set; }
    }

    public class Challenge
    {
        public string? timestamp { get; set; }
        public string? origin { get; set; } 
    }
}
