$(window).scroll(function () {
    var fixedNavbar = $('.fixed-navbar'); // Блок с категориями
    var scrollPosition = $(window).scrollTop();
    var navbarLogo = $('.navbar-logo'); // Логотип

    // Если прокрутка началась, делаем блок фиксированным и показываем логотип
    if (scrollPosition > 0) {
        fixedNavbar.addClass('fixed-top'); // Добавляем класс для фиксированной позиции
        navbarLogo.addClass('logo-visible').removeClass('logo-hidden'); // Показываем логотип
    } else {
        fixedNavbar.removeClass('fixed-top'); // Убираем фиксированную позицию
        navbarLogo.removeClass('logo-visible').addClass('logo-hidden'); // Скрываем логотип
    }
});
