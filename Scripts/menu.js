$(function () {
    $.ajax({
        url: "/Menu/GetMenuForWeb",
        type: 'POST',
        datatype: 'json',
        async: false,
        cache: false,
        success: function (data) {
            fillDataMenu(data);
        }
    })

    //method fill data menu when success data 
    function fillDataMenu(dataMenu)
    {
        $.ajax({
            type: "GET" ,
            url: "/XML/OrderModuleOfMenu.xml",
            dataType: "xml",
            success: function (xml) {
                var objectArray = [];
                var htmlMenuArray = [];

                //chia data menu thành những mảng nhỏ được sắp xếp theo thứ tự của menu
                for (var i = 1 ; i <= $(xml).find("OrderModuleOfMenu").children().length ; i++)
                {
                    var arrayChild = [];
                    var key = $(xml).find('order' + i).text();
                    $.each(dataMenu, function (index, item) {
                        if(key == item.ModuleCode)
                        {
                            arrayChild.push(item);
                        }
                    });
                    objectArray.push(arrayChild);
                }
                    
                //mỗi mảng nhỏ tương ứng là một html tương ứng cho menu
                $.each(objectArray, function (index, arrayChild) {
                    if (arrayChild.length > 0) {
                        var html = '<li>';
                        var htmlArrayChild = [];
                        var itemParent;
                        $.each(arrayChild, function (index1, item) {
                            if (item.SortOrder == 0) {
                                itemParent = item;
                            }
                            else {
                                htmlArrayChild[item.SortOrder -1 ] = "<a href='" + item.Path + "'><i class='" + item.IconURL + "'></i>" + item.Description + "</a>";
                            }
                        });

                        if (htmlArrayChild.length > 0) {
                            html += "<a><i class='" + itemParent.IconURL + "'></i> " + itemParent.Description + " <span class='fa fa-chevron-down'></span></a>";
                            html += "<ul class='nav child_menu'>";
                            $.each(htmlArrayChild, function (index1, item) {
                                html += '<li>';
                                html += item;
                                html += '</li>';
                            });
                            html += "</ul>";
                        } else {
                            html += "<a href='" + itemParent.Path + "'><i class='" + itemParent.IconURL + "'></i> " + itemParent.Description + " </a>";
                        }

                        html += '</li>';

                        //xuất ra giao diện
                        $("#menu_slider").append(html);
                    }
                });
                init_sidebar();
           }
        });
    }
})