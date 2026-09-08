namespace ChallengerPetCenter.Domain.Rules
{
    public static class UserRules
    {
        public static bool IsNomeValido(string? nome) =>
            !string.IsNullOrWhiteSpace(nome);

        public static bool IsEmailValido(string? email) =>
            !string.IsNullOrWhiteSpace(email) && email.Contains('@');

        public static bool IsTipoUsuarioValido(string? tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
            {
                return false;
            }

            var normalizado = tipo.Trim().ToUpperInvariant();
            return normalizado is "TUTOR" or "VETERINARIO";
        }

        public static string? ValidarCriacao(string? nome, string? email, string? tipoUsuario)
        {
            if (!IsNomeValido(nome))
            {
                return "Nome é obrigatório.";
            }

            if (!IsEmailValido(email))
            {
                return "Email é obrigatório.";
            }

            if (!IsTipoUsuarioValido(tipoUsuario))
            {
                return "Tipo de usuário inválido. Use TUTOR ou VETERINARIO.";
            }

            return null;
        }
    }
}
