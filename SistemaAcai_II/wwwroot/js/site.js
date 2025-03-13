$(document).ready(function () {
    $('.cep').mask('00000-000');
    $('.Telefone').mask('(00) 0000-0000');
    $('.dineiro').mask("#.##0,00", { reverse: true });
    $('.cpf').mask('000.000.000-00', { reverse: true });
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.ie').mask('000.000.000/0000', { reverse: true });
});

$(document).ready(function () {
    $('[data-toggle="popover"]').popover();
});
document.addEventListener("DOMContentLoaded", function () {
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});
$(document).ready(function () {

    function limpa_formulário_cep() {
        // Limpa valores do formulário de cep.
        $("#Estado").val("");
        $("#Cidade").val("");
        $("#Endereco").val("");
        $("#Bairro").val("");
        $("#Complemento").val("");
    }

    //Quando o campo cep perde o foco.
    $("#CEP").blur(function () {

        //Nova variável "cep" somente com dígitos.
        var cep = $(this).val().replace(/\D/g, '');

        //Verifica se campo cep possui valor informado.
        if (cep != "") {

            //Expressão regular para validar o CEP.
            var validacep = /^[0-9]{8}$/;

            //Valida o formato do CEP.
            if (validacep.test(cep)) {

                //Preenche os campos com "..." enquanto consulta webservice.
                $("#Estado").val("...");
                $("#Cidade").val("...");
                $("#Logradouro").val("...");
                $("#Bairro").val("...");
                $("#Complemento").val("...");

                //Consulta o webservice viacep.com.br/
                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

                    if (!("erro" in dados)) {
                        //Atualiza os campos com os valores da consulta.
                        $("#Estado").val(dados.uf);
                        $("#Cidade").val(dados.localidade);
                        $("#Logradouro").val(dados.logradouro);
                        $("#Bairro").val(dados.bairro);
                        $("#Complemento").val(dados.complemento);
                    } //end if.
                    else {
                        //CEP pesquisado não foi encontrado.
                        limpa_formulário_cep();
                        alert("CEP não encontrado.");
                    }
                });
            } //end if.
            else {
                //cep é inválido.
                limpa_formulário_cep();
                alert("Formato de CEP inválido.");
            }
        } //end if.
        else {
            //cep sem valor, limpa formulário.
            limpa_formulário_cep();
        }
    });
});
