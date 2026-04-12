/**
 * site.js — SysGestionVentas
 * Funciones globales y utilitarias del sistema.
 */
(function () {
    'use strict';

    /* ── Flash messages con auto-dismiss ────────────────── */
    document.querySelectorAll('.alert:not([data-auto-dismiss])').forEach(alert => {
        // Agregar botón de cierre si no tiene
        if (!alert.querySelector('.btn-close')) {
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btn-close btn-close-sm ms-auto';
            btn.setAttribute('data-bs-dismiss', 'alert');
            btn.setAttribute('aria-label', 'Cerrar');
            alert.appendChild(btn);
            alert.classList.add('alert-dismissible', 'fade', 'show');
        }

        // Auto-dismiss después de 5s
        setTimeout(() => {
            try {
                bootstrap.Alert.getOrCreateInstance(alert)?.close();
            } catch (_) { }
        }, 5000);
    });

    /* ── Global TempData fade-in ─────────────────────────── */
    document.querySelectorAll('.alert').forEach((el, i) => {
        el.style.animationDelay = `${i * .08}s`;
        el.classList.add('alert-animate');
    });

    /* ── Número de registros en tabla ────────────────────── */
    const countEl = document.querySelector('[data-record-count]');
    if (countEl) {
        const table = document.querySelector(countEl.dataset.recordCount);
        if (table) {
            const rows = table.querySelectorAll('tbody tr').length;
            countEl.textContent = `${rows} registro${rows !== 1 ? 's' : ''}`;
        }
    }

    /* ── Copy to clipboard ───────────────────────────────── */
    document.querySelectorAll('[data-copy]').forEach(btn => {
        btn.addEventListener('click', () => {
            const text = btn.dataset.copy;
            navigator.clipboard?.writeText(text).then(() => {
                const icon = btn.querySelector('.material-symbols-outlined');
                if (icon) {
                    const orig = icon.textContent;
                    icon.textContent = 'check';
                    setTimeout(() => { icon.textContent = orig; }, 1500);
                }
            });
        });
    });

    /* ── Print button ────────────────────────────────────── */
    document.querySelectorAll('[data-print]').forEach(btn => {
        btn.addEventListener('click', () => window.print());
    });

    /* ── Back button ─────────────────────────────────────── */
    document.querySelectorAll('[data-go-back]').forEach(btn => {
        btn.addEventListener('click', () => history.back());
    });

    /* ── Format: moneda en spans ─────────────────────────── */
    document.querySelectorAll('[data-format="currency"]').forEach(el => {
        const val = parseFloat(el.textContent.replace(/[^0-9.-]/g, ''));
        if (!isNaN(val)) {
            el.textContent = new Intl.NumberFormat('es-SV', {
                style: 'currency',
                currency: 'USD'
            }).format(val);
        }
    });

    /* ── Confirm antes de salir con cambios sin guardar ──── */
    let formDirty = false;
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('change', () => { formDirty = true; });
        form.addEventListener('submit', () => { formDirty = false; });
    });
    window.addEventListener('beforeunload', e => {
        if (formDirty) {
            e.preventDefault();
            e.returnValue = '';
        }
    });

})();