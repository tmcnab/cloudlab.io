/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="Index.js" />
/// <reference path="../util/SmartStorage.js" />

var Plotting = {
    Initialize: function () {
        $('#graphingTabs > ul.nav > li > a > i').on('click', function (e) {
            console.log('closing graph');
        });
        Plotting.store = new SmartStorage("plotting-config");
    },
    Plot: function (series, options) {
        $('a[data-target="#T4"]').click();

        var guid = Plotting.GuidGenerator();
        var pTitle = options.title === '' ? 'Untitled Plot' : options.title;
        var removeButton = $('<i title="close" class="icon-remove"></i>').on('click', Plotting.Events.OnPlotClose);
        var tablet = $('<a data-target="#' + guid + '" data-toggle="tab">' + pTitle + ' </a>').append(removeButton);

        $('#graphingTabs > ul.nav > li.active').removeClass('active');
        $('#graphingTabs > ul.nav').append($('<li class="active"></li>').append(tablet));

        $('#graphingTabs > div.tab-content > div.active').removeClass('active');
        $('#graphingTabs > div.tab-content').append('<div class="tab-pane active" id="' + guid + '" style="width:100%;height:500px"></div>');
        $('#graphingTabs > div.tab-content > div.active').click();

        $.extend(true, options, Plotting.plotConfig);
        var plot = $.jqplot(guid, series, options);
        if (plot != null) {
            plot.replot({
                resetAxes: true
            });
        }
        //console.log($('#' + guid).jqplotToImageElem()); where to save this?
    },
    GuidGenerator: function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    },
    Events: {
        OnPlotClose: function (e) {
            var uuid = $(this).parent().attr('data-target');
            $(uuid).remove();
            $(this).parent().parent().remove();
        }
    },

    Settings: {
        
    },

    // Internal Vars
    plotConfig: {
        animate: true,
        animateReplot: true,
        cursor: {
            show: true,
            zoom: true,
            looseZoom: true,
            showTooltip: true
        },
        highlighter: {
            show: true,
            showLabel: true,
            tooltipAxes: 'y',
            sizeAdjust: 7.5, tooltipLocation: 'ne'
        },
        axes: {
            xaxis: {
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica Neue',
                    fontSize: '12pt'
                }
            },
            yaxis: {
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica Neue',
                    fontSize: '12pt'
                }
            },
            x2axis: {
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica Neue',
                    fontSize: '12pt'
                }
            },
            y2axis: {
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Helvetica Neue',
                    fontSize: '12pt'
                }
            }
        }
    },
    store: null
};