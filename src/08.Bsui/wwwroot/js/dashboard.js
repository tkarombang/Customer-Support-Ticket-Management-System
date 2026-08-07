
$(document).ready(function () {
    loadDashboard();
});

function loadDashboard() {
    Swal.fire({
        title: 'Memuat Data...',
        html: 'Mohon tunggu sebentar.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        url: '/dashboard?handler=Summary',
        method: 'GET',
        success: function (summary) {
            Swal.close();
            renderSummary(summary);
            renderStatusChart(summary)
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
}

function renderSummary(summary) {
    $('#totalTickets').text(summary.totalTickets);
    $('#openCount').text(summary.openCount);
    $('#inProgressCount').text(summary.inProgressCount);
    $('#resolvedCount').text(summary.resolvedCount);
    $('#closedCount').text(summary.closedCount);

    //const tbody = $('#workloadTableBody');
    //tbody.empty();

    //if (!summary.workloadPerAgent || summary.workloadPerAgent.length === 0) {
    //    tbody.append('<tr><td colspan="2" class="text-center">Belum ada tiket yang ditugaskan</td></tr>');
    //    return;
    //}

    //summary.workloadPerAgent.forEach(function (w) {
    //    tbody.append(`<tr><td>${w.agentName}</td><td>${w.assignedTicketCount}</td></tr>`);
    //});
}

function renderStatusChart(summary) {
     new Chart($('#chartDashStatus'), {
        type: 'doughnut',
        data: {
            labels: ['Open', 'In Progress', 'Resolved', 'Closed'],
            datasets: [{ data: [summary.openCount, summary.inProgressCount, summary.resolvedCount, summary.closedCount],
                backgroundColor: ['#2563eb', '#f59e0b', '#16a34a', '#64748b'] }]
        },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
    });
}
