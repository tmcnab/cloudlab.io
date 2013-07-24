/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="../ace/ace.js" />
/// <reference path="Index.js" />
/// <reference path="../util/SmartStorage.js" />

var Editor = {
    Initialize: function () {
        Editor.editor = ace.edit('editorContainer');
        Editor.editorStore = new SmartStorage("ace-general");
        Editor.prefStore = new SmartStorage("ace-preferences");

        var previousContent = Editor.editorStore.get('saved-content');
        if (previousContent === null) previousContent = '';
        Editor.editor.getSession().setValue(previousContent);
        Editor.editor.getSession().on('change', function () {
            Editor.editorStore.set('saved-content', Editor.editor.getSession().getValue());
        });

        $('a[data-editor-action="execute-content"]').click(function () {
            Editor.Run();
        });

        Editor.Preference.Init();
        $('a[data-ace-preference]').on('click', Editor.Preference.Clicked);

        Editor.Theme.Init();
        $('a[data-ace-theme]').on('click', Editor.Theme.Clicked);

        Editor.Mode.Init();
        $('a[data-ace-mode]').on('click', Editor.Mode.Clicked);
    },

    Refresh: function () {
        Editor.editor.resize();
        Editor.editor.focus();
    },

    Open: function (path) {
        $.ajax('/Dropbox/Download/?path=' + encodeURIComponent(path), {
            success: function (model) {
                Editor.SetModeFromPath(path);
                $('#T_Editor_Path').text(path);
                Editor.editor.getSession().setValue(model);
                $('a[data-target="#T2"]').click();
            }
        });
    },

    Save: function (path, content) {
        $.post('/Dropbox/Upload/?path=' + encodeURIComponent(path), content, function (result) {
            UI.Notify('Editor Save', result.toString(), false, '');
        });
    },

    Run: function () {
        //Terminal.Execute(Editor.editor.getSession().getValue());
        $.ajax({
            url: '/vm/',
            type: 'post',
            timeout: 0,
            data: Editor.editorStore.get('saved-content'),
            beforeSend: function () {
                UI.Notify('Executing', 'Executing your script', false, '');
            },
            success: function (response) {
                if (response.IsError) {
                    UI.Notify('Script Error', response.Text, true, '');
                    //Terminal.terminal.error(response.Text);
                }
                else if (response.IsJS) {
                    window.eval(response.Text);
                }
                else {
                    //Terminal.terminal.echo(response.Text);
                    $('a[data-target="#T1"]').click();
                }
            },
            error: function () {
                UI.Notify('Server Error', 'There was an error contacting the server.', true, '');
                //Terminal.terminal.error('Server error.');
            },
            complete: function () {

            }
        });
    },

    Theme: {
        Init: function () {
            var theme = Editor.editorStore.get('theme');
            if (theme === null) {
                Editor.editorStore.set('theme', 'ace/theme/textmate');
                theme = 'ace/theme/textmate';
            }

            Editor.SetTick($('a[data-ace-theme="' + theme + '"]'), true);
            Editor.editor.setTheme(theme);
        },
        Clicked: function (elem) {
            // Grab the theme from the data attribute, set it, store it
            var theme = $(this).attr('data-ace-theme');
            Editor.editor.setTheme(theme);
            Editor.editorStore.set('theme', theme);

            // Untick the old value, tick the new value, refresh the editor
            Editor.SetTick($(this).parent().parent().find('li > a[data-ace-theme] > i:not(.invisible)').parent(), false);
            Editor.SetTick($(this), true)
            Editor.Refresh();
        }
    },

    Mode: {
        Init: function () {
            var mode = Editor.editorStore.get('mode');
            if (mode === null) {
                Editor.editorStore.set('mode', 'ace/mode/javascript');
                mode = 'ace/theme/textmate';
            }

            Editor.SetTick($('a[data-ace-mode="' + mode + '"]'), true);
            Editor.editor.getSession().setMode(new (require(mode).Mode));
        },
        Clicked: function (elem) {
            // Grab the theme from the data attribute, set it, store it
            var mode = $(this).attr('data-ace-mode');
            Editor.editor.getSession().setMode(new (require(mode).Mode));
            Editor.editorStore.set('mode', mode);

            // Untick the old value, tick the new value, refresh the editor
            Editor.SetTick($(this).parent().parent().find('li > a[data-ace-mode] > i:not(.invisible)').parent(), false);
            Editor.SetTick($(this), true)
            Editor.Refresh();
        }
    },

    Preference: {
        Init: function () {
            $('a[data-ace-preference]').each(function (i, item) {
                var preference = $(this).attr('data-ace-preference');
                var currentVal = Editor.prefStore.get(preference);
                var newVal = currentVal;
                if (currentVal === null) {
                    newVal = true;
                    Editor.prefStore.set(preference, newVal);
                }

                switch (preference) {
                    case 'highlight-line':
                        Editor.editor.setHighlightActiveLine(newVal);
                        break;

                    case 'show-gutter':
                        Editor.editor.renderer.setShowGutter(newVal);
                        break;

                    case 'show-invisibles':
                        Editor.editor.setShowInvisibles(newVal);
                        break;

                    case 'show-print-margin':
                        Editor.editor.setShowPrintMargin(newVal);
                        break;

                    case 'use-word-wrap':
                        Editor.editor.getSession().setUseWrapMode(newVal);
                        break;

                    case 'use-soft-tabs':
                        Editor.editor.getSession().setUseSoftTabs(newVal);
                        break;
                }

                Editor.SetTick($(this), newVal);
            });
        },
        Clicked: function (elem) {
            var preference = $(this).attr('data-ace-preference');
            var currentVal = Editor.prefStore.get(preference);
            var newVal = !currentVal;

            switch (preference) {
                case 'highlight-line':
                    Editor.editor.setHighlightActiveLine(newVal);
                    break;

                case 'show-gutter':
                    Editor.editor.renderer.setShowGutter(newVal);
                    break;

                case 'show-invisibles':
                    Editor.editor.setShowInvisibles(newVal);
                    break;

                case 'show-print-margin':
                    Editor.editor.setShowPrintMargin(newVal);
                    break;

                case 'use-word-wrap':
                    Editor.editor.getSession().setUseWrapMode(newVal);
                    break;

                case 'use-soft-tabs':
                    Editor.editor.getSession().setUseSoftTabs(newVal);
                    break;
            }

            Editor.SetTick($(this), newVal);
            Editor.prefStore.set(preference, newVal);
        }
    },

    SetTick: function (elem, val) {
        if (val) elem.children(":first").removeClass('invisible');
        else elem.children(":first").addClass('invisible');
    },

    SetModeFromPath: function (path) {
        // This is a really stupid way of achiving this! (probably work a switch)
        if (Editor.endsWith(path, '.css')) {
            $('a[data-ace-mode="ace/mode/css"]').click();
        }
        else if (Editor.endsWith(path, '.js')) {
            $('a[data-ace-mode="ace/mode/javascript"]').click();
        }
        else if (Editor.endsWith(path, '.json')) {
            $('a[data-ace-mode="ace/mode/json"]').click();
        }
        else if (Editor.endsWith(path, '.xml')) {
            $('a[data-ace-mode="ace/mode/xml"]').click();
        }
        else if (Editor.endsWith(path, '.sql')) {
            $('a[data-ace-mode="ace/mode/sql"]').click();
        }
        else if (Editor.endsWith(path, '.md') || Editor.endsWith(path, '.markdown') || Editor.endsWith(path, '.mdown')) {
            $('a[data-ace-mode="ace/mode/markdown"]').click();
        }
        else if (Editor.endsWith(path, '.htm') || Editor.endsWith(path, '.html')) {
            $('a[data-ace-mode="ace/mode/html"]').click();
        }
        else {
            $('a[data-ace-mode="ace/mode/text"]').click();
        }
    },

    endsWith: function (str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    },

    // Internal Vars
    prefStore: null,
    editorStore: null,
    editor: null
};