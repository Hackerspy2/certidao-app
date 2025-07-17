var validarEmail = function (email) {
    var re = /\S+@\S+\.\S+/;
    return re.test(email);
}
var validaCnpj = function (cnpj) {
    var b = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    var c = String(cnpj).replace(/[^\d]/g, '');

    if (c.length !== 14)
        return false;

    if (/0{14}/.test(c))
        return false;

    for (var i = 0, n = 0; i < 12; n += c[i] * b[++i]);
    if (c[12] != (((n %= 11) < 2) ? 0 : 11 - n))
        return false;

    for (var i = 0, n = 0; i <= 12; n += c[i] * b[i++]);
    if (c[13] != (((n %= 11) < 2) ? 0 : 11 - n))
        return false;

    return true;
};

var validaCpf = function (cpf) {
    let soma = 0;
    let resto;
    const strCpf = String(cpf).replace(/[^\d]/g, "");
    if (strCpf.length !== 11)
        return false;

    if (["00000000000", "11111111111", "22222222222", "33333333333", "44444444444", "55555555555", "66666666666", "77777777777", "88888888888", "99999999999"].indexOf(strCpf) !== -1)
        return false;

    for (var i = 1; i <= 9; i++)
        soma = soma + parseInt(strCpf.substring(i - 1, i)) * (11 - i);

    resto = (soma * 10) % 11;

    if ((resto == 10) || (resto == 11))
        resto = 0;

    if (resto != parseInt(strCpf.substring(9, 10)))
        return false;

    soma = 0;

    for (i = 1; i <= 10; i++)
        soma = soma + parseInt(strCpf.substring(i - 1, i)) * (12 - i);

    resto = (soma * 10) % 11;

    if ((resto == 10) || (resto == 11))
        resto = 0;

    if (resto != parseInt(strCpf.substring(10, 11)))
        return false;

    return true;
};
var showLogin = function () {
    if ($("#recoverform").is(":visible")) {
        $("#recoverform").slideUp();
    }
    if ($("#cadastroform").is(":visible")) {
        $("#cadastroform").slideUp();
    }
    $("#loginform").fadeIn();
};

var functionCarregaValidacoes = function () {
    $("label:contains('*')").parent().find("input,select,radio,textarea,checkbox").each(function () {
        AddReq($(this), true);
    });
    $(".preloader").fadeOut();
    $(".floating-labels .form-control").on("focus blur",
        function (e) {
            $(this).parents(".form-group").toggleClass("focused", "focus" === e.type || this.value.length > 0);
        });
    $('[data-toggle="tooltip"]').tooltip();
    setaValidacoes();
};

var formatarValor = function (valor) {
    if (valor == null || valor == "") return "R$ 0,0";
    return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
};

var showHidenCampos = function (campos, show) {
    $.each(campos,
        function (indexInArray, valueOfElement) {
            if (show) {
                $(`#${valueOfElement}`).show();
            } else {
                $(`#${valueOfElement}`).hide();
            }
        });
};

function validarEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function validaCpf(cpf) {
    cpf = cpf.replace(/[^\d]+/g, '');
    if (cpf == '') return false;
    if (cpf.length != 11 ||
        cpf == "00000000000" ||
        cpf == "11111111111" ||
        cpf == "22222222222" ||
        cpf == "33333333333" ||
        cpf == "44444444444" ||
        cpf == "55555555555" ||
        cpf == "66666666666" ||
        cpf == "77777777777" ||
        cpf == "88888888888" ||
        cpf == "99999999999")
        return false;
    var add = 0;
    for (var i = 0; i < 9; i++)
        add += parseInt(cpf.charAt(i)) * (10 - i);
    var rev = 11 - (add % 11);
    if (rev == 10 || rev == 11)
        rev = 0;
    if (rev != parseInt(cpf.charAt(9)))
        return false;
    add = 0;
    for (var i = 0; i < 10; i++)
        add += parseInt(cpf.charAt(i)) * (11 - i);
    rev = 11 - (add % 11);
    if (rev == 10 || rev == 11)
        rev = 0;
    if (rev != parseInt(cpf.charAt(10)))
        return false;
    return true;
}

var setValorInput = function (input, valor) {
    if (valor !== null) {
        $(input).attr("aria-invalid", "false").val(valor).parent().addClass("has-success focused");
    }
};
var carregarDadosComoAddUrlSetValSelectItem = function (campo, url, urlAdd, title, campoSetVal) {
    const dados = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        sufficient: 1,
        indexRemote: true,
        remote: { wildcard: "%QUERY", url: url + "?q=%QUERY" }
    });
    dados.initialize();
    $(campo).typeahead({ hint: true, highlight: true, minLength: 1 },
        {
            display: "nome",
            source: dados,
            templates: {
                empty: [
                    `<div class='empty-message'><a href='${urlAdd}' class="detalhesPaiPai hand" data-original-title='${title
                    }' title='${title}'>Nenhuma informação encontrado, clique aqui e faça o cadastro</a></div>`
                ].join("\n"),
                suggestion: function (data) { return `<p>${data.nome}</p>`; }
            }
        }).bind("typeahead:select",
            function (ev, item) {
                $(campoSetVal).val(item.id);
            });
};

var carregarDadosObjectCallFuncionClear = function (campo, url, callFunction) {
    $.typeahead({
        filter: false,
        maxItem: 0,
        freeInput: false,
        input: campo,
        template: function (query, item) {
            return "<span>{{nome}}</span>";
        },
        source: {
            Opcoes: {
                display: "nome",
                accent: true,
                ajax: function () {
                    return {
                        type: "GET",
                        url: url,
                        data: { termo: "{{query}}", unidade: $("#Unidade").val() },
                        path: "data",
                        callback: {
                            done: function (data) {
                                return data;
                            }
                        }
                    };
                }
            }
        },
        minLength: 1,
        emptyTemplate: "Nenhuma informação encontrada para <strong>{{query}}</strong>",
        order: "asc",
        dynamic: true,
        callback: {
            onClick: function (node, a, item, event) {
                callFunction(item.Id, item.Nome, item.Valor, item.Estoque);
            },
            onClickAfter: function (node, a, item, event) {
                node.val("");
            }
        }
    });
};

var carregarDadosSetValIdCallFuncion = function (campo, url, campoSetVal, callFunction) {
    $.typeahead({
        filter: false,
        maxItem: 0,
        freeInput: false,
        input: campo,
        template: function (query, item) {
            return "<span>{{nome}}</span>";
        },
        source: {
            Opcoes: {
                display: "nome",
                accent: true,
                ajax: function () {
                    return {
                        type: "GET",
                        url: url,
                        data: { q: "{{query}}", unidade: $("#Unidade").val() },
                        path: "data",
                        callback: {
                            done: function (data) {
                                return data;
                            }
                        }
                    };
                }
            }
        },
        minLength: 1,
        emptyTemplate: "Nenhuma informação encontrada para <strong>{{query}}</strong>",
        order: "asc",
        dynamic: true,
        callback: {
            onClick: function (node, a, item, event) {
                $(campoSetVal).val(item.Id);
                callFunction();
            }
        }
    });
};

var carregarDadosCallFuncion = function (campo, url, callFunction) {
    const dados = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        sufficient: 1,
        indexRemote: true,
        remote: { wildcard: "%QUERY", url: url + "?q=%QUERY" }
    });
    dados.initialize();
    $(campo).typeahead({ hint: true, highlight: true, minLength: 1 },
        {
            display: "nome",
            source: dados,
            templates: {
                empty: [
                    `<div class='empty-message'><a href='${urlAdd}' class="detalhesPaiPai hand" data-orginal-title='${title
                    }' title='${title}'>Nenhuma informação encontrado, clique aqui e faça o cadastro</a></div>`
                ].join("\n"),
                suggestion: function (data) { return `<p>${data.nome}</p>`; }
            }
        }).bind("typeahead:select",
            function (node, item) {
                node.val(item.Nome);
                callFunction();
            });
};

