// ===== TRANSPORTAGENCY - SITE.JS =====

// Inicialización cuando el DOM está listo
$(document).ready(function () {
    initializeApp();
});

// Función principal de inicialización
function initializeApp() {
    setupAnimations();
    setupInteractions();
    setupFormValidations();
    setupTooltips();
    setupModals();
    setupTables();
    setupCharts();
}

// ===== ANIMACIONES =====
function setupAnimations() {
    // Animación de entrada para tarjetas
    $('.card').each(function (index) {
        $(this).css({
            'animation-delay': (index * 0.1) + 's',
            'opacity': '0'
        }).addClass('fade-in-up');

        setTimeout(() => {
            $(this).css('opacity', '1');
        }, index * 100);
    });

    // Animación para estadísticas
    animateCounters();

    // Animación de progreso
    animateProgressBars();
}

// Animar contadores numéricos
function animateCounters() {
    $('.counter').each(function () {
        const $this = $(this);
        const countTo = parseInt($this.text().replace(/,/g, ''));

        $({ countNum: 0 }).animate({
            countNum: countTo
        }, {
            duration: 2000,
            easing: 'swing',
            step: function () {
                $this.text(Math.floor(this.countNum).toLocaleString());
            },
            complete: function () {
                $this.text(countTo.toLocaleString());
            }
        });
    });
}

// Animar barras de progreso
function animateProgressBars() {
    $('.progress-bar').each(function () {
        const $this = $(this);
        const width = $this.attr('style').match(/width:\s*(\d+%)/);

        if (width) {
            $this.css('width', '0%');
            setTimeout(() => {
                $this.css({
                    'width': width[1],
                    'transition': 'width 2s ease-in-out'
                });
            }, 500);
        }
    });
}

// ===== INTERACCIONES =====
function setupInteractions() {
    // Efecto hover para tarjetas
    $('.card').hover(
        function () {
            $(this).addClass('shadow-gold');
            $(this).find('.card-header').addClass('text-glow');
        },
        function () {
            $(this).removeClass('shadow-gold');
            $(this).find('.card-header').removeClass('text-glow');
        }
    );

    // Efecto hover para botones
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

    // Efecto click para botones con loading
    $('.btn:not(.no-loading)').on('click', function (e) {
        const $btn = $(this);
        const originalText = $btn.html();

        if (!$btn.hasClass('disabled') && !$btn.prop('disabled')) {
            $btn.html('<i class="fas fa-spinner fa-spin me-2"></i>Procesando...');
            $btn.addClass('disabled');

            // Restaurar después de 2 segundos si no hay navegación
            setTimeout(() => {
                if ($btn.length) {
                    $btn.html(originalText);
                    $btn.removeClass('disabled');
                }
            }, 2000);
        }
    });

    // Navegación suave
    setupSmoothScrolling();

    // Navbar dinámico
    setupDynamicNavbar();
}

// Navegación suave
function setupSmoothScrolling() {
    $('a[href*="#"]:not([href="#"])').click(function () {
        if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '')
            && location.hostname === this.hostname) {
            let target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');

            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top - 100
                }, 1000);
                return false;
            }
        }
    });
}

// Navbar dinámico en scroll
function setupDynamicNavbar() {
    $(window).scroll(function () {
        const scrollTop = $(window).scrollTop();

        if (scrollTop > 50) {
            $('.navbar').addClass('scrolled');
            $('.navbar').css({
                'background': 'rgba(20, 20, 20, 0.98)',
                'backdrop-filter': 'blur(15px)'
            });
        } else {
            $('.navbar').removeClass('scrolled');
            $('.navbar').css({
                'background': 'rgba(20, 20, 20, 0.95)',
                'backdrop-filter': 'blur(10px)'
            });
        }
    });
}

// ===== VALIDACIONES DE FORMULARIOS =====
function setupFormValidations() {
    // Validación en tiempo real
    $('.form-control').on('input blur', function () {
        validateField($(this));
    });

    // Estilos para campos válidos/inválidos
    $('.form-control').on('focus', function () {
        $(this).removeClass('is-invalid is-valid');
        $(this).addClass('focused');
    });

    $('.form-control').on('blur', function () {
        $(this).removeClass('focused');
    });
}

// Validar campo individual
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

    // Aplicar estilos
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
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Tooltips personalizados para botones
    $('.btn[title]').each(function () {
        new bootstrap.Tooltip(this);
    });
}

// ===== MODALES =====
function setupModals() {
    // Efecto de entrada para modales
    $('.modal').on('show.bs.modal', function () {
        $(this).find('.modal-dialog').addClass('fade-in-up');
    });

    // Auto-focus en primer input de modales
    $('.modal').on('shown.bs.modal', function () {
        $(this).find('input:text:visible:first').focus();
    });

    // Limpiar formularios al cerrar modales
    $('.modal').on('hidden.bs.modal', function () {
        $(this).find('form')[0]?.reset();
        $(this).find('.form-control').removeClass('is-valid is-invalid');
    });
}

