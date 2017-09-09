var prefixId = 'table_product_material_';
$(function () {
    $.cookie('beforePageIsViewProductMaterial', new Boolean(true));
    //lấy dữ liệu datacombobox
    var dataSelect = [];
    var arrayTable = [];
    $("#cancelbutton").click(function () {
        if (confirm("Bạn có chắc hủy. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            window.location.href = "../Party/MaterialParty";
        }
    });
    $("#updateButton").click(function () {
        if (confirm("Bạn có đồng ý lấy lại dữ liệu. Mọi thao tác của bạn sẽ không được lưu lại.")) {
            $("form div.x_content").each(function (index) {
                $this = $(this);
                var productId = $this.find("div.form-group")[0].id.replace(prefixId, "");
                addItemForDatatable(productId, true, arrayTable[index], index, dataSelect, '/Party/MethodFillDataWhenClickGetData');
            });
        }
    });
    $("#printButton").click(function () {
        window.location.href = "/PrintDetailMaterial/GetViewReportQuantitative?id=" + $.urlParam("id");
    });
    $("#saveButton").click(function () {
        var lsPartyProductMaterialInsert = [];
        var lsPartyProductMaterialUpdate = [];
        $("form div.x_content").each(function (index) {
            $parent = $(this);
            var productId = $parent.find("div.form-group")[0].id.replace(prefixId, "");
            var table = arrayTable[index];
            $parent.find("table tbody tr").each(function (indexDatable, tr) {
                $row = $(this);
                var object = new Object({
                    ID: table.rows(indexDatable).data()[0][7],
                    PartyID: $("#partyID").val(),
                    ProductID: productId,
                    MaterialID: table.rows(indexDatable).data()[0][6],
                    Quantity: table.rows(indexDatable).data()[0][1].replace(/,/gi, ''),
                    UnitPrice: $row.find("input[name='unitPrice']")[0].value.replace(/,/gi, ''),
                    VendorID: $row.find("select[name='vendorID']")[0].value,
                    IsDelivery: $row.find("input[name='isDelivery']").is(':checked'),
                });
                if (table.rows(indexDatable).data()[0][7] > 0)
                    lsPartyProductMaterialUpdate.push(object);
                else
                    lsPartyProductMaterialInsert.push(object);
            });
        });
        $.ajax({
            url: "/Party/AddOrUpdatePartyProductMaterial",
            type: 'post',
            datatype: 'json',
            contentType: 'application/json',
            data: JSON.stringify({
                "lsPartyProductMaterialInsert": lsPartyProductMaterialInsert,
                "lsPartyProductMaterialUpdate": lsPartyProductMaterialUpdate
            }),
            async: false,
            cache: false,
            success: function (data) {
                if (data == "0") {
                    toastr["error"]("Xử lý dữ liệu thất bại. Xin vui lòng thử lại", "Thông báo")
                    flag = false;
                }
                else {
                    window.location.href = "/Party/MaterialParty";
                }
            }
        })
    });
    $.ajax({
        url: '/Party/GetIdAndCountProductInParty',
        type: 'post',
        datatype: 'json',
        contentType: 'application/json',
        data: JSON.stringify({
            "partyId": $('#partyID').val()
        }),
        async: false,
        cache: false,
        success: function (arrayDatatable) {
            if (arrayDatatable != "null") {
                $.ajax({
                    url: '/Vendor/GetEntityForCombobox',
                    type: 'post',
                    datatype: 'json',
                    async: false,
                    cache: false,
                    success: function (data) {
                        if (data != "null") {
                            dataSelect = data;
                        }
                    }
                })
                $.each(arrayDatatable, function (index_dataTable, dataTable) {
                    var table;
                    var htmlDatatable = $('#newDatatableProduct table').prop('outerHTML');
                    var htmlLabelProduct = "<div class='x_title'><h4><strong>Tên món ăn: " + dataTable.ProductName + "</strong></h4><ul class='nav navbar-right panel_toolbox'><li style='float: right;'><a class='collapse-link'><i class='fa fa-chevron-up'></i></a></ul><div class='clearfix'></div></div>";
                    htmlDatatable = "<div class='x_content' style='display: block;'><div class='form-group' id='" + prefixId + dataTable.ProductID + "' >" + htmlDatatable + "</div></div>";
                    $("form").append("<div class='x_panel'>" + htmlLabelProduct + htmlDatatable + "</div>");
                    addItemForDatatable(dataTable.ProductID, false, table, 0, dataSelect, '/Party/MethodFillDataForDatatablesProductMaterial');
                    table = $('#' + prefixId + dataTable.ProductID + ' table').DataTable({
                        "processing": false,
                        "serverSide": false,
                        "paging": false,
                        "ordering": false,
                        "info": false,
                        "searching": false,
                        "columnDefs": [
                            { "width": "15%", "targets": 0 },
                            { "width": "10%", "targets": 1, "class": "dt-body-right" },
                            { "width": "10%", "targets": 2 },
                            { "width": "45%", "targets": 3 },
                            { "width": "15%", "targets": 4 },
                            { "width": "5%", "targets": 5, "class": "dt-body-center" },
                            { "visible": false, "targets": 6, },
                            { "visible": false, "targets": 7, }
                        ],
                        "drawCallback": function (settings) {
                            init_controlInputMarskNumber("input[name='unitPrice']");
                            $("select[name='vendorID']").select2({ dropdownAutoWidth: true });
                            $("input[name='isDelivery']").iCheck({
                                checkboxClass: 'icheckbox_flat-green',
                                radioClass: 'iradio_flat-green'
                            });
                        }
                    });
                    arrayTable.push(table);
                });

            }
        }
    })
});
function addItemForDatatable(productId, tableIsExist, currentTable, indexTable, dataSelect, urlAjax) {
    if (tableIsExist)
        currentTable.clear().draw();
    $.ajax({
        url: urlAjax,
        type: 'post',
        datatype: 'json',
        contentType: 'application/json',
        data: JSON.stringify({
            "productId": productId,
            "partyID": $("#partyID").val()
        }),
        async: false,
        cache: false,
        success: function (itemsTable) {
            if (itemsTable != "null") {
                $.each(itemsTable, function (index_itemTable, itemTable) {
                    var htmlMaterial = "<th>" + itemTable.MaterialName + "</th>";
                    var htmlMaterialId = "<th name='materialID'>" + itemTable.MaterialID + "</th>";
                    var htmlId = "<th name='iD'>" + itemTable.IdPartyProductMaterial + "</th>";
                    var htmlQuantity = "<th name='quantity'>" + $.number(itemTable.Quantity, 4, '.', ',') + "</th>";
                    var htmlUOMName = "<th>" + itemTable.UOMName + "</th>";
                    var htmlUnitPrice = "<th>" + getHtmlControlInput(itemTable.UnitPrice, "unitPrice") + "</th>";
                    var htmlVendorID = "<th>" + getHtmlControlSelect(itemTable.VendorID, "vendorID", dataSelect) + "</th>";
                    var htmlIsDelivery = "<th>" + getHtmlControlCheckBox(itemTable.IsDelivery, "isDelivery") + "</th>";
                    var htmlRow = "<tr>" + htmlMaterial + htmlQuantity + htmlUOMName + htmlUnitPrice + htmlVendorID + htmlIsDelivery + htmlMaterialId + htmlId + "</tr>";
                    if (tableIsExist) {
                        currentTable.row.add($(htmlRow));
                    }
                    else {
                        $("#" + prefixId + productId).find("table tbody").append(htmlRow);
                    }
                });
                if (tableIsExist)
                    currentTable.draw();
            }
        }
    });
}
function getHtmlControlInput(value, nameElement) {
    return "<input style='width:100%;text-align:right' type='text' name='" + nameElement + "' value='" + value + "' class='form-control'/>"
}
function getHtmlControlCheckBox(value, nameElement) {
    return "<input type='checkbox' name='" + nameElement + "' " + (value == true ? "checked" : "") + " />";
}
function getHtmlControlSelect(value, nameElement, dataSelect) {
    var html = "<select name='" + nameElement + "' style='width:100%' >";
    html += "<option value='0' " + ((value == 0) ? "selected" : "") + ">Chọn nhà cung cấp</option>";
    $.each(dataSelect, function (index, valueSelect) {
        html += "<option value='" + valueSelect.VendorID + "'" + ((value == valueSelect.VendorID) ? "selected" : "") + ">" + valueSelect.VendorName + "</option>";
    });
    html += "</select>";
    return html;
}