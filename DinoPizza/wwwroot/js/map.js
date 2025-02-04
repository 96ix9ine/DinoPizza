// Глобальные переменные
let myMap, myPlacemark;
let isMoving = false;  // Флаг для отслеживания состояния перемещения

function initMap() {
    // Инициализация карты с центром на Челябинск по умолчанию
    myMap = new ymaps.Map('map', {
        center: [55.1603, 61.4044], // Челябинск
        zoom: 12, // Уровень масштабирования
    });

    // Создаем маркер в центре Челябинска
    myPlacemark = new ymaps.Placemark(
        myMap.getCenter(),
        {},
        {
            draggable: true, // Маркер можно перемещать
            iconOffset: [0, 0], // Сдвиг иконки по центру
        }
    );

    // Добавляем маркер на карту
    myMap.geoObjects.add(myPlacemark);

    // Слушатель на перемещение карты
    myMap.events.add("move", function () {
        if (!isMoving) {  // Мы не обновляем адрес, пока перемещение не завершится
            return;
        }

        // Устанавливаем маркер в центр карты
        const centerCoords = myMap.getCenter();
        myPlacemark.geometry.setCoordinates(centerCoords);
    });

    // Отслеживаем все действия перемещения карты
    myMap.events.add('actiontick', function (e) {
        var tick = e.get('tick');
        // Получаем координаты глобального центра карты
        const centerCoords = myMap.options.get('projection').fromGlobalPixels(tick.globalPixelCenter, myMap.getZoom());
        // Обновляем маркер на эти координаты
        myPlacemark.geometry.setCoordinates(centerCoords);
    });

    // Слушатель на перемещение маркера
    myPlacemark.events.add("dragstart", function () {
        isMoving = true;  // Начинаем перемещение
    });

    myPlacemark.events.add("dragend", function () {
        isMoving = false;  // Перемещение завершено
        const coords = myPlacemark.geometry.getCoordinates();
        updateAddress(coords);  // Обновляем адрес
    });

    // Слушатель на завершение перемещения карты
    myMap.events.add("actionend", function () {
        isMoving = false;  // Перемещение завершено
        const centerCoords = myMap.getCenter();
        updateAddress(centerCoords);  // Обновляем адрес
    });

    // Инициализация адреса по умолчанию
    updateAddress(myMap.getCenter());

    // Обработчик на кнопку "Определить местоположение"
    document.getElementById("findLocationBtn").addEventListener("click", function (event) {
        event.preventDefault(); // Предотвращаем перезагрузку страницы

        // Запрашиваем геолокацию
        ymaps.geolocation.get({
            provider: 'browser', // Используем геолокацию через браузер
            mapStateAutoApply: true
        }).then(function (result) {
            // Получаем координаты местоположения пользователя
            const userCoords = result.geoObjects.get(0).geometry.getCoordinates();

            // Перемещаем карту на местоположение пользователя
            myMap.setCenter(userCoords, 14, {
                checkZoomRange: true,
                duration: 1000 // Плавное перемещение
            });

            // Обновляем маркер на новое местоположение
            myPlacemark.geometry.setCoordinates(userCoords);

            // Обновляем адрес на основе местоположения пользователя
            updateAddress(userCoords);
        }, function () {
            alert("Не удалось получить ваше местоположение.");
        });
    });
}

// Функция для получения адреса
function updateAddress(coords) {
    ymaps.geocode(coords).then((res) => {
        const firstGeoObject = res.geoObjects.get(0);

        if (firstGeoObject) {
            // Получение полного адреса
            const address = firstGeoObject.getAddressLine();

            // Запись в поле полного адреса
            document.getElementById("fullAddress").value = address;

            // Полный адрес (для отладки)
            console.log("Полный адрес:", address);
        } else {
            alert("Не удалось получить адрес. Попробуйте снова.");
        }
    });
}

// Инициализация карты после загрузки API
ymaps.ready(initMap);
