/**
 * layout.js — SysGestionVentas
 * Manejo del sidebar, overlay mobile, submenús colapsables y scroll-to-top.
 */
(function () {
    'use strict';

    /* ── Sidebar mobile ─────────────────────────────────── */
    const sidebar = document.querySelector('.sidebar');
    const overlay = document.querySelector('.sidebar-overlay') || createOverlay();
    const toggleBtn = document.querySelector('.topbar__toggle');

    function createOverlay() {
        const el = document.createElement('div');
        el.className = 'sidebar-overlay';
        document.body.appendChild(el);
        return el;
    }

    function openSidebar() {
        sidebar?.classList.add('is-open');
        overlay.classList.add('active');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        sidebar?.classList.remove('is-open');
        overlay.classList.remove('active');
        document.body.style.overflow = '';
    }

    toggleBtn?.addEventListener('click', () => {
        sidebar?.classList.contains('is-open') ? closeSidebar() : openSidebar();
    });

    overlay.addEventListener('click', closeSidebar);

    /* ── Submenús colapsables ────────────────────────────── */
    document.querySelectorAll('.sidebar__item[data-bs-toggle="collapse"]').forEach(trigger => {
        const targetId = trigger.getAttribute('data-bs-target') ||
            trigger.getAttribute('href');
        const target = document.querySelector(targetId);
        if (!target) return;

        // Estado inicial según URL activa
        if (target.querySelector('.sidebar__submenu-item.active')) {
            target.classList.add('open');
            trigger.setAttribute('aria-expanded', 'true');
        }

        trigger.addEventListener('click', (e) => {
            e.preventDefault();
            const isOpen = target.classList.toggle('open');
            trigger.setAttribute('aria-expanded', String(isOpen));
        });
    });

    /* ── Marcar ítem activo en sidebar ───────────────────── */
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar__item, .sidebar__submenu-item').forEach(link => {
        const href = link.getAttribute('href');
        if (!href || href === '#') return;
        const linkPath = href.toLowerCase().split('?')[0];
        if (currentPath === linkPath || (linkPath !== '/' && currentPath.startsWith(linkPath))) {
            link.classList.add('active');
            // Abrir submenu padre si corresponde
            const parentMenu = link.closest('.sidebar__submenu');
            if (parentMenu) {
                parentMenu.classList.add('open');
                const parentTrigger = document.querySelector(
                    `[data-bs-target="#${parentMenu.id}"], [href="#${parentMenu.id}"]`
                );
                parentTrigger?.setAttribute('aria-expanded', 'true');
            }
        }
    });

    /* ── Scroll to top ───────────────────────────────────── */
    const scrollBtn = document.createElement('button');
    scrollBtn.className = 'scroll-top';
    scrollBtn.setAttribute('aria-label', 'Volver arriba');
    scrollBtn.innerHTML = '<span class="material-symbols-outlined" style="font-size:20px">keyboard_arrow_up</span>';
    document.body.appendChild(scrollBtn);

    window.addEventListener('scroll', () => {
        scrollBtn.classList.toggle('visible', window.scrollY > 300);
    }, { passive: true });

    scrollBtn.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));

    /* ── Cierre sidebar al navegar (mobile) ──────────────── */
    document.querySelectorAll('.sidebar__item[href], .sidebar__submenu-item').forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth < 992) closeSidebar();
        });
    });

})();