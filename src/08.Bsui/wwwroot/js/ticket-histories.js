let currentPage = 1

$(document).ready(function () {
    loadHistories(1)
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
            renderTable(result.items)
            renderSummary(result.items)
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

function renderSummary(items) {
    $('#totalHistories').text(items.length)
    $('#statusChangedCount').text(items.filter(i => i.action === "StatusChanged").length)
    $('#assigneeChangedCount').text(items.filter(i => i.action === "AssigneeChanged").length)
    $('#commentCount').text(items.filter(i => i.action === "CommentAdded").length)
}

function describeChange(item) {
    if (item.action === 'StatusChanged' && item.previous && item.newStatus) {
        return `Status changed from <b>${item.previousStatus}</b> to <b>${item.newStatus}</b>`
    }
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
                <td><span class="badge bg-primary">${h.action}</span></td>
                <td>${describeChange(h)}</td>
                <td>${h.changedByName}</td>
                <td>${new Date(h.timestamp).toLocaleString('id-ID')}</td>
            </tr>
        `)
    })
}

