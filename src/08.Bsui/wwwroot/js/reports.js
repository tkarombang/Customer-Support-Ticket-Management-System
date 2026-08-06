let statusChart, trendChart;

$(document).ready(function () {
    switchTab('overview');

    $('.tab-btn').on('click', function () {
        switchTab($(this).data('tab'));
    });

    $('#btnExport').on('click', function () {
        window.location.href = '/Reports?handler=Export';
    });
});


function getDateRange() {
    return {
        startDate: $("#reportStartDate").val() || null,
        endDate: $("#reportEndDate").val() || null
    }
}

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
        data: { PageNumber: 1, PageSize: 100, ...getDateRange() },
        success: function (result) {
            const items = result.items;

            $('#ov-total').text(result.totalCount);
            $('#ov-resolved').text(items.filter(i => i.status === 'Resolved').length);
            $('#ov-inprogress').text(items.filter(i => i.status === 'InProgress').length);
            $('#ov-closed').text(items.filter(i => i.status === 'Closed' || i.status === 'Cancelled').length);

            renderStatusChart(items)
            renderTrendChart(items)

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


function renderStatusChart(items) {
    const counts = { Open: 0, InProgress: 0, Resolved: 0, Closed: 0, Cancelled: 0 };
    items.forEach(i => { if (counts[i.status] !== undefined) counts[i.status]++; });

    if (statusChart) statusChart.destroy();
    statusChart = new Chart($('#chartStatusDonut'), {
        type: 'doughnut',
        data: {
            labels: Object.keys(counts),
            datasets: [{
                data: Object.values(counts),
                backgroundColor: ['#2563eb', '#f59e0b', '#16a34a', '#64748b', '#dc2626']
            }]
        },
        options: { plugins: { legend: { position: 'bottom' } } }
    });
}

function renderTrendChart(items) {
    // Kelompokkan jumlah tiket dibuat per tanggal
    const grouped = {};
    items.forEach(i => {
        const date = i.createdDate.split('T')[0];
        grouped[date] = (grouped[date] || 0) + 1;
    });

    const labels = Object.keys(grouped).sort();
    const data = labels.map(d => grouped[d]);

    if (trendChart) trendChart.destroy();
    trendChart = new Chart($('#chartTrend'), {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Tiket Dibuat',
                data: data,
                borderColor: '#2563eb',
                tension: 0.3
            }]
        },
        options: { plugins: { legend: { display: false } } }
    });
}

function loadResponseTime() {
    $.ajax({
        url: '/Reports?handler=ResponseTime',
        method: 'GET',
        data: getDateRange(),
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