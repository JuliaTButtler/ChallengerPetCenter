namespace ChallengerPetCenter.Domain.Rules
{
    public static class DiarioEntradaRules
    {
        public static bool IsDataValida(DateTime data) =>
            data != default;

        public static bool PodeCriarEntradaParaData(bool jaExisteEntradaNaData) =>
            !jaExisteEntradaNaData;

        public static string? ValidarCriacao(DateTime data, bool petExiste, bool jaExisteEntradaNaData)
        {
            if (!petExiste)
            {
                return "Pet não encontrado.";
            }

            if (!IsDataValida(data))
            {
                return "Data da entrada é obrigatória.";
            }

            if (!PodeCriarEntradaParaData(jaExisteEntradaNaData))
            {
                return "Já existe uma entrada para este pet nesta data.";
            }

            return null;
        }
    }
}
