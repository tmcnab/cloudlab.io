/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="../jquery.terminal-0.4.11.js" />
/// <reference path="../util/SmartStorage.js" />


var Terminal = {

    //-- Functions
    Initialize: function () {
        Terminal.terminal = $('#terminalContainer').terminal(Terminal.Handlers.Input.Return, Terminal.options);
        $('#vmResetButton').on('click', Terminal.Handlers.Button.Reset);
        $('#vmClearButton').on('click', Terminal.Handlers.Button.Clear);
        Terminal.Settings.Initialize();
    },

    Execute: function (script) {
        $('a[data-target="#T1"]').click();
        Terminal.terminal.insert(script);
        var event = jQuery.Event('keydown');
        event.keyCode = 13;
        $('.clipboard').trigger(event);
    },
    Interpret: function (command) {
        Terminal.Pause();
        $.ajax({
            url: '/vm/',
            type: 'post',
            timeout: 0,
            data: command,
            success: function (response) {
                if (response.IsError) {
                    Terminal.terminal.error(response.Text);
                }
                else if (response.IsJS) {
                    window.eval(response.Text);
                }
                else {
                    if (response.Text === "undefined" && Terminal.store.get('repl-undefined') === false) {
                        return;
                    }
                    Terminal.terminal.echo(response.Text);
                }
            },
            error: function () {
                Terminal.terminal.error('Server error.');
            },
            complete: Terminal.Resume
        });
    },
    Pause: function () {
        Terminal.terminal.pause();
    },
    Resume: function () {
        Terminal.terminal.resume();
    },

    //-- Special/Testing
    WriteError: function (message) {
        Terminal.terminal.error('WriteError: ' + message);
    },

    Console: {
        ApplySettings: function (s, t) {
            if (Terminal.store.get('repl-console-timestamp') === true) {
                s = '<' + t + '|' + (new Date()).toLocaleTimeString() + '> ' + s;
            }
            else {
                s = '<' + t + '> ' + s;
            }
            return s;
        },
        Debug: function (s) {
            s = Terminal.Console.ApplySettings(s, 'DEBUG');
            Terminal.terminal.echo(s);
        },
        Error: function (s) {
            s = Terminal.Console.ApplySettings(s, 'ERROR');
            Terminal.terminal.echo(s);
        },
        Info: function (s) {
            s = Terminal.Console.ApplySettings(s, 'INFO-');
            Terminal.terminal.echo(s);
        },
        Log: function (s) {
            s = Terminal.Console.ApplySettings(s, 'LOG--');
            Terminal.terminal.echo(s);
        },
        Warn: function (s) {
            s = Terminal.Console.ApplySettings(s, 'WARN-');
            Terminal.terminal.echo(s);
        }
    },

    //-- Event Handlers
    Handlers: {
        Input: {
            Return: function (command, term) {
                //-- Guard
                command = $.trim(command);
                if (command === null || command.length === 0) {
                    return;
                }

                if (Terminal.prevcmd === '') {
                    if (Terminal.Handlers.Input.parensMatch(command)) {
                        Terminal.Interpret(command);
                        Terminal.terminal.set_prompt('>>');
                    }
                    else {
                        Terminal.prevcmd += command;
                        Terminal.terminal.set_prompt('  ');
                    }
                }
                else {
                    if (Terminal.Handlers.Input.parensMatch(Terminal.prevcmd + command)) {
                        command = Terminal.prevcmd + command;
                        Terminal.Interpret(command);
                        Terminal.prevcmd = '';
                        Terminal.terminal.set_prompt('>>');
                    }
                    else {
                        Terminal.prevcmd += command;
                        Terminal.terminal.set_prompt('  ');
                    }
                }
            },
            parensMatch: function (str) {
                var parens = (str.split('(').length - str.split(')').length) === 0;
                var bracks = (str.split('[').length - str.split(']').length) === 0;
                var braces = (str.split('{').length -
                str.split('}').length) === 0;
                return (parens && braces && bracks);
            }
        },
        Button: {
            Reset: function () {
                Terminal.Pause();
                $.post('/VM/Reset', null, function (response) {
                    if (response) {
                        Terminal.terminal.echo('Your session has been reset');
                    }
                    else {
                        Terminal.terminal.error('There was an error resetting your session');
                    }
                });
                Terminal.Resume();
            },
            Clear: function () {
                Terminal.terminal.clear();
            }
        }
    },

    //-- Settings & Persistence
    Settings: {
        Initialize: function () {
            Terminal.store = new SmartStorage("terminal-settings");
            $('a[data-term-setting]').click(Terminal.Settings.OnClick);

            $('a[data-term-setting]').each(function (i, item) {
                var setting = $(item).attr('data-term-setting');
                var val = Terminal.store.get(setting);
                val = (val === null) ? false : val;
                Terminal.Settings.SetTick($(item), val);
            });
        },
        OnClick: function (e) {
            var preference = $(this).attr('data-term-setting');
            var currentVal = Terminal.store.get(preference);
            var newVal = !currentVal;
            Editor.SetTick($(this), newVal);
            Terminal.store.set(preference, newVal);
        },

        SetTick: function (elem, val) {
            if (val) elem.children(":first").removeClass('invisible');
            else elem.children(":first").addClass('invisible');
        },

        PrependConsoleMessages: true,
        PrependWithTime: false
    },

    //-- Internal Vars
    store: null,
    prevcmd: '',
    terminal: null,
    options: {
        history: true,
        prompt: '>>',
        greetings: 'Welcome',
        exit: false,
        clear: false
    }
};