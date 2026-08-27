function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val()
}

$(document).ready(function () {
    switchTab(initialTab)
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


    $('#btnShowIntegrationForm').on('click', function () {
        $('#integrationForm').toggle()
    })
    $('#btnSaveIntegration').on('click', function () {
        const dto = {
            name: $('#int-name').val(),
            webhookUrl: $('#int-webhook').val(),
            apiKey: $('#int-apikey').val()
        };
        $.ajax({
            url: '/Settings?handler=Integration',
            method: 'POST',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() },
            data: JSON.stringify(dto),
            success: function () {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Berhasil Menambahkan Integrasi Baru.',
                    showConfirmButton: false,
                    timer: 3000
                });
                $('#integrationForm').hide();
                loadIntegrations();
            }
        });
    });

    $('#btnTriggerBackup').on('click', function () {
        Swal.fire({
            title: "Apakah kamu yakin?",
            text: "Buat backup database sekarang?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Ya, Backuup Sekarang!"
        }).then((result) => {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/Settings?handler=Backup',
                method: 'POST',
                headers: { 'RequestVerificationToken': getAntiForgeryToken() },
                success: function (response) {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'success',
                        title: 'Berhasil Backup.',
                        showConfirmButton: false,
                        timer: 3000
                    });
                    loadBackupHistory()
                },
                error: function () {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'error',
                        title: 'Backup Gagal.',
                        showConfirmButton: false,
                        timer: 3000
                    });
                }
            })
        });
        
    })



    $('#btnRestore').on('click', function () {
        const file = $('#restoreFile')[0].files[0]
        if (!file) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Pilih file .bak terlebih dahulu.',
                showConfirmButton: false,
                timer: 3000
            });
            return
        }

        Swal.fire({
            title: "PERINGATAN",
            text: "Restore akan Menimpa data saat ini.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Ya, Lanjutkan?"
        }).then((result) => {
            const formData = new FormData();
            formData.append('file', file)

            $.ajax({
                url: '/Settings?handler=Restore',
                method: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                headers: { 'RequestVerificationToken': getAntiForgeryToken() },
                success: function (response) {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'success',
                        title: 'Berhasil Restore.',
                        showConfirmButton: false,
                        timer: 3000
                    });
                },
                error: function () {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'error',
                        title: 'Restore Gagal.',
                        showConfirmButton: false,
                        timer: 3000
                    });
                }
            })

        })
    })





    $('#btnFilterLogs').on('click', function () { loadLogs(1) })
    $('#btnResetLogs').on('click', function () {
        $('#log-search').val('')
        $('#log-action').val('')
        loadLogs(1)
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
    if (tabName === 'integrations') loadIntegrations()
    if (tabName === 'backup') loadBackupHistory()
    if (tabName === 'logs') loadLogs(1)
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


// INTEGRATIONS
function loadIntegrations() {
    $.ajax({
        url: '/Settings?handler=Integrations',
        method: 'GET',
        success: function (list) {
            const tbody = $('#integrationTableBody')
            tbody.empty()

            if (!list || list.length === 0) {
                tbody.append(`
                <tr>
                    <td colspan="4" class="text-center">Belum ada integrasi</td >
                </tr>
                `)
                return
            }

            list.forEach(i => {
                const statusBadge = i.isActive
                    ? '<span class="badge bg-success">Active</span>'
                    : '<span class="badge bg-secondary">Inactive</span>'

                tbody.append(`
                    <tr data-id="${i.integrationId}">
                        <td>${i.name}</td>
                        <td>${i.webhookUrl}</td>
                        <td>${i.hasApiKey ? '●●●●●●●●' : '-'}</td>
                        <td><span class="toggle-active" style="cursor:pointer;">${statusBadge}</span></td>
                    </tr>
                `)
            })

        }
    })
}


// BACKUP
function loadBackupHistory() {
    $.ajax({
        url: '/Settings?handler=BackupHistory',
        method: 'GET',
        success: function (list) {
            const tbody = $("#backupTableBody")
            tbody.empty()
            if (!list || list.length === 0) {
                tbody.append(`
                    <tr>
                        <td colspan="5" class="text-center">Belum ada riwayat backup</td>
                    </tr>
                    return`)
            }

            list.forEach(b => {
                const sizeKb = b.fileSizeBytes ? (b.fileSizeBytes / 1024).toFixed(0) + ' KB' : '-'
                const statusBadge = b.status === 'Success'
                    ? '<span class="badge bg-success">Success</span>'
                    : '<span class="badge bg-danger">Failed</span>'
                tbody.append(`
                    <tr>
                        <td>${b.fileName}</td>
                        <td>${sizeKb}</td>
                        <td>${b.type}</td>
                        <td>${statusBadge}</td>
                        <td>${new Date(b.createdDate).toLocaleString('id-ID')}</td>
                    </tr>`)
            })
        }
    })
}



// SYSTEM LOGS
function loadLogs(page) {
    Swal.fire({
        title: 'Memuat Data...',
        html: 'Mohon tunggu sebentar.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    const params = {
        SearchTerm: $('#log-search').val(),
        Action: $('#log-action').val(),
        PageNumber: page, PageSize: 10
    }
    $.ajax({
        url: '/Settings?handler=SystemLogs',
        method: 'GET',
        data: params,
        success: function (result) {
            Swal.close()
            const tbody = $('#logTableBody')
            tbody.empty()

            if (!result.items || result.items.length === 0) {
                tbody.append(`
                <tr>
                    <td colspan="5" class='text-center'>Tidak Ada Log</td>
                </tr>`)
            }

            result.items.forEach(l => tbody.append(`
            <tr>
                <td>${l.userName ?? 'System'}</td>
                <td><span class="badge bg-primary">${l.action}</span></td>
                <td>${l.description}</td>
                <td>${l.ipAddress ?? '-'}</td>
                <td>${new Date(l.timestamp).toLocaleString('id-ID')}</td>
            </tr>`))

            $('#logPaginationInfo').text(`Halaman ${result.pageNumber} dari ${result.totalPages} (${result.totalCount} total log)`)
        },
        error: function () {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Gagal Memuat hasil filter.',
                showConfirmButton: false,
                timer: 3000
            });
        }
    })
}