//$(document).ready(function () {
//    $('.cep').mask('00000-000');
//    $('.Telefone').mask('(00) 0000-0000');
//    $('.dineiro').mask("#.##0,00", { reverse: true });
//    $('.cpf').mask('000.000.000-00', { reverse: true });
//    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
//    $('.ie').mask('000.000.000/0000', { reverse: true });
//   /* $('.peso').mask("#0.000", { reverse: true });*/
//    $('.peso').keyup(function () {
//        var v = this.value,
//            integer = v.split('.')[0];

//        v = v.replace(/\D/g, "");

//        v = v.replace(/^[0]+/, "");

//        if (v.length <= 3 || !integer) {

//            if (v.length === 1) v = '0.00' + v;

//            if (v.length === 2) v = '0.0' + v;

//            if (v.length === 3) v = '0.' + v;
//        } else { v = v.replace(/^(\d{1,})(\d{3})$/, "$1.$2"); }
//        this.value = v;
//    });
//});

//$(document).ready(function () {
//    $('[data-toggle="popover"]').popover();
//});
//document.addEventListener("DOMContentLoaded", function () {
//    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
//    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
//        return new bootstrap.Popover(popoverTriggerEl);
//    });
//});
//$(document).ready(function () {

//    function limpa_formulário_cep() {
//        // Limpa valores do formulário de cep.
//        $("#Estado").val("");
//        $("#Cidade").val("");
//        $("#Endereco").val("");
//        $("#Bairro").val("");
//        $("#Complemento").val("");
//    }

//    //Quando o campo cep perde o foco.
//    $("#CEP").blur(function () {

//        //Nova variável "cep" somente com dígitos.
//        var cep = $(this).val().replace(/\D/g, '');

//        //Verifica se campo cep possui valor informado.
//        if (cep != "") {

//            //Expressão regular para validar o CEP.
//            var validacep = /^[0-9]{8}$/;

//            //Valida o formato do CEP.
//            if (validacep.test(cep)) {

//                //Preenche os campos com "..." enquanto consulta webservice.
//                $("#Estado").val("...");
//                $("#Cidade").val("...");
//                $("#Logradouro").val("...");
//                $("#Bairro").val("...");
//                $("#Complemento").val("...");

//                //Consulta o webservice viacep.com.br/
//                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

//                    if (!("erro" in dados)) {
//                        //Atualiza os campos com os valores da consulta.
//                        $("#Estado").val(dados.uf);
//                        $("#Cidade").val(dados.localidade);
//                        $("#Logradouro").val(dados.logradouro);
//                        $("#Bairro").val(dados.bairro);
//                        $("#Complemento").val(dados.complemento);
//                    } //end if.
//                    else {
//                        //CEP pesquisado não foi encontrado.
//                        limpa_formulário_cep();
//                        alert("CEP não encontrado.");
//                    }
//                });
//            } //end if.
//            else {
//                //cep é inválido.
//                limpa_formulário_cep();
//                alert("Formato de CEP inválido.");
//            }
//        } //end if.
//        else {
//            //cep sem valor, limpa formulário.
//            limpa_formulário_cep();
//        }
//    });
//});

//document.addEventListener("DOMContentLoaded", function () {
//    const checkboxPeso = document.getElementById("checkboxPeso");
//    const checkboxUnidade = document.getElementById("checkboxUnidade");
//    const camposPeso = document.querySelectorAll(".campo-peso");
//    const camposQuantidade = document.querySelectorAll(".campo-quantidade");

//    function atualizarVisibilidade() {
//        if (checkboxPeso.checked) {
//            camposPeso.forEach(campo => campo.style.display = "inline");
//            camposQuantidade.forEach(campo => campo.style.display = "none");
//        } else if (checkboxUnidade.checked) {
//            camposPeso.forEach(campo => campo.style.display = "none");
//            camposQuantidade.forEach(campo => campo.style.display = "inline");
//        }
//    }

//    checkboxPeso.addEventListener("change", atualizarVisibilidade);
//    checkboxUnidade.addEventListener("change", atualizarVisibilidade);

//    // Chamada inicial para configurar a visibilidade correta
//    atualizarVisibilidade();
//});

// Este bloco jQuery é executado quando o documento HTML está pronto
$(document).ready(function () {
    // Máscaras de input
    $('.cep').mask('00000-000');
    $('.Telefone').mask('(00) 0000-0000');
    $('.dineiro').mask("#.##0,00", { reverse: true });
    $('.cpf').mask('000.000.000-00', { reverse: true });
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.ie').mask('000.000.000/0000', { reverse: true });

    // Lógica para o campo de peso (se necessário)
    $('.peso').keyup(function () {
        var v = this.value,
            integer = v.split('.')[0];
        v = v.replace(/\D/g, "");
        v = v.replace(/^[0]+/, "");
        if (v.length <= 3 || !integer) {
            if (v.length === 1) v = '0.00' + v;
            if (v.length === 2) v = '0.0' + v;
            if (v.length === 3) v = '0.' + v;
        } else {
            v = v.replace(/^(\d{1,})(\d{3})$/, "$1.$2");
        }
        this.value = v;
    });

    // Lógica para Popover do Bootstrap (jQuery)
    $('[data-toggle="popover"]').popover();

    // Lógica para consulta de CEP via ViaCEP
    function limpa_formulário_cep() {
        $("#Estado").val("");
        $("#Cidade").val("");
        $("#Endereco").val("");
        $("#Bairro").val("");
        $("#Complemento").val("");
    }
    $("#CEP").blur(function () {
        var cep = $(this).val().replace(/\D/g, '');
        if (cep != "") {
            var validacep = /^[0-9]{8}$/;
            if (validacep.test(cep)) {
                $("#Estado").val("...");
                $("#Cidade").val("...");
                $("#Logradouro").val("...");
                $("#Bairro").val("...");
                $("#Complemento").val("...");
                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {
                    if (!("erro" in dados)) {
                        $("#Estado").val(dados.uf);
                        $("#Cidade").val(dados.localidade);
                        $("#Logradouro").val(dados.logradouro);
                        $("#Bairro").val(dados.bairro);
                        $("#Complemento").val(dados.complemento);
                    } else {
                        limpa_formulário_cep();
                        alert("CEP não encontrado.");
                    }
                });
            } else {
                limpa_formulário_cep();
                alert("Formato de CEP inválido.");
            }
        } else {
            limpa_formulário_cep();
        }
    });
});

// Este bloco de JavaScript puro é executado quando o DOM está completamente carregado
document.addEventListener("DOMContentLoaded", function () {
    // Lógica para Popover do Bootstrap (JavaScript)
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Lógica para checkboxes de peso/unidade
    const checkboxPeso = document.getElementById("checkboxPeso");
    const checkboxUnidade = document.getElementById("checkboxUnidade");
    const camposPeso = document.querySelectorAll(".campo-peso");
    const camposQuantidade = document.querySelectorAll(".campo-quantidade");

    function atualizarVisibilidade() {
        if (checkboxPeso.checked) {
            camposPeso.forEach(campo => campo.style.display = "inline");
            camposQuantidade.forEach(campo => campo.style.display = "none");
        } else if (checkboxUnidade.checked) {
            camposPeso.forEach(campo => campo.style.display = "none");
            camposQuantidade.forEach(campo => campo.style.display = "inline");
        }
    }

    if (checkboxPeso && checkboxUnidade) {
        checkboxPeso.addEventListener("change", atualizarVisibilidade);
        checkboxUnidade.addEventListener("change", atualizarVisibilidade);
        atualizarVisibilidade();
    }
});