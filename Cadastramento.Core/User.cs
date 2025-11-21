namespace Cadastramento.Core
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<string> Perfis { get; set; } = new();
    }
}
