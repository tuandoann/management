Date.prototype.getFormattedTime = function () {
    var hours = this.getHours() == 0 ? "12" : this.getHours() > 12 ? this.getHours() - 12 : this.getHours();
    var minutes = (this.getMinutes() < 10 ? "0" : "") + this.getMinutes();
    var ampm = this.getHours() < 12 ? "AM" : "PM";
    var formattedTime = hours - 1 + ":" + minutes + " " + ampm;
    return formattedTime;
}
Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}
function loadDatatable(table) {
    table = $('#datatable-item').DataTable({
        "processing": true, // for show processing bar
        "serverSide": true, // for process on server side
        "orderMulti": false, // for disable multi column order
        "iDisplayLength": 20,
        "lengthMenu": [[10, 20, 50, 100], [10, 20, 50, 100]],
        "info": false,
        "searching": false,
        "language": {
            "url": languageDatatable
        },
        "ajax": {
            "url": "/PricePayroll/GetPricePayrollDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "formatNumber": function (toFormat) {
            return toFormat.toString().replace(
              /\B(?=(\d{3})+(?!\d))/g, "'"
            )
        },
        "columns": [
            {
                "data": "TimeFrom", "name": "TimeFrom", "width": "30%", "orderable": true, "class": "dt-body-center",
                "render": function (data) {
                        return new Date(parseInt(data.split('(')[1].split(')')[0])).getFormattedTime();
                }
            },
            {
                "data": "TimeTo", "name": "TimeTo", "width": "30%", "orderable": true, "class": "dt-body-center",
                "render": function (data) {
                    return new Date(parseInt(data.split('(')[1].split(')')[0])).getFormattedTime();
                }
            },
            { "data": "UnitPrice", "name": "UnitPrice", "width": "30%", "orderable": true, "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
            {
                "targets": -1,
                "data": null,
                "width": "10%",
                "orderable": false,
                "class": "dt-body-center",
                "render": function (data) {
                    return "<button class=' btn btn-info btn-xs' name='edit'>Sửa</button><button class=' btn btn-danger btn-xs' name='delete' >Xóa</button>";
                }
            }
        ]
    });
    return table;
}
$(function () {
    var table;
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../PricePayroll/Index";
        }
    });
    if ($('#datatable-item').length) {
        table = loadDatatable(table);
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/PricePayroll/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.PriceID
                    },
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            toastr["success"]("Xóa phần tử thành công.", "Thông báo");
                            table.row($(this).closest('tr')).remove().draw();
                        }
                        else {
                            toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại.", "Lỗi");
                        }
                    }
                })
            }
        });
        $('#datatable-item tbody').on('click', "tr td button[name='edit']", function () {
            var data = table.row($(this).closest('tr')).data();
            window.location.href = "../PricePayroll/Edit?id=" + data.PriceID;
        });
    } else {
        if ($('#priceID').length <= 0)
        {
            $('#timeTo, #timeFrom').timepicki();
        }
        init_controlInputMarskNumber('#unitPrice');
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"))) {
                $.ajax({
                    url: "/PricePayroll/AddOrUpdateEntity",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "item": new Object({
                            "PriceID": $('#priceID').length > 0 ? $('#priceID').val() : 0,
                            "TimeFrom": new Date('1/1/2000 ' + $('#timeFrom').val()).addHours(1),
                            "TimeTo": new Date('1/1/2000 ' +  $('#timeTo').val()).addHours(1),
                            "UnitPrice": $('#unitPrice').val().replace(',', '')
                        }),
                        "isEdit": $('#priceID').length > 0 ? true : false
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Thêm đơn giá lương thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Thêm đơn giá lương thành công.", "Thông báo")
                            window.location.href = "/PricePayroll/Index";
                        }
                    }
                })
            }
        });
    }
});