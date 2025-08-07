$.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
$.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';

$(document).ready(function() {

    $(document).ready(function () {
        var table = $("#inventoryDetailsTable").DataTable({
            dom: 'lrtip',
            pagingType: "numbers",
            autoWidth: false,
            paging: true,
            searching: true,
            info: false,
            order: [[4, 'desc']],
            columnDefs: [
                { width: 220, targets: 0 },
                { width: 100, targets: 1 },
                { width: 100, targets: 2 },
                { width: 120, targets: 3 },
                { width: 100, targets: 4 },
                { width: 60, targets: 5 },
                { width: 60, targets: 6 },
                { width: 60, targets: 7 }
            ],
            rowCallback: function (row, data) {
                $(row).removeClass("highlight-row-red highlight-row-green highlight-row-simple");

                var a = parseFloat(data[5]);
                var b = parseFloat(data[6]);

                if (a > b) {
                    $(row).addClass("highlight-row-red");
                } else if (a < b) {
                    $(row).addClass("highlight-row-green");
                } else {
                    $(row).addClass("highlight-row-simple");
                }
            }
        });

        $.fn.dataTable.ext.search.push(function (settings, data) {
            var a = parseFloat(data[5]);
            var b = parseFloat(data[6]);

            var showRed = $("#filterRedSwitch").is(":checked");
            var showGreen = $("#filterGreenSwitch").is(":checked");
            var showSimple = $("#filterSimpleSwitch").is(":checked");

            if (a > b && showRed) return true;
            if (a < b && showGreen) return true;
            if (a === b && showSimple) return true;

            return false;
        });

        $("#filterRedSwitch, #filterGreenSwitch, #filterSimpleSwitch").change(function () {
            table.draw();
        });

        $('#searchInventoryDetailsId').keyup(function () {
            table.search($(this).val()).draw();
        });

        $("#btnInventoryDetailsClearSearch").click(function () {
            $("#searchInventoryDetailsId").val("");
            table.search("").draw();
        });
    });

})