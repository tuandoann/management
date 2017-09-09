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
            "url": "/ReportSaleByCustomer/GetEntityForDatatable",
            "type": "POST",
            "datatype": "json",
            "data": {
                'dateFrom': $("#dateFrom").val(),
                'dateTo': $("#dateTo").val(),
            }
        },
        "columns": [
            {
                "data": "null", "width": "10%", "class": "dt-body-center",
                "render": function (data) {
                    return ++STT;
                }
            },
            { "data": "CustomerName", "name": "CustomerName", "width": "60%"},
            { "data": "SaleAmount", "name": "SaleAmount", "width": "30%", "render": $.fn.dataTable.render.number(',', '', 0, ''), "class": "dt-body-right" },
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var sum = 0;
            api.column(2, { page: 'current' }).data().each(function (saleAmount, i) {
                sum += saleAmount;
            });
            $(rows).eq(rows.length - 1).after(
                        '<tr><td class="dt-body-center" colspan="2"><strong>Cộng</strong></td><td class="dt-body-right"><strong>' + $.number(sum, 0, '', ',') + '</strong></td></tr>'
                    );
        }
    });
    return table;
}