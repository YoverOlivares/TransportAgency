// ===== TRANSPORTAGENCY - SITE.JS =====
// JavaScript principal para la aplicación TransportAgency

$(document).ready(function () {
    initializeTransportAgency();
});

// ===== INICIALIZACIÓN PRINCIPAL =====
function initializeTransportAgency() {
    console.log('🚌 TransportAgency - Sistema inicializado');

    setupAnimations();
    setupNavigation();
    setupInteractions();
    setupFormValidations();
    setupTooltips();
    setupModals();
    setupTables();
    setupAlerts();
    setupScroll();

    // Log de estado del sistema
    logSystemStatus();
}

// ===== ANIMACIONES =====
function setupAnimations() {
    // Activar animaciones fade-in-up
    setTimeout(() => {
        $('.fade-in-up').addClass('active');
    }, 100);

    // Animación de contadores numéricos
    animateCounters();

    // Animación de barras de progreso
    animateProgressBars();

    // Animaciones de entrada escalonada para cards
    $('.card').each(function (index) {
        const $card = $(this);
        setTimeout(() => {
            $card.addClass('fade-in');
        }, index * 100);
    });
}

function animateCounters() {
    $('.counter, .fw-bold:contains("0"), .fw-bold:contains("1"), .fw-bold:contains("2")').each(function () {
        const $this = $(this);
        const text = $this.text();
        const number = parseFloat(text.replace(/[^\d.-]/g, ''));

        if (!isNaN(number) && number > 0) {
            let start = 0;
            const increment = number / 30;
            const timer = setInterval(() => {
                start += increment;
                if (start >= number) {
                    $this.text(text);
                    clearInterval(timer);
                } else {
                    $this.text(Math.floor(start));
                }
            }, 50);
        }
    });
}

function animateProgressBars() {
    $('.progress-bar').each(function () {
        const $bar = $(this);
        const width = $bar.attr('style')?.match(/width:\s*(\d+%)/)?.[1] || '0%';

        $bar.css('width', '0%');
        setTimeout(() => {
            $bar.css({
                'width': width,
                'transition': 'width 1.5s ease-in-out'
            });
        }, 300);
    });
}

// ===== NAVEGACIÓN =====
function setupNavigation() {
    // Marcar enlace activo en navegación
    const currentPath = window.location.pathname.toLowerCase();
    $('.modern-nav-link').each(function () {
        const linkPath = $(this).attr('href')?.toLowerCase();
        if (linkPath && currentPath.includes(linkPath.split('/').pop())) {
            $(this).addClass('active');
        }
    });

    // Navbar dinámico en scroll
    setupDynamicNavbar();

    // Cerrar dropdown al hacer click fuera
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.dropdown').length) {
            $('.dropdown-menu').removeClass('show');
        }
    });
}

function setupDynamicNavbar() {
    let lastScroll = 0;

    $(window).scroll(function () {
        const currentScroll = $(this).scrollTop();
        const navbar = $('.modern-navbar');

        // Cambiar opacidad basado en scroll
        if (currentScroll > 50) {
            navbar.addClass('scrolled');
            navbar.css({
                'background': 'rgba(20, 20, 20, 0.98)',
                'backdrop-filter': 'blur(20px)'
            });
        } else {
            navbar.removeClass('scrolled');
            navbar.css({
                'background': 'rgba(20, 20, 20, 0.95)',
                'backdrop-filter': 'blur(15px)'
            });
        }

        // Auto-hide en mobile
        if ($(window).width() <= 768) {
            if (currentScroll > lastScroll && currentScroll > 100) {
                navbar.css('transform', 'translateY(-100%)');
            } else {
                navbar.css('transform', 'translateY(0)');
            }
        }

        lastScroll = currentScroll;
    });
}

// ===== INTERACCIONES =====
function setupInteractions() {
    // Efectos hover para cards
    $('.card').hover(
        function () {
            $(this).addClass('hover-lift shadow-gold');
        },
        function () {
            $(this).removeClass('hover-lift shadow-gold');
        }
    );

    // Efectos para botones
    $('.btn').hover(
        function () {
            if (!$(this).hasClass('disabled')) {
                $(this).addClass('hover-lift');
            }
        },
        function () {
            $(this).removeClass('hover-lift');
        }
    );

    // Loading state para botones
    $('.btn:not(.no-loading)').on('click', function (e) {
        const $btn = $(this);
        const originalText = $btn.html();

        if (!$btn.hasClass('disabled') && !$btn.prop('disabled')) {
            $btn.html('<i class="fas fa-spinner fa-spin me-2"></i>Procesando...');
            $btn.addClass('disabled');

            // Restaurar después de navegación o timeout
            setTimeout(() => {
                if ($btn.length && $btn.hasClass('disabled')) {
                    $btn.html(originalText);
                    $btn.removeClass('disabled');
                }
            }, 3000);
        }
    });

    // Navegación suave
    setupSmoothScrolling();
}

