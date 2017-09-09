$(function () {
    $("#printButton").click(function () {
        $.ajax({
            url: '/Report/ExportEXcel',
            type: 'post',
            datatype: 'json',
            data: {
                timeTo: $("#dateFrom").val(),
                timeFrom: $("#dateTo").val(),
                selectEmployee: $("#employee").val(),
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
    $("#watch").click(function () {
        var DateFrom = new Date($('#dateFrom').val().split('/')[2], ($('#dateFrom').val().split('/')[1] - 1), $('#dateFrom').val().split('/')[0]);
        var DateTo = new Date($('#dateTo').val().split('/')[2], ($('#dateTo').val().split('/')[1] - 1), $('#dateTo').val().split('/')[0]);
        if(DateFrom.getTime() > DateTo.getTime())
        {
            $("#dateFrom").trigger('click');
            toastr["error"]("Ngày đến phải nhỏ hơn ngày đi.", "Lỗi");
        }
        else {
            $("#formReport").trigger('submit');
        }
    })
    $('#dateFrom').daterangepicker({
        "singleDatePicker": true,
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
        "locale": {
            "format": "DD/MM/YYYY",
            "applyLabel": "Đồng ý",
            "cancelLabel": "Hủy",
            "firstDay": 1
        }
    }, function (start, end, label) {
        debugger;
    });
    $('#employee').select2({ dropdownAutoWidth: true });
})