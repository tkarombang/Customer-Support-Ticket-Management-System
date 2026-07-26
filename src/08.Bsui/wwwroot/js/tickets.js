function loadTickets() {
    $.ajax({
        url: '/Tickets?handler=List',
        method: 'GET',
        success: function (tickets) {
            renderTickets(tickets);
        },
        error: function (xhr) {
            if (xhr.status === 401) window.location.href = '/Login';
            else alert('Gagal memuat tiket: ' + xhr.status);
        }
    });
}

function renderTickets(tickets) {
    const tbody = $('#ticketTableBody');
    tbody.empty();

    if (!tickets || tickets.length === 0) {
        tbody.append('<tr><td colspan="6" class="text-center">Belum ada tiket</td></tr>');
        return;
    }

    tickets.forEach(function (t) {
        const isClosed = t.status === 'Closed';
        const statusOptions = ['Open', 'InProgress', 'Resolved', 'Closed']
            .map(s => `<option value="${s}" ${s === t.status ? 'selected' : ''}>${s}</option>`)
            .join('');

        let actionHtml = '';
        if (!isClosed) {
            actionHtml += `
                <select class="form-select form-select-sm status-select" data-id="${t.ticketId}" ${isClosed ? 'disabled' : ''}>
                    ${statusOptions}
                </select>
                <button class="btn btn-sm btn-primary btn-save-status" data-id="${t.ticketId}">Simpan</button>
            `;
            if (currentRole === 'Manager') {
                actionHtml += `<input type="number" class="form-control form-control-sm assign-input" data-id="${t.ticketId}" placeholder="ID Agent" style="width:90px;display:inline-block;" />
                <button class="btn btn-sm btn-secondary btn-assign" data-id="${t.ticketId}">Assign</button>`;
            }
        } else {
            actionHtml = '<span class="text-muted">Closed (tidak dapat diubah)</span>';
        }

        tbody.append(`
            <tr data-id="${t.ticketId}">
                <td>${t.ticketNumber}</td>
                <td>${t.customerName}</td>
                <td>${t.title}</td>
                <td>${t.status}</td>
                <td>${t.assignedToAgentName ?? '-'}</td>
                <td>${actionHtml}</td>
            </tr>
        `);
    });
}

$(document).ready(function () {
    loadTickets();

    $('#btnShowCreate').on('click', function () {
        $('#createForm').toggle();
    });

    $('#btnSubmitCreate').on('click', function () {
        const dto = {
            customerName: $('#customerName').val(),
            customerEmail: $('#customerEmail').val(),
            title: $('#title').val(),
            description: $('#description').val()
        };
        function getAntiForgeryToken() {
            return $('input[name="__RequestVerificationToken"]').val();
        }

        $.ajax({
            url: '/Tickets?handler=Create',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            headers: {
                'RequestVerificationToken': getAntiForgeryToken()
            },
            success: function () {
                $('#createForm').hide();
                loadTickets();
            },
            error: function (xhr) {
                alert('Gagal membuat tiket: ' + (xhr.responseJSON?.error ?? xhr.status));
            }
        });
    });

    $(document).on('click', '.btn-save-status', function () {
        const id = $(this).data('id');
        const row = $(this).closest('tr');
        const status = row.find('.status-select').val();
        const description = row.find('td').eq(2).text(); // pakai title lama sbg description sementara

        $.ajax({
            url: `/Tickets?handler=Update&id=${id}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({ status: status, description: description }),
            success: function () {
                loadTickets();
            },
            error: function (xhr) {
                alert('Gagal update: ' + (xhr.responseJSON?.error ?? xhr.status));
            }
        });
    });

    $(document).on('click', '.btn-assign', function () {
        const id = $(this).data('id');
        const row = $(this).closest('tr');
        const agentId = row.find('.assign-input').val();

        if (!agentId) {
            alert('Isi ID Agent terlebih dahulu.');
            return;
        }

        $.ajax({
            url: `/Tickets?handler=Assign&id=${id}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({ assignedToUserId: parseInt(agentId) }),
            success: function () {
                loadTickets();
            },
            error: function (xhr) {
                alert('Gagal assign: ' + (xhr.responseJSON?.error ?? xhr.status));
            }
        });
    });
});