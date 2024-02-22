$("#newInventoryDataButton").on('click',
    function () {
        $("#newInventoryDataModal").modal('show');

        $("#inventoryDataModalClose").click(function () {
            $("#newInventoryDataModal").modal('hide');
        });

        $("#inventoryDataModalComplete").submit(function () {

            var postData = $("#inventoryDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/InsertInventoryData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData,
                success: function () {
                    $("#newInventoryDataModal").modal('hide');
                },
                error: function () {
                    window.location.reload();
                }
            });
        });
    });

$('#editInventoryDataModal').on('show.bs.modal',
    function (event) {
        var button = $(event.relatedTarget);
        var id = button.data('id');
        var employee1Id = button.data('employee1id');
        var employee2Id = button.data('employee2id');

        var modal = $(this);
        modal.find('#id').val(id);
        modal.find('#employee1Id').val(employee1Id);
        modal.find('#employee2Id').val(employee2Id);

        $("#inventoryDataModalComplete").submit(function () {

            var postData = $("#inventoryDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/UpdateInventoryData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData
            })
                .done(function () {
                    $("#editInventoryDataModal").modal('hide');
                })
                .fail(function () {
                    window.location.reload();
                });
        });
    });


$('#deleteInventoryDataModal').on('show.bs.modal',
    function (event) {
        var button = $(event.relatedTarget);
        var id = button.data('id');

        var modal = $(this);
        modal.find('#id').val(id);

        $("#btnYes").click(function () {
            $.ajax({
                type: "post",
                url: "/DeleteInventoryData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: {
                    Id: id
                }
            })
                .done(function () {
                    $("#deleteInventoryDataModal").modal('hide');
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
        var searchText = $("#searchInventoryDataId").val();
        url = url.replace("dummyText", searchText);

        window.location.href = url;

    });

    $("#btnInventoryDataClearSearch").click(function () {
        $("#searchInventoryDataId").val("");
        $("#btnSearchInventoryData").click();
    });

    $(document).on("keypress", function (e) {
        if (e.which === 13) {
            switch (document.activeElement.id) {
                case "searchInventoryDataId":
                    $("#btnSearchInventoryData").click();
                    break;
                default:
                    alert("No active element found!");
            }
        }
    });
});

$(document).ready(function () {

    $("#inventoryTable").dataTable({
        initComplete: function () {
            $('#divTable').show();
        },
        autoWidth: false,
        paging: false,
        info: false,
        searching: false,
        order: [[2, 'asc']],
        columnDefs: [
            { width: 250, targets: 0 },
            { width: 250, targets: 1 },
            { width: 200, targets: 2 },
            { orderable: false, targets: 3 }
        ],
    })
})



