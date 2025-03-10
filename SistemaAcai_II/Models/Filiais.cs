using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class Filiais
    {
        /* PK */
        [Display(Name = "Código", Description = "Código.")]
        public int Id { get; set; }
        [Display(Name = "Razão Social", Description = "Razão Social.")]
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia", Description = "Nome Fantasia")]
        public string? NomeFantasia { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = " O Email não é valido")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email válido...")]
        public string Email { get; set; }

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        public string CNPJ { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "O Celular é obrigatório")]
        public string Telefone { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        // Endereço cliente 
        [Display(Name = "CEP", Description = "CEP.")]
        [MaxLength(10, ErrorMessage = "A senha deve ter entre 6 e 10 caracteres")]
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string CEP { get; set; }

        [Display(Name = "Estado", Description = "Estado.")]
        [Required(ErrorMessage = "O Estado é obrigatório.")]
        public string Estado { get; set; }

        [Display(Name = "Cidade", Description = "Cidade.")]
        [Required(ErrorMessage = "A Cidade é obrigatório.")]
        public string Cidade { get; set; }

        [Display(Name = "Bairro", Description = "Bairro.")]
        public string? Bairro { get; set; }

        [Display(Name = "Endereco", Description = "Endereco.")]
        [Required(ErrorMessage = "O Endereco é obrigatório.")]
        public string Logradouro { get; set; }

        [Display(Name = "Complemento", Description = "Complemento.")]
        public string? Complemento { get; set; }

        [Display(Name = "Número", Description = "Complemento.")]
        public string? Numero { get; set; }
    }
}
