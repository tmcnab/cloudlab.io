/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="../cm/codemirror-custom.js" />
/// <reference path="../product/jquery.codemirror.intellihint.js" />

var IDE = {
    // General Funcs
    init: function () {
        this.ui_init();
        this.cmd_init();
    },
    updateData: function () {
        $.ajax({
            url: '/IDE/_ls/',
            data: {
                opts: '-d'
            },
            success: function (model) {
                $("#dataTable > tbody").empty();
                $.each(model, function (i, item) {
                    $("#dataTable > tbody").append("<tr><td>" + item.Id + "</td><td>" + item.Title + "</td></tr>");
                });
                if (!($('#dataTable').get(0).config)) {
                    $("#dataTable").tablesorter();
                }
                else {
                    $("#dataTable").trigger('update');
                }
            }
        });
    },
    updateTools: function () {
        $.ajax({
            url: '/IDE/_ls/',
            data: {
                opts: '-t'
            },
            success: function (model) {
                $("#toolTable > tbody").empty();
                $.each(model, function (i, item) {
                    $("#toolTable > tbody").append("<tr><td>" + item.Id + "</td><td>" + item.Title + "</td></tr>");
                });
                if (!($('#toolTable').get(0).config)) {
                    $("#toolTable").tablesorter();
                }
                else {
                    $("#toolTable").trigger('update');
                }
            }
        });
    },
    updateLocals: function () {
        $.ajax({
            url: '/IDE/_locals/',
            success: function (model) {
                $("#localsTable > tbody").empty();
                for (var item in model) {
                    $("#localsTable > tbody").append("<tr><td title='" + model[item] + "'>" + item + "</td></tr>");
                }

                if (!($('#localsTable').get(0).config)) {
                    $("#localsTable").tablesorter();
                }
                else {
                    $("#localsTable").trigger('update');//.trigger('sorton', localsTable.get(0).config.sortList);
                }
            }
        });
    },

    // Terminal Jazz
    cmd_opts: {
        history: true,
        prompt: '>>',
        greetings: '## cloudlab.io interactive terminal (alpha)',
        exit: false,
        clear: false
    },
    cmd_init: function () {
        this.terminal = $('#terminalContainer').terminal(this.cmd_onReturn, this.cmd_opts);
        $('textarea.clipboard').attr('spellcheck', 'false');
    },
    cmd_onReturn: function (command, term) {
        // Ensure commands is nonnull/empty
        if (command === null || command.length === 0) {
            return;
        }

        command = $.trim(command);
        if (command.charAt(0) === '@') {
            var cmdtoks = command.split(' ');

            switch (cmdtoks[0]) {
                case "@doc": IDE.cmd_doc(cmdtoks); break;
                case "@ls": IDE.cmd_list(cmdtoks); break;
                case "@reset": IDE.cmd_reset(); break;
                case "@rm": IDE.cmd_rm(cmdtoks); break;
                case "@clear": IDE.terminal.clear(); break;
                default: term.error('Terminal command not supported'); break;
            }
        }
        else {
            IDE.cmd_default(command);
        }
    },
    cmd_doc: function (params) {
        if (params.length == 1) {
            $('a[data-target="#documentation-tab"]').click();
        }
        else if (params.length == 2) {
            $.ajax({
                url: '/IDE/_doc/',
                data: {
                    name: params[1]
                },
                success: function (model) {
                    if (model == '') {
                        IDE.terminal.error("No documentation found for '" + params[1] + "'");
                    }
                    else {
                        $('a[data-target="#documentation-tab"]').click();
                        var mdd = new MarkdownDeep.Markdown();
                        mdd.ExtraMode = true;
                        mdd.MarkdownInHtml = true;
                        $('#doc-title').text(model.title);
                        $('#doc-body').html(mdd.Transform(model.body));
                        $('#doc-body > table').addClass('table');
                    }
                },
                error: function () {
                    this.terminal.error("Documentation for '" + params[1] + "' does not exist.");
                }
            });
        }
        else if (params.length == 3) {
            $.ajax({
                url: '/IDE/_doc/',
                data: {
                    name: params[2],
                    opt: params[1]
                },
                success: function (model) {
                    $('a[data-target="#documentation-tab"]').click();
                    var mdd = new MarkdownDeep.Markdown();
                    mdd.ExtraMode = true;
                    mdd.SafeMode = true;
                    $('#doc-title').text(model.title);
                    $('#doc-body').html(mdd.Transform(model.body));
                },
                error: function () {
                    this.terminal.error("Documentation for '" + params[1] + "' does not exist.");
                }
            });
        }
        else {
            this.terminal.echo("Usage:\tdoc [command-name]");
        }
    },
    cmd_list: function (params) {
        if (params.length != 2) {
            IDE.terminal.echo("Usage:\tls <-d|-t>");
        }
        else {
            IDE.terminal.pause();
            $.ajax({
                url: '/IDE/_ls/',
                data: {
                    opts: params[1]
                },
                success: function (model) {
                    $.each(model, function (i, item) {
                        IDE.terminal.echo(item.Id + "\t" + item.Title);
                    });
                },
                error: function () {
                    IDE.terminal.error("Incorrect usage of ls");
                },
                complete: function () {
                    IDE.terminal.resume();
                }
            });
        }
    },
    cmd_rm: function (params) {
        if (params.length === 3) {
            IDE.terminal.pause();
            $.getJSON('/IDE/_rm/', {
                opt: params[1],
                id: params[2]
            },
            function (response) {
                if (response) {
                    IDE.terminal.echo('cloudlab>> ok');
                }
                else {
                    IDE.terminal.error('cloudlab>> there was an error deleting the item you selected.');
                }
                IDE.terminal.resume();
            });
        }
        else {
        }
    },
    cmd_reset: function () {
        this.terminal.pause();
        $.ajax({
            url: '/IDE/_reset/',
            success: function () {
                IDE.terminal.echo("Terminal session has been reset.");
            },
            error: function () {
                IDE.terminal.error("Error resetting session. Please try again.");
            },
            complete: function () {
                IDE.terminal.resume();
            }
        });
    },
    cmd_default: function (params) {
        this.terminal.pause();
        $.ajax({
            url: '/IDE/_vm/',
            timeout: 0,
            data: {
                cmd: params
            },
            success: function (data) {
                if (data.Error) {
                    IDE.terminal.error(data.Error);
                }
                else if (data.JSAction) {
                    console.log(data.JSAction);
                    eval(data.JSAction);
                }
                else {
                    if (params.charAt(params.length - 1) != ';' || params.charAt(params.length - 1) != '}') {
                        IDE.terminal.echo(data.Text);
                    }
                }
            },
            error: function () {
                IDE.terminal.error('Unable to contact server.');
            },
            complete: function () {
                IDE.terminal.resume();
            }
        });
    },

    // Graphing Skullduggery
    graph_show: function () {

    },
    graph_refresh: function () {
        if (this.plot != null) {
            this.plot.replot();
        }
    },
    graph_options: function () {
        return {
            grid: {
                drawBorder: false,
                drawGridlines: false,
                background: '#ffffff',
                shadow: false
            }
        };
    },

    // UI Fuckery
    ui_init: function () {
        $('#nav_ide').addClass("active");

        $('a[data-toggle="tab"]').live('shown', IDE.ui_onTabChange);

        $(document).bind('keydown', 'ctrl+shift+1', function () {
            $('a[data-target="#terminal-tab"]').click();
        });
        $(document).bind('keydown', 'ctrl+shift+2', function () {
            $('a[data-target="#locals-tab"]').click();
        });
        $(document).bind('keydown', 'ctrl+shift+3', function () {
            $('a[data-target="#data-tab"]').click();
        });
        $(document).bind('keydown', 'ctrl+shift+4', function () {
            $('a[data-target="#tools-tab"]').click();
        });
        $(document).bind('keydown', 'ctrl+shift+5', function () {
            $('a[data-target="#scratchpad-tab"]').click();
        });
        $(document).bind('keydown', 'ctrl+shift+6', function () {
            $('a[data-target="#documentation-tab"]').click();
        });
        $(document).bind('keydown', 'ctrl+shift+7', function () {
            $('a[data-target="#graphing-tab"]').click();
        });

        $('#dataTable tbody > tr').live('click', function (e) {
            IDE.ui_onRowSelect('data', $(this).children(":first").text());
        });
        $('#toolTable tbody > tr').live('click', function (e) {
            IDE.ui_onRowSelect('tool', $(this).children(":first").text());
        });
    },
    ui_onTabChange: function (e) {
        switch (e.relatedTarget.text) {
            case "Terminal":
                IDE.terminal.disable();
                break;
        }
        switch (e.target.text) {
            case "Terminal":
                IDE.terminal.enable();
                break;

            case "Scratchpad":
                ScratchPad.refreshAndFocus();
                break;

            case "Graphing":
                IDE.graph_refresh();
                break;

            case "Locals":
                IDE.updateLocals();
                break;

            case "Data":
                IDE.updateData();
                break;

            case "Tools":
                IDE.updateTools();
                break;
        }
    },
    ui_onRowSelect: function (type, id) {
        switch (type) {
            case 'data':
                $.ajax({
                    url: '/Data/_Details/',
                    data: {
                        id: id
                    },
                    success: function (model) {
                        var mdd = new MarkdownDeep.Markdown();
                        mdd.ExtraMode = true;
                        mdd.SafeMode = true;
                        $('#dataMeta').html(mdd.Transform(model.Description));
                    },
                    error: function () {
                        // do an error here
                    }
                });
                break;

            case 'tool':
                $.ajax({
                    url: '/Tools/_Details/',
                    data: {
                        id: id
                    },
                    success: function (model) {
                        if (model != null) {
                            var mdd = new MarkdownDeep.Markdown();
                            mdd.ExtraMode = true;
                            mdd.SafeMode = true;
                            $('#toolMeta').html(mdd.Transform(model.body));
                        }
                        else {
                            bootbox.alert("There was an error getting the tool information.");
                        }
                    },
                    error: function () {
                        // do an error here
                    }
                });
                break;
        }
    },

    // Vars
    terminal: null,
    editor: null,
    plot: null,
    mainTabIndex: 0,
    mainTabNameArray: []
}

