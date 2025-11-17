// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ============================================
// TOASTR CONFIGURATION
// ============================================
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// ============================================
// DATATABLES DEFAULT CONFIGURATION
// ============================================
$.extend(true, $.fn.dataTable.defaults, {
    "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
    "pageLength": 25,
    "language": {
        "search": "Search:",
        "lengthMenu": "Show _MENU_ entries",
        "info": "Showing _START_ to _END_ of _TOTAL_ entries",
        "infoEmpty": "No entries available",
        "infoFiltered": "(filtered from _MAX_ total entries)",
        "zeroRecords": "No matching records found",
        "emptyTable": "No data available in table",
        "paginate": {
            "first": "First",
            "last": "Last",
            "next": "Next",
            "previous": "Prev"
        }
    },
    "responsive": true,
    "autoWidth": false,
    "order": [[0, "desc"]]
});

// Fix for DataTables select dropdown display issue
$(document).on('init.dt', function (e, settings) {
    $('.dataTables_length select').css({
        '-webkit-appearance': 'menulist',
        '-moz-appearance': 'menulist',
        'appearance': 'menulist',
        'background-color': 'white',
        'line-height': 'normal'
    });
});

// ============================================
// UTILITY FUNCTIONS
// ============================================

// Format currency with Indian Rupee symbol
function formatCurrency(amount) {
    if (isNaN(amount)) return '₹0.00';
    return '₹' + parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

// Format date in Indian format
function formatDate(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return 'Invalid Date';
    const options = {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    };
    return date.toLocaleDateString('en-IN', options);
}

// Format date without time
function formatDateOnly(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return 'Invalid Date';
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return date.toLocaleDateString('en-IN', options);
}

// Transaction type text and badge
function getTransactionTypeText(type) {
    const types = {
        1: 'Deposit',
        2: 'Withdrawal',
        3: 'Transfer',
        4: 'Interest',
        5: 'Fee'
    };
    return types[type] || 'Unknown';
}

function getTransactionTypeBadge(type) {
    const badges = {
        1: '<span class="badge bg-success">Deposit</span>',
        2: '<span class="badge bg-danger">Withdrawal</span>',
        3: '<span class="badge bg-info">Transfer</span>',
        4: '<span class="badge bg-primary">Interest</span>',
        5: '<span class="badge bg-warning">Fee</span>'
    };
    return badges[type] || '<span class="badge bg-secondary">Unknown</span>';
}

// Transaction status text and badge
function getTransactionStatusText(status) {
    const statuses = {
        1: 'Pending',
        2: 'Approved',
        3: 'Rejected'
    };
    return statuses[status] || 'Unknown';
}

function getTransactionStatusBadge(status) {
    const badges = {
        1: '<span class="badge bg-warning">Pending</span>',
        2: '<span class="badge bg-success">Approved</span>',
        3: '<span class="badge bg-danger">Rejected</span>'
    };
    return badges[status] || '<span class="badge bg-secondary">Unknown</span>';
}

// Application status text and badge
function getApplicationStatusText(status) {
    const statuses = {
        1: 'Pending',
        2: 'Approved',
        3: 'Rejected'
    };
    return statuses[status] || 'Unknown';
}

function getApplicationStatusBadge(status) {
    const badges = {
        1: '<span class="badge bg-warning">Pending</span>',
        2: '<span class="badge bg-success">Approved</span>',
        3: '<span class="badge bg-danger">Rejected</span>'
    };
    return badges[status] || '<span class="badge bg-secondary">Unknown</span>';
}

// Confirm action with custom message
function confirmAction(message) {
    return confirm(message || 'Are you sure you want to perform this action?');
}

// Loading overlay functions
function showLoading(message) {
    if ($('#loadingOverlay').length === 0) {
        const loadingHtml = `
            <div id="loadingOverlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; 
                 background: rgba(0,0,0,0.7); z-index: 99999; display: flex; align-items: center; 
                 justify-content: center; flex-direction: column;">
                <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                    <span class="visually-hidden">Loading...</span>
                </div>
                ${message ? `<p class="text-white mt-3">${message}</p>` : ''}
            </div>
        `;
        $('body').append(loadingHtml);
    }
}

function hideLoading() {
    $('#loadingOverlay').fadeOut(300, function () {
        $(this).remove();
    });
}

// ============================================
// FORM VALIDATION HELPERS
// ============================================

// Validate email format
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Validate phone number (Indian format)
function isValidPhone(phone) {
    const phoneRegex = /^[6-9]\d{9}$/;
    return phoneRegex.test(phone.replace(/\s+/g, ''));
}

// Validate PAN number
function isValidPAN(pan) {
    const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;
    return panRegex.test(pan);
}

// Validate Aadhar number
function isValidAadhar(aadhar) {
    const aadharRegex = /^\d{12}$/;
    return aadharRegex.test(aadhar.replace(/\s+/g, ''));
}

// ============================================
// DOCUMENT READY FUNCTIONS
// ============================================
$(document).ready(function () {

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert:not(.alert-permanent)').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);

    // Confirm delete/dangerous actions
    $('.btn-danger[data-confirm], [data-confirm]').on('click', function (e) {
        const message = $(this).data('confirm') || 'Are you sure you want to delete this item?';
        if (!confirmAction(message)) {
            e.preventDefault();
            return false;
        }
    });

    // Numeric input validation
    $('input[type="number"]').on('keypress', function (e) {
        // Allow: backspace, delete, tab, escape, enter
        if ($.inArray(e.which, [8, 46, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
            (e.which === 65 && e.ctrlKey === true) ||
            (e.which === 67 && e.ctrlKey === true) ||
            (e.which === 86 && e.ctrlKey === true) ||
            (e.which === 88 && e.ctrlKey === true)) {
            return;
        }
        // Ensure it's a number or decimal point
        if ((e.which < 48 || e.which > 57) && e.which !== 46) {
            e.preventDefault();
        }
        // Ensure only one decimal point
        if (e.which === 46 && $(this).val().indexOf('.') !== -1) {
            e.preventDefault();
        }
    });

    // PAN uppercase conversion and validation
    $('input[name="PAN"], input[id*="PAN"]').on('input', function () {
        let value = $(this).val().toUpperCase().replace(/[^A-Z0-9]/g, '');
        if (value.length > 10) {
            value = value.substring(0, 10);
        }
        $(this).val(value);
    });

    // Aadhar formatting (add spaces for readability)
    $('input[name="AadharNumber"], input[id*="Aadhar"]').on('input', function () {
        let value = $(this).val().replace(/\s+/g, '').replace(/[^0-9]/g, '');
        if (value.length > 12) {
            value = value.substring(0, 12);
        }
        // Format as XXXX XXXX XXXX
        if (value.length > 4) {
            value = value.substring(0, 4) + ' ' + value.substring(4);
        }
        if (value.length > 9) {
            value = value.substring(0, 9) + ' ' + value.substring(9);
        }
        $(this).val(value);
    });

    // Phone number validation (only 10 digits)
    $('input[name*="Phone"], input[name*="Mobile"], input[id*="Phone"], input[id*="Mobile"]').on('input', function () {
        let value = $(this).val().replace(/[^0-9]/g, '');
        if (value.length > 10) {
            value = value.substring(0, 10);
        }
        $(this).val(value);
    });

    // Disable submit button on form submission to prevent double-submit
    $('form').on('submit', function () {
        const $form = $(this);
        const $submitBtn = $form.find('button[type="submit"], input[type="submit"]');

        // Don't disable if form is invalid
        if ($form[0].checkValidity && !$form[0].checkValidity()) {
            return true;
        }

        $submitBtn.prop('disabled', true);
        $submitBtn.html('<span class="spinner-border spinner-border-sm me-2"></span>Processing...');

        // Re-enable after 5 seconds as a fallback
        setTimeout(function () {
            $submitBtn.prop('disabled', false);
            $submitBtn.html($submitBtn.data('original-text') || 'Submit');
        }, 5000);
    });

    // Store original button text
    $('button[type="submit"], input[type="submit"]').each(function () {
        $(this).data('original-text', $(this).html());
    });

    // Smooth scroll to top button
    if ($('#scrollToTop').length === 0) {
        $('body').append('<button id="scrollToTop" style="display: none; position: fixed; bottom: 70px; right: 20px; z-index: 9998; border: none; outline: none; background-color: #1e3a5f; color: white; cursor: pointer; padding: 12px 15px; border-radius: 50%; font-size: 18px; box-shadow: 0 4px 8px rgba(0,0,0,0.3);"><i class="fas fa-arrow-up"></i></button>');
    }

    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('#scrollToTop').fadeIn();
        } else {
            $('#scrollToTop').fadeOut();
        }
    });

    $('#scrollToTop').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 600);
        return false;
    });

    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});

// ============================================
// AJAX ERROR HANDLER
// ============================================
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    hideLoading();
    toastr.error('An error occurred while processing your request. Please try again.');
    console.error('Ajax Error:', thrownError);
});

// ============================================
// EXPORT FUNCTIONS FOR GLOBAL USE
// ============================================
window.bankingApp = {
    formatCurrency: formatCurrency,
    formatDate: formatDate,
    formatDateOnly: formatDateOnly,
    showLoading: showLoading,
    hideLoading: hideLoading,
    confirmAction: confirmAction,
    isValidEmail: isValidEmail,
    isValidPhone: isValidPhone,
    isValidPAN: isValidPAN,
    isValidAadhar: isValidAadhar
};
