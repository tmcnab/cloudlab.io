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
        title: 'Fork into Workspace'
    })
    .click(function () {
        window.location = '/' + $('#type').text() + '/Fork/' + $('#N').text();
        bootbox.dialog("<div style='text-align:center;'><img src='/Content/i/spinner.gif' width='24px' /><h4>Forking</h4></div>");
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
                if ($('#type').text() === 'data') {
                    window.location = '/Account/Report/?dataId=' + $('#N').text();
                }
                else {
                    window.location = '/Account/Report/?toolId=' + $('#N').text();
                }
                bootbox.dialog("<div style='text-align:center;'><img src='/Content/i/spinner.gif' width='24px' /><h4>Reporting</h4></div>");
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
            window.location = '/' + $('#type').text() + '/Edit/' + $('#N').text();
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
                    window.location = '/' + $('#type').text() + '/Delete/' + $('#N').text();
                    bootbox.dialog("<div style='text-align:center;'><img src='/Content/i/spinner.gif' width='24px' /><h4>Deleting</h4></div>");
                }
            });
        });
    }

    $('#toggleButton').click(function () {
        if ($('#jsContainer').hasClass('hidden')) {
            $('#jsContainer').removeClass('hidden');
            $('#details-pane').addClass('hidden');
            $('#toggleButton').text('View Description');
        }
        else {
            $('#jsContainer').addClass('hidden');
            $('#details-pane').removeClass('hidden');
            $('#toggleButton').text('View Source');
        }
    });

    // Prettify the Raw JSON
    $.SyntaxHighlighter.init({
        wrapLines: false
    });
    $('#jsContainer').text(js_beautify($('#jsContainer').text(), {
        indent_size: 2
    })).syntaxHighlight();

    $('table').addClass('table table-bordered table-condensed');
});