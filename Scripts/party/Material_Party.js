$(function () {
    var beforePageIsViewProductMaterial = $.cookie('beforePageIsViewProductMaterial');
    var valueDateMaterialParty = $.cookie('valueDateMaterialParty');
    var dateStartPartyDate = new Date();
    if (valueDateMaterialParty  != undefined)
    {
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
    }, function (start, end, label) {
        debugger;
    }); 
    var table;
    var date = new Date();
    table = loadDatable(table, true, getFormattedDate(dateStartPartyDate));
    $("#printButton").click(function () {
        window.location.href = "../Party/ReportMaterialParty?date=" + $('#partyDate').val();
    });
    $("#searchFrom").click(function () {
        table.destroy();
        table = loadDatable(table, true, $('#partyDate').val());
    });
    $('#datatable-item tbody').on('click', "tr td button[name='watch']", function () {
        var data = table.row($(this).closest('tr')).data();
        window.location.href = "../Party/ViewProductMaterial?id=" + data.PartyID;
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
        "language": {
            "url": languageDatatable
        },
        "ajax": {
            "url": "/Party/GetPartyDatatableIndex",
            "type": "POST",
            "datatype": "json",
            "data": {
                "isSearchDate": isSearchDate,
                "valueDate" : valueDate,
            }
        },
        "columns": [
            {
                "data": "PartyDate", "name": "PartyDate", "width": "10%", "orderable": true, "class": "dt-body-center",
                "render": function (data) {
                    return (data.split('/')[0].length == 1 ? "0" + data.split('/')[0] : data.split('/')[0]) + "/" + (data.split('/')[1].length == 1 ? "0" + data.split('/')[1] : data.split('/')[1]) + "/" + data.split('/')[2];
                }
            },
            { "data": "CustomerName", "name": "CustomerName", "width": "50%", "orderable": true },
            {
                "data": "NumberTablePlan", "name": "NumberTablePlan", "width": "10%", "orderable": true, "class": "dt-body-right",
                "render": function (data) {
                    return $.number(data, 0, '', ',');
                }
            },
            {
                "data": "NumberTableException", "name": "NumberTableException", "width": "10%", "orderable": true, "class": "dt-body-right",
                "render": function (data) {
                    return $.number(data, 0, '', ',');
                }
            },
            {
                "data": "NumberTableVegetarian", "name": "NumberTableVegetarian", "width": "10%", "orderable": true, "class": "dt-body-right",
                "render": function (data) {
                    return $.number(data, 0, '', ',');
                }
            },
            {
                "targets": -1,
                "data": null,
                "width": "10%",
                "orderable": false,
                "class": "dt-body-center",
                "render": function (data) {
                    return "<button class=' btn btn-success btn-xs' name='watch' type='button' >Xem</button>";
                }
            }
        ]
    });
    return table;
}