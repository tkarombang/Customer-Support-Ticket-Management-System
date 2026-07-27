let currentPage = 1;

function loadReport(page = 1) {
    currentPage = page;

    const params = {
        StartDate: $('#startDate').val(),
        EndDate: $('#endDate').val(),
        Status: $('#status').val(),
        SearchTerm: $('#searchTerm').val(),
        PageNumber: page,
        PageSize: 10
    };

    $.ajax({
        url: '/Reports/ManagerReport?handler=Filter',
        method: 'GET',
        data: params,
        success: function (result) {
            renderTable(result.items);
            renderPagination(result.pageNumber, result.totalPages);
        },
        error: function (xhr) {
            alert('Gagal memuat data: ' + xhr.status);
        }
    });
}

function renderTable(items) {
    const tbody = $('#reportTableBody');
    tbody.empty();

    if (!items || items.length === 0) {
        tbody.append('<tr><td colspan="6" class="text-center">Tidak ada data</td></tr>');
        return;
    }

    items.forEach(function (t) {
        tbody.append(`
            <tr>
                <td>${t.ticketNumber}</td>
                <td>${t.customerName}</td>
                <td>${t.title}</td>
                <td>${t.status}</td>
                <td>${t.assignedToAgentName ?? '-'}</td>
                <td>${new Date(t.createdDate).toLocaleDateString('id-ID')}</td>
            </tr>
        `);
    });
}

function renderPagination(currentPage, totalPages) {
    const pagination = $('#pagination');
    pagination.empty();

    for (let i = 1; i <= totalPages; i++) {
        const activeClass = i === currentPage ? 'btn-primary' : 'btn-outline-primary';
        pagination.append(`<button class="btn ${activeClass} btn-sm page-btn" data-page="${i}">${i}</button> `);
    }
}

$(document).ready(function () {
    loadReport(1);

    $('#btnFilter').on('click', function () {
        loadReport(1);
    });

    $(document).on('click', '.page-btn', function () {
        loadReport($(this).data('page'));
    });
});