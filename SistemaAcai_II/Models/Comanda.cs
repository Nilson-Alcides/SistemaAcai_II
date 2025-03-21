using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class Comanda
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        
       
        [Display(Name = "Colaborador")]
        [Required(ErrorMessage = "O código do colaborador é obrigatório.")]
        public Colaborador RefColaborador { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "O nome do é obrigatório.")]
        public string NomeCliente { get; set; }
       
        [Display(Name = "Abertura")]
        [Required(ErrorMessage = "A data de abrtura do é obrigatório.")]
        public DateTime? DataAbertura { get; set; } = DateTime.Now;

        [Display(Name = "Fechamento")]
        public DateTime? DataFechamento { get; set; }

        [Display(Name = "Total Comanda")]
        public decimal? ValorTotal { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        public List<ItemComanda> Itens { get; set; } = new List<ItemComanda>();

    }
}