// ===== TABLAS =====
function setupTables() {
    // Efecto hover mejorado para filas de tabla
    $('.table tbody tr').hover(
        function () {
            $(this).addClass('table-hover-effect');
        },
        function () {
            $(this).removeClass('table-hover-effect');
        }
    );

    // Búsqueda en tablas
    $('.table-search').on('keyup', function () {
        const value = $(this).val().toLowerCase();
        const table = $($(this).data('table'));

        table.find('tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Ordenamiento de tablas
    $('.sortable th').click(function () {
        const table = $(this).parents('table').eq(0);
        const index = $(this).index();
        const rows = table.find('tr:gt(0)').toArray().sort(comparer(index));

        this.asc = !this.asc;
        if (!this.asc) {
            rows = rows.reverse();
        }

        for (let i = 0; i < rows.length; i++) {
            table.append(rows[i]);
        }

        // Actualizar indicadores de ordenamiento
        $('.sortable th').removeClass('sort-asc sort-desc');
        $(this).addClass(this.asc ? 'sort-asc' : 'sort-desc');
    });
}

// Función comparadora para ordenamiento
function comparer(index) {
    return function (a, b) {
        const valA = getCellValue(a, index);
        const valB = getCellValue(b, index);
        return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB);
    };
}

function getCellValue(row, index) {
    return $(row).children('td').eq(index).text();
}

// ===== GRÁFICOS =====
function setupCharts() {
    // Configuración base para Chart.js
    if (typeof Chart !== 'undefined') {
        Chart.defaults.color = 'rgba(255, 255, 255, 0.9)';
        Chart.defaults.borderColor = 'rgba(197, 167, 80, 0.2)';
        Chart.defaults.backgroundColor = 'rgba(197, 167, 80, 0.1)';
    }
}

// ===== UTILIDADES =====

// Mostrar notificación toast
function showToast(message, type = 'info', duration = 5000) {
    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas fa-${getIconForType(type)} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    const $toast = $(toastHtml);
    $('.toast-container').append($toast);

    const toast = new bootstrap.Toast($toast[0], { delay: duration });
    toast.show();

    // Remover del DOM después de que se oculte
    $toast.on('hidden.bs.toast', function () {
        $(this).remove();
    });
}

// Obtener icono según tipo de notificación
function getIconForType(type) {
    const icons = {
        'success': 'check-circle',
        'danger': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle',
        'primary': 'star'
    };
    return icons[type] || 'info-circle';
}

// Formatear números como moneda
function formatCurrency(amount) {
    return new Intl.NumberFormat('es-PE', {
        style: 'currency',
        currency: 'PEN'
    }).format(amount);
}

// Formatear fechas
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

// Debounce function para optimizar eventos
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

// Auto-refresh para páginas que lo necesiten
function setupAutoRefresh(interval = 300000) { // 5 minutos por defecto
    if ($('body').data('auto-refresh') === 'true') {
        setTimeout(function () {
            if (!$('.modal.show').length) { // No refrescar si hay modales abiertos
                location.reload();
            }
        }, interval);
    }
}

// Configurar auto-refresh
$(document).ready(function () {
    setupAutoRefresh();
});

// ===== EVENTOS GLOBALES =====

// Manejo de errores de AJAX
$(document).ajaxError(function (event, xhr, settings) {
    console.error('Error AJAX:', xhr.status, xhr.statusText);
    showToast('Error en la comunicación con el servidor', 'danger');
});

// Loading global para AJAX
$(document).ajaxStart(function () {
    $('body').addClass('loading');
}).ajaxStop(function () {
    $('body').removeClass('loading');
});

// Prevenir double-click en formularios
$('form').on('submit', function () {
    $(this).find('button[type="submit"]').prop('disabled', true);
});

// ===== EFECTOS VISUALES ADICIONALES =====

// Partículas de fondo (opcional)
function createParticleEffect() {
    if ($('.particle-container').length === 0) {
        $('body').append('<div class="particle-container"></div>');

        for (let i = 0; i < 50; i++) {
            const particle = $('<div class="particle"></div>');
            particle.css({
                left: Math.random() * 100 + '%',
                animationDelay: Math.random() * 20 + 's',
                animationDuration: (Math.random() * 10 + 10) + 's'
            });
            $('.particle-container').append(particle);
        }
    }
}

// Inicializar efectos opcionales
$(document).ready(function () {
    // Descomentar si quieres partículas de fondo
    // createParticleEffect();
});