var carregarDados = function (campo, url) {
    $.typeahead({
        filter: false,
        maxItem: 0,
        freeInput: false,
        input: campo,
        template: function (query, item) {
            return "<span>{{nome}}</span>";
        },
        source: {
            Opcoes: {
                display: "nome",
                accent: true,
                ajax: function () {
                    return {
                        type: "GET",
                        url: url,
                        data: { q: "{{query}}", tipo: $(campo).attr("name") },
                        path: "data",
                        callback: {
                            done: function (data) {
                                return data;
                            }
                        }
                    };
                }
            }
        },
        minLength: 1,
        emptyTemplate: "Nenhuma informação encontrada para <strong>{{query}}</strong>",
        order: "asc",
        dynamic: true
    });
};

var carregarDadosComoAddUrlSetVal = function (campo, url, urlAdd, title, campoSetVal) {
    const dados = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        sufficient: 1,
        indexRemote: true,
        remote: { wildcard: "%QUERY", url: url + "?q=%QUERY" }
    });
    dados.initialize();
    $(campo).typeahead({ hint: true, highlight: true, minLength: 1 },
        {
            display: "nome",
            source: dados,
            templates: {
                empty: [
                    `<div class='empty-message'><a href='${urlAdd}' class="detalhesPaiPai hand" data-orginal-title='${title
                    }' title='${title}'>Nenhuma informação encontrado, clique aqui e faça o cadastro</a></div>`
                ].join("\n"),
                suggestion: function (data) { return `<p>${data.nome}</p>`; }
            }
        }).bind("typeahead:select",
            function (node, item) {
                node.val(item.Nome);
                callFunction();
            });
};

var carregarDadosCombo = function (campo, url) {
    $.typeahead({
        filter: false,
        maxItem: 0,
        freeInput: false,
        input: campo,
        template: function (query, item) {
            return "<span>{{nome}}</span>";
        },
        source: {
            Opcoes: {
                display: "nome",
                ajax: function () {
                    return {
                        type: "GET",
                        url: url,
                        data: { q: "{{query}}", tipo: campo.replace("#", "") },
                        path: "data",
                        callback: {
                            done: function (data) {
                                return data;
                            }
                        }
                    };
                }
            }
        },
        minLength: 1,
        emptyTemplate: "Nenhuma informação encontrado para <strong>{{query}}</strong>",
        order: "asc",
        dynamic: true,
        callback: {
            onClick: function (node, a, item, event) {
                $(campo).val(item.id);
            }
        }
    });
};

var carregarDadosComboSetCampo = function (campo, url, setCampo) {
    $.typeahead({
        filter: false,
        maxItem: 0,
        freeInput: false,
        input: campo,
        template: function (query, item) {
            return "<span>{{nome}}</span>";
        },
        source: {
            Opcoes: {
                display: "nome",
                ajax: function () {
                    return {
                        type: "GET",
                        url: url,
                        data: { q: "{{query}}", tipo: campo.replace("#", "") },
                        path: "data",
                        callback: {
                            done: function (data) {
                                return data;
                            }
                        }
                    };
                }
            }
        },
        minLength: 1,
        emptyTemplate: "Nenhuma informação encontrado para <strong>{{query}}</strong>",
        order: "asc",
        dynamic: true,
        callback: {
            onClick: function (node, a, item, event) {
                $(setCampo).val(item.Id);
            },
            onCancel: function (node, a, item, event) {
                $(setCampo).val("");
            }
        }
    });
};

var carregarDadosDrop = function (campo, url, campoSetVal) {
    $(".preloader").show();

    $.fn.addItems = function (data, id) {
        return this.each(function () {
            var list = this;
            $.each(data,
                function (index, itemData) {
                    const option = new Option(itemData.text,
                        itemData.value,
                        itemData.value.toString() == id ? true : false,
                        itemData.value.toString() == id ? true : false);
                    list.add(option);
                });
        });
    };

    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (ret) {
            $(".preloader").hide();
            $(campo).empty();
            $(campo).addItems(ret.data, campoSetVal);
            if (campoSetVal != "") {
                $(campo).parent().addClass("focused");
            }
        }
    });
};
var carregarDadosDropItemTodosArray = function (campo, opcoes, campoSetVal, rotulo) {
    $(campo).empty();
    $(campo).add(new Option(rotulo, ""));
    opcoes.forEach(dd => {
        var option = new Option(dd, dd, (dd == campoSetVal ? true : false));
        $(campo).add(option);
    });
};
var carregarDadosDropItemTodos = function (campo, url, campoSetVal, rotulo) {
    $(".preloader").show();
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (ret) {
            $(".preloader").hide();
            $(campo).empty();
            $(campo).addItemsRotulo(ret.data, campoSetVal, rotulo);
            if (campoSetVal != "") {
                $(campo).parent().addClass("focused");
            }
        }
    });
};
var carregarDadosDropOnVazio = function (campo, url, campoSetVal) {
    $(".preloader").show();
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (ret) {
            $(".preloader").hide();
            $(campo).empty();
            $(campo).addItemsVazio(ret.data, campoSetVal);
        }
    });
};

var uploadImagem = function (controller, id) {
    $(document).delegate("#Upload",
        "change",
        function () {
            const campo = $(this);
            const formData = new FormData();
            const totalFiles = document.getElementById("Upload").files.length;
            for (let i = 0; i < totalFiles; i++) {
                const file = document.getElementById("Upload").files[i];
                formData.append("Upload", file);
            }
            $(".preloader").show();
            $.ajax({
                type: "POST",
                url: `/${controller}/EnviarFile/?id=${id}`,
                data: formData,
                dataType: "json",
                contentType: false,
                processData: false,
                success: function (ret) {
                    Sucesso();
                },
                error: function (error) {
                    window.parent.NotificaClose("Erro", error, "error");
                }
            });
        });
};

var removeImagem = function (controller, id) {
    $("#removerFoto").click(function (e) {
        e.preventDefault();
        const link = $(this);
        const modal = window.parent.$("#modalPaiConfirm").one("click",
            "#btnPaiConfirme",
            function () {
                $.ajax({
                    url: `/${controller}/RemoveFoto/`,
                    type: "get",
                    data: { id: id },
                    contentType: "application/json",
                    success: function (result) {
                        Sucesso();
                    }
                });
            });
        modal.find(".modal-footer").find("#btnPaiConfirme").html("Sim");
        modal.find(".modal-footer").find("#btnPaiCancelar").html("Não");
        modal.find(".modal-body").html("Tem certeza que deseja remover esta imagem?");
        modal.on("show", function () { }).modal({ show: true, keyboard: false });
    });
};

var uploadArquivo = function (controller, id) {
    $(document).delegate("#UploadFile",
        "change",
        function () {
            const campo = $(this);
            const formData = new FormData();
            const totalFiles = document.getElementById("UploadFile").files.length;
            for (let i = 0; i < totalFiles; i++) {
                const file = document.getElementById("UploadFile").files[i];
                formData.append("UploadFile", file);
            }
            $(".preloader").show();
            $.ajax({
                type: "POST",
                url: `/${controller}/EnviarArquivo/?id=${id}`,
                data: formData,
                dataType: "json",
                contentType: false,
                processData: false,
                success: function (ret) {
                    Sucesso();
                },
                error: function (error) {
                    window.parent.NotificaClose("Erro", error, "error");
                }
            });
        });
};

