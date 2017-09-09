function loadDatatable(table) {
    table = $('#datatable-user').DataTable({
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
            "url": "/User/GetUserDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "UserName", "name": "UserName", "width": "3%0" },
            { "data": "FullName", "name": "FullName", "width": "30%" },
            { "data": "Email", "name": "Email", "width": "30%" },
            {
                "targets": -1,
                "data": null,
                "width": "10%",
                "class": "dt-body-center",
                "orderable": false,
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
    var userNameBefore;
    if ($("#userId").val()) {
        userNameBefore = $("#userName").val();
    }
    $('.js-switch').click();
    //js for page change password
    if ($("#passOld").length) {
        $("#cancelbutton").click(function () {
            if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
                window.location.href = "../Home/Index";
            }
        });
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"))) {
                //check element
                var passOld = $("#passOld").val();
                var passNew = $("#passNew").val();
                var passNewAgain = $("#passNewAgain").val();
                if (passOld == passNew) {
                    toastr["error"]("Mật khẩu mới không đươc giống mật khẩu hiện tại.", "Lỗi");
                    return;
                }
                if (passNewAgain != passNew) {
                    toastr["error"]("Mật khẩu nhập lại không giống với mật khẩu mới.", "Lỗi");
                    return;
                }
                $.ajax({
                    url: "/User/ChangePassWord",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "passwordOld": $('#passOld').val(),
                        "passworldNew": $('#passNew').val(),
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "1") {
                            toastr["success"]("Mật khẩu đã thay đổi thành công.", "Thông báo")
                            window.setTimeout("window.location.href = '/Home/Index';", 2000);
                        }
                        else {
                            toastr["error"]("Đổi mật khẩu thất bại. Lỗi " + data, "Thông báo");
                        }
                    }
                })
            }
        });
    }
    else {
        if ($('#datatable-user').length) {
            table = loadDatatable(table);
            $('#datatable-user tbody').on('click', "tr td button[name='delete']", function () {
                if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                    var data = table.row($(this).closest('tr')).data();
                    $.ajax({
                        url: '/User/delete',
                        type: 'post',
                        datatype: 'json',
                        data: {
                            id: data.UserID
                        },
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data == "1") {
                                toastr["success"]("Xóa phần tử thành công.", "Thông báo");
                                table.destroy();
                                loadDatatable(table);
                            }
                            else {
                                toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại.", "Lỗi");
                            }
                        }
                    })
                }
            });
            $('#datatable-user tbody').on('click', "tr td button[name='edit']", function () {
                var data = table.row($(this).closest('tr')).data();
                window.location.href = "../User/Edit?id=" + data.UserID;
            });
        }
        else {
            $("#cancelbutton").click(function () {
                if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
                    window.location.href = "../User/Index";
                }
            });

            $("button[name='showOrHidePass']").click(function () {
                var span = $(this).find("span")[0];
                var className = span.className;
                $(this).parent().parent().find("input")[0].type = (className == "glyphicon glyphicon-eye-close") ? "text" : "password";
                span.className = (className == "glyphicon glyphicon-eye-close") ? "glyphicon glyphicon-eye-open" : "glyphicon glyphicon-eye-close";
                $(this).attr("class", (className == "glyphicon glyphicon-eye-close") ? "btn btn-success" : "btn btn-primary");
            });


            $("#sumbitbutton").click(function () {
                if (validator.checkAll($("form"))) {
                    var id = $("#userId").val();
                    var userName = $("#userName").val();
                    //check id excit is edit form contrary create form
                    if (userName) {
                        if (!id || (id && userName != userNameBefore)) {
                            $.ajax({
                                url: '/User/checkExistUser',
                                type: 'post',
                                datatype: 'json',
                                data: {
                                    userName: userName
                                },
                                async: true,
                                cache: false,
                                success: function (data) {
                                    if (data == '1') {
                                        $("#userName").focus();
                                        toastr["error"]("Tên tài khoản đã tồn tại.", "Lỗi");
                                        return;
                                    } else {
                                        addOrUpdateEntity();
                                    }
                                }
                            })
                        }
                        if (id && userName == userNameBefore) {
                            addOrUpdateEntity();
                        }
                    }
                }
            });
        }
    }
});
function addOrUpdateEntity() {
    $.ajax({
        url: "/User/AddOrUpdateEntity",
        type: 'post',
        datatype: 'json',
        contentType: 'application/json',
        data: JSON.stringify({
            "item": new Object({
                "UserID": $('#userId').length > 0 ? $('#userId').val() : 0,
                "UserName": $('#userName').val(),
                "Password": $('#pass').val(),
                "FullName": $('#fullName').val(),
                "IsActive": document.querySelector('.js-switch').checked,
                "Email": $('#email').val(),
                "CardNumber": "0",
                "RoleID": null,
                "IsAdmin": false,
                "IsConfig": $('#IsConfig')[0].checked,
                "IsRegisterParty": $('#IsRegisterParty')[0].checked,
                "IsMaterial": $('#IsMaterial')[0].checked,
                "IsAttendance": $('#IsAttendance')[0].checked,
                "IsList": $('#IsList')[0].checked,
                "IsReport": $('#IsReport')[0].checked
            }),
            "isEdit": $('#userId').length > 0 ? true : false
        }),
        async: true,
        cache: false,
        success: function (data) {
            if (data == "0") {
                toastr["error"]("Tài khoản lưu thất bại.", "Thông báo")
                flag = false;
            }
            else {
                toastr["success"]("Tài khoản lưu thành công.", "Thông báo")
                window.location.href = "/User/Index";
            }
        }
    })
}