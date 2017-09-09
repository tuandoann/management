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
            "url": "/ReportProductList/GetEntityForDatatable",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [
            { "visible": false, "targets": 1 },
            { "visible": false, "targets": 2 }
        ],
        "columns": [
            {
                "data": "null", "width": "10%", "class": "dt-body-center",
                "render": function (data) {
                    return 1;
                }
            },
            { "data": "GroupName2", "name": "GroupName2", "width": "60%" },
            { "data": "GroupName1", "name": "GroupName1", "width": "60%" },
            { "data": "ProductName", "name": "ProductName", "width": "60%" },
            { "data": "Notes", "name": "Notes", "width": "60%" },
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var groupName1Before = null;
            var groupName2Before = null;
            var stt = 0;
            $.each(api.rows().data(), function (index, row) {
                if (groupName2Before != row.GroupName2)
                {
                    $(rows).eq(index).before(
                        '<tr class="group"><td colspan="3" class="dt-body-center"><h4><strong>' + row.GroupName2 + '</strong></h4></td></tr>'
                    );
                    groupName2Before = row.GroupName2;
                    groupName1Before = null;
                }
                if (groupName1Before != row.GroupName1) {
                    $(rows).eq(index).before(
                        '<tr class="group"><td colspan="3"><p><strong>' + row.GroupName1 + '</strong></p></td></tr>'
                    );
                    groupName1Before = row.GroupName1;
                    stt = 0;
                }
                $(rows).eq(index)[0].cells[0].innerHTML = ++stt;
            });
        }
    });
    return table;
}