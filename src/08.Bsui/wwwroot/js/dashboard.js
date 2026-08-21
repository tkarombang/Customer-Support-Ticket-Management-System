
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

function formatChangePercent(percent) {
    const arrow = percent >= 50 ? `<i class="fa-solid fa-angles-up" style="font-size: 0.6rem;"></i>` : `<i class="fa-solid fa-angles-down" style="font-size: 0.6rem;"></i>`;
    const colorClass = percent >= 50 ? 'color: #089de7;' : 'color: #e75208;'
    return `<span style="${colorClass}">${arrow} ${Math.abs(percent)}%</span> dari periode lalu`
}

function renderSummary(summary) {
    $('#totalTickets').text(summary.totalTickets);
    $('#openCount').text(summary.openCount);
    $('#inProgressCount').text(summary.inProgressCount);
    $('#resolvedCount').text(summary.resolvedCount);
    $('#closedCount').text(summary.closedCount);
    $('#workloadPerAgent').text(summary.workloadPerAgent.length)

    $('#totalChange').html(formatChangePercent(summary.totalChangePercent))
    $('#resolvedChange').html(formatChangePercent(summary.resolvedChangePercent))
    $('#inProgressChange').html(formatChangePercent(summary.inProgressChangePercent))
    $('#closedChange').html(formatChangePercent(summary.closedChangePercent))
    $('#openChange').html(formatChangePercent(summary.openChangePercent))
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
            <td><span class="badge ${statusBadgeClass(t.status)}">${t.status}</span></td>
            <td>${t.assignedToAgentName ?? 'Unassigned'}</td>
        </tr>
    `).join('')

    tbody.html(`
        <tr>
            <th>Ticket Number</th>
            <th>Title</th>
            <th>Customer</th>
            <th>Status</th>
            <th>Assigned To</th>
        </tr>${rows
    }`)
}


function renderActivityTimeline(items) {
    const container = $('#activityTimeline')

    if (!items || items.length === 0) {
        container.html('<p class="text-muted">Belum Ada Aktivitas</p>')
        return
    }

    const html = items.map(h => {
        const activity = getActivityIcon(h.action, h.newStatus)

        let label = formatActionLabel(h.action, h.newStatus)
        let detail = `Ticket ${h.ticketNumber}`


        if (h.action === 'StatusChanged' && h.previousStatus && h.newStatus) {
            detail += `• ${h.previousStatus} → ${h.newStatus}`
        } 

        if (h.changeByName) {
            detail += ` • oleh ${h.changeByName}`;
        }


        return `
            <div class="timeline-item">
                <div class="timeline-dot ${activity.className}">
                    <i class="${activity.icon} ${activity.className}"></i>
                </div>
                <div>
                    <div><b>${label}</b></div>
                    <div class="text-muted" style="font-size:12px;">${detail}</div>
                    <div class="text-muted" style="font-size:11px;">${formatActivityTime(h.timestamp)}</div>
                </div>
            </div>
        `
    }).join('')

    container.html(html)
}


function formatActionLabel(action, newStatus) {
    if (!action) return ''

    switch (action) {
        case 'Created':
            return 'Ticket created';
        case 'StatusChanged':
            return `Status changed to ${formatCamelCase(newStatus)}`;
        case 'AssigneChanged':
            return 'Assign Changed';
        case 'PriorityChanged':
            return 'Priority Changed';
        case 'CommentAdded':
            return 'Comment Added'
        case 'TicketUpdate':
            return 'Ticket Update';
        default:
            return formatCamelCase(action);
    }
}


function statusBadgeClass(status) {
    switch (status) {
        case 'Open': return 'bg-primary';
        case 'InProgress': return 'bg-warning';
        case 'Resolved': return 'bg-success';
        case 'Closed': return 'bg-secondary';
        case 'Cancelled': return 'bg-danger';
        default: return 'bg-secondary';
    }
}


function getActivityIcon(action, newStatus) {

    switch (action) {

        case 'Created':
            return {
                icon: 'fa-solid fa-ticket',
                className: 'activity-created'
            };

        case 'StatusChanged':

            switch ((newStatus || '').toLowerCase()) {

                case 'open':
                    return {
                        icon: 'fa-solid fa-arrow-right-to-bracket',
                        className: 'activity-open'
                    };

                case 'in progress':
                    return {
                        icon: 'fa-solid fa-clock',
                        className: 'activity-progress'
                    };

                case 'resolved':
                    return {
                        icon: 'fa-solid fa-circle-check',
                        className: 'activity-resolved'
                    };

                case 'closed':
                    return {
                        icon: 'fa-solid fa-check',
                        className: 'activity-closed'
                    };

                case 'cancelled':
                case 'canceled':
                    return {
                        icon: 'fa-solid fa-xmark',
                        className: 'activity-cancelled'
                    };

                default:
                    return {
                        icon: 'fa-solid fa-arrows-rotate',
                        className: 'activity-default'
                    };
            }

        case 'AssigneeChanged':
            return {
                icon: 'fa-solid fa-user-check',
                className: 'activity-assignee'
            };

        case 'PriorityChanged':
            return {
                icon: 'fa-solid fa-flag',
                className: 'activity-priority'
            };

        case 'CommentAdded':
            return {
                icon: 'fa-solid fa-comment',
                className: 'activity-comment'
            };

        case 'TicketUpdated':
            return {
                icon: 'fa-solid fa-pen',
                className: 'activity-updated'
            };

        default:
            return {
                icon: 'fa-solid fa-circle-info',
                className: 'activity-default'
            };
    }
}




function parseApiDate(timeStamp) {
    if (!timeStamp) return null

    const normalized = timeStamp.replace(/\.(\d{3})\d+/, '.$1')
    return new Date(normalized)
}

function formatActivityTime(timestamp) {
    const date = parseApiDate(timestamp);

    if (!date || isNaN(date.getTime())) return '-'

    return date.toLocaleString('id-ID', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    });
}

function formatCamelCase(text) {
    if (!text) return null
    return text.replace(/([a-z])([A-Z])/g, '$1 $2')
}
