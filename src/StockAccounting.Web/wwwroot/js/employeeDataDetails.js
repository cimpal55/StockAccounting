
$(document).ready(function () {

    var table = $("#employeeDetailsTable").DataTable({
        initComplete: function () {
            $('#employeeDetailsDiv').show();
        },
        autoWidth: true,
        paging: false,
        searching: false,
        info: false,
        order: [[1, 'asc']],
        columnDefs: [
            {
                "targets": [4],
                "createdCell": function (td, cellData, rowData, row, col) {
                    if (cellData > "0") {
                        $(td).css('color', 'green')
                    }
                    else if (cellData < "0") {
                        $(td).css('color', 'red')
                    }
                }
            },
        ],
    });

    $('#searchEmployeeDataId').keyup(function () {
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
