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
            "url": "/Department/GetDepartmentDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },

        "columns": [
            { "data": "DepartmentName", "name": "DepartmentName", "width": "80%", "orderable": true },
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
            window.location.href = "../Department/Index";
        }
    });
    if ($('#datatable-item').length) {
        table = loadDatatable(table);
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Department/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.DepartmentID
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
            window.location.href = "../Department/Edit?id=" + data.DepartmentID;
        });
    } else {
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"))) {
                $.ajax({
                    url: "/Department/AddOrUpdateEntity",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "item": new Object({
                            "DepartmentID": $('#departmentID').length > 0 ? $('#departmentID').val() : 0,
                            "DepartmentName": $('#departmentName').val()
                        }),
                        "isEdit": $('#departmentID').length > 0 ? true : false
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Thêm phòng ban thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Thêm phòng ban thành công.", "Thông báo")
                            window.location.href = "/Department/Index";
                        }
                    }
                })
            }
        });
    }
});