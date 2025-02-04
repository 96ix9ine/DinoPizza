document.addEventListener('DOMContentLoaded', function () {
    const menu = document.querySelector('.menu-container');
    const main = document.querySelector('main'); // Получаем элемент main
    const animatedBg = document.getElementById('animated-bg'); // Получаем фон
    const header = document.querySelector('header'); // Получаем header
    const fixedNavbar = document.querySelector('.fixed-navbar'); // Получаем nav с классом fixed-navbar
    const footer = document.querySelector('footer'); // Получаем footer

    // Функция для скрытия псевдоэлементов
    function hideBodyPseudoElements() {
        document.body.setAttribute('data-pseudo-elements', 'hidden'); // Добавляем атрибут для скрытия псевдоэлементов
    }

    // Функция для восстановления псевдоэлементов
    function restoreBodyPseudoElements() {
        document.body.removeAttribute('data-pseudo-elements'); // Убираем атрибут для восстановления псевдоэлементов
    }

    // Функция для установки высоты main
    function setMainHeightAuto() {
        if (main) {
            main.style.height = 'auto'; // Устанавливаем высоту auto на главной странице
        }
    }

    // Функция для сброса высоты main
    function resetMainHeight() {
        if (main) {
            main.style.height = '100vh'; // Сбрасываем высоту для других страниц
        }
    }

    // Функция для управления видимостью меню и фона в зависимости от пути
    function toggleMenuVisibility() {
        const currentPath = window.location.pathname;
        window.addEventListener('popstate', toggleMenuVisibility);

        // Скрываем header и fixed-navbar для /Identity/Account/Manage
        if (currentPath.startsWith('/Identity/Account/Manage')) {
            if (header) header.style.display = 'none'; // Скрываем header
            if (fixedNavbar) fixedNavbar.style.display = 'none'; // Скрываем fixed-navbar
        } else if (
            currentPath === '/Identity/Account/Login' ||
            currentPath === '/Identity/Account/Register' ||
            currentPath === '/Identity/Account/RegisterCourier' ||
            currentPath.startsWith('/Admin') ||
            currentPath.startsWith('/ContentManager') ||
            currentPath.startsWith('/Courier') ||
            (currentPath === '/Order/Checkout' || currentPath.startsWith('/Order'))
        ) {
            // Скрытие для страниц регистрации/авторизации
            if (header) header.style.display = 'none';
            if (fixedNavbar) fixedNavbar.style.display = 'none';
        } else {
            // Показываем header и fixed-navbar на всех остальных страницах
            if (header) header.style.display = '';
            if (fixedNavbar) fixedNavbar.style.display = '';
        }

        // Для главной страницы
        if (currentPath === '/' || currentPath === '') {
            menu.classList.remove('hidden'); // Показываем меню
            restoreBodyPseudoElements(); // Восстанавливаем псевдоэлементы на главной странице
            setMainHeightAuto(); // Устанавливаем height: auto для main на главной
            if (animatedBg) animatedBg.classList.add('hidden'); // Скрываем фон
        } else {
            menu.classList.add('hidden'); // Скрываем меню
            hideBodyPseudoElements(); // Скрываем псевдоэлементы на остальных страницах
            resetMainHeight(); // Сбрасываем высоту main на других страницах
            if (animatedBg) animatedBg.classList.remove('hidden'); // Показываем фон
        }
    }

    toggleMenuVisibility(); // Выполняем проверку при загрузке страницы

    // Обработчик для кликов по ссылкам
    document.querySelectorAll('a.nav-link, .navbar-brand').forEach(link => {
        link.addEventListener('click', function () {
            menu.classList.add('hidden'); // Скрываем меню при клике на ссылку
            hideBodyPseudoElements(); // Скрываем псевдоэлементы при переходе по ссылке
            resetMainHeight(); // Сбрасываем высоту main при переходе по ссылке
            if (animatedBg) animatedBg.classList.remove('hidden'); // Показываем фон при переходе
        });
    });
});
