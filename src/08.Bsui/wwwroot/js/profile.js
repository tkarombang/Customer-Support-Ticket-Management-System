$(document).ready(function () {
    loadProfile()
    loadActivityLog()

    $('#btnSaveProfile').on('click', function () {
        const dto = {
            name: $('#editName').val(),
            phoneNumber: $('#editPhone').val(),
            jobTitle: $('#editAddress').val()
        };

        $.ajax({
            url: '/Profile?handler=Update',
            method: 'PUT',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Done',
                    text: 'Profile Berhasil Diupdate'
                });
                loadProfile()
                loadActivityLog();
            }
        })
    })


    $('#btnChangePassword').on('click', function () {
        const dto = { oldPassword: $('#oldPassword').val(), newPassword: $('#newPassword').val() }

        if (!dto.oldPassword || !dto.newPassword) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                text: 'Isi Password Lama dan Baru',
                showConfirmButton: false,
                timer: 3000
            });
            return
        }

        $.ajax({
            url: '/Profile?handler=ChangePassword',
            method: 'PUT',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Password berhasil diubah.',
                    showConfirmButton: false,
                    timer: 3000
                });
                $('#oldPassword, #newPassword').val('');
                loadActivityLog();
            },
            error: function (xhr) {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal mengubah password. (' + xhr.status + ')'
                });
            }
        });
    })
})


function loadProfile() {
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
        url: '/Profile?handler=Detail',
        method: 'GET',
        success: function (p) {
            Swal.close();
            $('#avatarInitial').text(p.name.charAt(0).toUpperCase());
            $('#profileName').text(p.name);
            $('#profileRole').text(p.role);
            $('#infoUsername').text(p.username);
            $('#infoEmail').text(p.email);
            $('#infoJoined').text(new Date(p.createdDate).toLocaleDateString('id-ID'));

            $('#editName').val(p.name);
            $('#editPhone').val(p.phoneNumber);
            $('#editJobTitle').val(p.jobTitle);
            $('#editAddress').val(p.address);
        },
        error: function (xhr) {
            Swal.close();
            if (xhr.status === 401) {
                window.location.href = '/Login';
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Gagal',
                    text: 'Gagal memuat users. (' + xhr.status + ')'
                });
            }
        }
    });
}

function loadActivityLog() {
    $.ajax({
        url: '/Profile?handler=ActivityLog',
        method: 'Get',
        success: function (logs) {
            if (!logs || logs.length === 0) {
                $('#activityTable')
                    .html('<tr><td>Belum ada aktivitas</td></tr>');
                return;
            }
            const rows = logs.map(l =>
                `<tr>
                    <td>${l.description}</td>
                    <td>${new Date(l.timestamp).toLocaleString('id-ID')}</td>
                </tr>`
            ).join('');
            $('#activityTable')
                .html(`
                <tr>
                    <th>Aktivitas</th>
                    <th>Waktu</th>
                </tr>${rows}`);
        }
    })
}


function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}