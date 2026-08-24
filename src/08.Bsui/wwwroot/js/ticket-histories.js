let currentPage = 1

$(document).ready(function () {
    loadHistories(1)
    loadUserOption()

    $('#btnFilter').on('click', function () { loadHistories(1); });

    $('#btnReset').on('click', function () {
        $('#searchTerm, #startDate, #endDate').val('');
        $('#filterAction, #filterUser').val('');
        loadHistories(1);
    });

    $(document).on('click', '.page-btn', function () {
        loadHistories($(this).data('page'));
    });
})

function loadHistories(page = 1) {
    Swal.fire({
        title: 'Memuat Data...',
        html: 'Mohon tunggu sebentar.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    currentPage = page

    const params = {
        SearchTerm: $('#searchTerm').val(),
        StartDate: $('#startDate').val(),
        EndDate: $('#endDate').val(),
        Action: $('#filterAction').val(),
        UserId: $('#filterUser').val(),
        PageNumber: page,
        PageSize: 10
    }


    $.ajax({
        url: '/TicketHistories?handler=Filter',
        method: 'GET',
        data: params,
        success: function (result) {
            Swal.close();

            renderTable(result.items);
            renderSummary(result.items);
            renderPagination(result.pageNumber, result.totalPages, result.totalCount);
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                window.location.href = '/Login'
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal memuat tiket. (' + xhr.status + ')'
                });
            }
        }
    })
}

function loadUserOption() {
    $.ajax({
        url: '/TicketHistories?handler=Users',
        method: 'GET',
        success: function (users) {
            const select = $('#filterUser');
            users.forEach(u => select.append(`<option value="${u.userId}">${u.name}</option>`));
        }
    });
}

function renderSummary(items) {
    $('#totalHistories').text(items.length)
    $('#statusChangedCount').text(items.filter(i => i.action === "StatusChanged").length)
    $('#assigneeChangedCount').text(items.filter(i => i.action === "AssigneeChanged").length)
    $('#commentCount').text(items.filter(i => i.action === "CommentAdded").length)
}

function describeChange(item) {
    if (item.action === 'StatusChanged' && item.previousStatus && item.newStatus) {
        return `Status changed from <b>${formatActionLabel(item.previousStatus)}</b> ▶️▶️ <b>${formatActionLabel(item.newStatus)}</b>`
    }
    return formatActionLabel(item.action)
}

function renderTable(items) {
    const tbody = $('#historyTableBody')
    tbody.empty();

    if (!items || items.length === 0) {
        tbody.append('<tr><td colspan="5" class="text-center">Tidak ada riwayat ditemukan</td></tr>')
        return
    }

    items.forEach(function (h) {
        tbody.append(`
            <tr>
                <td>${h.ticketNumber}</td>
                <td><span class="badge ${statusBadgeActionClass(h.action)}">${formatActionLabel(h.action)}</span></td>
                <td>${describeChange(h)}</td>
                <td>${h.changedByName}</td>
                <td>${new Date(h.timestamp).toLocaleString('id-ID')}</td>
            </tr>
        `)
    })
}

function renderPagination(page, totalPages, totalCount) {
    $('#paginationInfo').text(
        `Menampilkan halaman ${page} dari ${totalPages} (${totalCount} total riwayat)`
    )

    const container = $('#paginationButtons')
    container.empty();

    for (let i = 1; i <= totalPages; i++) {
        const activeClass = i === page ? 'btn-primary' : 'btn-outline-secondary';
        container.append(`<button class="btn ${activeClass} btn-sm page-btn" 
            data-page="${i}">
            ${i}
            </button>
        `)
    }
}


function formatActionLabel(action) {
    return action.replace(/([a-z])([A-Z])/g, '$1 $2')
}


function statusBadgeActionClass(status) {
    switch (status) {
        case 'Created': return 'bg-primary';
        case 'StatusChanged': return 'bg-secondary';
        case 'AssigneeChanged': return 'bg-success';
        case 'PriorityChanged': return 'bg-warning';
        case 'CommentAdded': return 'bg-danger';
        case 'TicketUpdate': return 'bg-thernary';

        default: return 'bg-secondary';
    }
}