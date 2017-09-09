$(function () {
    $.cookie('beforePageIsViewProductMaterial', new Boolean(true));
    $("#titlePage").html("ĐỊNH LƯỢNG THEO TIỆC NGÀY " + $.urlParam("date")); 
    $("#backButton").click(function () {
        window.location.href = "../Party/MaterialParty";
    });
    $("#printButton").click(function () {
        $.ajax({
            url: '/Party/ExportMaterialParty',
            type: 'post',
            datatype: 'json',
            data: {
                valueDate: $.urlParam("date")
            },
            async: false,
            cache: false,
            success: function (data) {
                if (data.split('!')[0] == "0") {
                    ShowError("Xuất báo cáo thất bại");
                    console.log(data.split('!')[1]);
                    flag = false;
                }
                else {
                    debugger;
                    window.location.href = window.location.origin + (data.split('!')[1]);
                }
            }
        })
    });
    var STT = 0;
    var table = $('#datatable-report').DataTable({
        "processing": false, // for show processing bar
        "serverSide": false, // for process on server side
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false,
        "language": {
            "url": languageDatatable
        },
        "columnDefs": [
            { "width": "10%", "targets": 0 },
            { "width": "35%", "targets": 1 },
            { "width": "10%", "targets": 2 },
            { "width": "10%", "targets": 3 },
            { "width": "10%", "targets": 4 },
            { "width": "15%", "targets": 5 },
            { "width": "10%", "targets": 6 }
        ],
        "formatNumber": function (toFormat) {
            return toFormat.toString().replace(
              /\B(?=(\d{3})+(?!\d))/g, "'"
            )
        },
        "columns": [
           { "data": "null", "class": "dt-body-center", },
           { "data": "MaterialName", "name": "MaterialName"},
           {"data": "Quantity", "name": "Quantity", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '.', 4, '') },
           { "data": "UOMName", "name": "UOMName" },
           { "data": "UnitPrice", "name": "UnitPrice", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, ''), },
           { "data": "VendorName", "name": "VendorName" },
           { "data": "IsDelivery", "name": "IsDelivery", "class": "dt-body-center",
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
})