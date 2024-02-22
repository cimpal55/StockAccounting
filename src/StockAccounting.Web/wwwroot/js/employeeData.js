$.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
$.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';
$(document).ready(function () {

    var table = $("#employeeTable").DataTable({
        initComplete: function () {
            $('#divTable').show();
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
        order: [[2, 'asc']],
        columnDefs: [
            { width: 50, targets: 0, className: 'select-checkbox' },
            { width: 200, targets: 1 },
            { width: 200, targets: 2 },
            { width: 200, targets: 3 },
            { width: 300, targets: 4 },
            { orderable: false, targets: [0, 5] }
        ],
    });

    $("#employeeTable").on('draw.dt', function () {
        $('.page-link-datatable').each(function () {
            $(this).attr("aria-pressed", true);
        })
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

    $('#checkBoxAll').click(function () {
        if ($(this).is(":checked")) {
            $(".chkCheckBoxId").prop("checked", true)
        }
        else {
            $(".chkCheckBoxId").prop("checked", false)
        }
    });

    $('#employeeTable').on('length.dt', function (e, settings, len) {
        $('#checkBoxAll').prop('checked', false);
    });

});

$(function () {
    $("#btnEmployeeDataClearSearch").click(function () {
        $("#searchEmployeeDataId").val("");
        $('#employeeTable').DataTable().search("").draw();
    });

    $(document).on('click', '.paginate_button', function () {
        $('#checkBoxAll').prop('checked', false);
    })
});

$(function () {
    $(".dropdown-item").click(function () {
        var table = $('#employeeTable').DataTable();
        var idList = [];
        var mode = $(this).attr("value");
        var checkboxes = table.$('.custom-checkbox:checked', { "page": "all" });

        checkboxes.each(function () {
            idList.push($(this).attr("value"));
        });

        $.ajax({
            type: "POST",
            url: "/ScannedReportExcel",
            data: { employeeList: idList, mode: mode},
            xhrFields: {
                responseType: 'blob'
            }
        })

        .done(function (blob, status, xhr) {
            var filename = "";
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }

            if (typeof window.navigator.msSaveBlob !== 'undefined') {
                window.navigator.msSaveBlob(blob, filename);
            } else {
                var URL = window.URL || window.webkitURL;
                var downloadUrl = URL.createObjectURL(blob);

                if (filename) {
                    var a = document.createElement("a");
                    if (typeof a.download === 'undefined') {
                        window.location.href = downloadUrl;
                    } else {
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                    }
                } else {
                    window.location.href = downloadUrl;
                }

                setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
                window.location.reload();
            }
        })

        .fail(function () {
            window.location.reload();
        })
    })
});
