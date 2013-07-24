var PageHelper = {
    init: function (e) {
        this.elem = e;
        this.ui_init();
        this.ed_init();
        this.md_init();
    },
    ui_init: function () {
        $('#nav_workspace').addClass('active');
    },
    ed_init: function () {
        $(PageHelper.elem).val(js_beautify($(PageHelper.elem).val()));

        PageHelper.codeEditor = CodeMirror.fromTextArea(document.getElementById(PageHelper.elem.substring(1)), {
            lineNumbers: true,
            tabSize: 4,
            indentUnit: 4,
            indentWithTabs: true,
            mode: "application/json",
            onChange: PageHelper.updateValidation
        });

        //$('#edUndo').click(PageHelper.codeEditor.undo);
        //$('#edRedo').click(PageHelper.codeEditor.redo);
    },
    md_init: function () {
        this.mdEditor = new MarkdownDeepEditor.Editor(document.getElementById('Description'), document.getElementById('markdownPreview'));

        $('#charCount').text(4000 - ($('#Description').val()).length);
        $('#Description').keyup(function () {
            $('#charCount').text(4000 - ($('#Description').val()).length);
        });

        $('#mdUndo').click(function () { PageHelper.mdEditor.InvokeCommand('undo'); });
        $('#mdRedo').click(function () { PageHelper.mdEditor.InvokeCommand('redo'); });

        $('#mdBold').click(function () { PageHelper.mdEditor.InvokeCommand('bold'); });
        $('#mdItalic').click(function () { PageHelper.mdEditor.InvokeCommand('italic'); });
        $('#mdOutdent').click(function () { PageHelper.mdEditor.InvokeCommand('outdent'); });
        $('#mdIndent').click(function () { PageHelper.mdEditor.InvokeCommand('indent'); });

        $('#mdUL').click(function () { PageHelper.mdEditor.InvokeCommand('ullist'); });
        $('#mdOL').click(function () { PageHelper.mdEditor.InvokeCommand('ollist'); });
        $('#mdHeading').click(function () { PageHelper.mdEditor.InvokeCommand('heading'); });
        $('#mdCode').click(function () { PageHelper.mdEditor.InvokeCommand('code'); });
        $('#mdSeparator').click(function () { PageHelper.mdEditor.InvokeCommand('hr'); });

        $('#mdLink').click(function () { PageHelper.mdEditor.InvokeCommand('link'); });
        $('#mdImage').click(function () { PageHelper.mdEditor.InvokeCommand('img'); });
    },
    updateValidation: function () {
        var result = JSHINT(PageHelper.codeEditor.getValue(), {
            curly: true,
            immed: true,
            strict: true,
            trailing: true,
            es5: true
        });
        if (!result) {
            var error = JSHINT.errors[0];
            $('#validationMsg').removeClass('alert-info').addClass('alert-error').html('<strong>Error [' + error.line + ':' + error.character + ']</strong>: ' + error.reason);
        } else {
            $('#validationMsg').removeClass('alert-error').addClass('alert-info').html('Validation OK');
        }
    },

    elem: null,
    codeEditor: null,
    mdEditor: null
};