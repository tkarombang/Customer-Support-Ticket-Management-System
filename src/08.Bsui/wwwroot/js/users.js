function loadUsers() {
    $.ajax({
        url: '/Users?handler=List',
        method: 'GET',
        success: function (users) {
            renderUsers(users);
            renderSummary(users);
        },
        error: function (xhr) {
            if (xhr.status === 401) window.location.href = '/Login';
            else alert('Gagal memuat users: ' + xhr.status);
        }
    });
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
            ? '<span class="badge bg-success">Active</span>'
            : '<span class="badge bg-secondary">Inactive</span>';

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
            },
            error: function (xhr) {
                alert('Gagal membuat user: ' + (xhr.responseJSON?.error ?? xhr.status));
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
            },
            error: function (xhr) {
                alert('Gagal ubah status: ' + xhr.status);
            }
        });
    });
});