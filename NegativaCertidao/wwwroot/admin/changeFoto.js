$(function () {

                var $image = $(".image-crop > img");
                $($image).cropper({
                    aspectRatio: 1 / 1,
                    minWidth: "100px",
                    minHeight: "100px",
                    maxWidth: "200px",
                    maxHeight: "200px",
                    preview: ".img-preview",
                    zoomable: false,
                    scalable: true,
                    crop: function (e) {
                        $("#imgX").val(e.x);
                        $("#imgY").val(e.y);
                        $("#imgW").val(e.width);
                        $("#imgH").val(e.height);
                    }
                });

                var $inputImage = $("#inputImage");
                if (window.FileReader) {
                    $inputImage.change(function () {
                        var fileReader = new FileReader(),
                            files = this.files;
                        if (!files.length) {
                            return;
                        }
                        var file = files[0];
                        if (/^image\/\w+$/.test(file.type)) {
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function () {
                                $image.cropper("reset", true).cropper("replace", this.result);
                            };
                            $(".textoInicial").hide();
                            $(".fotoAtual").hide();
                            $(".enviarFoto").show();
                        } else {
                            alert("Por favor, escolha um arquivo de imagem.");
                        }
                    });
                } else {
                    $inputImage.addClass("hide");
                }
                $(".bt-cancelar").bind("click", function () {
                    $inputImage.val("");
                    $(".textoInicial").show();
                    $(".fotoAtual").show();
                    $(".enviarFoto").hide();
                });
                $("body").delegate("#removerFoto", 'click', function (e) {
                    e.preventDefault();
                    var modal = window.parent.$('#modalConfirm').modal({ keyboard: false }).one('click', '#btnConfirme', function () {
                        $.ajax({
                            url: "/@NomeController/RemoveFoto/?id=@Model.Id",
                            type: "get",
                            dataType: "json",
                            success: function (ret) {
                                window.parent.NotificaClose("Sucesso!", "Foto removida com sucesss!", "success");
                                setInterval(function () { window.location.reload(); }, 2000);
                            }
                        });
                    });
                    modal.find(".modal-footer").find("#btnConfirme").html("Sim");
                    modal.find(".modal-footer").find("#btnCancelar").html("Não");
                    modal.find(".modal-body").html("Tem certeza que deseja remover a imagem?");
                });
            });