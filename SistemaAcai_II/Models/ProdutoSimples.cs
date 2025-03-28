using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class ProdutoSimples
    {
        [Display(Name = "Código", Description = "Código.")]
        public int Id { get; set; }

        [Display(Name = "Produto", Description = "Produto")]
        [Required(ErrorMessage = "A Descrição é obrigatório.")]
        public string Descricao { get; set; }

        [Display(Name = "Valor", Description = "Valor.")]
        [Required(ErrorMessage = "A valor é obrigatória")]
        public decimal PrecoUn { get; set; }

        public int? Quantidade { get; set; }

        public decimal peso { get; set; }
    }
}
