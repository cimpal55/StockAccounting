function getExternalTotalData() {
    $.ajax({
        type: "post",
        url: "/GetExternalTotalData",
        dataType: "json",
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: function (data) {

            $.each(data, function (key, item) {
                $("#totalSumBefore").find("b").html(item.totalSumBefore);
                $("#totalSumAfter").find("b").html(item.totalSumAfter);
                $("#totalSumDifference").find("b").html(item.totalSumDifference);
            });
        },
        error: function () {
            alert("Unsuccessful ajax request!!!");
        }
    });
}

$(function () {
    $("a.search-link").click(function (e) {
        e.preventDefault();

        //Build the new URL
        var url = $(this).attr("href");
        var searchText = $("#searchExternalDataId").val();
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
    autoWidth: false,
    paging: false,
    info: false,
    searching: false,
    order: [[1, 'asc']],
    columnDefs: [
        { width: 500, targets: 0 },
        { width: 200, targets: 1 },
        { width: 200, targets: 2 },
        { width: 200, targets: 3 }
    ],
})
