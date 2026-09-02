let allTickets = [];

$(document).ready(function () {
    loadTickets()
    loadAssigneeOptions()

    $('#btnFilter').on('click', applyFilters);

    $('#btnReset').on('click', function () {
        $('#searchTerm').val('');
        $('#filterStatus').val('');
        $('#filterAssignee').val('');
        $('#filterPriority').val('');
        $('#filterCategory').val('');
        applyFilters();
    });
});

function loadTickets() {
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
        url: '/Tickets?handler=List',
        method: 'GET',
        success: function (tickets) {
            Swal.close();
            allTickets = tickets
            applyFilters()
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
                    text: 'Gagal memuat tiket. (' + xhr.status + ')'
                });
            }
        }
    });
}


function loadAssigneeOptions() {
    $.ajax({
        url: '/Tickets?handler=Agents',
        method: 'GET',
        success: function (agents) {
            const select = $('#filterAssignee')
            agents.forEach(a => select.append(`<option value="${a.name}">${a.name}</option>`))
        }
    })
}

function applyFilters() {
    const search = $('#searchTerm').val().toLowerCase().trim();
    const status = $('#filterStatus').val();
    const assignee = $('#filterAssignee').val();
    const priority = $('#filterPriority').val();
    const category = $('#filterCategory').val();

    const filtered = allTickets.filter(function (t) {
        const matchSearch = !search ||
            t.ticketNumber.toLowerCase().includes(search) ||
            t.title.toLowerCase().includes(search) ||
            t.customerName.toLowerCase().includes(search);

        const matchStatus = !status || t.status === status
        const matchAssignee = !assignee || t.assignedToAgentName === assignee;
        const matchPriority = !priority || t.priority === priority
        const matchCategory = !category || t.category === category

        return matchSearch && matchStatus && matchAssignee && matchPriority && matchCategory
    })

    renderSummary(allTickets)
    renderTickets(filtered)
    $('#paginationInfo').text(`Menampilkan ${filtered.length} dari ${allTickets.length} tiket`)
}


function renderSummary(tickets) {
    $("#totalTickets").text(tickets.length)
    $("#openCount").text(tickets.filter(t => t.status === "Open").length)
    $("#inProgressCount").text(tickets.filter(t => t.status === "InProgress").length)
    $("#closedCount").text(tickets.filter(t => t.status === "Closed").length)
    $("#cancelledCount").text(tickets.filter(t => t.status === "Cancelled").length)
}

function priorityBadgeClass(priority) {
    if (priority === 'High') return 'bg-danger';
    if (priority === 'Medium') return 'bg-warning';
    return 'bg-secondary';
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

function renderTickets(tickets) {
    //renderSummary(tickets)
    const tbody = $('#ticketTableBody');
    tbody.empty();

    if (!tickets || tickets.length === 0) {
        tbody.append('<tr><td colspan="6" class="text-center">Belum ada tiket</td></tr>');
        return;
    }

    tickets.forEach(function (t) {
        const createdDate = new Date(t.createdDate).toLocaleDateString('id-ID', {
            day: '2-digit',
            month: 'short',
            year: 'numeric'
        })

        tbody.append(`
            <tr>
                <td><a href="/Tickets/Update?id=${t.ticketId}">${t.ticketNumber}</a></td>
                <td>${t.title}</td>
                <td>${t.customerName}</td>
                <td class="is-overdue">
                    <span class="badge ${statusBadgeClass(t.status)}">${formatActionLabel(t.status)}</span>
                    ${t.isOverdue ? '<span class="badge bg-danger">⚠ Overdue</span>' : ''}
                </td>
                <td><span class="badge ${priorityBadgeClass(t.priority)}">${t.priority}</span></td>
                <td>${t.assignedToAgentName ?? '-'}</td>
                <td>${createdDate}</td>
                <td><a href="/Tickets/Update?id=${t.ticketId}" class="btn btn-sm btn-outline-secondary">Update</a></td>
            </tr>
        `);
    });
}

function formatActionLabel(action) {
    return action.replace(/([a-z])([A-Z])/g, '$1 $2')
}