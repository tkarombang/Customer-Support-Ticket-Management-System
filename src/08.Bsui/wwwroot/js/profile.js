$(document).ready(function () {
    loadProfile()
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