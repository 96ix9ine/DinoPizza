document.addEventListener('DOMContentLoaded', () => {
    const canvas = document.getElementById('snow-canvas');
    const ctx = canvas.getContext('2d');

    // Установка размеров canvas
    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }

    window.addEventListener('resize', resizeCanvas);
    resizeCanvas();

    // Настройки снежинок
    const snowflakes = [];
    const snowflakeCount = 120; // Количество снежинок

    function createSnowflake() {
        return {
            x: Math.random() * canvas.width, // Горизонтальное положение
            y: Math.random() * canvas.height, // Вертикальное положение
            radius: Math.random() * 2 + 1, // Радиус
            opacity: Math.random() * 0.3 + 0.2, // Прозрачность
            speedY: Math.random() * 1 + 0.5, // Скорость падения
            speedX: Math.random() * 0.5 - 0.25, // Горизонтальное движение
        };
    }

    function updateSnowflakes() {
        for (let flake of snowflakes) {
            flake.y += flake.speedY;
            flake.x += flake.speedX;

            // Перемещаем снежинки обратно наверх, если они выходят за экран
            if (flake.y > canvas.height) {
                flake.y = -flake.radius;
                flake.x = Math.random() * canvas.width;
            }
            if (flake.x > canvas.width) {
                flake.x = -flake.radius;
            }
            if (flake.x < -flake.radius) {
                flake.x = canvas.width;
            }
        }
    }

    function drawSnowflakes() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        for (let flake of snowflakes) {
            ctx.beginPath();
            ctx.arc(flake.x, flake.y, flake.radius, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(255, 255, 255, ${flake.opacity})`; // Белые снежинки
            ctx.fill();
        }
    }

    function loop() {
        updateSnowflakes();
        drawSnowflakes();
        requestAnimationFrame(loop);
    }

    // Инициализация снежинок
    for (let i = 0; i < snowflakeCount; i++) {
        snowflakes.push(createSnowflake());
    }

    loop(); // Запуск анимации

    // Убедитесь, что канвас видим
    const animatedBg = document.getElementById('animated-bg');
    if (animatedBg) {
        animatedBg.classList.remove('hidden'); // Убедитесь, что фон (включая канвас) видим
    }
});