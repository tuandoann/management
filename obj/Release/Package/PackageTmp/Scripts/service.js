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
            "url": "/Service/GetServiceDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "formatNumber": function ( toFormat ) {
            return toFormat.toString().replace(
              /\B(?=(\d{3})+(?!\d))/g, "'"
            )},
        "columns": [
            { "data": "ServiceName", "name": "ServiceName", "width": "3%0" },
            {
                "data": "UnitPrice",
                "name": "UnitPrice",
                "class": "dt-body-right",
                "width": "30%",
                "render": $.fn.dataTable.render.number(',', '', 0, '')
            },
            { "data": "Notes", "name": "Notes", "width": "30%" },
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
    $('.js-switch').click();
    if ($('#unitPrice').length)
    {
        init_controlInputMarskNumber("#unitPrice");
    }
    if ($('#datatable-item').length) {
        table = loadDatatable(table);
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Service/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.ServiceID
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
            window.location.href = "../Service/Edit?id=" + data.ServiceID;
        });
    }
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Service/Index";
        }
    });
});