var removeArquivo = function (controller, id) {
    $("#removerArquivo").click(function (e) {
        e.preventDefault();
        const link = $(this);
        const modal = window.parent.$("#modalPaiConfirm").one("click",
            "#btnPaiConfirme",
            function () {
                $.ajax({
                    url: `/${controller}/RemoveArquivo/`,
                    type: "get",
                    data: { id: id },
                    contentType: "application/json",
                    success: function (result) {
                        Sucesso();
                    }
                });
            });
        modal.find(".modal-footer").find("#btnPaiConfirme").html("Sim");
        modal.find(".modal-footer").find("#btnPaiCancelar").html("Não");
        modal.find(".modal-body").html("Tem certeza que deseja remover este arquivo?");
        modal.on("show", function () { }).modal({ show: true, keyboard: false });
    });
};

function Sucesso() {
    window.location.reload();
}

function closeModalCall() {
    $("#modalAdm").modal("toggle");
    window.location.reload();
}

function CloseModalPai() {
    $(".bootbox:last").modal("hide");
}

function showLoad() {
    $(".preloader").show();
}

function hideLoad() {
    $(".preloader").hide();
}

function Nda() {
    return;
}

function getTitle(valor) {
    switch (valor) {
        case "success":
            return "Sucesso!";
        case "info":
            return "Atenção!";
        case "error":
            return "Erro!";
        default:
            return "Atenção!";
    }
}

var carregarDadosRedirect = function (campo, url) {
    $.typeahead({
        input: campo,
        template: function (query, item) {
            return "<span>{{nome}}</span>";
        },
        source: {
            Opcoes: {
                display: "nome",
                ajax: function () {
                    return {
                        type: "GET",
                        url: url,
                        data: { q: "{{query}}", tipo: campo.replace("#", "") },
                        path: "data",
                        callback: {
                            done: function (data) {
                                return data;
                            }
                        }
                    };
                }
            }
        },
        minLength: 1,
        //emptyTemplate: 'Nenhuma informação encontrado para',
        order: "asc",
        dynamic: true,
        callback: {
            onClick: function (node, a, item, event) {
                window.location.href = `/Orcamento/Add/${item.Id}`;
                //$(campoSetVal).val(item.Id);
            }
        }
    });
};
var AddReq = function (controle, add) {
    $(function () {
        if (add === true) {
            $(controle).prop("required", true);
            $(controle).attr("data-validation-required-message", "obrigatório");
            if ($(controle).attr("type") === "email") {
                $(controle).attr("data-validation-email-message", "E-mail inválido");
            }
        } else {
            $(controle).prop("required", false);
            $(controle).removeAttr("data-validation-required-message", "");
        }
    });
};
var ReqsList = function (controles, add) {
    $(function () {
        controles.forEach(controle => {
            if (add === true) {
                $(controle).prop("required", true);
                $(controle).attr("data-validation-required-message", "obrigatório");
                if ($(controle).attr("type") === "email") {
                    $(controle).attr("data-validation-email-message", "E-mail inválido");
                }
            } else {
                $(controle).prop("required", false);
                $(controle).removeAttr("data-validation-required-message");
            }
        });
    });
};
var setaValidacoes = function () {
    $(function () {
        $(".formAsync").jqBootstrapValidation("destroy");
        $("input,select,radio,textarea,checkbox").not("[type=submit]").jqBootstrapValidation();
    });
};
var ShowHideCampo = function (controle, addSelecionado, valorValidar) {
    $(function () {
        if (addSelecionado == valorValidar) {
            $(controle).show();
        } else {
            $(controle).hide();
        }
    });
};
var AddReqValor = function (controle, add, valor) {
    $(function () {
        if (add == valor) {
            $(controle).rules("add", { required: true, messages: { required: "Obrigatório" } });
        } else {
            $(controle).rules("remove");
        }
    });
};
var carregarEstados = function (inputEstado, inputCidade, valorEstado, valorCidade, callback= null) {
    $(function () {
        $.getJSON("/admin/estados_cidades.json",
            function (data) {
                const items = [];
                var options = '<option value=""></option>';
                $.each(data,
                    function (key, val) {
                        const select = valorEstado == val.sigla ? "selected=selected" : "";
                        options += `<option value="${val.sigla}" ${select}>${val.nome}</option>`;
                    });
                if (valorEstado != "") {
                    $(inputEstado).html(options).parent().addClass("focused");
                } else {
                    $(inputEstado).html(options);
                }

                $(inputEstado).change(function () {
                    var options_cidades = "";
                    var str = "";
                    $(inputEstado + " option:selected").each(function () {
                        str += $(this).text();
                    });
                    $.each(data,
                        function (key, val) {
                            if (val.nome == str) {
                                $.each(val.cidades,
                                    function (key_city, val_city) {
                                        const select = valorCidade == val_city ? "selected=selected" : "";
                                        options_cidades += `<option value="${val_city}" ${select}>${val_city}</option>`;
                                    });
                            }
                        });
                    $(inputCidade).html(options_cidades).parent().addClass("focused");
                });

                if ($(inputEstado).val() != "") {
                    if ($(inputCidade).val() == "") {
                        $(inputEstado).change();
                    }
                }
                // Chame a função de callback no final
                if (typeof callback === 'function') {
                    callback();
                }
            });
    });
};

var carregarEstadosRemove = function (inputEstado, inputCidade, valorEstado, valorCidade, remove) {
    $(function () {
        $.getJSON("/admin/estados_cidades.json",
            function (data) {
                const items = [];
                var options = '<option value=""></option>';
                $.each(data,
                    function (key, val) {
                        if (remove != val.sigla) {
                            const select = valorEstado == val.sigla ? "selected=selected" : "";
                            options += `<option value="${val.sigla}" ${select}>${val.nome}</option>`;
                        }
                    });
                if (valorEstado != "") {
                    $(inputEstado).html(options).parent().addClass("focused");
                } else {
                    $(inputEstado).html(options);
                }

                $(inputEstado).change(function () {
                    var options_cidades = "";
                    var str = "";
                    $(inputEstado + " option:selected").each(function () {
                        str += $(this).text();
                    });
                    $.each(data,
                        function (key, val) {
                            if (val.nome == str) {
                                $.each(val.cidades,
                                    function (key_city, val_city) {
                                        const select = valorCidade == val_city ? "selected=selected" : "";
                                        options_cidades += `<option value="${val_city}" ${select}>${val_city}</option>`;
                                    });
                            }
                        });
                    $(inputCidade).html(options_cidades).parent().addClass("focused");
                });

                if ($(inputEstado).val() != "") {
                    if ($(inputCidade).val() == "") {
                        $(inputEstado).change();
                    }
                }

            });
    });
};
var carregarEstadosOnly = function (inputEstado, valorEstado) {
    $(function () {
        $.getJSON("/js/estados_cidades.json",
            function (data) {
                const items = [];
                var options = '<option value=""></option>';
                $.each(data,
                    function (key, val) {
                        const select = valorEstado == val.sigla ? "selected=selected" : "";
                        options += `<option value="${val.sigla}" ${select}>${val.nome}</option>`;
                    });
                if (valorEstado != "") {
                    $(inputEstado).html(options).parent().addClass("focused");
                } else {
                    $(inputEstado).html(options);
                }

            });
    });
};
var getDataAtual = function () {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;
    const yyyy = today.getFullYear();
    if (dd < 10) {
        dd = `0${dd}`;
    }
    if (mm < 10) {
        mm = `0${mm}`;
    }
    today = dd + "/" + mm + "/" + yyyy;
    return today;
};
var carregarEndereco = function (cep, logradouro, bairro, estado, cidade) {
    if (cep.val().replace(/_/g, "").length == 9) {
        $(function () {
            window.parent.$(".preloader").show();
            $.ajax({
                type: "GET",
                url: "/Home/GetEndereco",
                data: { cep: cep.val() },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    window.parent.$(".preloader").hide();
                    if (msg != "") {
                        $(logradouro).val(msg[0]).parent().addClass("focused");
                        $(bairro).val(msg[1]).parent().addClass("focused");
                        carregarEstados(estado, cidade, msg[3], msg[2]);
                    }
                },
                error: function () { window.parent.$(".preloader").hide(); }
            });
        });
    }
};

