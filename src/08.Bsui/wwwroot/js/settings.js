function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val()
}

$(document).ready(function () {
    switchTab('general')

    $('#btnSaveGeneral').on('click', function () {
        const dto = {
            appName: $('#gen-appName').val(),
            appDescription: $('#gen-appDesc').val(),
            timeZone: $('#gen-timezone').val(),
            language: $('#gen-language').val(),
            dateFormat: $('#gen-dateFormat').val(),
            itemsPerPage: parseInt($('#gen-itemsPerPage').val())
        };

        $.ajax({
            url: '/Settings?handler=General',
            method: 'PUT', contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Pengaturan General Berhasil disimpan.',
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        });
    })
})

function switchTab(tabName) {
    loadTabData(tabName)
}

function loadTabData(tabName) {
    if (tabName === 'general') loadGeneral()
}

function loadGeneral() {
    $.ajax({
        url: '/Settings?handler=General',
        method: 'GET',
        success: function (g) {
            $('#gen-appName').val(g.appName);
            $('#gen-appDesc').val(g.appDescription);
            $('#gen-timezone').val(g.timeZone);
            $('#gen-language').val(g.language);
            $('#gen-dateFormat').val(g.dateFormat);
            $('#gen-itemsPerPage').val(g.itemsPerPage);
        }
    });
}