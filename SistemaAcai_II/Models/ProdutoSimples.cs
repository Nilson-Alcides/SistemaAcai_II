using System;
using System.ComponentModel.DataAnnotations;
using SistemaAcai_II.Models.Constants;
using TipoMedidaEnumType = SistemaAcai_II.Models.Constants.TipoMedida;



namespace SistemaAcai_II.Models
{

    public class ProdutoSimples
    {

        [Display(Name = "Código", Description = "Código.")]
        public int Id { get; set; }

        [Display(Name = "Produto", Description = "Produto")]
        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Display(Name = "Valor", Description = "Valor.")]
        [Required(ErrorMessage = "O valor é obrigatório.")]
        public decimal PrecoUn { get; set; }

        [Display(Name = "Tipo de Medida", Description = "Tipo de Medida")]
        [Required(ErrorMessage = "O Tipo de Medida é obrigatório.")]
        public string TipoMedida { get; set; }   // vem do banco: "Unidade" ou "Kg"

        // 🔹 Propriedade auxiliar que converte a string em enum
        public TipoMedidaEnumType TipoMedidaEnum =>
            string.Equals(TipoMedida, "Kg", StringComparison.OrdinalIgnoreCase)
                ? TipoMedidaEnumType.Kg
                : TipoMedidaEnumType.Unidade;


        public int? Quantidade { get; set; }

        [Display(Name = "Peso", Description = "Peso")]
        public decimal Peso { get; set; }

    }

}