function Notifica(titulo, mensagem, tipo, funcao) {
    Swal.fire({
        title: titulo,
        html: mensagem,
        type: tipo,
        cancelButtonText: "fechar"
    },
        function () {
            if (funcao != "") {
                funcao();
            }
        });
}

function NotificaNotFunction(titulo, mensagem, tipo) {
    Swal.fire({
        title: titulo,
        html: mensagem,
        type: tipo,
        cancelButtonText: "fechar"
    });
}

function NotificaClose(titulo, mensagem, tipo) {
    Swal.fire({
        title: titulo,
        html: mensagem,
        type: tipo,
        showConfirmButton: false,
        timer: 1500
    });
}

function NotificaConfirrme(titulo, mensagem, tipo) {
    Swal.fire({
        title: titulo,
        html: mensagem,
        type: tipo,
        showConfirmeButton: true
    });
}

function NotificaCall(titulo, mensagem, botaoSim, retorno) {
    Swal.fire({
        title: titulo,
        text: mensagem,
        type: "warning",
        showCancelButton: true,
        confirmButtonText: botaoSim,
        confirmButtonColor: "#DD6B55",
        closeOnConfirm: false,
        showLoaderOnConfirm: true,
        html: true
    },
        function () {
            setTimeout(function () {
                retorno();
            },
                3000);
        });
}

var WinModal = function (controle) {
    $(function () {
        $(document).delegate(controle,
            "click",
            function (e) {
                e.preventDefault();
                const c = $(this);
                const modal = $("#modalAdm");
                $("#modalAdm iframe").attr({
                    'src': c.attr("href"),
                    'allowfullscreen': ""
                    //'height': height,
                    //'width': width,
                });
                modal.modal({ show: true });
                modal.find(".modal-body").height($(window).height() - 50);
                modal.find(".modal-title").html(c.attr("data-original-title"));
                modal.on("show", function () { }).on("hidden.bs.modal",
                    function () {
                        $("#modalAdm iframe").attr({
                            'src': "/carregando.html",
                            'allowfullscreen': ""
                        });
                    });
            });
    });
};
var WinModalLink = function (url, titulo) {
    $(function () {
        const modal = $("#modalAdm");
        $("#modalAdm iframe").attr({
            'src': url,
            'allowfullscreen': ""
        });
        modal.modal({ show: true });
        modal.find(".modal-body").height($(window).height() - 50);
        modal.find(".modal-title").html(titulo);
        modal.on("show", function () { }).on("hidden.bs.modal",
            function () {
                $("#modalAdm iframe").attr({
                    'src': "/carregando.html",
                    'allowfullscreen': ""
                });
            });

    });
};
var WinModalParent = function (controle, funcao) {
    $(function () {
        $(document).delegate(controle,
            "click",
            function (e) {
                e.preventDefault();
                const c = $(this);
                const modal = window.parent.$("#modalPaiAdm");
                window.parent.$("#modalPaiAdm iframe").attr({
                    'src': c.attr("href"),
                    'allowfullscreen': ""
                    //'height': height,
                    //'width': width,
                });
                modal.modal({ show: true });
                modal.find(".modal-body").height($(window).height() - 50);
                modal.find(".modal-title").html(c.attr("data-original-title"));
                modal.on("show", function () { }).on("hidden.bs.modal",
                    function () {
                        funcao();
                        window.parent.$("#modalPaiAdm iframe").attr({
                            'src': "/carregando.html",
                            'allowfullscreen': ""
                        });
                    });
            });
    });
};
var WinModalParentParent = function (controle, funcao) {
    $(function () {
        $(document).delegate(controle,
            "click",
            function (e) {
                e.preventDefault();
                const c = $(this);
                const modal = window.parent.$("#modalPaiPaiAdm");
                window.parent.$("#modalPaiPaiAdm iframe").attr({
                    'src': c.attr("href"),
                    'allowfullscreen': ""
                    //'height': height,
                    //'width': width,
                });
                modal.modal({ show: true });
                modal.find(".modal-body").height($(window).height());
                modal.find(".modal-title").html(c.attr("data-original-title"));
                modal.on("show", function () { }).on("hidden.bs.modal",
                    function () {
                        funcao();
                        window.parent.$("#modalPaiPaiAdm iframe").attr({
                            'src': "/carregando.html",
                            'allowfullscreen': ""
                        });
                    });
            });
    });
};
var WinModalParentParentParent = function (controle, funcao) {
    $(function () {
        $(document).delegate(controle,
            "click",
            function (e) {
                e.preventDefault();
                const c = $(this);
                const modal = window.parent.parent.$("#modalPaiPaiPaiAdm");
                window.parent.parent.$("#modalPaiPaiPaiAdm iframe").attr({
                    'src': c.attr("href"),
                    'allowfullscreen': ""
                    //'height': height,
                    //'width': width,
                });
                modal.modal({ show: true });
                modal.find(".modal-body").height($(window).height());
                modal.find(".modal-title").html(c.attr("data-original-title"));
                modal.on("show", function () { }).on("hidden.bs.modal",
                    function () {
                        funcao();
                        window.parent.parent.$("#modalPaiPaiPaiAdm iframe").attr({
                            'src': "/carregando.html",
                            'allowfullscreen': ""
                        });
                    });
            });
    });
};
var WinModalEvento = function (controle) {
    $(function () {
        $(document).delegate(controle,
            "click",
            function (e) {
                e.preventDefault();
                const c = $(this);
                const modal = $("#modalAdm");
                $("#modalAdm iframe").attr({
                    'src': c.attr("href"),
                    'allowfullscreen': ""
                    //'height': height,
                    //'width': width,
                });
                modal.modal({ show: true });
                modal.find(".modal-body").height(400);
                modal.find(".modal-title").html("Visualizar");
                modal.on("show", function () { });
                modal.on("hidden.bs.modal",
                    function () {
                        $("#modalAdm iframe").attr({
                            'src': "/carregando.html",
                            'allowfullscreen': ""
                        });
                    });
            });
    });
};

WinModalEvento(".fc-event", Nda);
WinModal(".detalhes", Nda);
WinModal(".cadastrar", Sucesso);
WinModal(".alterar", Sucesso);

WinModalParent(".detalhesPai", Nda);
WinModalParent(".alterarPai", Sucesso);
WinModalParent(".cadastrarPai", Sucesso);

WinModalParentParent(".cadastrarPaiPai", Sucesso);
WinModalParentParent(".alterarPaiPai", Sucesso);
WinModalParentParent(".detalhesPaiPai", Nda);
WinModalParentParentParent(".detalhesPaiPaiPai", Nda);

var activeUrl = function (url) {
    $(`[href='${url}']`).parent().addClass("active");
    $(`[href='${url}']`).parent().parent().addClass("in");
    $(`[href='${url}']`).parent().parent().parent().addClass("active");
    $(`[href='${url}']`).parent().parent().parent().parent().parent().addClass("active");
};

