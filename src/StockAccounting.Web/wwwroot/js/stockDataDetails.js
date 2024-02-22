$.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
$.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';
$(document).ready(function () {

    var table = $("#stockDetailsTable").DataTable({
        initComplete: function () {
            $('#stockDetailsTable').show();
        },
        dom: 'lrtip',
        pagingType: "numbers",
        "drawCallback": function (settings) {
            $(".paging_numbers a").attr("href", "#")
        },

        "createdCell": function (td, cellData, rowData, row, col) {
            if (rowData[3] == 'Taken') {
                $(td).css('background-color', 'green')
            }
            else if (rowData[3] == 'Returned') {
                $(td).css('background-color', 'red')
            }
        },
        autoWidth: false,
        paging: false,
        searching: true,
        info: false,
        order: [[2, 'asc']],
        columnDefs: [
            { width: 200, targets: 0},
            { width: 200, targets: 1 },
            { width: 200, targets: 2 },
            { width: 200, targets: 3 },
            {
                "targets": [2],
                "createdCell": function (td, cellData, rowData, row, col) {
                    if (cellData == 'Returned') {
                        $(td).css('color', 'green')
                    }
                    else if (cellData == 'Taken') {
                        $(td).css('color', 'red')
                    }
                }
            },
        ],
    });

    $('#searchStockDataId').keyup(function () {
        table.search($(this).val()).draw();
    })

    $.extend(true, $.fn.dataTable.defaults, {
        language: {
            search: ""
        }
    });

    $('input.column_filter').on('keyup click', function () {
        filterColumn(0);
    });
    $('[type=search]').each(function () {
        $(this).attr("placeholder", "Search...");
        $(this).before('<span class="fa fa-search"></span>');
    });

});

$(function () {
    $("#btnStockDataClearSearch").click(function () {
        $("#searchStockDataId").val("");
        $('#stockTable').DataTable().search("").draw();
    });
});
