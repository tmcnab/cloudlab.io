/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="Index.js" />
/// <reference path="../ui/jquery.dataTables.js" />
/// <reference path="../ui/bootstrap.js" />

var Resources = {

    //-- Functions
    Initialize: function () {
        Resources.Handlers.UI.File.List();
    },

    Refresh: function () {
        Resources.Handlers.UI.File.List();
    },

    //-- Event Handlers
    Handlers: {
        Event: {
            Table: {
                RowSelect: function (e) {
                    if ($(this).attr('data-fstype') === 'dir') {
                        Resources.Settings.currentPath = $(this).attr('data-path');
                        Resources.Handlers.UI.File.List();
                    }
                    else {
                        Resources.Handlers.UI.File.Open($(this).attr('data-path'));
                    }
                },
                RightClick: function (e) {

                    // Remove the old stuff
                    $('#cx').remove();
                    $('div.popover').remove();

                    var _cursor = $('<div id="cx"></div>')
                        .css('position', 'absolute')
                        .css('width', '1px')
                        .css('height', '1px')
                        .css('top', e.pageY)
                        .css('left', e.pageX)
                        .css('background-color', 'black')
                        .attr("tabindex", -1)
                        .blur(function () {
                            $('#cx').remove();
                            $('div.popover').remove();
                        });

                    $('body').append(_cursor);
                    $('#cx').focus().blur(function () {

                        $('#cx').remove();
                        $('div.popover').remove();
                    })
                        .popover({
                            placement: (e.pageX - this.offsetLeft < (window.innerWidth / 2))
                                    ? 'right'
                                    : 'left',
                            trigger: 'focus',
                            content: '<h1>Hello!</h1>'
                        });
                    $('#cx').popover('show');
                }
            }
        },
        UI: {
            File: {
                List: function (e) {
                    $.getJSON('/api/dropbox/isauthorized', function (result) {
                        if (result) {
                            $.getJSON('/IDE/DropboxListDir/?path=' + encodeURIComponent(Resources.Settings.currentPath), function (response) {

                                if (!Resources.dataTableInit) {
                                    $('#T_Resources_List > tbody > tr').remove();
                                    Resources.dataTableInit = true;
                                }
                                else {
                                    var nRows = $('#T_Resources_List > tbody > tr').length;
                                    var dTable = $('#T_Resources_List').dataTable();
                                    for (var i = 0; i < nRows; i++) {
                                        dTable.fnDeleteRow(0);
                                    }
                                    dTable.fnDraw(false);
                                    $('div.tooltip').remove();

                                    if (Resources.Settings.currentPath !== '/') {
                                        var _fPath = Resources.Settings.currentPath.split('/');
                                        _fPath.pop();
                                        _fPath = _fPath.join('/');

                                        var _fIcon = '<td><span style="display:none">FOLDER</span><i class="icon-folder-close"></i></td>';
                                        var _fName = '<td>[Parent Folder]</td>';
                                        var _fSize = '<td></td>';
                                        var elem = $('<tr data-path="' + (_fPath === '' ? '/' : _fPath) + '" data-fstype="dir">' + _fIcon + _fName + _fSize + '</tr>');
                                        elem.tooltip({
                                            title: 'Click to Open Directory',
                                            placement: 'left'
                                        }).css('cursor', 'pointer');
                                        $('#T_Resources_List').append(elem);
                                    }
                                }

                                var _table = $('#T_Resources_List');
                                $.each(response, function (i, item) {
                                    var _fIcon = !item["Is_Dir"] ? '<td><span style="display:none">FILE</span><i class="icon-file"></i></td>'
                                                         : '<td><span style="display:none">FOLDER</span><i class="icon-folder-close"></i></td>';
                                    var _fName = '<td>' + item["Name"] + '</td>';
                                    var _fSize = '<td>' + (!item["Is_Dir"] ? item["Size"] : '&nbsp;') + '</td>';
                                    var elem = $('<tr data-path="' + item["Path"] + '" data-fstype="' + (item["Is_Dir"] ? 'dir' : 'file') + '">' + _fIcon + _fName + _fSize + '</tr>');
                                    elem.tooltip({
                                        title: 'Click to Open ' + (item["Is_Dir"] ? ' Directory' : 'in Editor'),
                                        placement: 'left'
                                    }).css('cursor', 'pointer');
                                    _table.append(elem);
                                });


                                //$('#T_Resources_List').dataTable().fnClearTable();
                                $('#T_Resources_List').dataTable({
                                    "bAutoWidth": false,
                                    "bLengthChange": false,
                                    "iDisplayLength": 20,
                                    "sPaginationType": "bootstrap",
                                    "bDestroy": true
                                }).fnSort([[0, 'desc']]);

                                $('div.dataTables_filter').remove();
                                $('#T_Resources_List_info').remove();

                                $('#T_Resources_List > tbody > tr').on('click', Resources.Handlers.Event.Table.RowSelect);

                            });
                        }
                        else {
                            UI.Notify('Oh snap!', 'You need to authorize Cloudlab for Dropbox usage (Account &gt; Dropbox)', true, '');
                        }
                    });
                },
                Open: function (path) {
                    Editor.Open(path); // OPen in the editor for now
                }
            }
        }
    },

    Utils: {
        ConvertDate: function (d) {
            var _date = Date(d);
            return _date.getTime() + ' ' + _date.getDate();
        }
    },

    //-- Settings & Persistence
    Settings: {
        currentPath: '/'
    },

    //-- Internal Vars
    dataTableInit: false
};