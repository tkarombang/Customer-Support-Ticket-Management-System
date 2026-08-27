$(document).ready(function () {
    loadProfile(true)
    loadActivityLog()

    $('#btnSaveProfile').on('click', function () {
        const dto = {
            name: $('#editName').val(),
            phoneNumber: $('#editPhone').val(),
            jobTitle: $('#editJobTitle').val(),
            address: $('#editAddress').val()
        };

        $.ajax({
            url: '/Profile?handler=Update',
            method: 'PUT',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function (response, textStatus, xhr) {
                console.log('SUCCESS:', xhr.status);
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Berhasil Update Profile.',
                    showConfirmButton: false,
                    timer: 3000
                });
                loadProfile()
                loadActivityLog();
            },
            error: function (xhr) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'error',
                    text: 'Gagal memperbarui profile. (' + xhr.status + ')',
                    showConfirmButton: false,
                    timer: 3000
                });
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


function loadProfile(showLoading = false) {
    if (showLoading) {
        Swal.fire({
            title: 'Memuat Data...',
            html: 'Mohon tunggu sebentar.',
            allowOutsideClick: false,
            allowEscapeKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    }

    $.ajax({
        url: '/Profile?handler=Detail',
        method: 'GET',
        success: function (p) {
            if (showLoading) Swal.close()
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
            if (showLoading) Swal.close()
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
                `<tr calss="activity-item">
                    <td class="activity-icon" style="background:${actionColor(l.action)}10; color:${actionColor(l.action)};">
                        <i class="fa-solid ${actionIcon(l.action)}"></i>
                    </td>
                    <td class="activity-desc">${l.description}</td>
                    <td class="activity-time">${new Date(l.timestamp).toLocaleString('id-ID')}</td>
                </tr>`
            ).join('');
            $('#activityTable')
                .html(`
                <tr>
                    <th></th>
                    <th>Aktivitas</th>
                    <th>Waktu</th>
                </tr>${rows}`);
        },
        error: function (xhr) {
            if (xhr.status === 401) window.location.href = '/Logini'
        }
    })
}

function actionIcon(action) {
    switch (action) {
        case 'Login': return 'fa-right-to-bracket';
        case 'UpdateProfile': return 'fa-user-pen';
        case 'ChangePassword': return 'fa-key';
        case 'ChangeTicket': return 'fa-ticket';
        case 'UpdateTicket': return 'fa-pen-to-square';
        default: return 'fa-circle-info';
    }
}


function actionColor(action) {
    switch (action) {
        case 'Login': return '#2563eb';
        case 'UpdateProfile': return '#7c3aed';
        case 'ChangePassword': return '#f59e0b';
        case 'ChangeTicket': return '#16a34a';
        case 'UpdateTicket': return '#fffff';
        default: return '#64748b';
    }
}


function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}