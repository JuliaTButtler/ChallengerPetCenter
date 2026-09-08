namespace ChallengerPetCenter.Domain.Rules
{
    public static class PetRules
    {
        public static bool IsNomeValido(string? nome) =>
            !string.IsNullOrWhiteSpace(nome);

        public static bool IsEspecieValida(string? especie) =>
            !string.IsNullOrWhiteSpace(especie);

        public static string? ValidarCriacao(string? nome, string? especie)
        {
            if (!IsNomeValido(nome))
            {
                return "Nome é obrigatório.";
            }

            if (!IsEspecieValida(especie))
            {
                return "Espécie é obrigatória.";
            }

            return null;
        }
    }
}
