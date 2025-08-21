/**
 * TRANSPORT AGENCY - ELEGANT JAVASCRIPT
 * Funcionalidad avanzada con animaciones suaves y UX mejorada
 */

(function () {
    'use strict';

    // Configuración global
    const CONFIG = {
        animationDuration: 300,
        scrollOffset: 100,
        debounceDelay: 250,
        autoHideAlertDelay: 5000,
        intersectionThreshold: 0.1
    };

    // Estado de la aplicación
    const AppState = {
        isScrolling: false,
        currentTheme: 'light',
        activeModals: new Set(),
        animatedElements: new WeakSet()
    };

    // Inicialización cuando el DOM esté listo
    document.addEventListener('DOMContentLoaded', initializeApp);
    window.addEventListener('load', onWindowLoad);

    /**
     * Función principal de inicialización
     */
    function initializeApp() {
        console.log('🚀 Inicializando Transport Agency...');

        // Inicializar todos los módulos
        initializeAnimations();
        initializeNavigation();
        initializeFormEnhancements();
        initializeCustomSelects();
        initializeScrollEffects();
        initializeSearchForm();
        initializeTooltips();
        initializeModals();
        initializeAlerts();
        initializeTableEnhancements();
        initializeLoadingStates();
        initializeCounterAnimations();
        initializeParallaxEffects();

        console.log('✅ Transport Agency inicializado correctamente');
    }

    /**
     * Funciones ejecutadas cuando la ventana se carga completamente
     */
    function onWindowLoad() {
        hidePreloader();
        initializeLazyLoading();
        optimizeImages();
    }

    /**
     * ==========================================
     * ANIMACIONES Y EFECTOS VISUALES
     * ==========================================
     */

    function initializeAnimations() {
        // Configurar observer para animaciones de entrada
        const observerOptions = {
            threshold: CONFIG.intersectionThreshold,
            rootMargin: '0px 0px -50px 0px'
        };

        const animationObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !AppState.animatedElements.has(entry.target)) {
                    animateElement(entry.target);
                    AppState.animatedElements.add(entry.target);
                }
            });
        }, observerOptions);

        // Observar elementos para animación
        document.querySelectorAll(`
            .feature-card, .route-card, .service-item, 
            .testimonial-card, .stat-card, .search-card,
            .hero-content, .section-header
        `).forEach(el => {
            animationObserver.observe(el);
        });
    }

    function animateElement(element) {
        const animationType = element.dataset.animation || 'fadeInUp';

        element.style.opacity = '0';
        element.style.transform = getInitialTransform(animationType);

        // Forzar reflow
        element.offsetHeight;

        element.style.transition = `all ${CONFIG.animationDuration * 2}ms cubic-bezier(0.4, 0, 0.2, 1)`;
        element.style.opacity = '1';
        element.style.transform = 'translate3d(0, 0, 0) scale(1) rotate(0deg)';
    }

    function getInitialTransform(type) {
        const transforms = {
            fadeInUp: 'translate3d(0, 30px, 0)',
            fadeInDown: 'translate3d(0, -30px, 0)',
            fadeInLeft: 'translate3d(-30px, 0, 0)',
            fadeInRight: 'translate3d(30px, 0, 0)',
            scaleIn: 'scale(0.8)',
            rotateIn: 'rotate(-10deg) scale(0.8)'
        };
        return transforms[type] || transforms.fadeInUp;
    }

    /**
     * ==========================================
     * NAVEGACIÓN Y SCROLL
     * ==========================================
     */

    function initializeNavigation() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;

        let lastScrollTop = 0;
        let isNavbarHidden = false;

        const handleScroll = debounce(() => {
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            const scrollDirection = scrollTop > lastScrollTop ? 'down' : 'up';

            // Cambiar estilo del navbar según scroll
            if (scrollTop > 100) {
                navbar.classList.add('navbar-scrolled');
                navbar.style.background = 'rgba(44, 95, 65, 0.98)';
                navbar.style.backdropFilter = 'blur(20px)';
            } else {
                navbar.classList.remove('navbar-scrolled');
                navbar.style.background = 'rgba(44, 95, 65, 0.95)';
                navbar.style.backdropFilter = 'blur(15px)';
            }

            // Auto-hide/show navbar
            if (Math.abs(scrollTop - lastScrollTop) > 10) {
                if (scrollDirection === 'down' && scrollTop > 300 && !isNavbarHidden) {
                    hideNavbar();
                } else if (scrollDirection === 'up' && isNavbarHidden) {
                    showNavbar();
                }
                lastScrollTop = scrollTop;
            }
        }, 10);

        window.addEventListener('scroll', handleScroll, { passive: true });

        function hideNavbar() {
            navbar.style.transform = 'translateY(-100%)';
            isNavbarHidden = true;
        }

        function showNavbar() {
            navbar.style.transform = 'translateY(0)';
            isNavbarHidden = false;
        }

        // Smooth scroll para enlaces internos
        initializeSmoothScroll();
    }

    function initializeSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const targetId = this.getAttribute('href');
                const targetElement = document.querySelector(targetId);

                if (targetElement) {
                    const offsetTop = targetElement.offsetTop - CONFIG.scrollOffset;

                    window.scrollTo({
                        top: offsetTop,
                        behavior: 'smooth'
                    });
                }
            });
        });
    }

    function initializeScrollEffects() {
        // Parallax suave para hero section
        const heroSection = document.querySelector('.hero-section');
        if (heroSection) {
            window.addEventListener('scroll', () => {
                const scrolled = window.pageYOffset;
                const rate = scrolled * -0.5;
                heroSection.style.transform = `translate3d(0, ${rate}px, 0)`;
            }, { passive: true });
        }

        // Scroll indicator
        const scrollIndicator = document.querySelector('.hero-scroll-indicator');
        if (scrollIndicator) {
            scrollIndicator.addEventListener('click', () => {
                const featuresSection = document.querySelector('.features-section');
                if (featuresSection) {
                    featuresSection.scrollIntoView({ behavior: 'smooth' });
                }
            });
        }
    }

    /**
     * ==========================================
     * FORMULARIOS Y VALIDACIÓN
     * ==========================================
     */

    function initializeFormEnhancements() {
        // Auto-focus en primer input visible
        const firstVisibleInput = document.querySelector('form input:not([type="hidden"]):first-of-type, form select:first-of-type');
        if (firstVisibleInput && isElementVisible(firstVisibleInput)) {
            setTimeout(() => firstVisibleInput.focus(), 100);
        }

        // Validación en tiempo real
        const formInputs = document.querySelectorAll('input, select, textarea');
        formInputs.forEach(input => {
            input.addEventListener('blur', () => validateField(input));
            input.addEventListener('input', () => clearFieldValidation(input));

            // Formateo especial para campos específicos
            if (input.name === 'Phone') {
                input.addEventListener('input', () => formatPhoneNumber(input));
            }
            if (input.name === 'DocumentNumber') {
                input.addEventListener('input', () => formatDocumentNumber(input));
            }
        });

        // Prevenir envío con Enter en campos inapropiados
        document.querySelectorAll('input[type="text"], input[type="email"], input[type="tel"]').forEach(input => {
            input.addEventListener('keypress', handleEnterKeyPress);
        });

        // Envío de formularios con loading state
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', handleFormSubmit);
        });
    }

    function initializeSearchForm() {
        const searchForm = document.querySelector('.search-form');
        if (!searchForm) return;

        const originSelect = searchForm.querySelector('select[name="Origin"]');
        const destinationSelect = searchForm.querySelector('select[name="Destination"]');

        if (originSelect && destinationSelect) {
            // Validar que origen y destino sean diferentes
            const validateRoute = () => {
                const origin = originSelect.value;
                const destination = destinationSelect.value;

                if (origin && destination && origin === destination) {
                    showToast('El origen y destino deben ser diferentes', 'warning');
                    destinationSelect.value = '';
                    destinationSelect.focus();
                }
            };

            originSelect.addEventListener('change', validateRoute);
            destinationSelect.addEventListener('change', validateRoute);
        }

        // Validación de fecha mínima
        const dateInput = searchForm.querySelector('input[type="date"]');
        if (dateInput) {
            const today = new Date().toISOString().split('T')[0];
            dateInput.min = today;

            dateInput.addEventListener('change', function () {
                if (this.value < today) {
                    this.value = today;
                    showToast('La fecha debe ser hoy o posterior', 'info');
                }
            });
        }
    }

    function initializeCustomSelects() {
        const customSelects = document.querySelectorAll('.custom-select');

        customSelects.forEach(select => {
            // Mejorar la experiencia del select
            select.addEventListener('focus', function () {
                this.parentElement.classList.add('focused');
            });

            select.addEventListener('blur', function () {
                this.parentElement.classList.remove('focused');
            });

            // Animación de la flecha
            select.addEventListener('mousedown', function () {
                const icon = this.nextElementSibling;
                if (icon && icon.classList.contains('select-icon')) {
                    icon.style.transform = 'translateY(-50%) rotate(180deg)';
                }
            });

            select.addEventListener('change', function () {
                const icon = this.nextElementSibling;
                if (icon && icon.classList.contains('select-icon')) {
                    icon.style.transform = 'translateY(-50%) rotate(0deg)';
                }

                // Agregar clase de seleccionado
                if (this.value) {
                    this.classList.add('has-value');
                } else {
                    this.classList.remove('has-value');
                }
            });
        });
    }

    /**
     * ==========================================
     * VALIDACIÓN DE CAMPOS
     * ==========================================
     */

    function validateField(field) {
        const value = field.value.trim();
        const fieldType = field.type;
        const fieldName = field.name;

        // Limpiar validación anterior
        clearFieldValidation(field);

        // Validaciones específicas
        if (field.hasAttribute('required') && !value) {
            addFieldError(field, 'Este campo es obligatorio');
            return false;
        }

        if (fieldType === 'email' && value && !isValidEmail(value)) {
            addFieldError(field, 'Ingrese un email válido');
            return false;
        }

        if (fieldName === 'DocumentNumber' && value && !isValidDocument(value)) {
            addFieldError(field, 'Documento inválido (DNI: 8 dígitos)');
            return false;
        }

        if (fieldName === 'Phone' && value && !isValidPhone(value)) {
            addFieldError(field, 'Teléfono inválido (formato: +51 9XXXXXXXX)');
            return false;
        }

        // Validación exitosa
        addFieldSuccess(field);
        return true;
    }

    function clearFieldValidation(field) {
        field.classList.remove('is-invalid', 'is-valid');
        const feedback = field.parentElement.querySelector('.error-message');
        if (feedback) {
            feedback.remove();
        }
    }

    function addFieldError(field, message) {
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');

        const errorElement = document.createElement('span');
        errorElement.className = 'error-message';
        errorElement.textContent = message;

        field.parentElement.appendChild(errorElement);

        // Animación de entrada del error
        errorElement.style.opacity = '0';
        errorElement.style.transform = 'translateY(-10px)';

        requestAnimationFrame(() => {
            errorElement.style.transition = 'all 0.3s ease';
            errorElement.style.opacity = '1';
            errorElement.style.transform = 'translateY(0)';
        });
    }

    function addFieldSuccess(field) {
        field.classList.add('is-valid');
        field.classList.remove('is-invalid');
    }

    /**
     * ==========================================
     * FORMATEO DE CAMPOS
     * ==========================================
     */

    function formatPhoneNumber(input) {
        let value = input.value.replace(/\D/g, '');

        // Agregar prefijo peruano si es necesario
        if (value.length === 9 && value.startsWith('9')) {
            value = '51' + value;
        }

        if (value.startsWith('51') && value.length === 11) {
            value = '+51 ' + value.substring(2, 3) + ' ' +
                value.substring(3, 6) + ' ' +
                value.substring(6, 9) + ' ' +
                value.substring(9, 11);
        }

        input.value = value;
    }

    function formatDocumentNumber(input) {
        let value = input.value.replace(/\D/g, '');

        // Limitar a 8 dígitos para DNI
        if (value.length > 8) {
            value = value.substring(0, 8);
        }

        input.value = value;
    }

    /**
     * ==========================================
     * VALIDADORES
     * ==========================================
     */

    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    function isValidDocument(doc) {
        return /^\d{8}$/.test(doc);
    }

    function isValidPhone(phone) {
        const cleaned = phone.replace(/\D/g, '');
        return /^(51)?9\d{8}$/.test(cleaned);
    }

    /**
     * ==========================================
     * MANEJO DE EVENTOS
     * ==========================================
     */

    function handleEnterKeyPress(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            const nextInput = getNextInput(e.target);
            if (nextInput) {
                nextInput.focus();
            } else {
                const submitButton = e.target.form?.querySelector('button[type="submit"]');
                if (submitButton) {
                    submitButton.click();
                }
            }
        }
    }

    function handleFormSubmit(e) {
        const form = e.target;
        const submitButton = form.querySelector('button[type="submit"]');

        if (submitButton && !submitButton.disabled) {
            showLoadingState(submitButton);
        }
    }

    function getNextInput(currentInput) {
        const form = currentInput.closest('form');
        const inputs = Array.from(form.querySelectorAll('input, select, textarea'));
        const currentIndex = inputs.indexOf(currentInput);
        return inputs[currentIndex + 1] || null;
    }

    /**
     * ==========================================
     * ESTADOS DE CARGA Y UI
     * ==========================================
     */

    function initializeLoadingStates() {
        // Auto-aplicar loading state a botones de submit
        document.querySelectorAll('button[type="submit"]').forEach(button => {
            button.addEventListener('click', function () {
                if (!this.disabled) {
                    setTimeout(() => showLoadingState(this), 100);
                }
            });
        });
    }

    function showLoadingState(button) {
        const originalContent = button.innerHTML;
        const originalWidth = button.offsetWidth;

        button.disabled = true;
        button.style.width = originalWidth + 'px';
        button.innerHTML = `
            <div class="d-flex align-items-center justify-content-center">
                <div class="spinner-border spinner-border-sm me-2" role="status"></div>
                Procesando...
            </div>
        `;

        // Auto-restaurar después de 10 segundos
        setTimeout(() => {
            if (button.disabled) {
                button.disabled = false;
                button.innerHTML = originalContent;
                button.style.width = '';
            }
        }, 10000);
    }

    /**
     * ==========================================
     * MODALES Y TOOLTIPS
     * ==========================================
     */

    function initializeModals() {
        // Mejorar modales existentes
        document.querySelectorAll('.modal').forEach(modal => {
            modal.addEventListener('show.bs.modal', function () {
                AppState.activeModals.add(this.id);
                document.body.style.overflow = 'hidden';
            });

            modal.addEventListener('hidden.bs.modal', function () {
                AppState.activeModals.delete(this.id);
                if (AppState.activeModals.size === 0) {
                    document.body.style.overflow = '';
                }
            });
        });
    }

    function initializeTooltips() {
        // Inicializar tooltips de Bootstrap
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"], [title]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl, {
                placement: 'top',
                trigger: 'hover focus',
                delay: { show: 500, hide: 100 }
            });
        });
    }

    /**
     * ==========================================
     * ALERTAS Y NOTIFICACIONES
     * ==========================================
     */

    function initializeAlerts() {
        // Auto-hide para alertas
        document.querySelectorAll('.alert').forEach(alert => {
            if (!alert.querySelector('.btn-close')) return;

            setTimeout(() => {
                if (alert.parentNode) {
                    fadeOutElement(alert);
                }
            }, CONFIG.autoHideAlertDelay);
        });
    }

    function showToast(message, type = 'info', duration = 4000) {
        const toastContainer = getOrCreateToastContainer();
        const toast = createToast(message, type);

        toastContainer.appendChild(toast);

        // Animación de entrada
        requestAnimationFrame(() => {
            toast.style.transform = 'translateX(0)';
            toast.style.opacity = '1';
        });

        // Auto-remove
        setTimeout(() => {
            fadeOutElement(toast);
        }, duration);
    }

    function getOrCreateToastContainer() {
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            container.style.zIndex = '9999';
            document.body.appendChild(container);
        }
        return container;
    }

    function createToast(message, type) {
        const toast = document.createElement('div');
        toast.className = `toast-custom toast-${type}`;
        toast.style.transform = 'translateX(100%)';
        toast.style.opacity = '0';
        toast.style.transition = 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)';

        const iconMap = {
            success: 'fas fa-check-circle',
            error: 'fas fa-exclamation-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle'
        };

        toast.innerHTML = `
            <div class="toast-content">
                <i class="${iconMap[type] || iconMap.info}"></i>
                <span>${message}</span>
                <button type="button" class="toast-close" onclick="this.parentElement.parentElement.remove()">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;

        return toast;
    }

    /**
     * ==========================================
     * MEJORAS DE TABLA
     * ==========================================
     */

    function initializeTableEnhancements() {
        // Efectos hover mejorados para tablas
        document.querySelectorAll('.table tbody tr').forEach(row => {
            row.addEventListener('mouseenter', function () {
                this.style.transform = 'scale(1.01)';
                this.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)';
                this.style.zIndex = '1';
            });

            row.addEventListener('mouseleave', function () {
                this.style.transform = 'scale(1)';
                this.style.boxShadow = '';
                this.style.zIndex = '';
            });
        });

        // Mejorar confirmaciones de eliminación
        document.querySelectorAll('[onclick*="confirmDelete"]').forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();
                const id = this.getAttribute('onclick').match(/\d+/)?.[0];
                if (id) {
                    showCustomConfirmDialog(id);
                }
            });
        });
    }

    function showCustomConfirmDialog(id) {
        const modal = document.getElementById('deleteModal');
        if (modal) {
            const bsModal = new bootstrap.Modal(modal);
            bsModal.show();

            const form = modal.querySelector('#deleteForm');
            if (form) {
                const baseAction = form.getAttribute('action').replace(/\/\d+$/, '');
                form.setAttribute('action', `${baseAction}/${id}`);
            }
        }
    }

    /**
     * ==========================================
     * ANIMACIONES DE CONTADOR
     * ==========================================
     */

    function initializeCounterAnimations() {
        const counters = document.querySelectorAll('.stat-number[data-target]');
        if (counters.length === 0) return;

        const observerOptions = {
            threshold: 0.5,
            rootMargin: '0px'
        };

        const counterObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    animateCounter(entry.target);
                    counterObserver.unobserve(entry.target);
                }
            });
        }, observerOptions);

        counters.forEach(counter => counterObserver.observe(counter));
    }

    function animateCounter(element) {
        const target = parseInt(element.getAttribute('data-target'));
        const duration = 2000;
        const increment = target / (duration / 16);
        let current = 0;

        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                current = target;
                clearInterval(timer);
            }

            element.textContent = formatCounterNumber(Math.floor(current), target);
        }, 16);
    }

    function formatCounterNumber(current, target) {
        if (target >= 1000000) {
            return (current / 1000000).toFixed(1) + 'M+';
        } else if (target >= 1000) {
            return (current / 1000).toFixed(0) + 'K+';
        }
        return current.toString();
    }

    /**
     * ==========================================
     * EFECTOS PARALLAX
     * ==========================================
     */

    function initializeParallaxEffects() {
        const parallaxElements = document.querySelectorAll('.floating-feature');

        window.addEventListener('scroll', () => {
            const scrollTop = window.pageYOffset;

            parallaxElements.forEach((element, index) => {
                const speed = 0.5 + (index * 0.1);
                const yPos = -(scrollTop * speed);
                element.style.transform = `translate3d(0, ${yPos}px, 0)`;
            });
        }, { passive: true });
    }

    /**
     * ==========================================
     * OPTIMIZACIONES Y PERFORMANCE
     * ==========================================
     */

    function initializeLazyLoading() {
        const images = document.querySelectorAll('img[data-src]');
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src;
                        img.classList.remove('lazy');
                        imageObserver.unobserve(img);
                    }
                });
            });

            images.forEach(img => imageObserver.observe(img));
        }
    }

    function optimizeImages() {
        // Precargar imágenes críticas
        const criticalImages = document.querySelectorAll('img[data-critical]');
        criticalImages.forEach(img => {
            const link = document.createElement('link');
            link.rel = 'preload';
            link.as = 'image';
            link.href = img.src;
            document.head.appendChild(link);
        });
    }

    function hidePreloader() {
        const preloader = document.querySelector('.preloader');
        if (preloader) {
            fadeOutElement(preloader);
        }
    }

    /**
     * ==========================================
     * UTILIDADES
     * ==========================================
     */

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    function fadeOutElement(element) {
        element.style.transition = 'opacity 0.5s ease-out';
        element.style.opacity = '0';
        setTimeout(() => {
            if (element.parentNode) {
                element.parentNode.removeChild(element);
            }
        }, 500);
    }

    function isElementVisible(element) {
        return element.offsetParent !== null;
    }

    /**
     * ==========================================
     * API GLOBAL PARA COMPATIBILIDAD
     * ==========================================
     */

    // Funciones globales para compatibilidad con código existente
    window.confirmDelete = function (saleId) {
        showCustomConfirmDialog(saleId);
    };

    window.showToast = showToast;

    window.selectRoute = function (origin, destination) {
        const originSelect = document.querySelector('select[name="Origin"]');
        const destinationSelect = document.querySelector('select[name="Destination"]');

        if (originSelect && destinationSelect) {
            originSelect.value = origin;
            destinationSelect.value = destination;

            // Trigger events
            originSelect.dispatchEvent(new Event('change', { bubbles: true }));
            destinationSelect.dispatchEvent(new Event('change', { bubbles: true }));

            // Scroll al formulario
            const searchForm = document.querySelector('.search-form');
            if (searchForm) {
                searchForm.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    };

    // Manejo de errores global
    window.addEventListener('error', function (e) {
        console.warn('Error capturado:', e.error);

        // Solo mostrar al usuario en desarrollo
        if (window.location.hostname === 'localhost') {
            showToast('Se produjo un error. Revisa la consola.', 'error');
        }
    });

    // Performance monitoring
    if ('performance' in window) {
        window.addEventListener('load', () => {
            setTimeout(() => {
                const perfData = performance.getEntriesByType('navigation')[0];
                console.log(`🚀 Página cargada en ${Math.round(perfData.loadEventEnd - perfData.fetchStart)}ms`);
            }, 0);
        });
    }

    console.log('🎉 Transport Agency JavaScript cargado completamente');

})();