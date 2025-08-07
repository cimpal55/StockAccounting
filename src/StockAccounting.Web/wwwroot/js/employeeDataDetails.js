
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

$(function () {
    $("#dropdownMenuLink").click(function () {
        printDetails()
    });
})

function printDetails() {
    var name = $("#employeeName").text();
    var panel = $("#employeeDetailsTable").html();
    var printWindow = window.open('', '', 'height=1920,width=1080');
    printWindow.document.write('<html><head><title>'+name+'</title>');
    printWindow.document.write('</head><body>');
    printWindow.document.write(`<table style='margin-bottom: 0; width: 100%' align='center' border='1'>
    <tbody>`)
    printWindow.document.write(panel)
    printWindow.document.write('</tbody></table></body></html>');
    printWindow.document.close();
    setTimeout(function () {
        printWindow.print();
    }, 500);
    return false;
}
