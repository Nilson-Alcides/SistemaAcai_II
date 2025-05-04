using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class Caixa
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Status E-mail")]
        public string StatusEmail { get; set; }

        [Display(Name = "Situacao")]
        public string Situacao { get; set; }

        [Display(Name = "Abertura")]
        [Required(ErrorMessage = "A data de abrtura do é obrigatório.")]
        public DateTime DataAbertura { get; set; } = DateTime.Now;

        [Display(Name = "Fechamento")]
        public DateTime? DataFechamento { get; set; }

        [Display(Name = "Valor Inicial")]
        public decimal ValorInicial { get; set; }
    }
}
