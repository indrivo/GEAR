/*!
  * FreakPixels v1.1.0 (http://freakpixels.com/)
  * Copyright 2011-2018 The FreakPixels Authors 
  * Licensed under MIT    
  */

$(window).on("load", function() {

	$('.loader-wrapper').fadeOut(1000, function() {
		$(this).remove();
	});

});

$(function(){



        $('[data-toggle="tooltip"]').tooltip();
        $('[data-toggle="popover"]').popover();

        $(document).on('click', '[data-toggle="lightbox"]', function(event) {
            event.preventDefault();
            $(this).ekkoLightbox();
        });



        /* Side Bar Toggle*/
		$('.toggle-sidebar').click(function(){
			$('.sidebar').toggleClass('collapsed');

			if( localStorage.getItem("asideMode") === 'collapsed' ){
				localStorage.setItem("asideMode", 'expanded')
			}else{
				localStorage.setItem("asideMode", 'collapsed')
			}
		});

		var p;
		$('.hide-sidebar').click(function(){
			if ( p ) {
				p.prependTo(".wrapper");
				p = null;
			} else {
				p = $(".sidebar").detach();
			}
		});

		$.fn.setAsideMode = function(){
			if( localStorage.getItem("asideMode") === null ){

			}
			else if( localStorage.getItem("asideMode") === 'collapsed' ){
				$('.sidebar').addClass('collapsed');
			}
			else{
				$('.sidebar').removeClass('collapsed');
			}
		}

		$.fn.setAsideMode();
      





		$('.navigation li:has(.sub-nav) a').click(function(){
			/*$('.navigation li').removeClass('open');*/
			$(this).siblings('.sub-nav').slideToggle();
			$(this).parent().toggleClass('open');
		});


		$('.navigation ul li:has(.sub-nav)').on('mouseover', function() {

			if ($(".sidebar").hasClass("collapsed")) {

		    var $menuItem = $(this),
		        $submenuWrapper = $('> .sub-nav', $menuItem);
		    
		    // grab the menu item's position relative to its positioned parent
		    var menuItemPos = $menuItem.position();
		    
		    // place the submenu in the correct position relevant to the menu item
		    $submenuWrapper.css({
		      top: menuItemPos.top,
		      left: menuItemPos.left + $menuItem.outerWidth()
		    });
			}

		});


		$('.toggle-controls').click(function(){
			$('.controls-wrapper').toggle().toggleClass('d-none');
		});
		






		$('[data-toggle="toast"]').click(function(){

				var dataAlignment = $(this).attr( 'data-alignment' );
				var dataPlacement = $(this).attr( 'data-placement' );
				var dataContent = $(this).attr( 'data-content' );
				var dataStyle = $(this).attr( 'data-style' );


				if( $('.toaGR.' + dataAlignment + '-' + dataPlacement).length  ){
					
					$('.toaGR.' + dataAlignment + '-' + dataPlacement ).append('<div class="alert alert-dismissible fade show alert-' + dataStyle + ' "> ' + dataContent + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" class="material-icons md-18">clear</span></button></div>');

				}
				else{
					$('body').append('<div class="toast ' +  dataAlignment + '-' + dataPlacement + '"> <div class="alert alert-dismissible fade show alert-' + dataStyle + ' "> ' + dataContent + '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" class="material-icons md-18">clear</span></button></div> </div>');
				}
				

		});





		/**************************************
		Chosen Form Control
		**************************************/
		$('.form-control-chosen').chosen({
		  allow_single_deselect: true,
		  width: '100%'
		});
		$('.form-control-chosen-required').chosen({
		  allow_single_deselect: false,
		  width: '100%'
		});
		$('.form-control-chosen-search-threshold-100').chosen({
		  allow_single_deselect: true,
		  disable_search_threshold: 100,
		  width: '100%'
		});
		$('.form-control-chosen-optgroup').chosen({
		  width: '100%'
		});
		$(function() {
  		$('[title="clickable_optgroup"]').addClass('chosen-container-optgroup-clickable');
		});
		$(document).on('click', '[title="clickable_optgroup"] .group-result', function() {
		  var unselected = $(this).nextUntil('.group-result').not('.result-selected');
		  if(unselected.length) {
		    unselected.trigger('mouseup');
		  } else {
		    $(this).nextUntil('.group-result').each(function() {
		      $('a.search-choice-close[data-option-array-index="' + $(this).data('option-array-index') + '"]').trigger('click');
		    });
		  }
		});






		/*****************************************
		Themer
		*****************************************/

		 $.fn.removeClassStartingWith = function (filter) {
		    $(this).removeClass(function (index, className) {
		        return (className.match(new RegExp("\\S*" + filter + "\\S*", 'g')) || []).join(' ')
		    });
		    return this;
		};


		$('.theme-changer').click(function(){
			var primaryColor = $(this).attr('primary-color');
			var sidebarBg = $(this).attr('sidebar-bg');
			var logoBg = $(this).attr('logo-bg');
			var headerBg = $(this).attr('header-bg');

			localStorage.setItem("primaryColor", primaryColor);
			localStorage.setItem("sidebarBg", sidebarBg);
			localStorage.setItem("logoBg", logoBg);
			localStorage.setItem("headerBg", headerBg);

			$.fn.setThemeTone(primaryColor);
		});



		 $.fn.setThemeTone = function(primaryColor){

		 	if(localStorage.getItem("primaryColor") === null){

		 	}
		 	else{

					/* SIDEBAR */
					if( localStorage.getItem("sidebarBg") === "light" ){
						$('.sidebar ').addClass('sidebar-light');
					}
					else{
						$('.sidebar').removeClass('sidebar-light');
					}

					
					
					/* PRIMARY COLOR */
					if( localStorage.getItem("primaryColor") === 'primary' ){
						document.documentElement.style.setProperty('--theme-colors-primary' , '#4B89FC');
					}else{
						var colorCode = getComputedStyle(document.body).getPropertyValue('--theme-colors-' + localStorage.getItem("primaryColor") );
						document.documentElement.style.setProperty('--theme-colors-primary' , colorCode);
					}

					
					/* LOGO */
					if( localStorage.getItem("logoBg") === 'white' || localStorage.getItem("logoBg") === 'light'){
						$('.sidebar .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-dark').addClass('navbar-light bg-' + localStorage.getItem("logoBg"));
					}else{
						$('.sidebar .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-light').addClass('navbar-dark bg-' + localStorage.getItem("logoBg"));
					}



					/* HEADER */
					if( localStorage.getItem("headerBg") === "light" || localStorage.getItem("headerBg") === "white"){
						$('.header .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-dark').addClass('navbar-light bg-' + localStorage.getItem("headerBg"));
					}
					else{
						$('.header .navbar').removeClassStartingWith('bg').removeClassStartingWith('navbar-light').addClass('navbar-dark bg-' + localStorage.getItem("headerBg"));
					}

			}



		}

		
		 
		$.fn.setThemeTone();
		



		



		

});



function toggleFullScreen() {
	if ((document.fullScreenElement && document.fullScreenElement !== null) || (!document.mozFullScreen && !document.webkitIsFullScreen)) {
		if (document.documentElement.requestFullScreen) {
			document.documentElement.requestFullScreen();
		} else if (document.documentElement.mozRequestFullScreen) {
			document.documentElement.mozRequestFullScreen();
		} else if (document.documentElement.webkitRequestFullScreen) {
			document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
		}
	} else {
		if (document.cancelFullScreen) {
			document.cancelFullScreen();
		} else if (document.mozCancelFullScreen) {
			document.mozCancelFullScreen();
		} else if (document.webkitCancelFullScreen) {
			document.webkitCancelFullScreen();
		}
	}
}




