/// <reference path="jquery-1.7.1-vsdoc.js" />
/// <reference path="bootbox.js"/>
var Terminal = null;
$().ready(function () {

    $('form').focusin(function () {
        Terminal.pause();
    }).focusout(function () {
        Terminal.resume();
    });

    //$().popover({});


    Terminal = $('#terminal').terminal(function (command, term) {
        if (command === null || command.length === 0) return;
        else {
            command = $.trim(command);
            if (command === '@clear') {
                term.clear();
                return;
            }
            else if (command === '@reset') {
                term.pause();
                $.ajax({
                    url: '/Home/_reset/',
                    success: function () {
                        term.echo("Session has been reset.");
                    },
                    error: function () {
                        term.error('There was an error contacting the server.');
                    },
                    complete: function () {
                        term.resume();
                    }
                });
                return;
            }
            else if (command === '@help') {
                window.open('/Documentation/', '_blank');
                return;
            }

            term.pause();
            $.ajax({
                url: '/Home/_vm/',
                timeout: 0,
                data: {
                    cmd: command
                },
                success: function (data) {
                    if (data.Error) {
                        term.error(data.Error);
                    }
                    else if (data.JSAction) {
                        eval(data.JSAction);
                    }
                    else {
                        term.echo(data.Text);
                    }
                },
                error: function () {
                    term.error('There was an error contacting the server.');
                },
                complete: function () {
                    term.resume();
                }
            });
        }
    },
    {
        history: true,
        prompt: '>>',
        greetings: '>>>> cloudlab.io (demo mode)\n>>>> Try \'@help\' to view documentation',
        exit: false,
        clear: false
    });
});