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
            "url": "/Material/GetMaterialDatatableIndex",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
             {
                 "orderable": false, "data": null, "className": 'select-checkbox dt-head-center',
                 "render": function (data) {
                     return "";
                 }
             },
            { "data": "MaterialName", "name": "MaterialName", "width": "30%" },
            { "data": "UnitPrie", "name": "UnitPrie", "width": "10%", "class": "dt-body-right", "render": $.fn.dataTable.render.number(',', '', 0, '') },
            { "data": "UOMName", "name": "UOMName", "width": "20%" },
            { "data": "Notes", "name": "Notes", "width": "40%" },
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
        order: [[1, 'asc']],
        select: {
            style: 'multi',
            blurable: true,
            selector: 'td:first-child'
        },
    });
    return table;
}
$(function () {
    var table;
    $("#selectAll").iCheck({
        checkboxClass: 'icheckbox_flat-green',
        radioClass: 'iradio_flat-green'
    });
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Material/Index";
        }
    });
   
    if ($('#datatable-item').length) {
        $("#deleteMany").click(function () {
            if (confirm("Bạn có muốn xóa tất cả phần tử được chọn!")) {
                var itemArray = table.rows({
                    selected: true
                }).data();
                if (itemArray.length > 0) {
                    var lsIdItem = [];
                    $.each(itemArray, function (index, value) {
                        lsIdItem.push(value.MaterialID);
                    });
                    $.ajax({
                        url: '/Material/deleteMany',
                        type: 'post',
                        datatype: 'json',
                        contentType: 'application/json',
                        data: JSON.stringify({
                            "lsIdItem": lsIdItem
                        }),
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data == "1") {
                                toastr["success"]("Xóa phần tử được chọn thành công.", "Thông báo");
                                table.ajax.reload();
                            }
                            else {
                                toastr["error"]("Xóa phần tử thất bại. Xin vui lòng thử lại. Lỗi " + data, "Lỗi");
                            }
                        }
                    })
                }
                else {
                    toastr["success"]("Vui lòng chọn ít nhất 1 phần tử.", "Thông báo");
                }
            }
        });

        $('#selectAll').on('ifChanged', function (event) {
            if (event.target.checked)
                table.rows().select();
            else
                table.rows().deselect();
        });


        table = loadDatatable(table);
        $('#datatable-item tbody').on('click', "tr td button[name='delete']", function () {
            if (confirm("Bạn có chắc muốn xóa dữ liệu này hay không!")) {
                var data = table.row($(this).closest('tr')).data();
                $.ajax({
                    url: '/Material/delete',
                    type: 'post',
                    datatype: 'json',
                    data: {
                        id: data.MaterialID
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
            window.location.href = "../Material/Edit?id=" + data.MaterialID;
        });
    }
    else {
        init_controlInputMarskNumber("#unitPrie");
        $('#uOMID').select2({ dropdownAutoWidth: true });
        $("#sumbitbutton").click(function () {
            if (validator.checkAll($("form"))) {
                $.ajax({
                    url: "/Material/AddOrUpdateEntity",
                    type: 'post',
                    datatype: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        "item": new Object({
                            "MaterialID": $('#materialID').length > 0 ? $('#materialID').val() : 0,
                            "MaterialName": $('#materialName').val(),
                            "Notes": $('#notes').val(),
                            "UOMID": $('#uOMID').val(),
                            "UnitPrie": $('#unitPrie').val().replace(/,/gi, ''),
                        }),
                        "isEdit": $('#materialID').length > 0 ? true : false
                    }),
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data == "0") {
                            toastr["error"]("Nguyên liệu lưu thất bại.", "Thông báo")
                            flag = false;
                        }
                        else {
                            toastr["success"]("Nguyên liệu lưu thành công.", "Thông báo")
                            window.location.href = "/Material/Index";
                        }
                    }
                })
            }
        });
    }

});