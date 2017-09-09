$(function () {
    var STT = 0;
    $('#dateFrom').daterangepicker({
        "singleDatePicker": true,
        "startDate": new Date(),
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
    $('#dateTo').daterangepicker({
        "singleDatePicker": true,
        "singleClasses": "picker_3",
        "startDate": new Date(),
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
        debugger;
    });
    $('#vendor').select2({ dropdownAutoWidth: true });
    var table = loadDataTable(table, STT);
    $("#printButton").click(function () {
        $.ajax({
            url: '/Report/ExportReportAttendance',
            type: 'post',
            datatype: 'json',
            data: {
                dateFrom: $("#dateFrom").val(),
                dateTo: $("#dateTo").val(),
                employeeId: $("#employee").val()
            },
            async: false,
            cache: false,
            success: function (data) {
                if (data.split('!')[0] == "0") {
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
    $("#watch").click(function () {
        var DateFrom = new Date($('#dateFrom').val().split('/')[2], ($('#dateFrom').val().split('/')[1] - 1), $('#dateFrom').val().split('/')[0]);
        var DateTo = new Date($('#dateTo').val().split('/')[2], ($('#dateTo').val().split('/')[1] - 1), $('#dateTo').val().split('/')[0]);
        if (DateFrom.getTime() > DateTo.getTime()) {
            $("#dateFrom").trigger('click');
            toastr["error"]("Ngày đến phải nhỏ hơn ngày đi.", "Lỗi");
        }
        else {
            table.destroy();
            table = loadDataTable(table, STT);
        }
    })
})
function loadDataTable(table, STT) {
    table = $('#datatable-report').DataTable({
        "processing": true, // for show processing bar
        "serverSide": true, // for process on server side
        "orderMulti": false, // for disable multi column order     
        "paging": false,
        "info": false,
        "searching": false,
        "ordering": false,
        "language": {
            "url": languageDatatable
        },
        "ajax": {
            "url": "/ReportMaterialByVendor/GetEntityForDatatable",
            "type": "POST",
            "datatype": "json",
            "data": {
                'dateFrom': $("#dateFrom").val(),
                'dateTo': $("#dateTo").val(),
                'vendor': $("#vendor").val()
            }
        },
        "columns": [
            {
                "data": "null", "width": "10%", "class": "dt-body-center",
                "render": function (data) {
                    return ++STT;
                }
            },
            { "data": "MaterialName", "name": "MaterialName", "width": "60%" },
            { "data": "VendorName", "name": "VendorName", "width": "20%" },
            { "data": "HomePhone", "name": "HomePhone", "width": "10%" },
            { "data": "PhoneNumber", "name": "PhoneNumber", "width": "10%" },
            { "data": "Address", "name": "Address", "width": "35%" },
        ]
    });
    return table;
}