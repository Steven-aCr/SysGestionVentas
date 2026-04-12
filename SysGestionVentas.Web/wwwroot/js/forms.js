/**
 * forms.js — SysGestionVentas
 * Utilidades para formularios: validación visual, password toggle,
 * confirmaciones de eliminación, feedback en tiempo real.
 */

(function () {
    'use strict';

    /* =====================================================
       1. VALIDACIÓN BOOTSTRAP
    ===================================================== */
    document.querySelectorAll('form.needs-validation').forEach(form => {
        form.addEventListener('submit', e => {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });

    /* =====================================================
       2. TOGGLE PASSWORD (GENÉRICO)
    ===================================================== */
    document.querySelectorAll('[data-password-toggle]').forEach(btn => {
        const targetId = btn.dataset.passwordToggle;
        const input = document.getElementById(targetId);
        if (!input) return;

        btn.addEventListener('click', () => {
            const isText = input.type === 'text';
            input.type = isText ? 'password' : 'text';

            const icon = btn.querySelector('.material-symbols-outlined');
            if (icon) {
                icon.textContent = isText ? 'visibility' : 'visibility_off';
            }
        });
    });

    /* =====================================================
       3. VALIDACIÓN CONFIRM PASSWORD
    ===================================================== */
    const formUser = document.getElementById("createUserForm");

    if (formUser) {
        formUser.addEventListener("submit", function (e) {

            const passInput = document.getElementById("Password");
            const confirmInput = document.getElementById("ConfirmPassword");
            const msg = document.getElementById("confirmMatchMsg");

            if (!passInput || !confirmInput || !msg) return;

            const pass = passInput.value.trim();
            const confirm = confirmInput.value.trim();

            if (pass !== confirm) {
                e.preventDefault();
                msg.style.display = "block";
                msg.textContent = "Las contraseñas no coinciden";
                confirmInput.classList.add("is-invalid");
            } else {
                msg.style.display = "none";
                confirmInput.classList.remove("is-invalid");
            }
        });
    }

    /* =====================================================
       4. CONFIRMACIONES (DELETE / ACCIONES)
    ===================================================== */
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', e => {
            const msg = el.dataset.confirm || '¿Está seguro de realizar esta acción?';
            if (!confirm(msg)) e.preventDefault();
        });
    });

    /* =====================================================
       5. ALERTAS AUTO-DISMISS
    ===================================================== */
    document.querySelectorAll('.alert[data-auto-dismiss]').forEach(alert => {
        const delay = parseInt(alert.dataset.autoDismiss) || 4000;

        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert?.close();
        }, delay);
    });

    /* =====================================================
       6. FORMATO MONEDA
    ===================================================== */
    document.querySelectorAll('input[data-type="currency"]').forEach(input => {
        input.addEventListener('blur', () => {
            const val = parseFloat(input.value);
            if (!isNaN(val)) input.value = val.toFixed(2);
        });
    });

    /* =====================================================
       7. SOLO NÚMEROS
    ===================================================== */
    document.querySelectorAll('input[data-type="integer"]').forEach(input => {
        input.addEventListener('keypress', e => {
            if (!/[\d]/.test(e.key)) e.preventDefault();
        });
    });

    /* =====================================================
       8. TOOLTIP BOOTSTRAP
    ===================================================== */
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
        new bootstrap.Tooltip(el, { trigger: 'hover' });
    });

    /* =====================================================
       9. BOTÓN SUBMIT LOADING
    ===================================================== */
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', () => {

            const submitBtn = form.querySelector('[type="submit"]');
            if (!submitBtn) return;

            submitBtn.disabled = true;

            const originalHtml = submitBtn.innerHTML;

            submitBtn.innerHTML = `
                <span class="spinner-border spinner-border-sm me-1"></span>
                Procesando…`;

            window.addEventListener('pageshow', () => {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalHtml;
            });
        });
    });

    /* =====================================================
       10. FLOATING LABEL
    ===================================================== */
    document.querySelectorAll('.form-floating input, .form-floating select').forEach(el => {

        const updateLabel = () => {
            el.closest('.form-floating')
                ?.classList.toggle('has-value', el.value.length > 0);
        };

        el.addEventListener('change', updateLabel);
        updateLabel();
    });

    /* =====================================================
       11. BUSCADOR TABLAS
    ===================================================== */
    document.querySelectorAll('[data-table]').forEach(input => {

        const tableId = input.dataset.table;
        const table = document.getElementById(tableId);
        if (!table) return;

        input.addEventListener('input', () => {
            const q = input.value.toLowerCase();

            table.querySelectorAll('tbody tr').forEach(row => {
                row.style.display =
                    row.textContent.toLowerCase().includes(q) ? '' : 'none';
            });
        });
    });

})();