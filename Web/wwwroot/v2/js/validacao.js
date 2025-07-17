function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0,
            v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
$(function () {    

    $(".mask_cpf").keyup(function (e) {
        let numero = $(this).val().replace(/\D/g, '');
        if (numero.length == 11) {
            if (!validaCpf(numero)) {
                $(this).val("");
                Notifica("Atenção", "CPF inválido", "warning");
                return;
            }
        }
    });
    $("#DataNascimento").blur(function (e) {
        if ($(this).val() != "") {
            if ($(this).val().replace(/\D/g, '').length < 8) {
                $(this).val("");
                Notifica("Atenção", "Data de nascimento inválido", "warning");
                return;
            }
        }
    });
    $("#Celular").blur(function (e) {
        if ($(this).val() != "") {
            if ($(this).val().replace(/\D/g, '').length < 11) {
                $(this).val("");
                Notifica("Atenção", "Telefone inválido", "warning");
                return;
            }
        }
    });
    $("#Email").blur(function (e) {
        if ($(this).val() != "") {
            if (!validarEmail($(this).val())) {
                $(this).val("");
                Notifica("Atenção", "E-mail inválido", "warning");
                return;
            }
        }
    });
    $("#Emissor").change(function() {
        if ($(this).val() == "OUTROS") {
            $("#divEmissorOutro").show();
        } else {
            $("#divEmissorOutro").hide();
        }
    });
});