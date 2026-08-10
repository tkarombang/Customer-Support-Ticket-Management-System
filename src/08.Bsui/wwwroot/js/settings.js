function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val()
}

$(document).ready(function () {
    switchTab('general')
    $('.tab-btn').on('click', function () {
        switchTab($(this).data('tab'))
    })


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
            method: 'PUT',
            contentType: 'application/json',
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

    $('#btnSaveSla').on('click', function () {
        const dto = {
            highPriorityHours: parseInt($('#sla-high').val()),
            mediumPriorityHours: parseInt($('#sla-medium').val()),
            lowPriorityHours: parseInt($('#sla-low').val())
        }

        $.ajax({
            url: '/Settings?handler=Sla',
            method: 'PUT',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Pengaturan SLA Berhasil disimpan.',
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        })
    })
})

function switchTab(tabName) {
    $('.tab-panel').hide()
    $(`#tab-${tabName}`).show()
    $('.tab-btn').removeClass('active')
    $(`.tab-btn[data-tab="${tabName}"]`).addClass('active');
    loadTabData(tabName)
}

function loadTabData(tabName) {
    if (tabName === 'general') loadGeneral()
    if (tabName === 'sla') loadSla()
}

// GENERAL
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

// SLA
function loadSla() {
    $.ajax({
        url: '/Settings?handler=Sla',
        method: 'GET',
        success: function (s) {
            $('#sla-high').val(s.highPriorityHours)
            $('#sla-medium').val(s.mediumPriorityHours)
            $('#sla-low').val(s.lowPriorityHours)
        }
    })
}