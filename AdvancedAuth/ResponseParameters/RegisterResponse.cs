namespace AdvancedAuth.ResponseParameters
{
    public class RegisterResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public bool Succeeded { get; set; }
    }
}
