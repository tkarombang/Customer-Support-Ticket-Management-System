let allUsers = [];

function loadUsers() {
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
        url: '/Users?handler=List',
        method: 'GET',
        success: function (users) {
            Swal.close();
            //renderUsers(users);
            //renderSummary(users);
            allUsers = users;
            applyFilters();
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

function applyFilters() {
    const search = $('#searchTerm').val().toLowerCase().trim();
    const role = $('#filterRole').val();
    const status = $('#filterStatus').val();
    const date = $('#filterDate').val();

    let filtered = allUsers.filter(function (u) {
        const matchSearch = !search ||
            u.name.toLowerCase().includes(search) ||
            u.email.toLowerCase().includes(search) ||
            u.role.toLowerCase().includes(search);

        const matchRole = !role || u.role === role;
        const matchStatus = !status || u.status === status;
        const matchDate = !date || u.createdDate.startsWith(date);

        return matchSearch && matchRole && matchStatus && matchDate;
    });

    renderUsers(filtered);
    renderSummary(allUsers); // summary cards tetap dari total keseluruhan, bukan hasil filter
    $('#paginationInfo').text(`Menampilkan ${filtered.length} dari ${allUsers.length} pengguna`);
}

function renderSummary(users) {
    $('#totalUsers').text(users.length);
    $('#activeUsers').text(users.filter(u => u.status === 'Active').length);
    $('#inactiveUsers').text(users.filter(u => u.status === 'Inactive').length);
}

function renderUsers(users) {
    const tbody = $('#userTableBody');
    tbody.empty();

    if (!users || users.length === 0) {
        tbody.append('<tr><td colspan="6" class="text-center">Belum ada user</td></tr>');
        return;
    }

    users.forEach(function (u) {
        const statusBadge = u.status === 'Active'
            ? '<span class="badge bg-success text-white">Active</span>'
            : '<span class="badge bg-secondary text-white">Inactive</span>';

        tbody.append(`
            <tr data-id="${u.userId}">
                <td>${u.username}</td>
                <td>${u.name}</td>
                <td>${u.email}</td>
                <td>${u.role}</td>
                <td>${statusBadge}</td>
                <td>
                    <button class="btn btn-sm btn-outline-secondary btn-toggle-status" data-id="${u.userId}">
                        ${u.status === 'Active' ? 'Nonaktifkan' : 'Aktifkan'}
                    </button>
                </td>
            </tr>
        `);
    });
}

function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

$(document).ready(function () {
    loadUsers();

    $('#btnShowCreate').on('click', function () {
        $('#createForm').toggle();
    });

    $('#btnSubmitCreate').on('click', function () {
        console.log("TOMBOL SIMPAN")
        const dto = {
            username: $('#username').val(),
            name: $('#name').val(),
            email: $('#email').val(),
            password: $('#password').val(),
            role: $('#role').val()
        };

        $.ajax({
            url: '/Users?handler=Create',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                $('#createForm').hide();
                loadUsers();
                Swal.fire({
                    icon: 'success',
                    title: 'Done',
                    text: 'User Berhasil Dibuat'
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

    $(document).on('click', '.btn-toggle-status', function () {
        const id = $(this).data('id');

        $.ajax({
            url: `/Users?handler=ToggleStatus&id=${id}`,
            method: 'PUT',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            success: function () {
                loadUsers();
                Swal.fire({
                    icon: 'success',
                    title: 'Done',
                    text: 'User Berhasil Diupdate'
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

    $('#btnFilter').on('click', applyFilters);

    $('#btnReset').on('click', function () {
        $('#searchTerm').val('');
        $('#filterRole').val('');
        $('#filterStatus').val('');
        $('#filterDate').val('');
        applyFilters();
    });
});