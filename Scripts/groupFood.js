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
            "url": "/GroupFood/GetGroupFoodDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "GroupName", "name": "GroupName", "width": "40%" },
            { "data": "ParentName", "name": "ParentName", "width": "40%" },
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
function DeleteConfirm(id)
{
    if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
        $.ajax({
            url: '/GroupFood/delete',
            type: 'post',
            datatype: 'json',
            data: {
                id: id
            },
            async: false,
            cache: false,
            success: function (data) {
                if (data == "1") {
                    toastr["success"]("Xóa phần tử thành công.", "Thông báo");
                    location.reload();
                }
                else {
                    toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại.", "Lỗi");
                }
            }
        })
    }
}
$(function () {
    var table;
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../GroupFood/Index";
        }
    });
    if ($('#datatable-item').length) {
        //table = loadDatatable(table);
        table = $('#datatable-item').DataTable({
                "paging": false,
                "searching": true,
                "ordering": false,
                "info": true,
                "scrollY": "600px",
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Vietnamese.json"
                }

            });
        $('.tree').treegrid();
        $('.treegrid-expander-collapsed').trigger("click");
        //$('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
        //    if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
        //        var data = table.row($(this).closest('tr')).data();
        //        $.ajax({
        //            url: '/GroupFood/delete',
        //            type: 'post',
        //            datatype: 'json',
        //            data: {
        //                id: data.ProductGroupID
        //            },
        //            async: false,
        //            cache: false,
        //            success: function (data) {
        //                if (data == "1") {
        //                    toastr["success"]("Xóa phần tử thành công.", "Thông báo");
        //                    table.row($(this).closest('tr')).remove().draw();;
        //                }
        //                else {
        //                    toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại.", "Lỗi");
        //                }
        //            }
        //        })
        //    }
        //});
        //$('#datatable-item tbody').on('click', "tr td button[name='edit']", function () {
        //    var data = table.row($(this).closest('tr')).data();
        //    window.location.href = "../GroupFood/Edit?id=" + data.ProductGroupID;
        //});
        //$('#datatable-item tbody').on('click', "tr td button[name='create']", function () {
        //    var data = table.row($(this).closest('tr')).data();
        //    window.location.href = "../GroupFood/Create?p=" + data.ParentID;
        //});
    }
    else
    {
        $('#parentName').select2({ dropdownAutoWidth: true });
    }
    
});