$.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
$.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';
$(document).ready(function () {

    var table = $("#stockLeftQuantityTable").DataTable({
        initComplete: function () {
            $('#stockLeftQuantityTable').show();
        },
        dom: 'lrtip',
        pagingType: "numbers",
        "drawCallback": function (settings) {
            $(".paging_numbers a").attr("href", "#")
        },
        autoWidth: false,
        paging: true,
        searching: true,
        info: false,
        order: [[1, 'desc']],
    });

});
