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
            "url": "/Employee/GetEmployeeDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "order": [[0, 'asc']],
        "columns": [
            { "data": "FullName", "name": "FullName", "width": "30%", "orderable": true },
            { "data": "PhoneNumber", "name": "PhoneNumber", "width": "15%", "orderable": true },
            { "data": "DepartmentName", "name": "DepartmentName", "width": "15%", "orderable": true },
            {
                "data": "IsCalculatePayroll", "name": "IsCalculatePayroll", "width": "10%", "class": "dt-body-center",
                "render": function (data) {
                    return "<th><input type='checkbox' name='isCalculatePayroll' " + (data == true ? "checked" : "") + " disabled /></th>";
                }
            },
            {
                "data": "IsCalAttendance", "name": "IsCalAttendance", "width": "10%", "class": "dt-body-center",
                "render": function (data) {
                    return "<th><input type='checkbox' name='isCalAttendance' " + (data == true ? "checked" : "") + " disabled /></th>";
                }
            },
            {
                "data": "IsLeaveOff", "name": "IsLeaveOff", "width": "10%", "class": "dt-body-center",
                "render": function (data) {
                    return "<th><input type='checkbox' name='isLeaveOff' " + (data == true ? "checked" : "") + " disabled /></th>";
                }
            },
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
        ],
        "drawCallback": function (settings) {
            $("input[name='isCalculatePayroll'],input[name='isCalAttendance'],input[name='isLeaveOff']").iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green'
            });
        }
    });
    return table;
}
$(function () {
    var table;
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Employee/Index";
        }
    });
    if ($('#datatable-item').length) {
        table = loadDatatable(table);
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Employee/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.EmployeeID
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
            window.location.href = "../Employee/Edit?id=" + data.EmployeeID;
        });
    } else {
        $('#departmentID').select2({});
        var isCalculatePayroll = document.querySelector('#isCalculatePayroll');
        var switchIsCalculatePayroll = new Switchery(isCalculatePayroll);

        var isCalAttendance = document.querySelector('#isCalAttendance');
        var switchIsCalAttendance = new Switchery(isCalAttendance);

        var isLeaveOff = document.querySelector('#isLeaveOff');
        var switchIsLeaveOff = new Switchery(isLeaveOff);

        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"),true)) {
                $.ajax({
                    url: "/Employee/AddOrUpdateEntity",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "item": new Object({
                            "EmployeeID": $('#employeeID').length > 0 ? $('#employeeID').val() : 0,
                            "FullName": $('#fullName').val(),
                            "IDCardNo": $('#iDCardNo').val(),
                            "PhoneNumber": $('#phoneNumber').val(),
                            "Address": $('#address').val(),
                            "IsCalculatePayroll": isCalculatePayroll.checked,
                            "IsCalAttendance": isCalAttendance.checked,
                            "IsLeaveOff": isLeaveOff.checked,
                            "DepartmentID": $('#departmentID').val(),
                        }),
                        "isEdit": $('#employeeID').length > 0 ? true : false
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Thêm nhân viên thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Thêm nhân viên thành công.", "Thông báo")
                            window.location.href = "/Employee/Index";
                        }
                    }
                })
            }
        });
    }
});