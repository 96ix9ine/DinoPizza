document.addEventListener('DOMContentLoaded', () => {
    const addressContainer = document.querySelector('.saved-addresses-container');
    const selectButton = document.getElementById('selectAddress');
    const userAddressField = document.getElementById('userAddress');
    const saveNewAddressButton = document.getElementById('saveNewAddress');
    const fullAddressInput = document.getElementById('fullAddress');
    const entranceInput = document.getElementById('entrance');
    const floorInput = document.getElementById('floor');
    const apartmentInput = document.getElementById('apartment');
    const backToAddressModal = document.getElementById('backToAddressModal');
    const mapModal = document.getElementById('mapModal');
    const addressModal = document.getElementById('addressModal');

    let selectedAddress = null; // Переменная для хранения выбранного адреса

    // Обработчик клика по адресу в списке
    addressContainer.addEventListener('click', (event) => {
        const target = event.target;

        if (target.classList.contains('address-item')) {
            // Убираем выделение с других адресов
            const allAddressItems = addressContainer.querySelectorAll('.address-item');
            allAddressItems.forEach(item => item.classList.remove('active'));

            // Выделяем текущий адрес
            target.classList.add('active');
            selectedAddress = target.dataset.address;

            // Активируем кнопку "Выбрать"
            selectButton.disabled = false;
        }
    });

    // Обработчик кнопки "Выбрать"
    selectButton.addEventListener('click', () => {
        if (selectedAddress) {
            // Устанавливаем выбранный адрес в поле формы
            userAddressField.value = selectedAddress;

            // Закрываем модальное окно
            const addressModalInstance = bootstrap.Modal.getInstance(addressModal);
            addressModalInstance.hide();
        }
    });

    // Обработчик сохранения нового адреса с карты
    saveNewAddressButton.addEventListener('click', () => {
        const fullAddress = fullAddressInput.value.trim();
        const entrance = entranceInput.value.trim();
        const floor = floorInput.value.trim();
        const apartment = apartmentInput.value.trim();

        if (!fullAddress) {
            alert("Пожалуйста, выберите адрес на карте.");
            return;
        }

        // Формирование полного адреса с дополнительной информацией
        let detailedAddress = fullAddress;
        if (entrance) detailedAddress += `, Подъезд: ${entrance}`;
        if (floor) detailedAddress += `, Этаж: ${floor}`;
        if (apartment) detailedAddress += `, Квартира: ${apartment}`;

        // Создаём новый элемент для списка адресов
        const newItem = document.createElement('button');
        newItem.type = 'button';
        newItem.className = 'address-button btn btn-light address-item w-100 text-start mb-2';
        newItem.dataset.address = detailedAddress;
        newItem.textContent = detailedAddress;

        // Добавляем новый адрес в контейнер
        addressContainer.appendChild(newItem);

        // Закрываем модальное окно карты
        const mapModalInstance = bootstrap.Modal.getInstance(mapModal);
        mapModalInstance.hide();

        // Очищаем поля ввода
        entranceInput.value = '';
        floorInput.value = '';
        apartmentInput.value = '';
    });

    // Переход из окна карты обратно в адресное окно
    backToAddressModal.addEventListener('click', () => {
        const mapModalInstance = bootstrap.Modal.getInstance(mapModal);
        mapModalInstance.hide();

        const addressModalInstance = new bootstrap.Modal(addressModal);
        addressModalInstance.show();
    });
});

document.addEventListener('DOMContentLoaded', () => {
    const deliveryTimeInput = document.getElementById('deliveryTimeInput');
    const buttons = document.querySelectorAll('.btn-group button');

    buttons.forEach(button => {
        button.addEventListener('click', () => {
            buttons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');

            const buttonId = button.id;
            let deliveryTime;

            switch (buttonId) {
                case 'fastDelivery':
                    deliveryTime = new Date(new Date().getTime() + getRandomMinutes(20, 30) * 60000); // + 20–30 минут
                    break;
                case 'time30':
                    deliveryTime = new Date(new Date().getTime() + 30 * 60000); // + 30 минут
                    break;
                case 'time45':
                    deliveryTime = new Date(new Date().getTime() + 45 * 60000); // + 45 минут
                    break;
                default:
                    deliveryTime = null;
            }

            if (deliveryTime) {
                deliveryTimeInput.value = deliveryTime.toLocaleTimeString('ru-RU', {
                    hour: '2-digit',
                    minute: '2-digit',
                });
            } else {
                deliveryTimeInput.value = ''; // Очистить, если выбран модальный ввод
            }
        });
    });

    function getRandomMinutes(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
});


