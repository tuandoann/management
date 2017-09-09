$(function () {
    var STT = 0;
    var table = $('#datatable-item').DataTable({
        "processing": true, // for show processing bar
        "serverSide": true, // for process on server side
        "orderMulti": false, // for disable multi column order     
        "paging": false,
        "info": false,
        "searching": false,
        "language": {
            "url": languageDatatable,
            //"decimal": ".",
            //"thousands": ","
        },
        "ajax": {
            "url": "/PrintDetailMaterial/GetViewReportDatatableIndex",
            "type": "POST",
            "datatype": "json",
            "data": {
                'id': $.urlParam('id'),
            }
        },
        "columns": [
            {
                "data": "null", "width": "40%", "class": "dt-body-center",
                "render": function (data) {
                    return ++STT;
                }
            },
            { "data": "MaterialName", "name": "MaterialName", "width": "40%", "class": "dt-body-left", },
            {
                "data": "Quantity", "name": "Quantity", "width": "40%", "class": "dt-body-right",
                "render": $.fn.dataTable.render.number(',', '.', 4, '')
            },
            { "data": "UOMName", "name": "UOMName", "width": "40%", "class": "dt-body-left", },
            {
                "data": "UnitPrice", "name": "UnitPrice", "width": "40%", "class": "dt-body-right",
                "render": $.fn.dataTable.render.number(',', '', 0, ''),
            },
            { "data": "VendorName", "name": "VendorName", "width": "40%", "class": "dt-body-left", },
            {
                "data": "IsDelivery", "name": "IsDelivery", "width": "40%", "class": "dt-body-center",
                "render": function (data) {
                    return "<th><input  type='checkbox' name='isDelivery' " + (data == true ? "checked" : "") + " disabled /></th>";
                }
            },
        ],
        "drawCallback": function (settings) {
            STT = 0;
            $("input[name='isDelivery']").iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green'
            });
        }
    });
    $("#backButton").click(function () {
        window.location.href = "../Party/ViewProductMaterial?id=" + $.urlParam('id');
    });


});
