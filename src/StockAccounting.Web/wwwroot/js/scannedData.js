var items = [];

$(function () {
    $.ajax({
        url: '/ScannedData/GetAutoCompleteExternal',
        type: 'GET',
        async: false,
        success: function (data) {
            items = data;
        }
    });

    var pageSize = 50;

    jQuery.fn.select2.amd.require(["select2/data/array", "select2/utils"], function (ArrayData, Utils) {
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

            var data = {
                results: results.slice((params.page - 1) * pageSize, params.page * pageSize),
                pagination: {
                    more: params.page * pageSize < results.length
                }
            };

            callback(data);
        };

        $(document).ready(function () {
            $("#selectExternalData").select2({
                dataAdapter: CustomData,
                dropdownParent: $("#newScannedDataModal")
            });

            //$("#externalDataId").select2({
            //    dataAdapter: CustomData,
            //    dropdownParent: $("#editScannedDataModal")
            //});
        });
    });
});


$("#newScannedDataButton").on('click',
    function () {
        $("#newScannedDataModal").modal('show');
        $("#scannedDataModalClose").click(function () {
            $("#newScannedDataModal").modal('hide');
        });

        $("#ScannedDataModalComplete").submit(function () {

            $("#quantity").val(function (_, val) {
                return val.replace(",", ".");
            });

            var postData = $("#scannedDataModalForm").serialize();

            $.ajax({
                type: "post",
                url: "/InsertScannedData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData,
                success: function () {
                    $("#newScannedDataModal").modal('hide');
                },
                error: function (textStatus) {
                    alert("Request failed: " + textStatus);
                }
            });
        });
    });


$('#editScannedDataModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);

    var id = button.data('id');
    var externalDataId = button.data('externaldataid');
    var documentDataId = button.data('documentdataid');
    var quantity = button.data('quantity');

    var modal = $(this);
    modal.find('#id').val(id);
    modal.find('#documentDataId').val(documentDataId);
    modal.find('#externalDataId').val(externalDataId);
    modal.find('#quantity').val(quantity);
    modal.find('#quantityBefore').val(quantity);

    var externalSelect = modal.find('#externalDataId');
    var selectedItem = items.find(x => x.id == externalDataId);

    if (selectedItem) {
        var newOption = new Option(selectedItem.text, selectedItem.id, true, true);
        externalSelect.append(newOption).trigger('change');
    }

    $("#scannedDataModalForm").off('submit').on('submit', function (e) {
        e.preventDefault();

        $("#quantity").val(function (_, val) {
            return val.replace(",", ".");
        });

        var postData = $(this).serialize();

        $.ajax({
                type: "post",
                url: "/UpdateScannedData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: postData
            })
            .done(function () {
                $("#editScannedDataModal").modal('hide');
            })
            .fail(function () {
                window.location.reload();
            });
    });
});


$('#deleteScannedDataModal').on('show.bs.modal',
    function (event) {
        var button = $(event.relatedTarget);
        var id = button.data('id');

        var modal = $(this);
        modal.find('#id').val(id);

        $("#btnYes").click(function () {
            $.ajax({
                type: "post",
                url: "/DeleteScannedData",
                dataType: "json",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: {
                    Id: id
                }
            })
                .done(function () {
                    window.location.reload();
                    $("#deleteScannedDataModal").modal('hide');
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
        var searchText = $("#searchScannedDataId").val();
        url = url.replace("dummyText", searchText);

        window.location.href = url;

    });

    $("#btnScannedDataClearSearch").click(function () {
        $("#searchScannedDataId").val("");
        $("#btnSearchScannedData").click();
    });

    $(document).on("keypress", function (e) {
        if (e.which === 13) {
            switch (document.activeElement.id) {
                case "searchScannedDataId":
                    $("#btnSearchScannedData").click();
                    break;
                default:
                    alert("No active element found!");
            }
        }
    });
});