var jDataTableAll = function (idTabela,
    url,
    controller,
    rowConfirm,
    classDetails,
    classEdit,
    classDel,
    janela = "",
    order = "",
    searching = true) {
    myTable = $(idTabela).dataTable({
        order: [[rowConfirm, order != "" ? order : "asc"]],
        processing: true,
        sAjaxSource: url,
        bServerSide: true,
        searching: searching,
        bDestroy: true,
        bPaginate: true,
        pageLength: 100,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        `<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right dropOptions' data-id='${id}'>`;
                    if (classDetails != "hide-item") {
                        html += `<a href='/${controller}/Details/?dados=${id
                            }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]
                            }' class='detalhes${janela} ${classDetails
                            } dropdown-item'> Visualizar</a>`;
                    }
                    if (classEdit != "hide-item") {
                        html += `<a href='/${controller}/Add/?id=${id
                            }&tipo=update' title='Alterar' data-original-title='Alterar - ${row[rowConfirm]
                            }' class='alterar${janela} ${classEdit
                            } dropdown-item'> Alterar</a>`;
                    }
                    if (classDel != "hide-item") {
                        html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                            } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                            }|Del'> Deletar</a>`;
                    }
                    html += "</div></div>";
                    if (classDetails == "hide-item" && classEdit == "hide-item" && classDel == "hide-item") return "";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input").unbind().bind("keyup",
                function (e) {
                    if (this.value.length == 0) {
                        myTable.fnFilter("");
                    } else {
                        if (this.value.length >= 3 && e.keyCode == 13) {
                            myTable.fnFilter(this.value);
                        }
                    }
                });
        }

    });
};
var jDataTable = function (idTabela,
    url,
    controller,
    rowConfirm,
    classDetails,
    classEdit,
    classDel,
    janela = "",
    order = "") {
    myTable = $(idTabela).dataTable({
        order: [[rowConfirm, order != "" ? order : "asc"]],
        processing: true,
        sAjaxSource: url,
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        `<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right dropOptions' data-id='${id}'>`;
                    if (classDetails != "hide-item") {
                        html += `<a href='/${controller}/Details/?dados=${id
                            }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]
                            }' class='detalhes${janela} ${classDetails
                            } dropdown-item'> Visualizar</a>`;
                    }
                    if (classEdit != "hide-item") {
                        html += `<a href='/${controller}/Edit/?id=${id
                            }' title='Alterar' data-original-title='Alterar - ${row[rowConfirm]}' class='alterar${janela
                            } ${classEdit
                            } dropdown-item'> Alterar</a>`;
                    }
                    if (classDel != "hide-item") {
                        html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                            } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                            }|Del'> Deletar</a>`;
                    }
                    html += "</div></div>";
                    if (classDetails == "hide-item" && classEdit == "hide-item" && classDel == "hide-item") return "";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            const cssClass = aData[6] == "Cancelado" ? "red" : "";
            $(nRow).addClass(cssClass);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });
        }

    });
};


var jDataTableLinkExtra = function (idTabela,
    url,
    controller,
    rowConfirm,
    classDetails,
    classEdit,
    classDel,
    janela = "",
    htmlExtra = "",
    order = "") {
    myTable = $(idTabela).dataTable({
        order: [[rowConfirm, order != "" ? order : "desc"]],
        sAjaxSource: url,
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [
            { extend: "excel", title: $(".page-titles").find("h3").html() },
            { extend: "pdf", title: $(".page-titles").find("h3").html() },
            { extend: "print", title: $(".page-titles").find("h3").html() },
        ],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        `<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right dropOptions' data-id='${id}'>`;
                    if (classDetails != "hide-item") {
                        html += `<a href='/${controller}/Details/?dados=${id
                            }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]
                            }' class='detalhes${janela} ${classDetails
                            } dropdown-item'> Visualizar</a>`;
                    }
                    if (htmlExtra != "") {
                        html += htmlExtra.replace("[id]", id);
                    }
                    if (classEdit != "hide-item") {
                        html += `<a href='/${controller}/Edit/?id=${id
                            }' title='Alterar' data-original-title='Alterar - ${row[rowConfirm]}' class='alterar${janela
                            } ${classEdit
                            } dropdown-item'> Alterar</a>`;
                    }
                    if (classDel != "hide-item") {
                        html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                            } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                            }|Del'> Deletar</a>`;
                    }
                    html += "</div></div>";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            const cssClass = aData[6] == "Cancelado" ? "red" : "";
            $(nRow).addClass(cssClass);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });
        }

    });
};

var jDataTableNoAlterouLinkExtra = function (idTabela,
    url,
    controller,
    rowConfirm,
    classDetails,
    classEdit,
    classDel,
    janela = "",
    htmlExtra = "",
    order = "") {
    myTable = $(idTabela).dataTable({
        order: [[rowConfirm, order != "" ? order : "desc"]],
        sAjaxSource: url,
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [
            { extend: "excel", title: $(".page-titles").find("h3").html() },
            { extend: "pdf", title: $(".page-titles").find("h3").html() },
            { extend: "print", title: $(".page-titles").find("h3").html() },
        ],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        `<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right dropOptions' data-id='${id}'>`;
                    if (classDetails != "hide-item") {
                        html += `<a href='/${controller}/Details/?dados=${id
                            }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]
                            }' class='detalhes${janela} ${classDetails
                            } dropdown-item'> Visualizar</a>`;
                    }
                    if (htmlExtra != "") {
                        html += htmlExtra.replace("[id]", id);
                    }
                    if (row[6] != "Finalizado") {
                        if (classDel != "hide-item") {
                            html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                                } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                                }|Del'> Deletar</a>`;
                        }
                    }
                    html += "</div></div>";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            const cssClass = aData[6] == "Cancelado" ? "red" : "";
            $(nRow).addClass(cssClass);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });
        }

    });
};

var jDataTableNoSearch =
    function (idTabela, url, controller, rowConfirm, classDetails, classEdit, classDel, janela = "") {
        myTable = $(idTabela).dataTable({
            sAjaxSource: url,
            bServerSide: true,
            searching: false,
            bDestroy: true,
            bPaginate: true,
            pageLength: 50,
            dom: "fBrtip",
            language: { url: "/admin/libs/datatables/pt-br.json" },
            buttons: [],
            aoColumnDefs: [
                { bSortable: false, aTargets: [-1] },
                { bSearchable: false, aTargets: [-1] },
                {
                    mRender: function (data, type, row) {
                        const id = data;
                        var html = "<div class='btn-group ml-auto m-t-10'>";
                        html +=
                            `<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right dropOptions' data-id='${id}'>`;
                        if (classDetails != "hide-item") {
                            html += `<a href='/${controller}/Details/?dados=${id
                                }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]
                                }' class='detalhes${janela} ${classDetails
                                } dropdown-item'> Visualizar</a>`;
                        }
                        if (classEdit != "hide-item") {
                            html += `<a href='/${controller}/Add/?tipo=update&janela=bisneta&id=${id
                                }' title='Alterar' data-original-title='Alterar - ${row[rowConfirm]
                                }' class='alterar${janela} ${classEdit} dropdown-item'> Alterar</a>`;
                        }
                        if (classDel != "hide-item") {
                            html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                                } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                                }|Del'> Deletar</a>`;
                        }
                        html += "</div></div>";
                        return html;
                    },
                    aTargets: [-1]
                }
            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $(nRow).attr("id", aData[aData.length - 1]);
                const cssClass = aData[6] == "Cancelado" ? "red" : "";
                $(nRow).addClass(cssClass);
                return nRow;
            },
            initComplete: function () {
                $(".dataTables_filter input")
                    .unbind()
                    .bind("keyup",
                        function (e) {
                            if (this.value.length == 0) {
                                myTable.fnFilter("");
                            } else {
                                if (this.value.length >= 3 && e.keyCode == 13) {
                                    myTable.fnFilter(this.value);
                                }
                            }
                        });
            }

        });
    };

