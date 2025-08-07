
$(document).ready(function () {
    $.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
    $.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';

    var table = $("#stockDetailsTable").DataTable({
        initComplete: function () {
            $('#stockDetailsTable').show();

            this.api().columns(1).every(function () {
                var column = this;
                var select = $('<select class="form-control form-select-sm"><option value="">All Employees</option></select>')
                    .appendTo($(column.header()))
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );
                        column
                            .search(val ? '^' + val + '$' : '', true, false)
                            .draw();
                    });
                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>');
                });
            });
        },
        dom: 'lrtip',
        pagingType: "numbers",
        autoWidth: false,
        paging: true,
        orderMulti: true,
        ordersCellsTop: true,
        processing: true,
        searching: true,
        serverSide: false,
        info: false,
        order: [[4, 'desc']],
        columnDefs: [
            { width: 200, targets: 0},
            { width: 200, targets: 1, orderable: false },
            { width: 200, targets: 2 },
            { width: 200, targets: 3 },
            {
                "targets": [3],
                "createdCell": function (td, cellData, rowData, row, col) {
                    if (cellData == 'Returned') {
                        $(td).css('color', 'green');
                    }
                    else if (cellData == 'Taken') {
                        $(td).css('color', 'red');
                    }
                }
            },
            {
                "type": 'date',
                "targets": [4]
            }
        ]
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
