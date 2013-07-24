/// <reference path="../codemirror/cm-2.2.min.js" />
/// <reference path="../jquery-1.7.1-vsdoc.js" />
(function ($) {

    var CMEditor = null;
    var foldFunc = CodeMirror.newFoldFunction(CodeMirror.braceRangeFinder);

    $.fn.Codify = function (opts) {
        'use strict';

        console.log(opts);

        var defaults = {
            lineNumbers: true,
            matchBrackets: true,
            onChange: onChangeEvent,
            onGutterClick: foldFunc,
            extraKeys: {
                "Shift-Space": function (cm) {
                    CodeMirror.intelliHint(cm, CodeMirror.javascriptHint);
                },
                "Ctrl-M": function (cm) {
                    foldFunc(cm, cm.getCursor().line);
                },
                "Esc": function (cm) {
                    goFullScreen();
                },

            }
        };

        if (opts) {
            $.extend(defaults, opts);
        }

        CMEditor = CodeMirror.fromTextArea($(this).get(0), defaults);
        onChangeEvent();
        $(document).keydown(function (e) { if (e.keyCode == 8) e.preventDefault(); });
        return this.each(function () {

        });

    };

    // public functions definition
    $.fn.Codify.functionName = function (foo) {
        return this;
    };

    function onChangeEvent(cm, evt) {
        var coords = CMEditor.getCursor();
        $('.cdfy-rowcol').text((parseInt(coords.line) + 1) + ":" + coords.ch);
        updateValidation();

        if (cm !== undefined) {
            //CodeMirror.simpleHint(cm, CodeMirror.javascriptHint);
            if (evt.text[0] === ' ') {
                //CodeMirror.simpleHint(cm, CodeMirror.javascriptHint);
            }
        }
    }

    // private functions definition
    function updateValidation() {
        var result = JSHINT(CMEditor.getValue(), {
            curly: true,
            immed: true,
            strict: true,
            trailing: true,
            es5: true
        });
        if (!result) {
            var error = JSHINT.errors[0];
            $('.cdfy-valmsg').removeClass('success').addClass('error').html('<strong>Error [' + error.line + ':' + error.character + ']</strong>: ' + error.reason);
        } else {
            $('.cdfy-valmsg').removeClass('error').html('No errors.');
        }
    }

    var toggleFullscreenEditing = {};

    function goFullScreen() {
        var editorDiv = $('.cdfy-container');
        if (!editorDiv.hasClass('fullscreen')) {
            toggleFullscreenEditing.beforeFullscreen = { height: editorDiv.height(), width: editorDiv.width() }
            editorDiv.addClass('fullscreen');
            editorDiv.height('100%');
            editorDiv.width('100%');
            CMEditor.refresh();
        }
        else {
            editorDiv.removeClass('fullscreen');
            editorDiv.height(toggleFullscreenEditing.beforeFullscreen.height);
            editorDiv.width(toggleFullscreenEditing.beforeFullscreen.width);
            CMEditor.refresh();
        }
    }

})(jQuery);