var jDataTableNoSearchAndNoAction =
    function (idTabela, url, controller, rowConfirm, classDetails, classEdit, classDel, janela = "") {
        myTable = $(idTabela).dataTable({
            sAjaxSource: url,
            bServerSide: true,
            searching: false,
            bDestroy: true,
            bPaginate: true,
            pageLength: 50,
            dom: "fBrtip",
            language: { url: "/admin/libs/datatables/pt-br.json" },
            buttons: [],
            aoColumnDefs: [
                { bSortable: false, aTargets: [-1] },
                { bSearchable: false, aTargets: [-1] }
            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $(nRow).attr("id", aData[aData.length - 1]);
                const cssClass = aData[6] == "Cancelado" ? "red" : "";
                $(nRow).addClass(cssClass);
                return nRow;
            },
            initComplete: function () {
                const coluna = $(idTabela + " th").length - 1;
                $(idTabela + " tr").find(`td:eq(${coluna}),th:eq(${coluna})`).remove();
                $(".dataTables_filter input")
                    .unbind()
                    .bind("keyup",
                        function (e) {
                            if (this.value.length == 0) {
                                myTable.fnFilter("");
                            } else {
                                if (this.value.length >= 3 && e.keyCode == 13) {
                                    myTable.fnFilter(this.value);
                                }
                            }
                        });
            }

        });
    };

var jDataTableNoAction = function (idTabela, url, orderColum, orderType) {
    myTable = $(idTabela).dataTable({
        sAjaxSource: url,
        order: [[orderColum, orderType]],
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });

        }

    });
};

var jDataTableNoActionAndNoSearch = function (idTabela, url, orderColum, orderType) {
    myTable = $(idTabela).dataTable({
        sAjaxSource: url,
        searching: false,
        processing: true,
        order: [[orderColum, orderType]],
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });

        }

    });
};


var jDataTableDellOnly = function (idTabela, url, controller, rowConfirm, classDel, janela = "") {
    myTable = $(idTabela).dataTable({
        sAjaxSource: url,
        bServerSide: true,
        bDestroy: true,
        searching: false,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [
            { extend: "excel", title: $(".page-titles").find("h3").html() },
            { extend: "pdf", title: $(".page-titles").find("h3").html() },
            { extend: "print", title: $(".page-titles").find("h3").html() }
        ],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        "<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right'>";
                    html += `<a href='javascript:;' title='Deletar' class='deletarPai ${janela} ${classDel
                        } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                        }|Del'> Deletar</a>`;
                    html += "</div></div>";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);

            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });
        }

    });
};

var jDataTableEditDel =
    function (idTabela, url, orderColum, orderType, controller, rowConfirm, classDel, classEdit, janela = "") {
        myTable = $(idTabela).dataTable({
            sAjaxSource: url,
            order: [[orderColum, orderType]],
            bServerSide: true,
            searching: false,
            bDestroy: true,
            bPaginate: true,
            pageLength: 50,
            dom: "fBrtip",
            language: { url: "/admin/libs/datatables/pt-br.json" },
            buttons: [],
            aoColumnDefs: [
                { bSortable: false, aTargets: [-1] },
                { bSearchable: false, aTargets: [-1] },
                {
                    mRender: function (data, type, row) {
                        const id = data;
                        var html = "<div class='btn-group ml-auto m-t-10'>";
                        html +=
                            "<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right'>";
                        if (classEdit != "hide-item") {
                            html += `<a href='/${controller}/Edit/?id=${id
                                }' title='Alterar' data-original-title='Alterar - ${row[rowConfirm]}' class='alterar${janela} ${classEdit
                                } dropdown-item'> Alterar</a>`;
                        }
                        html += `<a href='javascript:;' title='Deletar' class='deletarPai ${janela} ${classDel
                            } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                            }|Del'> Deletar</a>`;
                        html += "</div></div>";
                        return html;
                    },
                    aTargets: [-1]
                }
            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $(nRow).attr("id", aData[aData.length - 1]);

                return nRow;
            },
            initComplete: function () {
                $(".dataTables_filter input")
                    .unbind()
                    .bind("keyup",
                        function (e) {
                            if (this.value.length == 0) {
                                myTable.fnFilter("");
                            } else {
                                if (this.value.length >= 3 && e.keyCode == 13) {
                                    myTable.fnFilter(this.value);
                                }
                            }
                        });
            }

        });
    };

var jDataTableVisuDel = function (idTabela,
    url,
    orderColum,
    orderType,
    controller,
    rowConfirm,
    classDel,
    classDetails,
    janela = "") {
    myTable = $(idTabela).dataTable({
        sAjaxSource: url,
        order: [[orderColum, orderType]],
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [
            { extend: "excel", title: $(".page-titles").find("h3").html() },
            { extend: "pdf", title: $(".page-titles").find("h3").html() },
            { extend: "print", title: $(".page-titles").find("h3").html() }
        ],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        "<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right'>";
                    if (classDetails != "hide-item") {
                        html += `<a href='/${controller}/Details/?dados=${id
                            }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]
                            }' class='detalhes${janela} ${classDetails
                            } dropdown-item'> Visualizar</a>`;
                    }
                    html += `<a href='javascript:;' title='Deletar' class='deletarPai ${janela} ${classDel
                        } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                        }|Del'> Deletar</a>`;
                    html += "</div></div>";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);

            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });
        }

    });
};

