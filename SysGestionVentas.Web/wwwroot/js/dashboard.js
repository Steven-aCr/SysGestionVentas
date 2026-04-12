/**
 * dashboard.js — SysGestionVentas
 * Lógica interactiva del panel de administración:
 * - Animación del gráfico de barras al cargar la página.
 * - Actualización de la fecha/hora en el header.
 * - Animación de contadores KPI.
 */
(function () {
    'use strict';

    /* ── Fecha y hora en tiempo real ────────────────────── */
    const dateEl = document.getElementById('dashDate');

    /**
     * Formatea y muestra la fecha/hora actual en el elemento del header.
     */
    function updateDate() {
        if (!dateEl) return;
        const now = new Date();
        const opts = {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        };
        dateEl.textContent = now.toLocaleDateString('es-ES', opts);
    }

    updateDate();

    /* ── Animación del gráfico de barras ────────────────── */

    /**
     * Anima las barras del gráfico desde 0% hasta su valor real
     * usando la propiedad data-height del elemento.
     * Se ejecuta con un pequeño retraso escalonado por columna.
     */
    function animateBars() {
        const bars = document.querySelectorAll('.bar-chart__bar');
        bars.forEach((bar, index) => {
            const targetHeight = parseFloat(bar.dataset.height) || 0;
            setTimeout(() => {
                bar.style.height = targetHeight + '%';
            }, 100 + index * 80);
        });
    }

    // Ejecutar animación cuando el DOM esté listo
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', animateBars);
    } else {
        animateBars();
    }

    /* ── Animación de contadores KPI ────────────────────── */

    /**
     * Anima un número desde 0 hasta su valor final.
     * @param {HTMLElement} el - Elemento que contiene el número a animar.
     * @param {number} target - Valor objetivo.
     * @param {number} duration - Duración de la animación en ms.
     * @param {string} prefix - Prefijo antes del número (ej: "$").
     * @param {string} suffix - Sufijo después del número (ej: "%").
     * @param {boolean} isDecimal - Si true, formatea con 2 decimales.
     */
    function animateCounter(el, target, duration, prefix = '', suffix = '', isDecimal = false) {
        if (!el) return;
        const start = performance.now();

        function update(timestamp) {
            const elapsed = timestamp - start;
            const progress = Math.min(elapsed / duration, 1);
            // Easing: ease-out cubic
            const eased = 1 - Math.pow(1 - progress, 3);
            const current = target * eased;

            el.textContent = prefix + (isDecimal
                ? current.toLocaleString('es-SV', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
                : Math.floor(current).toLocaleString('es-SV')) + suffix;

            if (progress < 1) requestAnimationFrame(update);
        }

        requestAnimationFrame(update);
    }

    /**
     * Inicializa todos los contadores KPI encontrados en la página
     * leyendo el valor actual del elemento y animando desde 0 hasta él.
     */
    function initCounters() {
        document.querySelectorAll('.kpi-card__value').forEach(el => {
            const text = el.textContent.trim();
            const isDecimal = text.includes('.');
            const prefix = text.startsWith('$') ? '$' : '';
            const cleanText = text.replace(/[$,]/g, '').trim();
            const target = parseFloat(cleanText);

            if (!isNaN(target) && target > 0) {
                animateCounter(el, target, 900, prefix, '', isDecimal);
            }
        });
    }

    // Pequeño delay para que las animaciones sean visibles después del render
    setTimeout(initCounters, 200);

    /* ── Tooltips en barras del gráfico ─────────────────── */

    /**
     * Los tooltips de las barras ya están manejados por CSS (:hover),
     * pero aquí añadimos soporte táctil para dispositivos móviles.
     */
    document.querySelectorAll('.bar-chart__bar').forEach(bar => {
        bar.addEventListener('touchstart', function (e) {
            e.preventDefault();
            document.querySelectorAll('.bar-chart__bar-tooltip').forEach(t => {
                t.style.display = 'none';
            });
            const tooltip = bar.querySelector('.bar-chart__bar-tooltip');
            if (tooltip) tooltip.style.display = 'block';
        }, { passive: false });
    });

    document.addEventListener('touchstart', function (e) {
        if (!e.target.closest('.bar-chart__bar')) {
            document.querySelectorAll('.bar-chart__bar-tooltip').forEach(t => {
                t.style.display = 'none';
            });
        }
    });

    /* ── Actualización automática cada 5 minutos ─────────── */

    /**
     * Recarga la página silenciosamente cada 5 minutos
     * para mantener los datos del dashboard actualizados.
     * Solo se ejecuta si la ventana está en foco.
     */
    let refreshTimer = null;

    function startRefreshTimer() {
        refreshTimer = setTimeout(() => {
            if (document.hasFocus()) {
                window.location.reload();
            } else {
                // Reintentar en 1 minuto si la ventana no está en foco
                startRefreshTimer();
            }
        }, 5 * 60 * 1000); // 5 minutos
    }

    startRefreshTimer();

    // Limpiar timer si el usuario abandona la página
    window.addEventListener('beforeunload', () => {
        if (refreshTimer) clearTimeout(refreshTimer);
    });

})();