using System.ComponentModel.DataAnnotations;

namespace SistemaAcai_II.Models
{
    public class Cliente
    {
        /* PK */
        [Display(Name = "Código", Description = "Código.")]
        public int Id { get; set; }

        [Display(Name = "Nome completo", Description = "Nome e Sobrenome.")]
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string Nome { get; set; }

        [Display(Name = "Nascimento")]
        [Required(ErrorMessage = "A data é obrigatorio")]
        public DateTime Nascimento { get; set; }

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "Selecione uma opção")]
        [StringLength(1, ErrorMessage = "Deve conter 1 caracter")]
        public string Sexo { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O CPF é obrigatorio")]     
        public string CPF { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "O Celular é obrigatorio")]
        public string Telefone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = " O Email não é valido")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email válido...")]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "A senha é obrigatorio")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 10 caracteres")]
        public string Senha { get; set; }
        
        [Display(Name = "Situação")]       
        public string? Situacao { get; set; }

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