var jDataTableDown = function (idTabela, url, controller, rowConfirm, classDetails, classEdit, classDel, janela = "") {
    myTable = $(idTabela).dataTable({
        sAjaxSource: url,
        bServerSide: true,
        bDestroy: true,
        bPaginate: true,
        pageLength: 50,
        dom: "fBrtip",
        language: { url: "/admin/libs/datatables/pt-br.json" },
        buttons: [
            { extend: "excel", title: $(".page-titles").find("h3").html() },
            { extend: "pdf", title: $(".page-titles").find("h3").html() },
            { extend: "print", title: $(".page-titles").find("h3").html() },
        ],
        aoColumnDefs: [
            { bSortable: false, aTargets: [-1] },
            { bSearchable: false, aTargets: [-1] },
            {
                mRender: function (data, type, row) {
                    const id = data;
                    var html = "<div class='btn-group ml-auto m-t-10'>";
                    html +=
                        "<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right'>";
                    html += `<a href='/${controller}/Baixar/?id=${id
                        }' title='Visualizar' data-original-title='Visualizar - ${row[rowConfirm]}' class='${classDetails
                        } dropdown-item'> Baixar</a>`;
                    html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                        } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                        }|Del'> Deletar</a>`;
                    html += "</div></div>";
                    return html;
                },
                aTargets: [-1]
            }
        ],
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $(nRow).attr("id", aData[aData.length - 1]);
            const cssClass = aData[6] == "Cancelado" ? "red" : "";
            $(nRow).addClass(cssClass);
            return nRow;
        },
        initComplete: function () {
            $(".dataTables_filter input")
                .unbind()
                .bind("keyup",
                    function (e) {
                        if (this.value.length == 0) {
                            myTable.fnFilter("");
                        } else {
                            if (this.value.length >= 3 && e.keyCode == 13) {
                                myTable.fnFilter(this.value);
                            }
                        }
                    });
        }

    });
};
var jDataTableDownFixo =
    function (idTabela, url, controller, rowConfirm, classDetails, classDel, rowArquivo, janela = "") {
        myTable = $(idTabela).dataTable({
            sAjaxSource: url,
            bServerSide: true,
            bDestroy: true,
            bPaginate: true,
            pageLength: 50,
            dom: "fBrtip",
            language: { url: "/admin/libs/datatables/pt-br.json" },
            buttons: [
                { extend: "excel", title: $(".page-titles").find("h3").html() },
                { extend: "pdf", title: $(".page-titles").find("h3").html() },
                { extend: "print", title: $(".page-titles").find("h3").html() },
            ],
            aoColumnDefs: [
                { bSortable: false, aTargets: [-1] },
                { bSearchable: false, aTargets: [-1] },
                {
                    mRender: function (data, type, row) {
                        const id = data;
                        var html = "<div class='btn-group ml-auto m-t-10'>";
                        html +=
                            "<a href='javascript:void(0)' class='icon-options-vertical link' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a><div class='dropdown-menu dropdown-menu-right'>";
                        html += `<a href='/images/${controller}/${row[rowArquivo]
                            }' title='Visualizar' target='_blank' data-original-title='Visualizar - ${row[rowConfirm]
                            }' class='${classDetails
                            } dropdown-item'> Visualizar</a>`;
                        html += `<a href='javascript:;' title='Deletar' class='deletar${janela} ${classDel
                            } dropdown-item' data-rel='${id}|${row[rowConfirm]}|${controller
                            }|Del'> Deletar</a>`;
                        html += "</div></div>";
                        return html;
                    },
                    aTargets: [-1]
                }, {
                    mRender: function (data, type, row) {
                        const id = data;
                        const html =
                            `<input type="text" name="Descricao" id="Descricao" value="${row[1] == null ? "" : row[1]
                            }" class="form-control set-descricao" placeholder="Informe uma descrição" data-id=${row[
                            3]
                            } data-validation-required-message="obrigatório" aria-invalid="false" />`;
                        return html;
                    },
                    aTargets: [1]
                }
            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $(nRow).attr("id", aData[aData.length - 1]);
                const cssClass = aData[6] == "Cancelado" ? "red" : "";
                $(nRow).addClass(cssClass);
                return nRow;
            },
            initComplete: function () {
                $(".dataTables_filter input")
                    .unbind()
                    .bind("keyup",
                        function (e) {
                            if (this.value.length == 0) {
                                myTable.fnFilter("");
                            } else {
                                if (this.value.length >= 3 && e.keyCode == 13) {
                                    myTable.fnFilter(this.value);
                                }
                            }
                        });
            }

        });
    };

