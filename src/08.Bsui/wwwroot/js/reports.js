let statusChart, trendChart, assigneeChart, categoryChart, priorityChart, slaTrendChart;
const defaultOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            position: 'bottom'
        }
    }
};


$(document).ready(function () {
    switchTab('overview');

    $('.tab-btn').on('click', function () {
        switchTab($(this).data('tab'));
    });

    $('#btnReset').on('click', function () {
        $('#reportStartDate, #reportEndDate').val('');
        switchTab('overview');
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
    if (tabName === 'overview') { loadManagerReport(); loadResponseTime(true); loadSla(true); }
    if (tabName === 'tickets') loadManagerReport();
    if (tabName === 'performance') loadResponseTime(false);
    if (tabName === 'sla') loadSla(false);
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
            renderAssigneeChart(items);
            renderCategoryChart(items);
            renderPriorityChart(items);
            renderCategoryTable(items);
            renderRecentClosedTable(items);

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
    items.forEach(i => {
        if (counts[i.status] !== undefined) counts[i.status]++;
    });

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
        options: defaultOptions
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
        options: defaultOptions
    });
}

function renderAssigneeChart(items) {
    const counts = {};
    items.forEach(i => {
        const name = i.assignedToAgentName ?? 'Unassigned';
        counts[name] = (counts[name] || 0) + 1;
    });
    const top6 = Object.entries(counts).sort((a, b) => b[1] - a[1]).slice(0, 6);

    if (assigneeChart) assigneeChart.destroy();
    assigneeChart = new Chart($('#chartAssignee'), {
        type: 'bar',
        data: { labels: top6.map(x => x[0]), datasets: [{ label: 'Jumlah Tiket', data: top6.map(x => x[1]), backgroundColor: '#2563eb' }] },
        options: { maintainAspectRatio: false, indexAxis: 'y', plugins: { legend: { display: false } } }
    });
}

function renderCategoryChart(items) {
    const counts = {};
    items.forEach(i => { counts[i.category] = (counts[i.category] || 0) + 1; });

    if (categoryChart) categoryChart.destroy();
    categoryChart = new Chart($('#chartCategory'), {
        type: 'doughnut',
        data: { labels: Object.keys(counts), datasets: [{ data: Object.values(counts), backgroundColor: ['#2563eb', '#f59e0b', '#16a34a', '#7c3aed'] }] },
        options: defaultOptions
    });
}

function renderPriorityChart(items) {
    const counts = { Low: 0, Medium: 0, High: 0 };
    items.forEach(i => { if (counts[i.priority] !== undefined) counts[i.priority]++; });

    if (priorityChart) priorityChart.destroy();
    priorityChart = new Chart($('#chartPriority'), {
        type: 'doughnut',
        data: { labels: Object.keys(counts), datasets: [{ data: Object.values(counts), backgroundColor: ['#16a34a', '#f59e0b', '#dc2626'] }] },
        options: defaultOptions
    });
}


function renderCategoryTable(items) {
    const counts = {};
    items.forEach(i => { counts[i.category] = (counts[i.category] || 0) + 1; });
    const total = items.length || 1;

    const rows = Object.entries(counts).map(([cat, count]) =>
        `<tr><td>${cat}</td><td>${count}</td><td>${(count / total * 100).toFixed(1)}%</td></tr>`
    ).join('');

    $('#categoryTable').html(`<tr><th>Category</th><th>Total</th><th>% of Total</th></tr>${rows}`);
}

function renderRecentClosedTable(items) {
    const closed = items
        .filter(i => i.status === 'Closed')
        .sort((a, b) => new Date(b.updatedDate) - new Date(a.updatedDate))
        .slice(0, 5);

    if (closed.length === 0) {
        $('#recentClosedTable').html('<tr><td>Belum ada tiket closed</td></tr>');
        return;
    }

    const rows = closed.map(t =>
        `<tr><td>${t.ticketNumber}</td><td>${t.title}</td><td>${t.updatedDate ? new Date(t.updatedDate).toLocaleDateString('id-ID') : '-'}</td></tr>`
    ).join('');

    $('#recentClosedTable').html(`<tr><th>Ticket Number</th><th>Title</th><th>Closed At</th></tr>${rows}`);
}

function loadResponseTime(isOverview) {
    $.ajax({
        url: '/Reports?handler=ResponseTime',
        method: 'GET',
        data: getDateRange(),
        success: function (result) {
            if (isOverview) $('#ov-avgHours').text(result.averageResponseHours + ' jam');
            else $('#perf-avgHours').text(result.averageResponseHours + ' jam');
        }
    });
}

function loadSla(isOverview) {
    $.ajax({
        url: '/Reports?handler=Sla',
        method: 'GET',
        data: getDateRange(),
        success: function (result) {
            if (isOverview) {
                $('#ov-slaPercentage').text(result.compliancePercentage + '%');
                $('#ov-slaWithin').text(result.withinSla);
                $('#ov-slaTotal').text(result.totalResolved);
                renderSlaTrendChart(result.trend);
            } else {
                $('#sla-percentage').text(result.compliancePercentage + '%');
                $('#sla-within').text(result.withinSla);
                $('#sla-total').text(result.totalResolved);
                $('#sla-breached').text(result.breachedSla);
            }
        }
    });
}

function renderSlaTrendChart(trend) {
    if (slaTrendChart) slaTrendChart.destroy();
    slaTrendChart = new Chart($('#chartSlaTrend'), {
        type: 'line',
        data: {
            labels: trend.map(t => new Date(t.date).toLocaleDateString('id-ID')),
            datasets: [{ label: 'SLA Compliance %', data: trend.map(t => t.compliancePercentage), borderColor: '#16a34a', tension: 0.3 }]
        },
        options: { maintainAspectRatio: false, scales: { y: { min: 0, max: 100 } }, plugins: { legend: { display: false } } }
    });
}

$(document).ready(function () {
    switchTab('overview');
    $('.tab-btn').on('click', function () { switchTab($(this).data('tab')); });
    $('#btnApplyDateRange').on('click', function () { loadTabData($('.tab-btn.active').data('tab')); });
    $('#btnExport').on('click', function () { window.location.href = '/Reports?handler=Export'; });
});