/// <reference path="../jquery-1.7.1-vsdoc.js" />
(function () {
    CodeMirror.intelliHint = function (editor, getHints) {
        // We want a single cursor position.
        if (editor.somethingSelected()) {
            return;
        }
        var result = getHints(editor);
        if (!result || !result.list.length) {
            return;
        }

        var completions = result.list;

        function insert(str) {
            editor.replaceRange(str, result.from, result.to);
        }

        console.log($('#intelliWnd').popover());

        // Build the IntelliHint Window
        $('#intelliWnd').popover({
            
        });

        console.log($('#intelliWnd').popover());

        /*var complete = $('<div></div>').addClass("popover fade right in CodeMirror-completions")
        .css('display', 'inline')
        ;
        var sel = $('<select></select>');

        complete.append(sel);


        for (var i = 0; i < completions.length; ++i) {
        var opt = $('<option></option>').text(completions[i]);
        sel.append(opt);
        }

        sel.get().selected = true;

        sel.size = Math.min(10, completions.length);
        var pos = editor.cursorCoords();
        complete.css('left', pos.x + "px");
        complete.css('top', pos.yBot + "px");
        //document.body.appendChild(complete.get());
        $('body').append(complete);


        //$('.cdfy-top').append(complete);
        // Hack to hide the scrollbar.
        //if (completions.length <= 10) {
        //complete.style.width = (sel.clientWidth - 1) + "px";
        //}
        */


        var done = false;

        function close() {
            if (done) {
                return;
            }
            done = true;
            $('.CodeMirror-completions').remove();
            //complete.parentNode.removeChild(complete);
        }

        function pick() {
            //insert(completions[sel.get().selectedIndex]);
            close();
            setTimeout(function () {
                editor.focus();
            }, 50);
        }

        CodeMirror.connect(sel[0], "blur", close);
        CodeMirror.connect(sel[0], "keydown", function (event) {
            // Enter Key
            if (event.keyCode === 13) {
                CodeMirror.e_stop(event);
                pick();
            }

            // Esc or Backspace
            else if (event.keyCode === 27 || event.keyCode === 8) {
                CodeMirror.e_stop(event);
                close();
                editor.focus();
            }

            else if (event.keyCode != 38 && event.keyCode != 40) {
                close();
                editor.focus();
                setTimeout(function () {
                    CodeMirror.simpleHint(editor, getHints);
                }, 50);
            }
        });
        CodeMirror.connect(sel[0], "dblclick", pick);

        sel[0].focus();
        // Opera sometimes ignores focusing a freshly created node
        if (window.opera) setTimeout(function () {
            if (!done) sel.get().focus();
        }, 100);
        return true;
    }
})();