namespace Cadastramento.Core
{
    public class SessionInfo
    {
        public string SessionId { get; set; } = string.Empty;
        public bool Acesso { get; set; }
        public int? IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public Dictionary<string, string> Raw { get; set; } = new();
    }
}
