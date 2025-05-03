using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class FormasPagamento
    {
        [Display(Name = "Código", Description = "Código.")]
        public int Id { get; set; }

        [Display(Name = "Pagamento", Description = "descrição")]
        [Required(ErrorMessage = "Uma forma de pagamento é obrigatória.")]
        public string Nome { get; set; }
    }
}
