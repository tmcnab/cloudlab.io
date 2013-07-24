/// <reference path="../jquery-1.7.1-vsdoc.js" />
$(document).ready(function () {
    $('#nav_workspace').addClass("active");
    $('#queryBox').tooltip({
        placement: 'bottom',
        trigger: 'focus',
        title: 'Words like this search in titles. [Words] like this search within the tags associated with tools and data.',
        delay: {
            show: 250,
            hide: 250
        }
    });
    $("#searchForm").submit(function (e) {
        e.preventDefault();
        $('#queryBox').blur().addClass('disabled');
        $.ajax({
            url: '/Workspace/_Search/',
            dataType: 'json',
            data: {
                q: $('#queryBox').val()
            },
            success: function (model) {
                $('#queryBox').removeClass('disabled');
                $.each(model, function (i, item) {
                    if (item.dataid === 0) {
                        var _html = "<tr data-target='" + item.toolid + "'>";
                        _html += "<td><strong>" + item.title + "</strong>";
                        $.each(item.tags, function (j, tag) {
                            _html += " <span class='label'>" + tag + "</span>";
                        });
                        _html += "</td></tr>"
                        $('#toolResults').append(_html);
                    }
                    else {
                        var _html = "<tr data-target='" + item.dataid + "'>";
                        _html += "<td><strong>" + item.title + "</strong>";
                        $.each(item.tags, function (j, tag) {
                            _html += " <span class='label'>" + tag + "</span>";
                        });
                        _html += "</td></tr>"
                        $('#dataResults').append(_html);
                    }
                });
            }
        });
        return false;
    });
    $("#dataResults > tbody > tr").live('click', function () {
        window.location = "/Data/Details/" + $(this).attr('data-target');
    });
    $("#toolResults > tbody > tr").live('click', function () {
        window.location = "/Tools/Details/" + $(this).attr('data-target');
    });
    $("#dataTable > tbody > tr").click(function () {
        window.location = "/Data/Details/" + $(this).children('td:first').text();
    });
    $("#toolTable > tbody > tr").click(function () {
        window.location = "/Tools/Details/" + $(this).children('td:first').text();
    });
    $("#dataTable").tablesorter().tablesorterPager({ container: $("#dataPager") });
    $("#toolTable").tablesorter().tablesorterPager({ container: $("#toolPager") });
});