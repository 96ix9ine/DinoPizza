// Функция для загрузки содержимого корзины и обновления иконки
function loadCartContent() {
    // Отправляем GET-запрос на сервер для получения содержимого корзины
    fetch('/Cart/GetCartContent')
        .then(response => {
            if (!response.ok) {
                throw new Error('Ошибка загрузки корзины.');
            }
            return response.text(); // Ожидаем, что сервер вернет HTML
        })
        .then(html => {
            // Вставляем содержимое корзины в модальное окно
            document.querySelector('#cartModal .modal-body').innerHTML = html;
        })
        .catch(error => {
            console.error('Ошибка при загрузке корзины:', error);
            document.querySelector('#cartModal .modal-body').innerHTML = '<p>Ошибка загрузки корзины. Попробуйте еще раз позже.</p>';
        });
}

document.addEventListener('DOMContentLoaded', function () {
    // Обработчик для кнопки уменьшения количества
    document.querySelectorAll('.btn-remove-item').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();  // Отменяем стандартное поведение формы

            const form = e.target.closest('form');
            const productId = form.querySelector('input[name="id"]').value;

            // Отправляем запрос на сервер для удаления товара
            fetch(form.action, {
                method: form.method,
                body: new FormData(form),
            })
                .then(response => response.text())
                .then(html => {
                    // Обновляем содержимое корзины в модальном окне
                    document.querySelector('#cartModal .modal-body').innerHTML = html;
                })
                .catch(error => {
                    console.error('Ошибка при обновлении корзины:', error);
                });
        });
    });

    // Обработчик для кнопки увеличения количества
    document.querySelectorAll('.btn-add-item').forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();  // Отменяем стандартное поведение формы

            const form = e.target.closest('form');
            const productId = form.querySelector('input[name="id"]').value;

            // Отправляем запрос на сервер для добавления товара
            fetch(form.action, {
                method: form.method,
                body: new FormData(form),
            })
                .then(response => response.text())
                .then(html => {
                    // Обновляем содержимое корзины в модальном окне
                    document.querySelector('#cartModal .modal-body').innerHTML = html;
                })
                .catch(error => {
                    console.error('Ошибка при обновлении корзины:', error);
                });
        });
    });
});



/* Обработчики для модалки */
function showBackdrop() {
    const backdrop = document.getElementById('modalBackdrop');
    backdrop.style.display = 'block';
    setTimeout(() => backdrop.classList.add('show'), 10);  // Добавляем класс show с небольшой задержкой
}

function hideBackdrop() {
    const backdrop = document.getElementById('modalBackdrop');
    backdrop.classList.remove('show');  // Убираем класс show
    setTimeout(() => backdrop.style.display = 'none', 300);  // Скрываем фон
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.btn.btn-outline').forEach(function (button) {
        button.addEventListener('click', function () {
            const modal = document.getElementById('cartModal');

            // Убираем класс скрытия
            modal.classList.remove('modal-hidden');
            modal.style.zIndex = 1050;
            modal.style.display = 'flex';  // Показываем окно

            // Инициализируем Bootstrap Modal
            const bootstrapModal = new bootstrap.Modal(modal);
            bootstrapModal.show();

            // Показываем backdrop
            showBackdrop();

            // Удаляем лишние .modal-backdrop элементы
            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(backdrop => backdrop.remove());
        });
    });

    // Закрытие модального окна по клику на фон
    document.getElementById('cartModal').addEventListener('click', function (e) {
        const modal = document.getElementById('cartModal');
        if (e.target.classList.contains('modal')) {
            modal.style.zIndex = -1;
            modal.style.display = 'none';
            modal.classList.add('modal-hidden');

            const bootstrapModal = bootstrap.Modal.getInstance(modal);
            bootstrapModal.hide();

            // Скрываем backdrop
            hideBackdrop();

            const backdrops = document.querySelectorAll('.modal-backdrop');
            backdrops.forEach(backdrop => backdrop.remove());
        }
    });
});
