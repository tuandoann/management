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
        "columnDefs": [
            { "visible": false, "targets": 1 }
        ],
        "ajax": {
            "url": "/ReportPartyList/GetEntityForDatatable",
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
                    return 1;
                }
            },
            { "data": "PartyDate", "name": "PartyDate", "width": "10%" },
            { "data": "CustomerName", "name": "CustomerName", "width": "20%" },
            { "data": "PartyType", "name": "PartyType", "width": "15%" },
            { "data": "PartyAddress", "name": "PartyAddress", "width": "15%" },
            { "data": "NumberTablePlan", "name": "NumberTablePlan", "width": "10%", "render": $.fn.dataTable.render.number(',', '', 0, ''), "class": "dt-body-right" },
            { "data": "NumberTableException", "name": "NumberTableException", "width": "10%", "render": $.fn.dataTable.render.number(',', '', 0, ''), "class": "dt-body-right" },
            { "data": "NumberTableVegetarian", "name": "NumberTableVegetarian", "width": "10%", "render": $.fn.dataTable.render.number(',', '', 0, ''), "class": "dt-body-right" },
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var stt = 0;
            api.column(1, { page: 'current' }).data().each(function (partyDate, i) {
                if (last !== partyDate) {
                    stt = 0;
                    $(rows).eq(i).before(
                        '<tr class="group"><td colspan="7" class="dt-body-center"><h4><strong>Ngày ' + partyDate + '</strong></h4></td></tr>'
                    );
                    last = partyDate;
                }
                $(rows).eq(i)[0].cells[0].innerHTML = ++stt;
            });
        }
    });
    return table;
}