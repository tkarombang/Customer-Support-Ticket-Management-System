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
            renderTickets(tickets);
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
                actionHtml += `<input class="form-control form-control-sm assign-input" data-id="${t.ticketId}" placeholder="ID Agent" style="width:90px;display:inline-block;" />
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
                Swal.fire({
                    icon: 'success',
                    title: 'Done',
                    text: 'Tiket Berhasil Dibuat'
                });
            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: xhr.responseJSON?.error ?? 'Gagal Membuat Tiket.'
                });
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
                Swal.fire({
                    icon: 'success',
                    title: 'Berhasil',
                    text: 'Status tiket berhasil diperbarui.'
                });
            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: xhr.responseJSON?.error ?? 'Gagal memperbarui tiket.'
                });
            }
        });
    });

    $(document).on('click', '.btn-assign', function () {
        const id = $(this).data('id');
        const row = $(this).closest('tr');
        const agentId = row.find('.assign-input').val();

        if (!agentId) {
            Swal.fire({
                icon: 'warning',
                title: 'Empty Field',
                text: 'Id Agent Tidak di Input.'
            });
            return;
        }

        $.ajax({
            url: `/Tickets?handler=Assign&id=${id}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify({ assignedToUserId: agentId }),
            success: function () {
                $('#createForm').hide()
                loadTickets();

                Swal.fire({
                    icon: 'success',
                    title: 'Berhasil',
                    text: 'Tiket berhasil dibuat.',
                    confirmButtonText: 'OK'
                });

            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: xhr.responseJSON?.error ?? 'Gagal membuat tiket.',
                    confirmButtonText: 'OK'
                });
            }
        });
    });
});