function setupSmoothScrolling() {
    $('a[href*="#"]:not([href="#"])').click(function () {
        if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '')
            && location.hostname === this.hostname) {
            let target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');

            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top - 100
                }, 800);
                return false;
            }
        }
    });
}

// ===== VALIDACIONES DE FORMULARIOS =====
function setupFormValidations() {
    // Validación en tiempo real
    $('.form-control').on('input blur', function () {
        validateField($(this));
    });

    // Estilos focus
    $('.form-control').on('focus', function () {
        $(this).removeClass('is-invalid is-valid');
        $(this).addClass('focused');
    });

    $('.form-control').on('blur', function () {
        $(this).removeClass('focused');
    });

    // Prevenir doble submit
    $('form').on('submit', function () {
        const $form = $(this);
        const $submitBtn = $form.find('button[type="submit"]');

        $submitBtn.prop('disabled', true);
        setTimeout(() => {
            $submitBtn.prop('disabled', false);
        }, 3000);
    });
}

function validateField($field) {
    const value = $field.val().trim();
    const isRequired = $field.prop('required');
    const type = $field.attr('type');
    let isValid = true;

    if (isRequired && !value) {
        isValid = false;
    } else if (value) {
        switch (type) {
            case 'email':
                isValid = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
                break;
            case 'tel':
                isValid = /^[\+]?[0-9\s\-\(\)]+$/.test(value);
                break;
            case 'number':
                isValid = !isNaN(value) && value > 0;
                break;
        }
    }

    // Aplicar clases de validación
    if (isValid && value) {
        $field.removeClass('is-invalid').addClass('is-valid');
    } else if (!isValid) {
        $field.removeClass('is-valid').addClass('is-invalid');
    } else {
        $field.removeClass('is-valid is-invalid');
    }

    return isValid;
}

// ===== TOOLTIPS Y POPOVERS =====
function setupTooltips() {
    // Inicializar tooltips de Bootstrap
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl, {
            delay: { show: 300, hide: 100 }
        });
    });

    // Tooltips automáticos para botones con íconos
    $('.btn[title]').each(function () {
        new bootstrap.Tooltip(this);
    });
}

// ===== MODALES =====
function setupModals() {
    // Efectos de entrada
    $('.modal').on('show.bs.modal', function () {
        $(this).find('.modal-dialog').addClass('fade-in-up');
    });

    // Auto-focus en inputs
    $('.modal').on('shown.bs.modal', function () {
        $(this).find('input:visible:first').focus();
    });

    // Limpiar formularios al cerrar
    $('.modal').on('hidden.bs.modal', function () {
        const $modal = $(this);
        $modal.find('form')[0]?.reset();
        $modal.find('.form-control').removeClass('is-valid is-invalid');
        $modal.find('.modal-dialog').removeClass('fade-in-up');
    });
}

// ===== TABLAS =====
function setupTables() {
    // Hover mejorado para filas
    $('.table tbody tr').hover(
        function () {
            $(this).addClass('table-hover-enhanced');
        },
        function () {
            $(this).removeClass('table-hover-enhanced');
        }
    );

    // Búsqueda en tablas
    $('.table-search').on('keyup', debounce(function () {
        const value = $(this).val().toLowerCase();
        const tableSelector = $(this).data('table') || '.table';
        const $table = $(tableSelector);

        $table.find('tbody tr').each(function () {
            const text = $(this).text().toLowerCase();
            $(this).toggle(text.indexOf(value) > -1);
        });
    }, 300));

    // Click en filas para selección
    $('.table-selectable tbody tr').click(function () {
        $(this).toggleClass('table-active');
        updateSelectedCount();
    });
}

// ===== ALERTAS Y NOTIFICACIONES =====
function setupAlerts() {
    // Auto-hide para alertas
    $('.alert:not(.alert-permanent)').each(function () {
        const $alert = $(this);
        setTimeout(() => {
            $alert.fadeOut();
        }, 5000);
    });
}

// ===== SCROLL EFFECTS =====
function setupScroll() {
    // Parallax suave para el fondo
    $(window).scroll(function () {
        const scrolled = $(this).scrollTop();
        const rate = scrolled * -0.5;

        $('body::before').css({
            'transform': `translate3d(0, ${rate}px, 0)`
        });
    });

    // Reveal animations on scroll
    $(window).scroll(function () {
        $('.fade-in-up:not(.active)').each(function () {
            const elementTop = $(this).offset().top;
            const elementBottom = elementTop + $(this).outerHeight();
            const viewportTop = $(window).scrollTop();
            const viewportBottom = viewportTop + $(window).height();

            if (elementBottom > viewportTop && elementTop < viewportBottom) {
                $(this).addClass('active');
            }
        });
    });
}

