using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class ItemComanda
    {
        [Display(Name = "Código do Item")]
        [JsonProperty("ProdutoId")]
        public int Id { get; set; }

        [Display(Name = "Comanda")]
        public Comanda RefComanda { get; set; }

        [Display(Name = "Produto")]
        public ProdutoSimples RefProduto { get; set; }

        [Display(Name = "Peso (Kg)")]
        [Required(ErrorMessage = "O peso é obrigatório.")]
        public decimal? Peso { get; set; }

        [Display(Name = "Quantidade")]
        public int? Quantidade { get; set; }       

        [Display(Name = "Subtotal")]
        public decimal? Subtotal { get; set; }
       
    }
}
