/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="../util/beautify.js" />
/// <reference path="../ui/jquery.syntaxhighlighter.js" />

$(document).ready(function () {
    $('#nav_workspace').addClass('active');

    $('#forkButton').tooltip({
        placement: 'bottom',
        delay: {
            show: 250,
            hide: 250
        },
        title: 'Download item into workspace'
    })
    .click(function () {
        window.location = '/Data/Fork/' + +$('#N').text();
        bootbox.dialog("<div style='text-align:center;'><img src='/Content/glyphs/spinner.gif' width='24px' /><h4>Forking</h4></div>");
    });

    $('#reportButton').tooltip({
        placement: 'bottom',
        delay: {
            show: 250,
            hide: 250
        },
        title: 'Report Item for moderation'
    })
    .click(function () {
        bootbox.confirm('Are you sure you want to report this data?', function (result) {
            if (result) {
                window.location = '/Account/Report/?dataId=' + +$('#N').text();
                bootbox.dialog("<div style='text-align:center;'><img src='/Content/glyphs/spinner.gif' width='24px' /><h4>Reporting</h4></div>");
            }
        });
    });

    if ($('#editButton')) {
        $('#editButton').tooltip({
            placement: 'bottom',
            delay: {
                show: 250,
                hide: 250
            },
            title: 'Edit this item'
        })
        .click(function () {
            window.location = '/Data/Edit/' + $('#N').text();
        });
    }

    if ($('#deleteButton')) {
        $('#deleteButton').tooltip({
            placement: 'bottom',
            delay: {
                show: 250,
                hide: 250
            },
            title: 'Delete this item'
        })
        .click(function () {
            bootbox.confirm('Are you sure you want to delete this data?', function (result) {
                if (result) {
                    window.location = '/Data/Delete/' + $('#N').text();
                    bootbox.dialog("<div style='text-align:center;'><img src='/Content/glyphs/spinner.gif' width='24px' /><h4>Deleting</h4></div>");
                }
            });
        });
    }

    // Prettify the Raw JSON
    $.SyntaxHighlighter.init({
        wrapLines: false
    });
    $('#jsContainer').text(js_beautify($('#jsContainer').text(), {
        indent_size: 2
    })).syntaxHighlight();
});