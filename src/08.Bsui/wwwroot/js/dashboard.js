
$(document).ready(function () {
    Swal.fire({
        title: 'Memuat Data...',
        html: 'Mohon tunggu sebentar.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    loadDashboard();
});

function loadDashboard() {
    Swal.close();
    $.ajax({
        url: '/dashboard?handler=Summary',
        method: 'GET',
        success: function (summary) {
            renderSummary(summary);
            renderStatusChart(summary)
            renderAssigneeChart(summary)
        },
        error: function (xhr) {
            Swal.close();
            if (xhr.status === 401) {
                window.location.href = '/Login';
                return
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal memuat Dashboard. (' + xhr.status + ')'
                });
            }
        }
    });

    $.ajax({
        url: '/Dashboard?handler=TrendData',
        method: 'GET',
        success: function (items) { renderTrendChart(items) },
        error: function (xhr) {
            Swal.close();
            if (xhr.status === 401) {
                window.location.href = '/Login';
                return
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal memuat Dashboard. (' + xhr.status + ')'
                });
            }
        }
    })
}

function renderSummary(summary) {
    $('#totalTickets').text(summary.totalTickets);
    $('#openCount').text(summary.openCount);
    $('#inProgressCount').text(summary.inProgressCount);
    $('#resolvedCount').text(summary.resolvedCount);
    $('#closedCount').text(summary.closedCount);
}

function renderStatusChart(summary) {
     new Chart($('#chartDashStatus'), {
        type: 'doughnut',
        data: {
            labels: ['Open', 'In Progress', 'Resolved', 'Closed'],
            datasets: [
                {
                    data: [summary.openCount, summary.inProgressCount, summary.resolvedCount, summary.closedCount],
                    backgroundColor: ['#2563eb', '#f59e0b', '#16a34a', '#64748b']
                }]
        },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
    });
}

function renderTrendChart(items) {
    const grouped = {}

    items.forEach(i => {
        const date = i.createdDate.split('T')[0]

        if (!grouped[date]) {
            grouped[date] = {
                open: 0,
                closed: 0
            };
        }

        if (i.status === 'Open') {
            grouped[date].open++
        }

        if (i.status === 'Closed') {
            grouped[date].closed++
        }
    })

    const labels = Object.keys(grouped).sort()

    new Chart($('#chartDashTrend'), {
        type: 'line',
        data: {
            labels,
            datasets: [
                {
                    label: 'Open',
                    data: labels.map(d => grouped[d].open),
                    borderColor: '#2563eb',
                    backgroundColor: '#2562ec',
                    tension: 0.3
                },
                {
                    label: 'Closed',
                    data: labels.map(d => grouped[d].closed),
                    borderColor: '#64748b',
                    backgroundColor: '#64748b',
                    tension: 0.3
                }
            ],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false,
                }
            }
        }
    });
}

function renderAssigneeChart(summary) {
    const workload = summary.workloadPerAgent || []
    new Chart($('#chartDashAssignee'), {
        type: 'bar',
        data: {
            labels: workload.map(w => w.agentName),
            datasets: [{
                label: 'Jumlah Tiket',
                data: workload.map(w => w.assignedTicketCount),
                backgroundColor: '#2563eb'
            }]
        },
        options: { responsive: true, maintainAspectRatio: false, indexAxis: 'y', plugins: { legend: { display: false } } }
    });
}