var ScratchPad = {

    init: function () {
        if (ScratchPad.Editor === null) {
            ScratchPad.foldingFunction = CodeMirror.newFoldFunction(CodeMirror.braceRangeFinder);
            ScratchPad.Editor = CodeMirror.fromTextArea($('#scratchpad-textarea').get(0), {
                lineNumbers: true,
                matchBrackets: true,
                mode: "text/javascript",
                onChange: ScratchPad.evt_OnChange,
                onGutterClick: ScratchPad.foldingFunction,
                extraKeys: {
                    "Ctrl-M": function (cm) {
                        ScratchPad.foldingFunction(cm, cm.getCursor().line);
                    },
                    "Ctrl-Space": function(cm) {
                        CodeMirror.simpleHint(cm, CodeMirror.javascriptHint);
                    },
                    "Ctrl-Enter": function(cm) {
                        $('#sp-execute-button').click();
                    }
                }
            });
            this.uiInit();
            ScratchPad.updateValidation();
        }
    },

    evt_OnChange: function (cm, evt) {
        var coords = ScratchPad.Editor.getCursor();
        $('#sp-rowcol-field').text((parseInt(coords.line) + 1) + ':' + coords.ch);
        ScratchPad.updateValidation();
    },

    refresh: function() {
        this.Editor.refresh();
    },

    refreshAndFocus :function() {
        this.Editor.refresh();
        this.Editor.focus();
    },

    uiInit : function() {
        // Prevent an intellihint backspace from navigating back
        $(document).keydown(function (e) { 
            if (e.keyCode == 8) e.preventDefault(); 
        });

        $('#sp-execute-button').click(function() {
            $('a[data-target="#terminal-tab"]').click();
            IDE.terminal.insert(ScratchPad.Editor.getValue());
            var evt = jQuery.Event('keydown');
            evt.keyCode = 13;
            $('.clipboard').trigger(evt);
        });
    },

    updateValidation : function() {
        var result = JSHINT(ScratchPad.Editor.getValue(), {
            curly: true,
            immed: true,
            strict: true,
            trailing: true,
            es5: true
        });
        if (!result) {
            var error = JSHINT.errors[0];
            $('#sp-validation-field').html('<strong>Error [' + error.line + ':' + error.character + ']</strong>: ' + error.reason);
        } 
        else {
            $('#sp-validation-field').html('No errors.');
        }
    },

    foldingFunction: null,
    Editor: null,
};

$(document).ready(function () {
    IDE.init();
    ScratchPad.init();
});