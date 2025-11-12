// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Configure Toastr
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// DataTables Default Configuration
$.extend(true, $.fn.dataTable.defaults, {
    "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
    "pageLength": 10,
    "language": {
        "search": "Search:",
        "lengthMenu": "Show _MENU_ entries",
        "info": "Showing _START_ to _END_ of _TOTAL_ entries",
        "infoEmpty": "Showing 0 to 0 of 0 entries",
        "infoFiltered": "(filtered from _MAX_ total entries)",
        "paginate": {
            "first": "First",
            "last": "Last",
            "next": "Next",
            "previous": "Previous"
        }
    },
    "responsive": true,
    "autoWidth": false
});

// Format currency
function formatCurrency(amount) {
    return '₹' + parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    const options = { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return date.toLocaleDateString('en-IN', options);
}

// Transaction type text
function getTransactionTypeText(type) {
    switch (type) {
        case 1: return 'Deposit';
        case 2: return 'Withdrawal';
        case 3: return 'Transfer';
        case 4: return 'Interest';
        case 5: return 'Fee';
        default: return 'Unknown';
    }
}

// Transaction status text
function getTransactionStatusText(status) {
    switch (status) {
        case 1: return 'Pending';
        case 2: return 'Approved';
        case 3: return 'Rejected';
        default: return 'Unknown';
    }
}

// Application status text
function getApplicationStatusText(status) {
    switch (status) {
        case 1: return 'Pending';
        case 2: return 'Approved';
        case 3: return 'Rejected';
        default: return 'Unknown';
    }
}

// Confirm action
function confirmAction(message) {
    return confirm(message);
}

// Loading overlay
function showLoading() {
    if ($('#loadingOverlay').length === 0) {
        $('body').append('<div id="loadingOverlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 9999; display: flex; align-items: center; justify-content: center;"><div class="spinner-border text-light" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    }
}

function hideLoading() {
    $('#loadingOverlay').remove();
}

// Initialize on document ready
$(document).ready(function () {
    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Confirm delete actions
    $('.btn-danger[data-confirm]').on('click', function (e) {
        if (!confirm($(this).data('confirm'))) {
            e.preventDefault();
        }
    });

    // Numeric input validation
    $('input[type="number"]').on('keypress', function (e) {
        if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 57) && e.which !== 46) {
            return false;
        }
    });

    // PAN uppercase conversion
    $('input[name="PAN"]').on('input', function () {
        $(this).val($(this).val().toUpperCase());
    });
});
