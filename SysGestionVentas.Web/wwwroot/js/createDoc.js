document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    /* ══════════════════════════════════════════════
       ESTADO EN MEMORIA
    ══════════════════════════════════════════════ */
    var lines = [];
    var currentStock = 0;

    /* ══════════════════════════════════════════════
       INICIALIZAR MODAL BOOTSTRAP 5
       Se usa getOrCreateInstance para ser idempotente.
    ══════════════════════════════════════════════ */
    var modalEl = document.getElementById('modalProducto');
    var bsModal = null;

    if (modalEl) {
        bsModal = bootstrap.Modal.getOrCreateInstance(modalEl, {
            backdrop: 'static',
            keyboard: false
        });
    }

    /* ══════════════════════════════════════════════
       REFERENCIAS DOM — todos los IDs del template
    ══════════════════════════════════════════════ */
    var btnAgregar = document.getElementById('btnAgregarLinea');
    var btnConfirmar = document.getElementById('btnConfirmarLinea');
    var btnCerrar = document.getElementById('btnCerrarModal');
    var btnCancelar = document.getElementById('btnCancelarModal');
    var linesBody = document.getElementById('linesBody');
    var linesContainer = document.getElementById('linesContainer');
    var emptyLines = document.getElementById('emptyLines');
    var hiddenInputs = document.getElementById('hiddenInputs');
    var grandTotal = document.getElementById('grandTotal');
    var submitBtn = document.getElementById('submitBtn');
    var documentForm = document.getElementById('documentForm');

    var selProduct = document.getElementById('modal_ProductId');
    var inpQty = document.getElementById('modal_Quantity');
    var inpPrice = document.getElementById('modal_UnitPrice');
    var inpDiscount = document.getElementById('modal_Discount');
    var inpTax = document.getElementById('modal_Tax');
    var inpTotal = document.getElementById('modal_LineTotal');
    var inpNotes = document.getElementById('modal_Notes');
    var stockInfo = document.getElementById('stockInfo');
    var stockBadge = document.getElementById('stockBadge');
    var stockValue = document.getElementById('stockValue');
    var stockError = document.getElementById('stockError');

    /* ── Guardia: abortar si algún elemento crítico falta ── */
    if (!btnAgregar || !modalEl || !documentForm) {
        console.error('[Documents/Create] Elementos DOM requeridos no encontrados.');
        return;
    }

    /* ══════════════════════════════════════════════
       UTILIDADES
    ══════════════════════════════════════════════ */

    /** Formatea un número como moneda: $X.XX */
    function fmt(v) {
        return '$' + parseFloat(v || 0).toFixed(2);
    }

    /** Muestra un elemento (block). */
    function show(el) { if (el) el.style.display = 'block'; }

    /** Oculta un elemento. */
    function hide(el) { if (el) el.style.display = 'none'; }

    /** Escapa texto para inserción segura en innerHTML. */
    function escHtml(str) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(str || ''));
        return div.innerHTML;
    }

    /* ══════════════════════════════════════════════
       CÁLCULO DE TOTALES DE LÍNEA
    ══════════════════════════════════════════════ */

    /**
     * Recalcula el total de la línea activa en el modal
     * a partir de los valores actuales de cantidad, precio,
     * descuento e IVA, y actualiza el banner de total.
     */
    function calcLineTotal() {
        var qty = parseInt(inpQty.value, 10) || 0;
        var price = parseFloat(inpPrice.value) || 0;
        var discount = parseFloat(inpDiscount.value) || 0;
        var tax = parseFloat(inpTax.value) || 0;
        var subtotal = Math.max((qty * price) - discount, 0);
        var total = subtotal + subtotal * (tax / 100);
        inpTotal.textContent = total.toFixed(2);
    }

    /* Recalcular en tiempo real */
    [inpQty, inpDiscount, inpTax].forEach(function (el) {
        if (el) el.addEventListener('input', calcLineTotal);
    });

    /* ══════════════════════════════════════════════
       AUTOCOMPLETADO AL SELECCIONAR PRODUCTO
    ══════════════════════════════════════════════ */

    /**
     * Consulta el endpoint /Documents/GetProductInfo para obtener
     * el precio de venta y el stock disponible del producto seleccionado.
     * Actualiza el campo de precio (readonly) y la insignia de stock.
     */
    if (selProduct) {
        selProduct.addEventListener('change', function () {
            var productId = parseInt(this.value, 10) || 0;

            /* Limpiar estado anterior */
            inpPrice.value = '';
            inpTotal.textContent = '0.00';
            currentStock = 0;
            hide(stockInfo);
            hide(stockError);

            if (!productId) return;

            fetch('/Documents/GetProductInfo?productId=' + productId)
                .then(function (res) {
                    if (!res.ok) throw new Error('HTTP ' + res.status);
                    return res.json();
                })
                .then(function (data) {
                    if (!data.success) {
                        stockError.textContent = data.message ||
                            'No se pudo obtener información del producto.';
                        show(stockError);
                        return;
                    }

                    inpPrice.value = parseFloat(data.salePrice).toFixed(2);
                    currentStock = data.currentStock;
                    stockValue.textContent = data.currentStock;

                    /* Colorear la insignia según el nivel de stock */
                    stockBadge.className = 'stock-badge ' +
                        (data.currentStock <= 0 ? 'stock-badge--zero' :
                            data.currentStock <= 5 ? 'stock-badge--low' :
                                'stock-badge--ok');
                    show(stockInfo);
                    calcLineTotal();
                })
                .catch(function (err) {
                    stockError.textContent = 'Error de conexión al obtener el producto.';
                    show(stockError);
                    console.error('[Documents/Create] GetProductInfo:', err);
                });
        });
    }

    /* ══════════════════════════════════════════════
       ABRIR MODAL
    ══════════════════════════════════════════════ */

    /**
     * Limpia el estado del modal y lo muestra.
     * Se vincula al botón "Agregar Producto".
     */
    btnAgregar.addEventListener('click', function () {
        /* Reset del formulario interno del modal */
        if (selProduct) { selProduct.value = ''; }
        if (inpQty) { inpQty.value = '1'; }
        if (inpPrice) { inpPrice.value = ''; }
        if (inpDiscount) { inpDiscount.value = '0'; }
        if (inpTax) { inpTax.value = '13'; }
        if (inpTotal) { inpTotal.textContent = '0.00'; }
        if (inpNotes) { inpNotes.value = ''; }
        currentStock = 0;
        hide(stockInfo);
        hide(stockError);

        if (bsModal) bsModal.show();
    });

    /* ══════════════════════════════════════════════
       CERRAR MODAL (botones ✕ y Cancelar)
    ══════════════════════════════════════════════ */

    [btnCerrar, btnCancelar].forEach(function (btn) {
        if (btn) {
            btn.addEventListener('click', function () {
                if (bsModal) bsModal.hide();
            });
        }
    });

    /* ══════════════════════════════════════════════
       CONFIRMAR LÍNEA
    ══════════════════════════════════════════════ */

    /**
     * Valida los datos del modal, construye el objeto de línea
     * y lo agrega al array lines[]. Luego re-renderiza la tabla
     * y cierra el modal.
     */
    if (btnConfirmar) {
        btnConfirmar.addEventListener('click', function () {
            var productId = parseInt(selProduct ? selProduct.value : '0', 10) || 0;
            var productName = selProduct
                ? (selProduct.options[selProduct.selectedIndex] &&
                    selProduct.options[selProduct.selectedIndex].text
                    ? selProduct.options[selProduct.selectedIndex].text.trim()
                    : '')
                : '';
            var qty = parseInt(inpQty ? inpQty.value : '0', 10) || 0;
            var price = parseFloat(inpPrice ? inpPrice.value : '0') || 0;
            var discount = parseFloat(inpDiscount ? inpDiscount.value : '0') || 0;
            var tax = parseFloat(inpTax ? inpTax.value : '0') || 0;
            var notes = inpNotes ? inpNotes.value.trim() : '';

            /* Limpiar error previo */
            hide(stockError);

            /* ── Validaciones ──────────────────────────── */
            if (!productId) {
                stockError.textContent = 'Debe seleccionar un producto.';
                show(stockError);
                if (selProduct) selProduct.focus();
                return;
            }

            if (qty < 1) {
                stockError.textContent = 'La cantidad debe ser mayor a 0.';
                show(stockError);
                if (inpQty) inpQty.focus();
                return;
            }

            if (price <= 0) {
                stockError.textContent = 'El precio unitario no está disponible. Seleccione el producto nuevamente.';
                show(stockError);
                return;
            }

            /* ── Calcular totales de línea ─────────────── */
            var subtotal = Math.max((qty * price) - discount, 0);
            var lineTotal = subtotal + subtotal * (tax / 100);

            /* ── Agregar al array y re-renderizar ──────── */
            lines.push({
                productId: productId,
                productName: productName,
                qty: qty,
                price: price,
                discount: discount,
                tax: tax,
                lineTotal: lineTotal,
                notes: notes
            });

            renderLines();
            if (bsModal) bsModal.hide();
        });
    }

    /* ══════════════════════════════════════════════
       RENDERIZAR TABLA DE LÍNEAS
    ══════════════════════════════════════════════ */

    /**
     * Reconstruye la tabla de líneas de detalle y los inputs hidden
     * para el model binding de ASP.NET MVC a partir del array lines[].
     * Gestiona también la visibilidad del estado vacío.
     */
    function renderLines() {
        if (!linesBody || !hiddenInputs) return;

        linesBody.innerHTML = '';
        hiddenInputs.innerHTML = '';

        if (lines.length === 0) {
            hide(linesContainer);
            show(emptyLines);
            updateGrandTotal();
            return;
        }

        show(linesContainer);
        hide(emptyLines);

        lines.forEach(function (line, i) {
            var tr = document.createElement('tr');
            tr.innerHTML =
                '<td>' +
                '<span style="font-weight:600;font-size:.875rem;">' +
                escHtml(line.productName) +
                '</span>' +
                '</td>' +
                '<td>' + line.qty + '</td>' +
                '<td>' + fmt(line.price) + '</td>' +
                '<td>' + fmt(line.discount) + '</td>' +
                '<td>' + parseFloat(line.tax).toFixed(0) + '%</td>' +
                '<td style="font-weight:700;color:var(--clr-primary);">' +
                fmt(line.lineTotal) +
                '</td>' +
                '<td style="text-align:right;">' +
                '<button type="button"' +
                ' class="btn btn-icon btn-danger-outline"' +
                ' data-remove-idx="' + i + '"' +
                ' title="Eliminar línea">' +
                '<span class="material-symbols-outlined"' +
                ' style="font-size:17px;pointer-events:none;">' +
                'delete' +
                '</span>' +
                '</button>' +
                '</td>';

            linesBody.appendChild(tr);

            /* Inputs hidden para ASP.NET model binding → Detalles[i].* */
            addHidden('Detalles[' + i + '].ProductId', line.productId);
            addHidden('Detalles[' + i + '].ProductName', line.productName);
            addHidden('Detalles[' + i + '].Quantity', line.qty);
            addHidden('Detalles[' + i + '].UnitPrice', line.price.toFixed(2));
            addHidden('Detalles[' + i + '].DiscountAmount', line.discount.toFixed(2));
            addHidden('Detalles[' + i + '].TaxPercentage', line.tax.toFixed(2));
            addHidden('Detalles[' + i + '].Notes', line.notes);
        });

        /* Delegación de eventos para botones "Eliminar línea" */
        linesBody.querySelectorAll('[data-remove-idx]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var idx = parseInt(this.getAttribute('data-remove-idx'), 10);
                lines.splice(idx, 1);
                renderLines();
            });
        });

        updateGrandTotal();
    }

    /* ══════════════════════════════════════════════
       HELPERS
    ══════════════════════════════════════════════ */

    function addHidden(name, value) {
        var inp = document.createElement('input');
        inp.type = 'hidden';
        inp.name = name;
        inp.value = (value !== undefined && value !== null) ? String(value) : '';
        hiddenInputs.appendChild(inp);
    }

    /**
     * Suma los totales de todas las líneas y actualiza el elemento
     * #grandTotal en el footer de la tabla.
     */
    function updateGrandTotal() {
        var total = lines.reduce(function (sum, l) { return sum + l.lineTotal; }, 0);
        if (grandTotal) grandTotal.textContent = fmt(total);
    }

    /* ══════════════════════════════════════════════
       VALIDACIÓN AL ENVIAR EL FORMULARIO
    ══════════════════════════════════════════════ */

    /**
     * Bloquea el submit si no hay líneas de detalle.
     * Desactiva el botón tras la validación exitosa para evitar
     * dobles envíos.
     */
    if (documentForm) {
        documentForm.addEventListener('submit', function (e) {
            if (lines.length === 0) {
                e.preventDefault();
                /* Mostrar alerta nativa Bootstrap 5 */
                var alertEl = document.createElement('div');
                alertEl.className = 'alert-custom alert-danger-custom mb-3';
                alertEl.innerHTML =
                    '<span class="material-symbols-outlined" style="font-size:18px;flex-shrink:0;">error</span>' +
                    '<span>Debe agregar al menos un producto antes de guardar el documento.</span>';

                var linesCardHeader = document.querySelector('.lines-card__header');
                if (linesCardHeader && linesCardHeader.parentNode) {
                    linesCardHeader.parentNode.insertBefore(alertEl, linesCardHeader.nextSibling);
                    setTimeout(function () { alertEl.remove(); }, 5000);
                }

                if (btnAgregar) btnAgregar.focus();
                return;
            }

            /* Deshabilitar botón para prevenir doble submit */
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML =
                    '<span class="spinner-border spinner-border-sm me-1"' +
                    ' role="status" aria-hidden="true"></span>' +
                    'Guardando\u2026';
            }
        });
    }

    /* Estado inicial */
    renderLines();

}); /* fin DOMContentLoaded */

