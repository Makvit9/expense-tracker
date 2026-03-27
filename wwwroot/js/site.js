// Site-wide JavaScript for Expense Tracker

$(document).ready(function () {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Make all Bootstrap tables scrollable on mobile
    $('table.table').wrap('<div class="table-responsive"></div>');

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert:not(.alert-permanent)').fadeOut('slow');
    }, 5000);

    // Confirm delete actions
    $('.btn-danger[href*="Delete"]').click(function (e) {
        if (!confirm('Are you sure you want to delete this item?')) {
            e.preventDefault();
            return false;
        }
    });

    // Format currency inputs
    $('input[type="number"][step="0.01"]').on('blur', function () {
        var val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2));
        }
    });

    // Auto-focus first input in forms
    $('form input:visible:enabled:first').focus();

    // Loading state for form submissions
    $('form').on('submit', function () {
        var $submitBtn = $(this).find('button[type="submit"]');
        $submitBtn.prop('disabled', true);
        var originalText = $submitBtn.html();
        $submitBtn.html('<i class="fas fa-spinner fa-spin"></i> Processing...');

        // Re-enable after 3 seconds (in case of validation errors)
        setTimeout(function () {
            $submitBtn.prop('disabled', false);
            $submitBtn.html(originalText);
        }, 3000);
    });
});

// Utility function to format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Utility function to calculate week of month
function getWeekOfMonth(date) {
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var dayOfMonth = date.getDate();
    var firstDayOfWeek = firstDay.getDay();
    
    return Math.ceil((dayOfMonth + firstDayOfWeek) / 7);
}

// Auto-calculate week number when date changes
$('#Date').on('change', function () {
    var selectedDate = new Date($(this).val());
    if (!isNaN(selectedDate.getTime())) {
        var weekNum = getWeekOfMonth(selectedDate);
        $('#WeekNumber').val(weekNum);
        
        // Also update month and year if needed
        $('#Month').val(selectedDate.getMonth() + 1);
        $('#Year').val(selectedDate.getFullYear());
    }
});
