    /**
    * create-user.js — lógica de validación en tiempo real para la vista Crear Usuario.
    * Valida: unicidad de UserName, Email, PhoneNumber y Dui contra el servidor.
    * Valida: fortaleza de contraseña y coincidencia de confirmación.
    * Bloquea el submit si hay errores de unicidad pendientes.
    */
    (function () {
        'use strict';

    /* ── Estado de unicidad por campo ─── */
    const uniqueState = {
        UserName:    {ok: null, pending: false },
    Email:       {ok: null, pending: false },
    PhoneNumber: {ok: null, pending: false },
    Dui:         {ok: null, pending: false }   // null = no validado (campo opcional)
        };

        /* ── Helpers de feedback visual ─────────────────────────── */

        /**
         * Muestra un mensaje de unicidad bajo el input.
         * @param {string} fieldName - Nombre del campo (atributo id del input).
    * @param {boolean} available - true si el valor está libre.
    * @param {string} errorMsg - Mensaje cuando no está disponible.
    */
    function setUniqueFeedback(fieldName, available, errorMsg) {
            const input    = document.getElementById(fieldName);
    const feedback = document.getElementById(fieldName + '-unique-feedback');
    if (!input) return;

    if (available) {
        input.classList.remove('is-invalid');
    input.classList.add('is-valid');
    if (feedback) {feedback.textContent = ''; feedback.style.display = 'none'; }
            } else {
        input.classList.remove('is-valid');
    input.classList.add('is-invalid');
    if (feedback) {feedback.textContent = errorMsg; feedback.style.display = 'block'; }
            }
        }

        /**
         * Restablece el estado visual de un input a neutro (sin is-valid / is-invalid).
         * @param {string} fieldName
    */
    function resetFieldState(fieldName) {
            const input    = document.getElementById(fieldName);
    const feedback = document.getElementById(fieldName + '-unique-feedback');
    if (!input) return;
    input.classList.remove('is-valid', 'is-invalid');
    if (feedback) {feedback.textContent = ''; feedback.style.display = 'none'; }
        }

    /* ── Validación de unicidad vía fetch ───────────────────── */

    const timers = { };

        /**
         * Inicia un debounce de 400 ms antes de consultar el servidor.
         * @param {HTMLInputElement} input
    */
    function scheduleUniqueCheck(input) {
            const field   = input.dataset.uniqueCheck;
    const url     = input.dataset.uniqueUrl  || '/Users/CheckUnique';
    const exclude = input.dataset.uniqueExclude || '0';
    const msg     = input.dataset.uniqueMsg  || 'Este valor ya está en uso.';

    clearTimeout(timers[field]);
    uniqueState[field].pending = true;

    const value = input.value.trim();

    // Si el campo está vacío, revertir a neutro (la validación requerida la hace MVC)
    if (!value) {
        uniqueState[field].ok = null;
    uniqueState[field].pending = false;
    resetFieldState(field);
    return;
            }

            timers[field] = setTimeout(async () => {
                try {
                    const params = new URLSearchParams({field, value, excludeId: exclude });
    const res    = await fetch(url + '?' + params.toString());
    const data   = await res.json();

    uniqueState[field].ok      = data.available;
    uniqueState[field].pending = false;
    setUniqueFeedback(field, data.available, msg);
                } catch {
        // Error de red: permitir envío
        uniqueState[field].ok = true;
    uniqueState[field].pending = false;
                }
            }, 400);
        }

    // Inicializar listeners en todos los inputs con data-unique-check
    document.querySelectorAll('[data-unique-check]').forEach(function (input) {
        input.addEventListener('input', function () { scheduleUniqueCheck(input); });
    input.addEventListener('blur',  function () {
        // Forzar verificación inmediata al salir del campo
        clearTimeout(timers[input.dataset.uniqueCheck]);
    if (input.value.trim()) scheduleUniqueCheck(input);
            });
        });

    /* ── Fortaleza de contraseña ────────────────────────────── */

    const passInput     = document.getElementById('Password');
    const strengthWrap  = document.getElementById('passwordStrength');
    const strengthBar   = document.getElementById('strengthBar');
    const strengthLabel = document.getElementById('strengthLabel');

    const strengthLevels = [
    {max: 0,  label: '',          color: '',          width: '0%'   },
    {max: 1,  label: 'Muy débil', color: '#ef4444',   width: '20%'  },
    {max: 2,  label: 'Débil',     color: '#f97316',   width: '40%'  },
    {max: 3,  label: 'Regular',   color: '#eab308',   width: '60%'  },
    {max: 4,  label: 'Fuerte',    color: '#22c55e',   width: '80%'  },
    {max: 5,  label: 'Muy fuerte',color: '#16a34a',   width: '100%' },
    ];

        /**
         * Calcula un puntaje de fortaleza de 0 a 5 para la contraseña.
         * @param {string} password
    * @returns {number}
    */
    function calcStrength(password) {
            if (!password) return 0;
    let score = 0;
            if (password.length >= 8)  score++;
            if (password.length >= 12) score++;
    if (/[A-Z]/.test(password)) score++;
    if (/[0-9]/.test(password)) score++;
    if (/[^A-Za-z0-9]/.test(password)) score++;
    return score;
        }

    if (passInput) {
        passInput.addEventListener('input', function () {
            const val = passInput.value;
            const score = calcStrength(val);

            if (!val) {
                strengthWrap.style.display = 'none';
                return;
            }

            strengthWrap.style.display = 'block';
            const level = strengthLevels[score] || strengthLevels[0];
            strengthBar.style.width = level.width;
            strengthBar.style.backgroundColor = level.color;
            strengthLabel.textContent = level.label;
            strengthLabel.style.color = level.color;

            // Actualizar también la validación de confirmación
            validatePasswordMatch();
        });
        }

    /* ── Confirmación de contraseña ─────────────────────────── */

    const confirmInput   = document.getElementById('ConfirmPassword');
    const confirmFeedback = document.getElementById('confirmMatchFeedback');

    function validatePasswordMatch() {
            if (!passInput || !confirmInput || !confirmInput.value) return;
    const match = passInput.value === confirmInput.value;
    if (match) {
        confirmInput.classList.remove('is-invalid');
    confirmInput.classList.add('is-valid');
    if (confirmFeedback) {confirmFeedback.textContent = ''; }
            } else {
        confirmInput.classList.add('is-invalid');
    confirmInput.classList.remove('is-valid');
    if (confirmFeedback) {
        confirmFeedback.textContent = 'Las contraseñas no coinciden.';
    confirmFeedback.style.display = 'block';
                }
            }
    return match;
        }

    if (confirmInput) {
        confirmInput.addEventListener('input', validatePasswordMatch);
        }

    /* ── Bloquear submit si hay errores de unicidad ─────────── */

    const form      = document.getElementById('createUserForm');
    const submitBtn = document.getElementById('submitBtn');

    if (form) {
        form.addEventListener('submit', function (e) {

            // 1. Verificar campos únicos con estado pendiente o inválido
            let hasUniqueError = false;

            Object.entries(uniqueState).forEach(function ([field, state]) {
                const input = document.getElementById(field);
                if (!input || !input.value.trim()) return; // campo vacío, MVC lo valida

                if (state.pending) {
                    // Aún esperando respuesta del servidor
                    hasUniqueError = true;
                } else if (state.ok === false) {
                    // Ya verificado: no disponible
                    hasUniqueError = true;
                }
            });

            if (hasUniqueError) {
                e.preventDefault();
                return;
            }

            // 2. Confirmar contraseñas
            if (passInput && confirmInput && passInput.value !== confirmInput.value) {
                e.preventDefault();
                validatePasswordMatch();
                return;
            }

            // 3. Deshabilitar botón para evitar doble submit
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML =
                    '<span class="spinner-border spinner-border-sm me-1" role="status"></span>Guardando…';
            }
        });
        }

    }());