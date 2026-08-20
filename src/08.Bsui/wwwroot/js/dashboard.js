
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
        success: function (items) {
            renderTrendChart(items)
            renderRecentTickets(items)
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
    })

    $.ajax({
        url: 'TicketHistories?handler=Filter',
        method: 'GET',
        data: { PageNumber: 1, PageSize: 5 },
        success: function (result) { renderActivityTimeline(result.items) }
    })

}

function formatChange(percent) {
    const arrow = percent >= 0 ? `<i class="fa-solid fa-angles-up" style="font-size: 0.6rem;"></i>` : `<i class="fa-solid fa-angles-down" style="font-size: 0.6rem;"></i>`;
    const colorClass = percent >= 0 ? 'color: #089de7;' : 'color: #e75208;'
    return `<span style="${colorClass}">${arrow} ${Math.abs(percent)}%</span> dari periode lalu`
}

function renderSummary(summary) {
    $('#totalTickets').text(summary.totalTickets);
    $('#openCount').text(summary.openCount);
    $('#inProgressCount').text(summary.inProgressCount);
    $('#resolvedCount').text(summary.resolvedCount);
    $('#closedCount').text(summary.closedCount);
    $('#workloadPerAgent').text(summary.workloadPerAgent.length)

    $('#totalChange').html(formatChange(summary.totalChangePercent))
    $('#resolvedChange').html(formatChange(summary.resolvedChangePercent))
    $('#inProgressChange').html(formatChange(summary.inProgressChangePercent))
    $('#closedChange').html(formatChange(summary.closedChangePercent))
    $('#openChange').html(formatChange(summary.openChangePercent))
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

function renderRecentTickets(items) {
    const recent = [...items].sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate)).slice(0, 5)
    const tbody = $('#recentTicketsTable')

    if (recent.length === 0) {
        tbody.html('<tr><td>Belum Ada Tiket</td></tr>')
        return
    }

    const rows = recent.map(t => `
        <tr>
            <td><a href="/Tickets/Update?id=${t.ticketId}">${t.ticketNumber}</a></td>
            <td>${t.title}</td>
            <td>${t.customerName}</td>
            <td><span class="badge bg-primary">${t.status}</span></td>
            <td>${t.assignedTicketName ?? 'Unassigned'}</td>
        </tr>
    `).join('')

    tbody.html(`
        <tr>
            <th>Ticket Number</th>
            <th>Customer</th>
            <th>Status</th>
            <th>Assigned To</th>
        </tr>${rows
    }`)
}


function formatActionLabel(action) {
    return action.replace(/([a-z])([A-Z])/g, '$1 $2')
}

function renderActivityTimeline(items) {
    const container = $('#activityTimeline')

    if (!items || items.length === 0) {
        container.html('<p class="text-muted">Belum Ada Aktivitas</p>')
        return
    }

    const html = items.map(h => {
        let label = formatActionLabel(h.action)
        let detail = h.ticketNumber
        if (h.action === 'StatusChanged' && h.previousStatus && h.newStatus) {
            detail = `Ticket ${h.ticketNumber} oleh ${h.changeByName}`;
        }

        return `
            <div class="timeline-item">
                <div class="timeline-dot"></div>
                <div>
                    <div><b>${label}</b></div>
                    <div class="text-muted" style="font-size:12px;">${detail}</div>
                    <div class="text-muted" style="font-size:11px;">${new Date(h.timestamp).toLocaleString('id-ID')}</div>
                </div>
            </div>
        `
    }).join('')

    container.html(html)
}
