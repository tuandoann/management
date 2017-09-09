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
function checkUserExcitedBeforeSumbit(e, userName) {
    $.ajax({
        url: '/User/checkExistUser',
        type: 'post',
        datatype: 'json',
        data: {
            userName: userName
        },
        async: false,
        cache: false,
        success: function (data) {
            if (data == '1') {
                e.preventDefault();
                toastr["error"]("Tên tài khoản đã tồn tại.", "Lỗi");
            }
        }
    })
}
$(function () {
    var table;
    var userNameBefore;
    if ($("#userId").val())
    {
        userNameBefore = $("#userName").val();
    }
    $('.js-switch').click();
    if ($('#datatable-user').length) {
        table = loadDatatable(table);
        $('#datatable-user tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!"))
            {
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
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại."))
        {
            window.location.href = "../User/Index";
        }
    });
    $("#sumbitbutton").click(function () {
        if (validator.checkAll($("form"))) {

        }
    });
    //event sumbit form check user excit
    //$("#formUser").submit(function (e) {
    //    var id = $("#userId").val();
    //    var userName = $("#userName").val();
    //    //check id excit is edit form contrary create form
    //    if(userName)
    //    {
    //        if (!id || (id && userName != userNameBefore) )
    //        {
    //            checkUserExcitedBeforeSumbit(e, userName);
    //        }
    //    }
    //});
});