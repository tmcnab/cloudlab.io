/// <reference path="../jquery-1.7.1-vsdoc.js" />
$(document).ready(function () {
    $('#nav_documentation').addClass("active");
    $('li.ml2').click(function () {
        var _id = $(this).attr('data-target');
        $("doc-body").html('<div style="width:100%; height:200px; text-align:center"><img src="../../Content/glyphs/spinner.gif" /></div>'); /// <reference path="../jqplot/" />

        $.ajax({
            url: '/Documentation/_Details/',
            data: {
                id: parseInt(_id)
            },
            success: function (model) {
                if (model.Object === "Guides" || model.Object === "Tutorials" || model.Object === "API") {
                    $("#doc-title").text(model.Title);
                }
                else {
                    if (model.Object !== model.Name) {
                        $("#doc-title").text(model.Object + "." + model.Name);
                    }
                    else {
                        $("#doc-title").text(model.Object + " Constructor");
                    }
                }

                var mdd = new MarkdownDeep.Markdown();
                mdd.ExtraMode = true;
                mdd.MarkdownInHtml = true;
                $("#doc-body").html(mdd.Transform(model.Body));
                $('table').addClass('table').addClass('table-bordered').addClass('table-condensed');
            }
        });
    });
});