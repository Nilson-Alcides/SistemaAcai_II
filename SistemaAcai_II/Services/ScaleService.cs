using System;

namespace SistemaAcai_II.Services
{
    public interface IScaleService
    {
        /// <summary>Retorna o último peso estável e o instante da leitura.</summary>
        (decimal weight, DateTime timestamp) GetLastStableWeight();
        /// <summary>Atualiza o último peso estável (chamado pelo leitor da serial).</summary>
        void UpdateStableWeight(decimal weight);
        /// <summary>Zera o peso (quando quiser descartar o valor após inserir item).</summary>
        void Clear();
    }

    public class ScaleService : IScaleService
    {
        private readonly object _lock = new();
        private decimal _lastWeight;
        private DateTime _ts = DateTime.MinValue;

        public (decimal weight, DateTime timestamp) GetLastStableWeight()
        {
            lock (_lock) return (_lastWeight, _ts);
        }

        public void UpdateStableWeight(decimal weight)
        {
            // Se quiser, aplique aqui filtros de estabilidade/ruído
            lock (_lock)
            {
                _lastWeight = weight < 0 ? 0 : weight;
                _ts = DateTime.Now;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _lastWeight = 0;
                _ts = DateTime.MinValue;
            }
        }
    }
}
