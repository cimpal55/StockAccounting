$.fn.dataTable.ext.classes.sPageButton = 'page-link-datatable search-link button-datatable';
$.fn.dataTable.ext.classes.sPageButtonActive = 'active-button-datatable';

$(document).ready(function () {

    var table = $("#toolkitTable").DataTable({
        initComplete: function () {
            $('#divTable').show();
        },
        dom: 'lrtip',
        autoWidth: false,
        paging: false,
        searching: false,
        info: false,
        order: [[3, 'desc']],
        columnDefs: [
            { width: 200, targets: 0 },
            { width: 230, targets: 1 },
            { width: 130, targets: 2 },
            { width: 130, targets: 3 },
            { width: 130, targets: 4 },
            { orderable: false, targets: [5] }
        ],
    });
});

$(function () {
    $("a.search-link").click(function (e) {
        e.preventDefault();

        //Build the new URL
        var url = $(this).attr("href");
        var searchText = $("#searchToolkitDataId").val();
        url = url.replace("dummyText", searchText);

        //Navigate to the new URL
        window.location.href = url;

    });

    $("#btnClearSearch").click(function () {
        $("#searchToolkitDataId").val("");
        $("#btnSearchToolkitData").click();
    });

    $(document).on("keypress", function (e) {
        if (e.which === 13) {
            switch (document.activeElement.id) {
                case "searchToolkitDataId":
                    $("#btnSearchToolkitData").click();
                    break;
                default:
                    alert("No active element found!");
            }
        }
    });
});

$(document).on('submit', '#toolkitForm', function (e) {
    e.preventDefault();

    var item = $("#toolkitForm").serialize();

    $.ajax({
        type: "post",
        url: "/ToolkitData/InsertToolkitData",
        data: item,
        dataType: "json",
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: function (response) {
            $("#newToolkitDataModal").modal('hide');
            window.location.href = response.redirectToUrl;
        },
        error: function (textStatus) {
            alert("Request failed: " + textStatus);
            console.log(JSON.stringify(textStatus));
        }
    });
})

$("#newToolkitDataButton").click(function (e) {
    $("#newToolkitDataModal").modal('show');

    $("#toolkitDataModalClose").click(function () {
        $("#newToolkitDataModal").modal('hide');
    }); 
});

$(function () {

    items = $.ajax({
        url: '/ToolkitData/GetAutoCompleteExternal',
        type: 'get',
        success: function (data) {
            items = data;
        },
    });

    pageSize = 50

    jQuery.fn.select2.amd.require(["select2/data/array", "select2/utils"],

        function (ArrayData, Utils) {
            function CustomData($element, options) {
                CustomData.__super__.constructor.call(this, $element, options);
            }
            Utils.Extend(CustomData, ArrayData);

            CustomData.prototype.query = function (params, callback) {

                var results = [];
                if (params.term && params.term !== '') {
                    results = _.filter(items, function (e) {
                        return e.text.toUpperCase().indexOf(params.term.toUpperCase()) >= 0;
                    });
                } else {
                    results = items;
                }

                if (!("page" in params)) {
                    params.page = 1;
                }
                var data = {};
                data.results = results.slice((params.page - 1) * pageSize, params.page * pageSize);
                data.pagination = {};
                data.pagination.more = params.page * pageSize < results.length;
                callback(data);
            };

            var j = 1;
            $(document).on('click', '#addRow', function () {
                $('#addTable').append(`$<tr>
                                    <td width="300px">
                                        <select class="external" id="externalData_${j}" name="ToolkitExternalDataModel[${j}].ExternalDataId" asp-for="ToolkitExternalDataModel[${j}].ExternalDataId" style="width: 280px">
                                            <option value="0" selected="selected">-Choose-</option>
                                        </select>
                                        <span asp-validation-for="ToolkitExternalDataModel[${j}].ExternalDataId" class="text-danger"></span>
                                    </td>
                                    <td>
                                        <input class="form-control" data-val="true" data-val-number="The field Quantity must be a number." data-val-required="The Quantity field is required." id="ToolkitExternalDataModel_0__Quantity" name="ToolkitExternalDataModel[${j}].Quantity" type="number" value="1">
                                        <span class="field-validation-valid" data-valmsg-for="ToolkitExternalDataModel[${j}].Quantity" data-valmsg-replace="true"></span>
                                    </td>
                                </tr>`);

                $('#externalData_' + j).select2({
                    ajax: {},
                    dataAdapter: CustomData,
                    dropdownParent: $("#newToolkitDataModal")
                });
                j++;
            });
            $(document).ready(function () {

                $("#externalData_0"/* + Number(j-1)*/).select2({
                    ajax: {},
                    dataAdapter: CustomData,
                    dropdownParent: $("#newToolkitDataModal")
                });

            });
        })

});