$(function () {

    $(document).delegate(".hand",
        "click",
        function () {
            $($(this).attr("data-campo")).val("");
            $(this).parent().parent().remove();
        });
    $(".js-switch").each(function () {
        new Switchery($(this)[0], $(this).data());
    });
    const myTable = null;
    const mTableInt = null;

    //mTableInt = $('.dataTablesInterna').DataTable({
    //       pageLength: 50,
    //       dom: 'fBrtip',
    //       language: { url: '/lib/datatables/pt-br.json' },
    //       buttons: [
    //           { extend: 'excel', title: $('.page-titles').find("h3").html() },
    //           { extend: 'pdf', title: $('.page-titles').find("h3").html() },
    //           { extend: 'print', title: $('.page-titles').find("h3").html() },
    //       ]

    //});


    $(".form-check label").click(function () {
        const input = $(this).parent().find("input[type=checkbox]");
        console.log(input);
        input.val(input.val() == "false" || input.val() == "False" ? true : false);
    });

    $(document).delegate(".changeStatusAss",
        "click",
        function (e) {
            e.preventDefault();
            const id = $(this).attr("data-id");
            const modal = window.parent.$("#modalPaiConfirm").one("click",
                "#btnPaiConfirme",
                function () {
                    window.parent.$(".preloader").show();
                    $.ajax({
                        url: "/Assinatura/ChangeStatus/",
                        type: "get",
                        data: { id: id },
                        contentType: "application/json",
                        success: function (result) {
                            window.parent.$(".preloader").hide();
                            window.parent.Notifica(getTitle(result.cssClass), result.mensagem, result.cssClass);
                            window.location.reload();
                        }
                    });
                });
            modal.find(".modal-body").html("Tem certeza que deseja alterar o status dessa assinatura?");
            modal.find(".modal-footer").find("#btnPaiConfirme").html("Sim");
            modal.find(".modal-footer").find("#btnPaiCancelar").html("Não");
            modal.on("show", function () { }).modal({ show: true, keyboard: false });
        });

    $(".formAAsync").on("submit",
        function (e) {
            e.preventDefault();
            var form = $(this);
            const $valid = !form.jqBootstrapValidation("hasErrors");
            if ($valid === true) {
                window.parent.$(".preloader").show();
                const formData = new FormData(this);
                $.ajax({
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: "POST",
                    dataType: "json",
                    url: form.attr("action"),
                    data: formData,
                    success: function (ret) {
                        window.parent.$(".preloader").hide();
                        if (ret.cssClass === "success") {
                            switch (ret.janela) {
                                case "pai":
                                    if (window.parent.myTable != null) {
                                        window.parent.myTable.api().ajax.reload();
                                    }
                                    if (ret.funcao != undefined) {
                                        Notifica(getTitle(ret.cssClass), ret.mensagem, ret.cssClass, ret.funcao);
                                    } else {
                                        NotificaClose(getTitle(ret.cssClass), ret.mensagem, ret.cssClass);
                                    }
                                    if (ret.url != "") {
                                        if (ret.url == "recarregar") {
                                            setTimeout(function () { window.location.reload(); }, 1500);
                                        } else {
                                            window.location.href = ret.url;
                                        }
                                    } else {
                                        setTimeout(function () {
                                            window.$("#modalAdm").modal("hide");
                                        },
                                            1500);
                                        $(":input", form)
                                            .not(":button, :submit, :reset, :hidden")
                                            .val("")
                                            .removeAttr("checked")
                                            .removeAttr("selected");
                                    }
                                    break;
                                case "filha":
                                    if (window.parent.myTable != null) {
                                        window.parent.myTable.api().ajax.reload();
                                    }
                                    if (ret.funcao != undefined) {
                                        window.parent.Notifica(getTitle(ret.cssClass),
                                            ret.mensagem,
                                            ret.cssClass,
                                            ret.funcao);
                                    } else {
                                        window.parent.NotificaClose(getTitle(ret.cssClass),
                                            ret.mensagem,
                                            ret.cssClass);
                                    }
                                    if (ret.url != "") {
                                        if (ret.url == "recarregar") {
                                            setTimeout(function () {
                                                window.parent.location.reload();
                                            },
                                                1500);
                                        } else {
                                            window.location.href = ret.url;
                                        }
                                    } else {
                                        window.parent.$("#modalAdm").modal("hide");
                                    }
                                    break;
                                case "neta":
                                    if (ret.funcao != undefined) {
                                        window.parent.parent.Notifica(getTitle(ret.cssClass),
                                            ret.mensagem,
                                            ret.cssClass,
                                            ret.funcao);
                                    } else {
                                        window.parent.parent.NotificaClose(getTitle(ret.cssClass),
                                            ret.mensagem,
                                            ret.cssClass);
                                    }
                                    if (ret.url != "") {
                                        if (ret.url == "recarregar") {
                                            setTimeout(function () {
                                                window.parent.location.reload();
                                            },
                                                1500);
                                        } else {
                                            window.location.href = ret.url;
                                        }
                                    } else {
                                        window.parent.$("#modalPaiAdm").modal("hide");
                                    }
                                    break;
                                case "bisneta":
                                    if (ret.funcao != undefined) {
                                        window.parent.Notifica(getTitle(ret.cssClass),
                                            ret.mensagem,
                                            ret.cssClass,
                                            ret.funcao);
                                    } else {
                                        window.parent.NotificaClose(getTitle(ret.cssClass),
                                            ret.mensagem,
                                            ret.cssClass);
                                    }
                                    if (ret.url != "") {
                                        if (ret.url == "recarregar") {
                                            setTimeout(function () {
                                                window.parent.location.reload();
                                            },
                                                1500);
                                        } else {
                                            window.location.href = ret.url;
                                        }
                                    } else {
                                        window.parent.$("#modalPaiPaiAdm").modal("hide");
                                    }
                                    break;
                            }
                        } else {
                            window.parent.Notifica(getTitle(ret.cssClass), ret.mensagem, ret.cssClass, ret.funcao);
                        }
                    },
                    error: function (error) {
                        $(".preloader").hide();
                        window.parent.$(".preloader").hide();
                        window.parent.parent.$(".preloader").hide();
                        Notifica("Atenção", "Ocorreu um de comunicação do app com o servidor da aplicação", "warning");
                    }
                });
            } else {
                window.parent.Notifica("Atenção", "Preencha os campos obrigatórios", "warning");
            }
        });

    $("input[type=file].addFile").change(function (e) {
        e.preventDefault();
        $(this).parent().find("label").html("<i class='fa fa-paperclip'></i> 01 arquivo selecionado");
    });

    $.fn.addItems = function (data, id) {
        return this.each(function () {
            var list = this;
            $.each(data,
                function (index, itemData) {
                    const option = new Option(itemData.text,
                        itemData.value,
                        itemData.value.toString() == id ? true : false,
                        itemData.value.toString() == id ? true : false);
                    list.add(option);
                });
        });
    };

    $.fn.addItemsVazio = function (data, id) {
        return this.each(function () {
            var list = this;
            list.add(new Option("", ""));
            $.each(data,
                function (index, itemData) {
                    const option = new Option(itemData.text,
                        itemData.value,
                        itemData.value.toString() == id ? true : false,
                        itemData.value.toString() == id ? true : false);
                    list.add(option);
                });
        });
    };

    $.fn.addItemsRotulo = function (data, id, rotulo) {
        return this.each(function () {
            var list = this;

            $.each(data,
                function (index, itemData) {
                    if (itemData.Value.toString() == "") {
                        list.add(new Option(rotulo, ""));
                    } else {
                        const option = new Option(itemData.Text,
                            itemData.Value,
                            itemData.Value.toString() == id ? true : false,
                            itemData.Value.toString() == id ? true : false);
                        list.add(option);
                    }

                });
        });
    };

    $(".disable").prop("disabled", true);

    //$('.chosen-select').chosen({ language: "pt-BR" });

    $('a[data-toggle="tab"]').on("show.bs.tab",
        function (e) {
            window.location.hash = e.target.hash;
        });

    //$(document).ajaxStart(function () {
    //    $(".preloader").show();
    //});

    //$(document).ajaxStop(function () {
    //    $(".preloader").hide();
    //});

    $(document).delegate(".so-numeros",
        "keyup",
        function () {
            const valor = $(this).val().replace(/[^0-9]+/g, "");
            $(this).val(valor);
        });

    $(document).delegate(".so-letras",
        "keyup",
        function () {
            const valor = $(this).val().replace(/[^a-z ]+/gi, "");
            $(this).val(valor);
        });


    $("body").delegate(".deletar",
        "click",
        function (e) {
            e.preventDefault();
            var link = $(this);
            var dados = link.attr("data-rel").split("|");
            const modal = $("#modalConfirm").one("click",
                "#btnConfirme",
                function () {
                    $.ajax({
                        url: `/${dados[2]}/${dados[3]}/`,
                        type: "get",
                        data: { id: dados[0] },
                        contentType: "application/json",
                        success: function (result) {
                            NotificaClose(getTitle(result.cssClass), result.mensagem, result.cssClass);
                            if (result.cssClass === "success") {
                                link.parents().find(`#${dados[0]}`).remove();
                            }
                        }
                    });
                });
            modal.find(".modal-footer").find("#btnConfirme").html("Sim");
            modal.find(".modal-footer").find("#btnCancelar").html("Não");
            modal.find(".modal-body").html(`Tem certeza que deseja deletar o(a) ${dados[1]} ?`);
            modal.on("show", function () { }).modal({ show: true, keyboard: false });
        });
    $("body").delegate(".deletarPai",
        "click",
        function (e) {
            e.preventDefault();
            var link = $(this);
            var dados = link.attr("data-rel").split("|");
            const modal = window.parent.$("#modalPaiConfirm").one("click",
                "#btnPaiConfirme",
                function () {
                    $.ajax({
                        url: `/${dados[2]}/${dados[3]}/`,
                        type: "get",
                        data: { id: dados[0] },
                        contentType: "application/json",
                        success: function (result) {
                            window.parent.NotificaClose(getTitle(result.cssClass), result.mensagem, result.cssClass);
                            if (result.cssClass === "success") {
                                link.parents().find(`#${dados[0]}`).remove();
                            }
                        }
                    });
                });
            modal.find(".modal-footer").find("#btnPaiConfirme").html("Sim");
            modal.find(".modal-footer").find("#btnPaiCancelar").html("Não");
            modal.find(".modal-body").html(`Tem certeza que deseja deletar o(a) ${dados[1]} ?`);
            modal.on("show", function () { }).modal({ show: true, keyboard: false });
        });
    $("body").delegate(".deletarPaiPai",
        "click",
        function (e) {
            e.preventDefault();
            var link = $(this);
            var dados = link.attr("data-rel").split("|");
            const modal = window.parent.parent.$("#modalPaiPaiConfirm").one("click",
                "#btnPaiPaiConfirme",
                function () {
                    $.ajax({
                        url: `/${dados[2]}/${dados[3]}/`,
                        type: "get",
                        data: { id: dados[0] },
                        contentType: "application/json",
                        success: function (result) {
                            window.parent.parent.NotificaClose(getTitle(result.cssClass),
                                result.mensagem,
                                result.cssClass);
                            if (result.cssClass === "success") {
                                link.parents().find(`#${dados[0]}`).remove();
                            }
                        }
                    });
                });
            modal.find(".modal-footer").find("#btnPaiPaiConfirme").html("Sim");
            modal.find(".modal-footer").find("#btnPaiPaiCancelar").html("Não");
            modal.find(".modal-body").html(`Tem certeza que deseja deletar o(a) ${dados[1]} ?`);
            modal.on("show", function () { }).modal({ show: true, keyboard: false });
        });
    $("body").delegate(".deletarDetalhes",
        "click",
        function (e) {
            e.preventDefault();
            var link = $(this);
            var dados = link.attr("data-rel").split("|");
            const modal = $("#modalPaiConfirm").one("click",
                "#btnPaiConfirme",
                function () {
                    $.ajax({
                        url: `/${dados[2]}/${dados[3]}/`,
                        type: "get",
                        data: { id: dados[0] },
                        contentType: "application/json",
                        success: function (result) {
                            window.parent.NotificaClose(getTitle(result.cssClass), result.mensagem, result.cssClass);
                            if (result.cssClass === "success") {
                                link.parents().find(`#${dados[0]}`).remove();
                            }
                        }
                    });
                });
            modal.find(".modal-footer").find("#btnPaiConfirme").html("Sim");
            modal.find(".modal-footer").find("#btnPaiCancelar").html("Não");
            modal.find(".modal-body").html(`Tem certeza que deseja deletar o(a) ${dados[1]} ?`);
            modal.on("show", function () { }).modal({ show: true, keyboard: false });
        });

    functionCarregaValidacoes();
});