var stt = 0;
var listParty = "";
$(function () {
    var beforePageIsViewProductMaterial = $.cookie('beforePageIsViewProductMaterial');
    var valueDateMaterialParty = $.cookie('valueDateMaterialParty');
    var dateStartPartyDate = new Date();
    if (valueDateMaterialParty != undefined) {
        var valueDateMaterialPartyArray = valueDateMaterialParty.split("/");
        dateStartPartyDate = (beforePageIsViewProductMaterial == "true") ? new Date(valueDateMaterialPartyArray[2], valueDateMaterialPartyArray[1] - 1, valueDateMaterialPartyArray[0]) : new Date();
    }
    $('#partyDate').daterangepicker({
        "singleDatePicker": true,
        "startDate": dateStartPartyDate,
        "singleClasses": "picker_3",
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    });
    var table;
    var date = new Date();
    $("#selectAll, #printAll, #printDinhLuong").iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });
    table = loadDatable(table, true, getFormattedDate(dateStartPartyDate));
    $("#searchFrom").click(function () {
        table.destroy();
        table = loadDatable(table, true, $('#partyDate').val());
    });
    $('#datatable-item tbody').on('click', "tr td button[name='watch']", function () {
        var data = table.row($(this).closest('tr')).data();
        window.location.href = "../Party/ViewProductMaterial?id=" + data.PartyID;
    });
    $('#selectAll').on('ifChanged', function (event) {
        if (event.target.checked)
            table.rows().select();
        else
            table.rows().deselect();
    }); 
    $("#excutePrint").click(function (event) {
        var groupName = "groupName=" +  1;
        var isHideRadio = "isHideRadio=" + true;
        var partyId = "listParty=" + listParty;
        var isPrintAll = "isPrintAll=" + ($('#printAll').is(':checked') ? true : false);
        var url = "/print/MaterialParty?" + partyId + "&&" + isPrintAll + "&&" + groupName + "&&" + isHideRadio;
        var win = window.open(url, '_blank');
        win.focus();
    });
    $("#printButton").click(function (event) {
        var countItemSelected = table.rows({ selected: true }).count();
        if (countItemSelected == 0) {
            toastr["info"]("Xin vui lòng chọn ít nhất một tiệc để in.", "Thông báo");
            event.stopPropagation();
        }
    });
    $("#calculatorButton").click(function () {
        if (confirm("Bạn đã sẵn sàng tính định lượng.")) {
            var arrayPartyID = "";
            var countItemSelected = table.rows({ selected: true }).count();
            if (countItemSelected == 0) {
                toastr["info"]("Xin vui lòng chọn ít nhất một tiệc.", "Thông báo");
                return;
            }
            $.each(table.rows({ selected: true }).data(), function (index, item) {
                arrayPartyID += (item.PartyID + ((index == countItemSelected - 1) ? "" : ","));
            });

            $.ajax({
                url: '/Party/CalculatorProductMaterial',
                type: 'post',
                datatype: 'json',
                contentType: 'application/json',
                data: JSON.stringify({
                    "arrayPartyID": arrayPartyID
                }),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.toString().toLowerCase() == "true") {
                        toastr["success"]("Tính dữ liệu thành công.", "Thông báo");
                        $("#lable-datatable_view").css("display", "block");
                        $("#partialViewTinhDinhLuong").css("display", "inline-block");
                        $("#partialViewTinhDinhLuong").load("/Party/Partial_TinhDinhLuong", { listPartyId: arrayPartyID, groupName: 1, isHideRadio: false }, function () {
                            console.log("Hoàn tất load datatable");
                        });
                    }
                    else {
                        toastr["error"]("Tính dữ liệu thất bại");
                    }
                }
            })
        }
    });
    table.on('select', function (e, dt, type, indexes) {
        $.each(indexes, function (index, item) {
            var rowData = table.rows(item).data()[0];
            listParty += (listParty == "" ? "" : ",") + rowData.PartyID;
        });
        $("#lable-datatable_view").css("display", "block");
        $("#partialViewTinhDinhLuong").css("display", "inline-block");
        $("#partialViewTinhDinhLuong").load("/Party/Partial_TinhDinhLuong", { listPartyId: listParty, groupName: 1, isHideRadio: false }, function () {
            console.log("Hoàn tất load datatable");
        });
    }).on('deselect', function (e, dt, type, indexes) {
        $.each(indexes, function (index, item) {
            var rowData = table.rows(item).data()[0];
            var list = "";
            var arrayListParty = listParty.split(',');
            $.each(arrayListParty, function (index, item) {
                if (item != rowData.PartyID)
                    list += (list == "" ? "" : ",") + item;
            });
            listParty = list;
        });
        $("#lable-datatable_view").css("display", "block");
        $("#partialViewTinhDinhLuong").css("display", "inline-block");
        $("#partialViewTinhDinhLuong").load("/Party/Partial_TinhDinhLuong", { listPartyId: listParty, groupName: 1, isHideRadio: false }, function () {
            console.log("Hoàn tất load datatable");
        });

    });
});
window.onbeforeunload = function () {
    $.cookie('valueDateMaterialParty', $('#partyDate').val());
    $.cookie('beforePageIsViewProductMaterial', new Boolean(false));
};
function getFormattedDate(date) {
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year;
}
function loadDatable(table, isSearchDate, valueDate) {
    table = $('#datatable-item').DataTable({
        "processing": true, // for show processing bar
        "serverSide": true, // for process on server side
        "orderMulti": false, // for disable multi column order
        "iDisplayLength": 20,
        "lengthMenu": [[10, 20, 50, 100], [10, 20, 50, 100]],
        "info": false,
        "searching": false,
        "order": [[1, 'asc']],
        "autoWidth": false,
        "language": {
            "url": languageDatatable
        },
        "ajax": {
            "url": "/Party/GetPartyDatatableIndex",
            "type": "POST",
            "datatype": "json",
            "data": {
                "isSearchDate": isSearchDate,
                "valueDate": valueDate,
            }
        },
        select: {
            style: 'multi',
            selector: 'td:last-child'
        },
        "columns": [
            {
                "data": null, "name": null, "width": "5%", "orderable": false, "class": "dt-body-center",
                render: function () {
                    return ++stt;
                }
            },
            { "data": "PartyID", "name": "PartyID", "visible": false },
            {
                "data": "PartyDate", "name": "PartyDate", "width": "10%", "class": "dt-body-center",
                render: function (data) {
                    var arraySplit = data.split(':');
                    return (arraySplit[0].length > 1 ? arraySplit[0] : "0" + arraySplit[0]) + ":" + (arraySplit[1].length > 1 ? arraySplit[1] : "0" + arraySplit[1]);
                }
            },
            { "data": "CustomerName", "name": "CustomerName", "width": "15%" },
            { "data": "PartyAddress", "name": "PartyAddress", "width": "25%" },
            { "data": "NumberTableReal", "name": "NumberTableReal", "width": "5%", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
            { "data": "NumberTableException", "name": "NumberTableException", "width": "10%", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
            { "data": "NumberTableVegetarian", "name": "NumberTableVegetarian", "width": "10%", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
            {
                "targets": -1,
                "data": null,
                "width": "10%",
                "orderable": false,
                "class": "dt-body-center",
                "render": function (data) {
                    return "<button class=' btn btn-success btn-xs' name='watch' type='button' >Xem</button>";
                }
            },
            { "orderable": false, "data": null, "name": null, "className": 'select-checkbox dt-head-center', "width": "5%", "render": function (data) { return ""; } },
        ],
        "drawCallback": function (settings) {
            stt = 0;
            listParty = "";
            $("#lable-datatable_view").css("display", "none");
            $("#partialViewTinhDinhLuong").css("display", "none");
        },
    });
    return table;
}