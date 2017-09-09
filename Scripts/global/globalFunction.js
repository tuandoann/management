//file store funciton global for web application

/* function for init control  */

function init_controlSelect2(elementSelector, urlAttactControler, placeholderText) {                                          //select 2
    if (typeof (select2) === 'undefined') {
        loadCSS("/Extension/select2/select2.css");
        $.getScript("Extension/select2/select2.min.js").done(function (script, textStatus) {
            callAjaxForControlSelect2(elementSelector, urlAttactControler, placeholderText);
        })
    }
    else {
        callAjaxForControlSelect2(elementSelector, urlAttactControler, placeholderText);
    }
}
function init_controlInputMarskNumber(elementSelector) {                                                                     //inputMask input number
    if (typeof ($.fn.inputmask) == 'undefined') {
        $.getMultiScripts(fileInputMask).done(function () {
            $(elementSelector).inputmask("999,999,999,999", { alias: "number", numericInput: true, integerOptional: true, showMaskOnFocus: true, jitMasking: true, removeMaskOnSubmit: true, rightAlign: true });
        }).fail(function (error) {
            console.log("error" + error);
        }).always(function () {
            $(window).load(function () {
                $(elementSelector).inputmask("999,999,999,999", { alias: "number", numericInput: true, integerOptional: true, showMaskOnFocus: true, jitMasking: true, removeMaskOnSubmit: true, rightAlign: true });
            });
        });
    } else {
        debugger;
        var dk =   $(elementSelector).val();
        $(elementSelector).inputmask("9,999,999,999", { alias: "number", numericInput: true, integerOptional: true, showMaskOnFocus: true, jitMasking: true, removeMaskOnSubmit: true, rightAlign: true });
        if ($(elementSelector).length == 1)
            $(elementSelector).val(dk == 0 ? 0 : $(elementSelector).val());
        $(elementSelector).focus(function () {
            this.select();
        })
    }
}


/* function different  */

loadCSS = function (href) {                                                                                          //method load css base url stylesheet

    var cssLink = $("<link>");
    $("head").append(cssLink);
    cssLink.attr({
        rel: "stylesheet",
        type: "text/css",
        href: href
    });

};

$.getMultiScripts = function (arr) {                                                                                //method get file js base array url
    var _arr = $.map(arr, function (scr) {
        return $.getScript(scr);
    });

    _arr.push($.Deferred(function (deferred) {
        $(deferred.resolve);
    }));

    return $.when.apply($, _arr);
}

function callAjaxForControlSelect2(elementSelector, urlAttactControler, placeholderText) {                                //method call ajax control select 2
    $(elementSelector).select2(
        {
            placeholder: placeholderText,
            //Does the user have to enter any data before sending the ajax request
            minimumInputLength: 0,
            allowClear: true,
            ajax: {
                //How long the user has to pause their typing before sending the next request
                quietMillis: 150,
                //The url of the json service
                url: urlAttactControler,
                dataType: 'jsonp',
                //Our search term and what page we are on
                data: function (term, page) {
                    return {
                        pageSize: pageSize,
                        pageNum: page,
                        searchTerm: term
                    };
                },
                results: function (data, page) {
                    //Used to determine whether or not there are more results available,
                    //and if requests for more data should be sent in the infinite scrolling                    
                    var more = (page * pageSize) < data.Total;
                    return { results: data.Results, more: more };
                }
            }
        });
}

function calculateSumItemDetailTable(idTable, nameArrayElement,actionAmongItem)                       // calculate sum value row ( value row is action among many item row)
{
    var sum = 0;
    $("#" + idTable + " tbody tr").each(function () {
        $this = $(this);
        var valueOneRow = 1;
        $.each(nameArrayElement, function (index, value1) {
            switch(actionAmongItem)
            {
                case 1:
                    valueOneRow *= $this.find(value1)[0].value.replace(/,/gi, '');
                    break;
                case 2:
                    valueOneRow += $this.find(value1)[0].value;
            }
            
        });
        sum += valueOneRow;
    });
    return sum;
}

function addNewRowDetail(table, idNewRow) {                                                             //add new row for datatable.net plugin jquery
    var rowHtml = $("#" + idNewRow + " tbody").find("tr")[0].outerHTML;
    table.row.add($(rowHtml)).draw();
}

Date.prototype.addHours = function (h) {                                                               //add hours for datetime
    this.setHours(this.getHours() + h);
    return this;
}

JSON.dateParser = function (value) {                                                                    //convert object (datetime from controler) to object datetime jquery
    if (typeof value === 'string') {
        var a = reISO.exec(value);
        if (a)
            return new Date(value);
        a = reMsAjax.exec(value);
        if (a) {
            var b = a[1].split(/[-+,.]/);
            return new Date(b[0] ? +b[0] : 0 - +b[1]);
        }
    }
    return value;
};
String.prototype.isDate = function isDate()
{
    var arrayDate = this.split('/');
    if(arrayDate.length == 3)
    {
        if(arrayDate[0]<0 && arrayDate >31)
        {
            return false;
        }
        if(arrayDate[1]<0 && arrayDate > 12)
        {
            return false;
        }
        if(arrayDate[2]< 1111)
        {
            return false;
        }
        return true;
    }
    return false;
}
jQuery.fn.putCursorAtEnd = function () {

    return this.each(function () {

        // Cache references
        var $el = $(this),
            el = this;

        // Only focus if input isn't already
        if (!$el.is(":focus")) {
            $el.focus();
        }

        // If this function exists... (IE 9+)
        if (el.setSelectionRange) {

            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
            var len = $el.val().length * 2;

            // Timeout seems to be required for Blink
            setTimeout(function () {
                el.setSelectionRange(len, len);
            }, 1);

        } else {

            // As a fallback, replace the contents with itself
            // Doesn't work in Chrome, but Chrome supports setSelectionRange
            $el.val($el.val());

        }

        // Scroll to the bottom, in case we're in a tall textarea
        // (Necessary for Firefox and Chrome)
        this.scrollTop = 999999;

    });

};


$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href );
    if (results == null) {
        return null;
    }
    else {
        return results[1] || 0;
    }
}

