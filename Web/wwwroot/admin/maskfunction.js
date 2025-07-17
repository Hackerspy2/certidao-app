var carregarMascaras = function () {
    $(document).ready(function () {
        moment.locale('pt-br');
        $(".mask_referencia").inputmask("99-9999");
        $(".mask_ano").inputmask("9999");
        $(".mask_data").inputmask("99/99/9999");
        $(".mask_data_sem").inputmask("99/99/9999");
        $(".mask_data_hora").inputmask("99/99/9999 99:99");
        $(".mask_mes_ano").inputmask("99/9999");
        $(".mask_cpf").inputmask('999.999.999-99');
        $(".mask_cnpj").inputmask("99.999.999/9999-99");
        $(".mask_ie").inputmask("99.999.999/999-99");
        $(".mask_cpfCnpj").inputmask({ mask: ['999.999.999-99', '99.999.999/9999-99'], keepStatic: true });;
        $(".mask_telefone").inputmask("(99)9999-9999");
        $(".mask_celular").inputmask("(99)99999-9999");
        $(".mask_cep").inputmask("99999-999");
        $(".mask_cartao").inputmask("9999.9999.9999.9999");
        $(".mask_hora").inputmask("99:99");
        $(".mask_hora_seg").inputmask("99:99:99");
        $('.mask_decimal').inputmask('decimal', {
            alias: "numeric",
            radixPoint: ",",
            groupSeparator: "",
            digits: 2,
            rightAlign: true,
            autoGroup: false,
            onBeforeMask: function (value, opts) {
                return value;
            }
        });
        $('.mask_latitude').inputmask('decimal', {
            radixPoint: ",",
            groupSeparator: ".",
            digits: 10,
            rightAlign: true,
            onBeforeMask: function (value, opts) {
                return value;
            }
        });
        $('.mask_data').bootstrapMaterialDatePicker({
            format: 'DD/MM/YYYY',
            time: false,
            lang: 'pt-br',
            cancelText: 'Cancelar',
            clearButton: true,
            clearText: "Limpar"
        });
    });
};
carregarMascaras();