function loadDatatable(table) {
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
            "url": "/Vendor/GetVendorDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },

        "columns": [
            { "data": "VendorName", "name": "VendorName", "width": "40%", "orderable": true },
            { "data": "PhoneNumber", "name": "PhoneNumber", "width": "10%", "orderable": true },
            { "data": "Address", "name": "Address", "width": "40%", "orderable": true },
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
            window.location.href = "../Vendor/Index";
        }
    });
    if ($('#datatable-item').length) {
        table = loadDatatable(table);
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Vendor/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.VendorID
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
            window.location.href = "../Vendor/Edit?id=" + data.VendorID;
        });
    } else {
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"), true)) {
                $.ajax({
                    url: "/Vendor/AddOrUpdateEntity",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "item": new Object({
                            "VendorID": $('#vendorID').length > 0 ? $('#vendorID').val() : 0,
                            "VendorName": $('#vendorName').val(),
                            "PhoneNumber": $('#phoneNumber').val(),
                            "Address": $('#address').val(),
                            "Notes": $('#notes').val()
                        }),
                        "isEdit": $('#vendorID').length > 0 ? true : false
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Thêm nhà cung cấp thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Thêm nhà cung cấp thành công.", "Thông báo")
                            window.location.href = "/Vendor/Index";
                        }
                    }
                })
            }
        });
    }
});