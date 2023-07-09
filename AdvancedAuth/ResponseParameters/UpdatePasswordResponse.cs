namespace AdvancedAuth.ResponseParameters
{
    public class UpdatePasswordResponse
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; }
    }
}
