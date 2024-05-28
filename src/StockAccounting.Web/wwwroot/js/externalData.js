
$(function () {
    $("a.search-link").click(function (e) {
        e.preventDefault();

        //Build the new URL
        let url = $(this).attr("href");
        let searchText = $("#searchExternalDataId").val();
        url = url.replace("dummyText", searchText);

        //Navigate to the new URL
        window.location.href = url;

    });

    $("#btnClearSearch").click(function() {
        $("#searchExternalDataId").val("");
        $("#btnSearchExternalData").click();
    });

    $(document).on("keypress", function (e) {
        if (e.which === 13) {
            switch (document.activeElement.id) {
                case "searchExternalDataId":
                    $("#btnSearchExternalData").click();
                break;
            default:
                alert("No active element found!");
            }
        }
    });
});

$("#newExternalDataButton").on('click',
    function () {
        $("#newExternalDataModal").modal('show');

        $("#externalDataModalClose").click(function () {
            $("#newExternalDataModal").modal('hide');
        });

        $("#externalDataModalComplete").submit(function () {

            let postData = $("#externalDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/InsertExternalData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData,
                success: function () {
                    $("#newExternalDataModal").modal('hide');
                },
                error: function () {
                    window.location.reload();
                }
            });
        });
    });


$('#editExternalDataModal').on('show.bs.modal',
    function (event) {
        let button = $(event.relatedTarget);
        let id = button.data('id');
        let name = button.data('name');
        let barcode = button.data('barcode');
        let plucode = button.data('plucode');
        let itemnumber = button.data('itemnumber');
        let unit = button.data('unit');

        let modal = $(this);
        modal.find('#id').val(id);
        modal.find('#Name').val(name);
        modal.find('#Barcode').val(barcode);
        modal.find('#PluCode').val(plucode);
        modal.find('#ItemNumber').val(itemnumber);
        modal.find('#selectUnits').val(unit);

        $("#externalDataModalComplete").submit(function () {

            let postData = $("#externalDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/UpdateExternalData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData
            })
                .done(function () {
                    $("#editExternalDataModal").modal('hide');
                })
                .fail(function () {
                    window.location.reload();
                });
        });
    });

//$(document).ready(function () {
//    new DataTable('#externalTable', {
//        paging: false,
//        info: false,
//        searching: false,
//        order: [[1, 'asc']],
//        columnDefs: [
//            { width: 500, targets: 0 },
//            { width: 200, targets: 1 },
//            { width: 200, targets: 2 },
//            { width: 200, targets: 3 }
//        ],
//        "initComplete": function () {
//            var api = this.api();
//            $('#divTable').show();
//            api.columns.adjust();
//        }
//    });
//});

$("#externalTable").dataTable({
    initComplete: function () {
        $('#divTable').show();
    },
    ajax: {
        url: "/ExternalData/GetExternalData",
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        data: function (d) {
            var dtParameters = JSON.stringify(d);
            return dtParameters;
        }
    },
    stateSave: true,
    //autoWidth: true,
    serverSide: true,
    searching: false,
    paging: true,
    lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
    pageLength: 10,
    pagingType: "full_numbers",
    order: [1, 'asc'],
    columns: [
        { data: "name" },
        { data: "itemNumber" },
        { data: "pluCode" },
        { data: "barcode" },
        { data: "unit" },
        { data: null },
    ],
    columnDefs: [
        { width: 700, targets: 0 },
        { width: 140, targets: 1 },
        { width: 200, targets: 2 },
        { width: 140, targets: 3 },
        { width: 100, targets: 4 },
        { orderable: false, searchable: false, targets: 5 },
    ],
});