/* ══════════════════════════════════════════════════════════
Calculadora de totales en tiempo real para el modal de línea.  
═════════════════════════════════════════════════════════════ */
function calcular() {
    const qty = parseFloat(document.getElementById('quantity').value) || 0;
    const price = parseFloat(document.getElementById('unitPrice').value) || 0;
    const discount = parseFloat(document.getElementById('discountAmount').value) || 0;
    const taxPct = parseFloat(document.getElementById('taxPercentage').value) || 0;

    const subtotal = Math.max((qty * price) - discount, 0);
    const tax = Math.round(subtotal * (taxPct / 100) * 100) / 100;
    const total = subtotal + tax;

    document.getElementById('previewSubtotal').textContent = `$${subtotal.toFixed(2)}`;
    document.getElementById('previewTax').textContent = `$${tax.toFixed(2)}`;
    document.getElementById('previewTotal').textContent = `$${total.toFixed(2)}`;
}

['quantity', 'unitPrice', 'discountAmount', 'taxPercentage']
    .forEach(id => document.getElementById(id)
        ?.addEventListener('input', calcular));

calcular();

/* ══════════════════════════════════════════════════════════
Calculadora de totales en tiempo real para el modal de línea.  
═════════════════════════════════════════════════════════════ */

function switchTab(tab) {
    document.querySelectorAll('.tab-panel').forEach(p => p.style.display = 'none');
    document.querySelectorAll('.tabs__link').forEach(l => l.classList.remove('active'));

    document.getElementById('tab-' + tab).style.display = '';
    document.getElementById('tab-' + tab + '-btn').classList.add('active');

    if (tab === 'movimientos') {
        cargarMovimientos(Model.DocumentId);
    }
}

