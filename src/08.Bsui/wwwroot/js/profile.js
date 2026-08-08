$(document).ready(function () {
    loadProfile()

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
                loadProfile()
                Swal.fire({
                    icon: 'success',
                    title: 'Done',
                    text: 'Profile Berhasil Diupdate'
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

function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

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

