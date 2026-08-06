function switchTab(tabName) {
    $('.tab-panel').hide();
    $(`#tab-${tabName}`).show();

    $('.tab-btn').removeClass('active');
    $(`.tab-btn[data-tab="${tabName}"]`).addClass('active');

    loadTabData(tabName);
}

function loadTabData(tabName) {
    if (tabName === 'overview' || tabName === 'tickets') loadManagerReport();
    if (tabName === 'performance') loadResponseTime();
    if (tabName === 'sla') loadSla();
}

function loadManagerReport() {
    $.ajax({
        url: '/Reports?handler=ManagerReport',
        method: 'GET',
        data: { PageNumber: 1, PageSize: 50 },
        success: function (result) {
            const items = result.items;

            $('#ov-total').text(result.totalCount);
            $('#ov-resolved').text(items.filter(i => i.status === 'Resolved').length);
            $('#ov-inprogress').text(items.filter(i => i.status === 'InProgress').length);
            $('#ov-closed').text(items.filter(i => i.status === 'Closed' || i.status === 'Cancelled').length);

            const tbody = $('#reportTicketsBody');
            tbody.empty();
            items.forEach(t => tbody.append(`
                <tr>
                    <td>${t.ticketNumber}</td><td>${t.title}</td><td>${t.status}</td>
                    <td>${t.assignedToAgentName ?? '-'}</td>
                    <td>${new Date(t.createdDate).toLocaleDateString('id-ID')}</td>
                </tr>
            `));
        },
        error: function (xhr) { if (xhr.status === 401) window.location.href = '/Login'; }
    });
}

function loadResponseTime() {
    $.ajax({
        url: '/Reports?handler=ResponseTime',
        method: 'GET',
        success: function (result) {
            $('#perf-avgHours').text(result.averageResponseHours + ' jam');
        }
    });
}

function loadSla() {
    $.ajax({
        url: '/Reports?handler=Sla',
        method: 'GET',
        success: function (result) {
            $('#sla-percentage').text(result.compliancePercentage + '%');
            $('#sla-within').text(result.withinSla);
            $('#sla-total').text(result.totalResolved);
            $('#sla-breached').text(result.breachedSla);
        }
    });
}

$(document).ready(function () {
    switchTab('overview');

    $('.tab-btn').on('click', function () {
        switchTab($(this).data('tab'));
    });

    $('#btnExport').on('click', function () {
        window.location.href = '/Reports?handler=Export';
    });
});