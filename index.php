<!DOCTYPE html>
<html lang="pl">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>kodOD</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;700&display=swap');

        :root {
            --bg-color: #0a0a0a;
            --text-color: #e0e0e0;
            --accent-color: #ff3c00;
            --secondary-color: #1a1a1a;
        }

        body, html {
            margin: 0;
            padding: 0;
            height: 100%;
            font-family: 'Orbitron', sans-serif;
            background-color: var(--bg-color);
            color: var(--text-color);
            overflow: hidden;
        }

        .container {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            height: 100%;
            background: radial-gradient(circle at center, #1a1a1a 0%, #0a0a0a 100%);
            position: relative;
        }

        .glitch-effect {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADIAAAAyCAYAAAAeP4ixAAAEJGlDQ1BJQ0MgUHJvZmlsZQAAOBGFVd9v21QUPolvUqQWPyBYR4eKxa9VU1u5GxqtxgZJk6XtShal6dgqJOQ6N4mpGwfb6baqT3uBNwb8AUDZAw9IPCENBmJ72fbAtElThyqqSUh76MQPISbtBVXhu3ZiJ1PEXPX6yznfOec7517bRD1fabWaGVWIlquunc8klZOnFpSeTYrSs9RLA9Sr6U4tkcvNEi7BFffO6+EdigjL7ZHu/k72I796i9zRiSJPwG4VHX0Z+AxRzNRrtksUvwf7+Gm3BtzzHPDTNgQCqwKXfZwSeNHHJz1OIT8JjtAq6xWtCLwGPLzYZi+3YV8DGMiT4VVuG7oiZpGzrZJhcs/hL49xtzH/Dy6bdfTsXYNY+5yluWO4D4neK/ZUvok/17X0HPBLsF+vuUlhfwX4j/rSfAJ4H1H0qZJ9dN7nR19frRTeBt4Fe9FwpwtN+2p1MXscGLHR9SXrmMgjONd1ZxKzpBeA71b4tNhj6JGoyFNp4GHgwUp9qplfmnFW5oTdy7NamcwCI49kv6fN5IAHgD+0rbyoBc3SOjczohbyS1drbq6pQdqumllRC/0ymTtej8gpbbuVwpQfyw66dqEZyxZKxtHpJn+tZnpnEdrYBbueF9qQn93S7HQGGHnYP7w6L+YGHNtd1FJitqPAR+hERCNOFi1i1alKO6RQnjKUxL1GNjwlMsiEhcPLYTEiT9ISbN15OY/jx4SMshe9LaJRpTvHr3C/ybFYP1PZAfwfYrPsMBtnE6SwN9ib7AhLwTrBDgUKcm06FSrTfSj187xPdVQWOk5Q8vxAfSiIUc7Z7xr6zY/+hpqwSyv0I0/QMTRb7RMgBxNodTfSPqdraz/sDjzKBrv4zu2+a2t0/HHzjd2Lbcc2sG7GtsL42K+xLfxtUgI7YHqKlqHK8HbCCXgjHT1cAdMlDetv4FnQ2lLasaOl6vmB0CMmwT/IPszSueHQqv6i/qluqF+oF9TfO2qEGTumJH0qfSv9KH0nfS/9TIp0Wboi/SRdlb6RLgU5u++9nyXYe69fYRPdil1o1WufNSdTTsp75BfllPy8/LI8G7AUuV8ek6fkvfDsCfbNDP0dvRh0CrNqTbV7LfEEGDQPJQadBtfGVMWEq3QWWdufk6ZSNsjG2PQjp3ZcnOWWing6noonSInvi0/Ex+IzAreevPhe+CawpgP1/pMTMDo64G0sTCXIM+KdOnFWRfQKdJvQzV1+Bt8OokmrdtY2yhVX2a+qrykJfMq4Ml3VR4cVzTQVz+UoNne4vcKLoyS+gyKO6EHe+75Fdt0Mbe5bRIf/wjvrVmhbqBN97RD1vxrahvBOfOYzoosH9bq94uejSOQGkVM6sN/7HelL4t10t9F4gPdVzydEOx83Gv+uNxo7XyL/FtFl8z9ZAHF4bBsrEwAAAAlwSFlzAAALEwAACxMBAJqcGAAAAsBJREFUaAXtms1LVFEYh8evNEQlxLJduMmgRRG0yo2IuFFaFBT9CS36A1oHUQqRi2jXIqIG2khY0SZw0a4sKnQ5lZArtTQr8/F7Zu6tuTNzz9x7Z7iBc+Hx3ve+73ng/aBz7txJpZIlCaQkkIgCbYlkOUt0Op32WCXdQ7XjzK/BAfBBPBeLxU9aV7EixZPtJXmOkpMRm93qEH+B7mQymSnmqW1SvCK34yRsZ9JHORke+zdq1XmL/G2Qy+VuEvwQXAKHwFEQ2p8Uf4HJJkZDrOWWG/wuOAYOg8CWz+cvV9WEJIrrmJ0HYucm8+dI8FNuG9/SN62rOJE2EhuMJU5sj9qQopbkQUZJ5oVPUgstQzb3QbWlwNfgYBQ3dK2iqsYYeA1c1g9ks1mz9rRKBCvN+EeQCpsMa5P2MXgJZlhNGTOp8E0OY58Au7+qhD28AffHBRZfIkrJsXwFl6oCW6LRmcD2IXgNRhOJRCNJwR/HjwKXPZYRt5y2YRXm6RgGNWe3KpKYkjFXwAFQswkJkr4Hb8FEfX19JJ/FEvkOcz+J7AFVZiuJ/AG3wG0SyURxPcOp+woMANv0IwuqxK0odtYlhZeZdFjO3GljliWxngDdrlGXGJ8CHaAJhbYXoA+MGWmvTqRcrewD3eAkaKRpkqfgIhNeSYQ6kRDgDqMncImhzWXmV6L6rH7F+0NafiSrEtHCtqfh8qZeBlFJ6Jz1+0K8CL6Bj+A5eA8+g1nwC0QyLQGrRBZJ5jDoAxfA/9gdZj0ABvF+rBWNvNUEgk7NITpugavgHDAykuoF+rkUPBM5gneAmzj16XQ6FcW6KeQeGMR/aIX9+0Scf+F2sj89Dxt1XaJedyIk0cWF7SHPVbVL8hMo0fkG41U2VFV5VuyCOsLzfbCbeUUbZKOeylZJ1pj0CvDt09cAe9nz3gqLOoYkkCSQJJAkkCSQJLApEvgHWMgqoXixJc8AAAAASUVORK5CYII=');
            opacity: 0.05;
            pointer-events: none;
        }

        h1 {
            font-size: 3rem;
            margin-bottom: 2rem;
            text-transform: uppercase;
            letter-spacing: 0.5rem;
            text-shadow: 0 0 10px var(--accent-color);
            animation: glow 2s ease-in-out infinite alternate;
        }

        @keyframes glow {
            from {
                text-shadow: 0 0 5px var(--accent-color);
            }
            to {
                text-shadow: 0 0 20px var(--accent-color), 0 0 30px var(--accent-color);
            }
        }

        #countdown {
            display: flex;
            justify-content: center;
            gap: 1.5rem;
        }

        .time-unit {
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        .time-value {
            font-size: 4rem;
            font-weight: bold;
            background-color: var(--secondary-color);
            color: var(--accent-color);
            padding: 1rem;
            border-radius: 10px;
            min-width: 100px;
            text-align: center;
            box-shadow: 0 0 15px rgba(255, 60, 0, 0.3);
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
        }

        .time-value::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: linear-gradient(
                to bottom right,
                rgba(255, 60, 0, 0.3) 0%,
                rgba(255, 60, 0, 0) 100%
            );
            transform: rotate(45deg);
            transition: all 0.3s ease;
        }

        .time-value:hover::before {
            top: -75%;
            left: -75%;
        }

        .time-label {
            font-size: 1rem;
            margin-top: 0.5rem;
            text-transform: uppercase;
            letter-spacing: 0.2rem;
            color: var(--text-color);
        }

        @media (max-width: 768px) {
            h1 {
                font-size: 2rem;
            }
            #countdown {
                flex-wrap: wrap;
            }
            .time-value {
                font-size: 3rem;
                min-width: 80px;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="glitch-effect"></div>
        <h1>kodOD</h1>
        <div id="countdown">
            <div class="time-unit">
                <div id="days" class="time-value">00</div>
                <div class="time-label">Dni</div>
            </div>
            <div class="time-unit">
                <div id="hours" class="time-value">00</div>
                <div class="time-label">Godzin</div>
            </div>
            <div class="time-unit">
                <div id="minutes" class="time-value">00</div>
                <div class="time-label">Minut</div>
            </div>
            <div class="time-unit">
                <div id="seconds" class="time-value">00</div>
                <div class="time-label">Sekund</div>
            </div>
        </div>
    </div>
    <script>
        // Ustaw datê docelow¹ (rok, miesi¹c (0-11), dzieñ, godzina, minuta, sekunda)
        const targetDate = new Date(2025, 9, 16, 16, 16, 16).getTime();

        function updateCountdown() {
            const now = new Date().getTime();
            const distance = targetDate - now;

            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            updateTimeUnit('days', days);
            updateTimeUnit('hours', hours);
            updateTimeUnit('minutes', minutes);
            updateTimeUnit('seconds', seconds);

            if (distance < 0) {
                clearInterval(x);
                document.querySelector('.container').innerHTML = "<h1>Czas min¹³!</h1>";
            }
        }

function updateTimeUnit(id, value) {
            const element = document.getElementById(id);
            const formattedValue = value.toString().padStart(2, '0');
            if (element.textContent !== formattedValue) {
                element.textContent = formattedValue;
                element.style.transform = 'scale(1.1)';
                setTimeout(() => {
                    element.style.transform = 'scale(1)';
                }, 100);
                glitchEffect(element);
            }
        }

        function glitchEffect(element) {
            const glitchDuration = 500;
            const glitchInterval = 50;
            const originalText = element.textContent;
            const glitchChars = '!<>-_\\/[]{}—=+*^?#________';

            let glitchCount = 0;
            const glitchIntervalId = setInterval(() => {
                element.textContent = originalText.split('').map((char, index) => {
                    if (Math.random() < 0.1) {
                        return glitchChars[Math.floor(Math.random() * glitchChars.length)];
                    }
                    return char;
                }).join('');

                glitchCount++;
                if (glitchCount > glitchDuration / glitchInterval) {
                    clearInterval(glitchIntervalId);
                    element.textContent = originalText;
                }
            }, glitchInterval);
        }

        // Aktualizuj odliczanie co sekundê
        const x = setInterval(updateCountdown, 1000);

        // Uruchom od razu, aby unikn¹æ opóŸnienia przy pierwszym za³adowaniu
        updateCountdown();
    </script>
</body>
</html>