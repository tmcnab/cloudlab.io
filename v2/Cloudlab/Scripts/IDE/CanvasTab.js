/// <reference path="../jquery-1.7.1-vsdoc.js" />

var Canvas = {

    //-- Functions
    Initialize: function () {
        Canvas.Demo();
    },
    Notify: function (title, text, sticky, time) {
        $.gritter.add({
            title: title,
            text: text,
            sticky: sticky,
            time: time
        });
    },

    Demo: function () {
        // stolen from http://www.williammalone.com/articles/create-html5-canvas-javascript-drawing-app/#demo-simple
        var canvas = document.getElementById('canvasContainer');
        var context = canvas.getContext("2d");
        $('#canvasContainer').mousedown(function (e) {
            var mouseX = e.pageX - this.offsetLeft;
            var mouseY = e.pageY - this.offsetTop;

            paint = true;
            addClick(e.pageX - this.offsetLeft, e.pageY - this.offsetTop);
            redraw();
        });
        $('#canvasContainer').mousemove(function (e) {
            if (paint) {
                addClick(e.pageX - this.offsetLeft, e.pageY - this.offsetTop, true);
                redraw();
            }
        });
        $('#canvasContainer').mouseup(function (e) {
            paint = false;
        });
        $('#canvasContainer').mouseleave(function (e) {
            paint = false;
        });
        var clickX = new Array();
        var clickY = new Array();
        var clickDrag = new Array();
        var paint;

        function addClick(x, y, dragging) {
            clickX.push(x);
            clickY.push(y);
            clickDrag.push(dragging);
        }
        function redraw() {
            canvas.width = canvas.width; // Clears the canvas

            context.strokeStyle = "#df4b26";
            context.lineJoin = "round";
            context.lineWidth = 5;

            for (var i = 0; i < clickX.length; i++) {
                context.beginPath();
                if (clickDrag[i] && i) {
                    context.moveTo(clickX[i - 1], clickY[i - 1]);
                } else {
                    context.moveTo(clickX[i] - 1, clickY[i]);
                }
                context.lineTo(clickX[i], clickY[i]);
                context.closePath();
                context.stroke();
            }
        }
    },

    //-- Event Handlers
    Handlers: {

    },

    //-- Settings & Persistence
    Settings: {

    }

    //-- Internal Vars

};