async function cargarMovimientos(docId) {
    const tbody = document.getElementById('movimientosBody');
    tbody.innerHTML = `<tr><td colspan="6" class="text-center" style="padding:20px;color:var(--clr-text-muted)">
                <span class="spinner-border spinner-border-sm me-2"></span>Cargando…</td></tr>`;

    try {
        const res = await fetch(`/Documents/Movimientos?id=${docId}`);
        const data = await res.json();

        if (!data || data.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center"
                        style="padding:30px;color:var(--clr-text-muted)">
                        Sin movimientos registrados.</td></tr>`;
            return;
        }

        tbody.innerHTML = data.map(m => `
                    <tr>
                        <td><span class="badge badge-neutral">${m.movementType ?? '—'}</span></td>
                        <td style="font-weight:600">${m.product ?? '—'}</td>
                        <td class="text-end">${m.quantity}</td>
                        <td class="text-end">$${(m.unitCost ?? 0).toFixed(2)}</td>
                        <td style="font-size:.825rem;color:var(--clr-text-muted)">${m.createdBy ?? '—'}</td>
                        <td style="font-size:.8rem;color:var(--clr-text-muted)">${m.notes ?? '—'}</td>
                    </tr>`).join('');
    } catch {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center"
                    style="color:var(--clr-danger);padding:20px">
                    Error al cargar los movimientos.</td></tr>`;
    }
}