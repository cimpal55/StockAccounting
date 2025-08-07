$.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
$.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';
$(document).ready(function () {
    var table = $("#DocumentTable").DataTable({
        initComplete: function () {
            $('#divTable').show();
        },
        dom: 'lrtip',
        pagingType: "numbers",
        "drawCallback": function (settings) {
            $(".paging_numbers a")
                .attr("href", "#")
                .off("click.preventScroll")
                .on("click.preventScroll", function (e) {
                    e.preventDefault();
                });
        },
        autoWidth: false,
        searching: true,
        info: false,
        processing: true,
        orderMulti: true,
        ordersCellsTop: true,
        paging: true,
        pageLength: 10,
        deferRender: true,
        serverSide: true,
        ajax: {
            url: "/DocumentData/List",
            type: "POST",
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            },
            data: function (d) {
                var columnMap = {
                    0: "employee1",
                    1: "employee2",
                    2: "created"
                };

                if (d.order && d.order.length > 0) {
                    d.sortColumn = columnMap[d.order[0].column];
                    d.sortDirection = d.order[0].dir;
                }

                d.pageId = (d.start / d.length) + 1;
                d.searchText = $("#searchDocumentDataId").val() || "dummyText";

                return d;
            },
            dataSrc: function(response) {
                return response.data;
            }
        },
        columns: [
            { data: 'employee1' },
            { data: 'employee2' },
            {
                data: 'created',
                render: function (data, type, row) {
                    if (!data) return '';

                    var date = new Date(data);

                    var yyyy = date.getFullYear();
                    var MM = String(date.getMonth() + 1).padStart(2, '0');
                    var dd = String(date.getDate()).padStart(2, '0');
                    var HH = String(date.getHours()).padStart(2, '0');
                    var mm = String(date.getMinutes()).padStart(2, '0');
                    var ss = String(date.getSeconds()).padStart(2, '0');

                    return `${yyyy}-${MM}-${dd} ${HH}:${mm}:${ss}`;
                }
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                className: 'text-right',
                render: function (data, type, row) {
                    var  id = row.id;
                    var created = row.created.substring(0, 16); // yyyy-MM-ddThh:mm

                    return `
                <a class="btn btn-outline-primary" href="/ScannedData/Details/${id}">Details</a>
          
                <button type="button" class="btn btn-danger" data-toggle="modal" 
                        data-target="#deleteDocumentDataModal"
                        data-id="${id}">Delete</button>
            `;
                }
            }
        ],
        columnDefs: [
            { orderable:false, width: 250, targets: 0 },
            { orderable: false, width: 250, targets: 1 },
            { width: 300, targets: 2 },
            { orderable: false, width: 400, targets: 3 }
        ],
        order: [[2, 'desc']]
    });
});

$(function () {
    // Обработка клика по кнопке "Search"
    $("#btnSearchDocumentData").click(function () {
        $("#DocumentTable").DataTable().search($("#searchDocumentDataId").val()).draw();
    });

    // Обработка Enter в поле поиска
    $("#searchDocumentDataId").on("keypress", function (e) {
        if (e.which === 13) {
            $("#btnSearchDocumentData").click();
        }
    });

    // Очистка поиска
    $("#btnDocumentDataClearSearch").click(function () {
        $("#searchDocumentDataId").val("");
        $("#btnSearchDocumentData").click();
    });
});


//<button type="button" class="btn btn-warning" data-toggle="modal"
//    data-target="#editDocumentDataModal"
//    data-id="${id}"
//    data-employee1id="${row.employee1Id || ''}"
//    data-employee2id="${row.employee2Id || ''}"
//    data-created="${created}">Edit</button>

$("#newDocumentDataButton").on('click',
    function () {
        $("#newDocumentDataModal").modal('show');

        $("#DocumentDataModalClose").click(function () {
            $("#newDocumentDataModal").modal('hide');
        });

        $("#DocumentDataModalComplete").submit(function () {

            var postData = $("#DocumentDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/InsertDocumentData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData,
                success: function () {
                    $("#newDocumentDataModal").modal('hide');
                },
                error: function () {
                    window.location.reload();
                }
            });
        });
});

$('#editDocumentDataModal').on('show.bs.modal',
    function (event) {
        var button = $(event.relatedTarget);
        var id = button.data('id');
        var employee1Id = button.data('employee1id');
        var employee2Id = button.data('employee2id');
        //var created = button.data('created');

        var modal = $(this);
        modal.find('#id').val(id);
        modal.find('#employee1Id').val(employee1Id);
        modal.find('#employee2Id').val(employee2Id);
        //modal.find('#created').val(created);

        $("#DocumentDataModalComplete").submit(function () {

            var postData = $("#DocumentDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/UpdateDocumentData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData
            })
                .done(function () {
                    $("#editDocumentDataModal").modal('hide');
                })
                .fail(function () {
                    window.location.reload();
                });
        });
    });


$('#deleteDocumentDataModal').on('show.bs.modal',
    function (event) {
        var button = $(event.relatedTarget);
        var id = button.data('id');

        var modal = $(this);
        modal.find('#id').val(id);

        $("#btnYes").click(function () {
            $.ajax({
                type: "post",
                url: "/DeleteDocumentData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: {
                    Id: id
                }
            })
                .done(function () {
                    $("#deleteDocumentDataModal").modal('hide');
                    $("#data_" + id).remove();
                })
                .fail(function () {
                    window.location.reload();
                });
        });
    });

$(function () {
    $("a.search-link").click(function (e) {
        e.preventDefault();

        var url = $(this).attr("href");
        var searchText = $("#searchDocumentDataId").val();
        url = url.replace("dummyText", searchText);

        window.location.href = url;

    });

    $("#btnDocumentDataClearSearch").click(function () {
        $("#searchDocumentDataId").val("");
        $("#btnSearchDocumentData").click();
    });

    $(document).on("keypress", function (e) {
        if (e.which === 13) {
            switch (document.activeElement.id) {
                case "searchDocumentDataId":
                    $("#btnSearchDocumentData").click();
                    break;
                default:
                    alert("No active element found!");
            }
        }
    });
});

$('a[href="#"]').on('click', function (e) {
    e.preventDefault();
});


