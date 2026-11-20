(function () {
    'use strict';

    // Initialize TempData toasts
    function initToasts() {
        if (typeof bootstrap === 'undefined' || !bootstrap.Toast) {
            return;
        }

        document.querySelectorAll('.pk-toast[data-pk-auto-show="true"]').forEach(function (el) {
            var toast = bootstrap.Toast.getOrCreateInstance(el);
            toast.show();
        });
    }

    // Staff sidebar mobile drawer
    function initStaffSidebar() {
        var sidebar = document.getElementById('pkSidebar');
        var overlay = document.getElementById('pkSidebarOverlay');
        var openBtn = document.getElementById('pkSidebarToggle');
        var closeBtn = document.getElementById('pkSidebarClose');

        if (!sidebar || !overlay) {
            return;
        }

        function openSidebar() {
            sidebar.classList.add('is-open');
            overlay.classList.add('is-visible');
            if (openBtn) {
                openBtn.setAttribute('aria-expanded', 'true');
            }
        }

        function closeSidebar() {
            sidebar.classList.remove('is-open');
            overlay.classList.remove('is-visible');
            if (openBtn) {
                openBtn.setAttribute('aria-expanded', 'false');
            }
        }

        if (openBtn) {
            openBtn.addEventListener('click', openSidebar);
        }
        if (closeBtn) {
            closeBtn.addEventListener('click', closeSidebar);
        }
        overlay.addEventListener('click', closeSidebar);

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape' && sidebar.classList.contains('is-open')) {
                closeSidebar();
            }
        });
    }

    // Reusable confirmation modal
    function initConfirmModal() {
        var modalEl = document.getElementById('pkConfirmModal');
        if (!modalEl || typeof bootstrap === 'undefined' || !bootstrap.Modal) {
            return;
        }

        var titleEl = document.getElementById('pkConfirmModalTitle');
        var messageEl = document.getElementById('pkConfirmModalMessage');
        var confirmBtn = document.getElementById('pkConfirmModalSubmit');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        var pendingCallback = null;

        function applyButtonVariant(variant) {
            confirmBtn.classList.remove('btn-danger', 'btn-primary', 'btn-success', 'btn-warning');
            switch (variant) {
                case 'primary':
                    confirmBtn.classList.add('btn-primary');
                    break;
                case 'success':
                    confirmBtn.classList.add('btn-success');
                    break;
                case 'warning':
                    confirmBtn.classList.add('btn-warning');
                    break;
                default:
                    confirmBtn.classList.add('btn-danger');
                    break;
            }
        }

        function showConfirm(options) {
            titleEl.textContent = options.title || 'Xác nhận thao tác';
            messageEl.textContent = options.message || 'Bạn có chắc chắn muốn thực hiện thao tác này không?';
            confirmBtn.textContent = options.confirmText || 'Xác nhận';
            applyButtonVariant(options.variant || 'danger');
            pendingCallback = typeof options.onConfirm === 'function' ? options.onConfirm : null;
            modal.show();
        }

        confirmBtn.addEventListener('click', function () {
            var callback = pendingCallback;
            pendingCallback = null;
            modal.hide();
            if (callback) {
                callback();
            }
        });

        modalEl.addEventListener('hidden.bs.modal', function () {
            pendingCallback = null;
        });

        // Declarative support on buttons/links with data-confirm
        document.addEventListener('click', function (event) {
            var trigger = event.target.closest('[data-confirm]');
            if (!trigger || trigger.tagName === 'FORM') {
                return;
            }

            if (trigger.dataset.pkConfirmed === 'true') {
                delete trigger.dataset.pkConfirmed;
                return;
            }

            event.preventDefault();
            var form = trigger.closest('form');

            showConfirm({
                title: trigger.dataset.confirmTitle,
                message: trigger.dataset.confirm,
                confirmText: trigger.dataset.confirmBtn,
                variant: trigger.dataset.confirmVariant,
                onConfirm: function () {
                    if (form) {
                        if (trigger.getAttribute('formaction')) {
                            form.setAttribute('action', trigger.getAttribute('formaction'));
                        }
                        if (trigger.getAttribute('formmethod')) {
                            form.setAttribute('method', trigger.getAttribute('formmethod'));
                        }
                        form.dataset.pkConfirmed = 'true';
                        form.submit();
                    } else if (trigger.tagName === 'A' && trigger.href) {
                        window.location.href = trigger.href;
                    } else {
                        trigger.dataset.pkConfirmed = 'true';
                        trigger.click();
                    }
                }
            });
        });

        // Declarative support on forms with data-confirm
        document.addEventListener('submit', function (event) {
            var form = event.target;
            if (!form || !form.dataset || !form.dataset.confirm) {
                return;
            }

            if (form.dataset.pkConfirmed === 'true') {
                delete form.dataset.pkConfirmed;
                return;
            }

            event.preventDefault();
            showConfirm({
                title: form.dataset.confirmTitle,
                message: form.dataset.confirm,
                confirmText: form.dataset.confirmBtn,
                variant: form.dataset.confirmVariant,
                onConfirm: function () {
                    form.dataset.pkConfirmed = 'true';
                    form.submit();
                }
            });
        });

        window.pkConfirm = showConfirm;
    }

    document.addEventListener('DOMContentLoaded', function () {
        initToasts();
        initStaffSidebar();
        initConfirmModal();
    });
})();
