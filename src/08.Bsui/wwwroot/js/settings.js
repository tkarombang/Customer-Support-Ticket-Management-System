function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val()
}

$(document).ready(function () {
    switchTab('general')
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