// ===== UTILIDADES =====
function showToast(message, type = 'info', duration = 5000) {
    const iconMap = {
        'success': 'check-circle',
        'danger': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle'
    };

    const icon = iconMap[type] || 'info-circle';

    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type} border-0 position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999;" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas fa-${icon} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    const $toast = $(toastHtml);
    $('body').append($toast);

    const toast = new bootstrap.Toast($toast[0], { delay: duration });
    toast.show();

    $toast.on('hidden.bs.toast', function () {
        $(this).remove();
    });
}

function formatCurrency(amount, currency = 'PEN') {
    return new Intl.NumberFormat('es-PE', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

function formatDate(date, format = 'dd/MM/yyyy') {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();

    switch (format) {
        case 'dd/MM/yyyy':
            return `${day}/${month}/${year}`;
        case 'MM/dd/yyyy':
            return `${month}/${day}/${year}`;
        case 'yyyy-MM-dd':
            return `${year}-${month}-${day}`;
        default:
            return d.toLocaleDateString('es-PE');
    }
}

function debounce(func, wait, immediate) {
    let timeout;
    return function executedFunction() {
        const context = this;
        const args = arguments;
        const later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        const callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
}

function updateSelectedCount() {
    const count = $('.table-active').length;
    $('.selected-count').text(`${count} elementos seleccionados`);
}

// ===== LOGGING Y DEBUG =====
function logSystemStatus() {
    const hasNavbar = $('.modern-navbar').length > 0;
    const hasFooter = $('.modern-footer').length > 0;
    const hasCss = window.getComputedStyle(document.body).backgroundImage.includes('radial-gradient');

    console.log('📊 Estado del Sistema TransportAgency:');
    console.log(`   Navbar: ${hasNavbar ? '✅' : '❌'}`);
    console.log(`   Footer: ${hasFooter ? '✅' : '❌'}`);
    console.log(`   CSS: ${hasCss ? '✅' : '❌'}`);
    console.log(`   jQuery: ${typeof $ !== 'undefined' ? '✅' : '❌'}`);
    console.log(`   Bootstrap: ${typeof bootstrap !== 'undefined' ? '✅' : '❌'}`);

    if (hasNavbar && hasFooter && hasCss) {
        console.log('🎉 Sistema completamente funcional');
    } else {
        console.warn('⚠️ Revisar configuración del sistema');
    }
}

// ===== EVENTOS GLOBALES =====
$(document).ajaxError(function (event, xhr, settings) {
    console.error('Error AJAX:', xhr.status, xhr.statusText);
    showToast('Error en la comunicación con el servidor', 'danger');
});

$(document).ajaxStart(function () {
    $('body').addClass('loading');
}).ajaxStop(function () {
    $('body').removeClass('loading');
});

// ===== FUNCIONES ESPECÍFICAS PARA TRANSPORTAGENCY =====

// Función para manejar selección de asientos
function selectSeat(seatId, seatNumber, price) {
    $('.seat-btn').removeClass('selected');
    $(`.seat-btn[data-seat-id="${seatId}"]`).addClass('selected');

    // Actualizar información del asiento seleccionado
    $('#selected-seat-info').show();
    $('#selected-seat-number').text(seatNumber);
    $('#selected-seat-price').text(formatCurrency(price));

    console.log(`Asiento seleccionado: ${seatNumber} - ${formatCurrency(price)}`);
}

// Función para actualizar disponibilidad de asientos
function updateSeatAvailability(tripId) {
    // Simular llamada AJAX para actualizar disponibilidad
    $.get(`/Seat/CheckAvailability/${tripId}`)
        .done(function (data) {
            if (data.success) {
                $('.available-count').text(data.available);
                $('.occupied-count').text(data.occupied);
                showToast('Disponibilidad actualizada', 'success');
            }
        })
        .fail(function () {
            showToast('Error al actualizar disponibilidad', 'danger');
        });
}

// Función para auto-refresh de páginas específicas
function setupAutoRefresh(interval = 300000) { // 5 minutos
    if ($('body').data('auto-refresh') === 'true') {
        setTimeout(function () {
            if (!$('.modal.show').length && !$('input:focus').length) {
                location.reload();
            }
        }, interval);
    }
}

// Inicializar auto-refresh para páginas que lo necesiten
$(document).ready(function () {
    setupAutoRefresh();
});

// ===== EXPORT FUNCTIONS =====
window.TransportAgency = {
    showToast,
    formatCurrency,
    formatDate,
    selectSeat,
    updateSeatAvailability,
    logSystemStatus
};

console.log('🚌 TransportAgency site.js cargado completamente');