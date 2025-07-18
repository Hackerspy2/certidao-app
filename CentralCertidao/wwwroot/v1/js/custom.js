﻿/*
Template Name: United Pets
Author: Ingrid Kuhn
Author URI: themeforest/user/ingridk
Version: 1.3
*/
jQuery(function ($) {

		$(document).ready(function () {
				baguetteBox.run('.tz-gallery');
				$("#preloader").fadeOut("slow");
				$('.page-scroll a').on('click', function (event) {
						var $anchor = $(this);
						$('html, body').stop().animate({
								scrollTop: $($anchor.attr('href')).offset().top
						}, 1500, 'easeInOutExpo');
						event.preventDefault();
				});
				if ($(window).width() > 1200) {
						$(".navbar .dropdown").on({
								mouseenter: function () {
										$(this).find('.dropdown-menu').first().stop(true, true).delay(50).slideDown();

								},
								mouseleave: function () {
										$(this).find('.dropdown-menu').first().stop(true, true).delay(100).fadeOut();
								}
						});
				}
				var offset = 200;
				var duration = 500;
				$(window).scroll(function () {
						if ($(this).scrollTop() > offset) {
								$('.back-to-top').fadeIn(400);
						} else {
								$('.back-to-top').fadeOut(400);
						}
				});
				
				$('.owl-stage').owlCarousel({
						loop: true,
						margin: 0,
						autoplay: true,
						nav: true,
						navText: [" <i class='fas fa-chevron-left'></i>", " <i class='fas fa-chevron-right'></i>"],
						dots: true,
						responsive: {
								0: { items: 1, stagePadding: 0 },
								767: { items: 2, stagePadding: 60 },
								1200: { items: 3, stagePadding: 120 }
						}
				});

				$(".carousel-4items").owlCarousel({
						nav: true,
						navText: ["<i class='fa fa-chevron-left'></i>", "<i class='fa fa-chevron-right'></i>"],
						dots: true,
						margin: 30,
						loop: true,
						autoplay: false,
						responsiveClass: true,
						responsive: {
								0: { items: 1 },
								767: { items: 2 },
								1200: { items: 4 }
						}
				});
				$(".carousel-3items").owlCarousel({
						nav: true,
						navText: ["<i class='fa fa-chevron-left'></i>", "<i class='fa fa-chevron-right'></i>"],
						dots: true,
						margin: 30,
						loop: true,
						autoplay: false,
						responsiveClass: true,
						responsive: {
								0: { items: 1 },
								767: { items: 2 },
								1200: { items: 3 }
						}
				});
				$(".carousel-2items").owlCarousel({
						nav: true,
						navText: ["<i class='fa fa-chevron-left'></i>", "<i class='fa fa-chevron-right'></i>"],
						dots: true,
						margin: 30,
						loop: true,
						autoplay: false,
						responsiveClass: true,
						responsive: {
								0: { items: 1 },
								991: { items: 2 }
						}
				});
				$(".carousel-1item").owlCarousel({
						nav: true,
						navText: ["<i class='fa fa-chevron-left'></i>", "<i class='fa fa-chevron-right'></i>"],
						dots: true,
						margin: 30,
						loop: true,
						autoplay: false,
						responsiveClass: true,
						responsive: {
								0: { items: 1 }
						}
				});

				$('.magnific-popup').magnificPopup({
						delegate: 'a', // child items selector, by clicking on it popup will open
						type: 'image',
						overflowY: 'scroll',
						gallery: {
								enabled: true
						},
						titleSrc: 'title',
						titleSrc: function (item) {
								return '<a href="' + item.el.attr('href') + '">' + item.el.attr('title') + '</a>';
						},
						callbacks: { open: function () { $('.fixed-top').css('margin-right', '17px'); }, close: function () { $('.fixed-top').css('margin-right', '0px'); } }
				});
				AOS.init({
						disable: 'mobile',
						duration: 1500,
						once: true
				});

				$('#gallery-isotope').isotope({
						itemSelector: '.col-lg-6',
						layoutMode: 'masonry'
				});
				$('.category-isotope a').click(function () {
						$('.category-isotope a').removeClass('active');
						$(this).addClass('active');
						var selector = $(this).attr('data-filter');
						$('#gallery-isotope').isotope({
								filter: selector
						});
						return false;
				});
				

				$("#submit_btn").on("click", function () {
						var proceed = true;
						$("#contact_form input[required], #contact_form textarea[required]").each(function () {
								$(this).css('border-color', '');
								if (!$.trim($(this).val())) { //if this field is empty 
										$(this).css('border-color', '#e44747');
										$("#contact_results").html('<br><div class="alert alert-danger">Preencha todos os campos.</div>').show();
										proceed = false; //set do not proceed flag
								}
								var email_reg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
								if ($(this).attr("type") === "email" && !email_reg.test($.trim($(this).val()))) {
										$(this).css('border-color', '#e44747');
										$("#contact_results").html('<br><div class="alert alert-danger">E-mail inválido.</div>').show();
										proceed = false; //set do not proceed flag				
								}
						});

						if (proceed)
						{
							$("#preloader").show();
								let postData = {
										'nome': $('input[name=nome]').val(),
										'email': $('input[name=email]').val(),
										'celular': $('input[name=celular]').val(),
										'mensagem': $('textarea[name=mensagem]').val(),
										'recaptcha': $('textarea[name=g-recaptcha-response]').val()
								};
								$.post('/Home/ContatoEnviar', postData, function (response) {
									let output;
									$("#preloader").hide();
									if (response.cssClass == 'error') { //load json data from server and output message     
										output = '<br><div class="alert alert-danger">' + response.mensagem + '</div>';
									} else {
										output = '<br><div class="alert alert-success">' + response.mensagem + '</div>';
												$("#contact_form input, #contact_form textarea").val('');
												setTimeout(function() { window.location.reload(); }, 3000);
									}
										$('html, body').animate({ scrollTop: $("#contact_form").offset().top - 50 }, 2000);
										$("#contact_results").hide().html(output).slideDown();
								}, 'json');
						}
				});
				$("#submit_btn_acender").on("click", function () {
						var proceed = true;
						$("#contact_form_acender input[required], #contact_form_acender textarea[required]").each(function () {
								$(this).css('border-color', '');
								if (!$.trim($(this).val())) { //if this field is empty 
										$(this).css('border-color', '#e44747');
										$("#contact_results").html('<br><div class="alert alert-danger">Preencha todos os campos.</div>').show();
										proceed = false; //set do not proceed flag
								}
								var email_reg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
								if ($(this).attr("type") === "email" && !email_reg.test($.trim($(this).val()))) {
										$(this).css('border-color', '#e44747');
										$("#contact_results").html('<br><div class="alert alert-danger">E-mail inválido.</div>').show();
										proceed = false; //set do not proceed flag				
								}
						});

						if (proceed)
						{
							$("#preloader").show();
								let postData = {
										'idDepoimento': $('input[name=idDepoimento]').val(),
										'nome': $('input[name=nome]').val(),
										'email': $('input[name=email]').val(),
										'mensagem': $('textarea[name=mensagem]').val(),
										'recaptcha': $('textarea[name=g-recaptcha-response]').val()
								};
								$.post('/DepoimentoComentario/Add', postData, function (response) {
									let output;
									$("#preloader").hide();
									if (response.cssClass == 'error') { //load json data from server and output message     
										output = '<br><div class="alert alert-danger">' + response.mensagem + '</div>';
									} else {
										output = '<br><div class="alert alert-success">' + response.mensagem + '</div>';
												$("#contact_form_acender input, #contact_form_acender textarea").val('');
										}
										$('html, body').animate({ scrollTop: $("#contact_form_acender").offset().top - 50 }, 2000);
										$("#contact_results").hide().html(output).slideDown();
								}, 'json');
						}
				});

				$("#contact_form  input[required=true], #contact_form textarea[required=true], #contact_form_acender input[required=true], #contact_form_acender textarea[required=true]").keyup(function () {
						$(this).css('border-color', '');
						$("#result").slideUp();
				});
				
				$(window).scroll(function () {
						if ($("#main-nav").offset().top > 60) {
								$('.top-bar').slideUp({
										duration: 250,
										easing: "easeInOutSine"
								}).fadeOut(120);
						} else {
								$('.top-bar').slideDown({
										duration: 0,
										easing: "easeInOutSine"
								}).fadeIn(120);
						}
				}); 
		});

});