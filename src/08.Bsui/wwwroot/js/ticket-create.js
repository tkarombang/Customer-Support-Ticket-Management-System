function loadAgents() {
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
        url: '/Tickets/Create?handler=Agents',
        method: 'GET',
        success: function (agents) {
            Swal.close();
            const assignSelect = $('#assignTo');
            const ccSelect = $('#ccUsers');

            agents.forEach(function (a) {
                assignSelect.append(`<option value="${a.userId}">${a.name}</option>`);
                ccSelect.append(`<option value="${a.userId}">${a.name}</option>`);
            });
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                window.location.href = '/Login';
                return
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal memuat halaman. (' + xhr.status + ')'
                });
            }
        }
    });
}

function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

function validateForm() {
    const required = ['Judul', 'Deskripsi', 'Namar Customer', 'Email Customer'];
    for (const id of required) {
        if (!$(`#${id}`).val()?.trim()) {
            Swal.fire({
                icon: 'error',
                title: 'Field Kosong',
                html: `<strong>${id}</strong> wajib diisi.`
            });
            return false;
        }
    }
    return true;
}

async function uploadAttachments(ticketId) {
    const files = $('#attachmentFiles')[0].files;
    if (!files || files.length === 0) return;

    for (const file of files) {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('ticketId', ticketId);

        try {
            await $.ajax({
                url: `/Tickets/Create?handler=UploadAttachment&ticketId=${ticketId}`,
                method: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                headers: { 'RequestVerificationToken': getAntiForgeryToken() },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Done',
                        text: 'Upload Berhasil'
                    });
                }
            });
        } catch (err) {
            console.error('Gagal upload attachment:', file.name, err);
            Swal.fire({
                icon: 'error',
                title: 'Gagal',
                text: file.name ?? `Gagal Upload File ${err}.`
            });
        }
    }
}

$(document).ready(function () {
    loadAgents();

    $('#title').on('input', function () { $('#titleCount').text($(this).val().length); });
    $('#description').on('input', function () { $('#descCount').text($(this).val().length); });

    $('#btnSubmit').on('click', async function () {
        if (!validateForm()) return;

        const dto = {
            type: $('#type').val(),
            impact: $('#impact').val(),
            category: $('#category').val(),
            applicationSystem: $('#applicationSystem').val() || null,
            priority: $('#priority').val(),
            dueDate: $('#dueDate').val() || null,
            customerName: $('#customerName').val(),
            customerEmail: $('#customerEmail').val(),
            title: $('#title').val(),
            description: $('#description').val(),
            assignedToUserId: $('#assignTo').val() || null,
            ccUserIds: $('#ccUsers').val() || []
        };

        $('#btnSubmit').prop('disabled', true).text('Menyimpan...');

        $.ajax({
            url: '/Tickets/Create?handler=Submit',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: async function (result) {
                Swal.fire({
                    icon: 'success',
                    title: 'Done',
                    text: 'Tiket Berhasil Dibuat'
                });
                await uploadAttachments(result.ticketId);
                window.location.href = '/Tickets';
            },
            error: function (xhr) {
                $('#btnSubmit').prop('disabled', false).text('Buat Tiket');
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: xhr.responseJSON?.error ?? 'Gagal membuat tiket.'
                });
            }
        });
    });
});