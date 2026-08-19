function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

function loadDetail() {
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
        url: `/Tickets/Update?handler=Detail&id=${ticketId}`,
        method: 'GET',
        success: function (t) {
            Swal.close();
            $('#ticketNumberBreadcrumb, #ticketNumber').text(t.ticketNumber).val(t.ticketNumber);
            $('#type').val(t.type);
            $('#impact').val(t.impact);
            $('#category').val(t.category);
            $('#applicationSystem').val(t.applicationSystem);
            $('#priority').val(t.priority);
            $('#dueDate').val(t.dueDate ? t.dueDate.split('T')[0] : '');
            $('#title').val(t.title);
            $('#description').val(t.description);
            $('#customerName').val(t.customerName);
            $('#customerEmail').val(t.customerEmail);
            $('#status').val(t.status);
            $('#currentStatusBadge').text(t.status);

            if (t.status === 'Closed') {
                $('input, select, textarea, button').not('#btnCancel, a').prop('disabled', true);
                Swal.fire({
                    icon: 'error',
                    title: 'Ticket Clossed',
                    text: 'Tidak dapat Melakukan Perubahan',
                    focusConfirm: true,
                    confirmButtonText: 'Thanks'
                }).then(result => {
                    if (result.isConfirmed) window.location.href = '/Tickets';
                });
            }

            renderAttachments(t.attachments);
            renderComments(t.comments);

            if (t.assignedToUserId) $('#assignTo').val(t.assignedToUserId);
        },
        error: function (xhr) {
            Swal.close();
            if (xhr.status === 401) {
                window.location.href = '/Tickets';
                return
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal memuat data. (' + xhr.status + ')'
                });
            }
        }
    });
}

function renderAttachments(attachments) {
    const el = $('#attachmentList');
    if (!attachments || attachments.length === 0) {
        el.html('Tidak ada lampiran.');
        return;
    }
    el.html(attachments.map(a =>
        `<div><a href="${a.filePath}" target="_blank">${a.fileName}</a> (${(a.fileSizeBytes / 1024).toFixed(0)} KB)</div>`
    ).join(''));
}

function renderComments(comments) {
    const el = $('#commentList');
    if (!comments || comments.length === 0) {
        el.html('Belum ada komentar.');
        return;
    }
    el.html(comments.map(c =>
        `<div class="mb-2"><b>${c.createdByName}</b> — ${new Date(c.createdDate).toLocaleString('id-ID')}<br>${c.content}</div>`
    ).join(''));
}

function loadAgents() {
    $.ajax({
        url: '/Tickets/Update?handler=Agents',
        method: 'GET',
        success: function (agents) {
            agents.forEach(a => $('#assignTo').append(`<option value="${a.userId}">${a.name}</option>`));
        }
    });
}

$(document).ready(function () {
    loadDetail();
    loadAgents();

    $('#btnSubmit').on('click', function () {
        const dto = {
            type: $('#type').val(),
            impact: $('#impact').val(),
            category: $('#category').val(),
            applicationSystem: $('#applicationSystem').val() || null,
            priority: $('#priority').val(),
            dueDate: $('#dueDate').val() || null,
            title: $('#title').val(),
            description: $('#description').val(),
            status: $('#status').val(),
            statusNote: $('#statusNote').val() || null
        };

        $.ajax({
            url: `/Tickets/Update?handler=Submit&id=${ticketId}`,
            method: 'PUT',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                window.location.href = '/Tickets';
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
                    text: xhr.responseJSON?.error ?? 'Gagal Update Tiket.'
                });
            }
        });
    });

    $('#btnAssign').on('click', function () {
        const agentId = $('#assignTo').val();
        if (!agentId) { alert('Pilih agent terlebih dahulu.'); return; }

        $.ajax({
            url: `/Tickets/Update?handler=Assign&id=${ticketId}`,
            method: 'PUT',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify({ assignedToUserId: agentId }),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Berhasil',
                    text: 'Berhasil di-assign.'
                });
                loadDetail();
            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: xhr.responseJSON?.error ?? 'Gagal Assign tiket.'
                });
            }
        });
    });
});