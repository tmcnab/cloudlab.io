/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="../util/jquery.form.js" />
/// <reference path="../ui/jquery.gritter.js" />
/// <reference path="../ui/bootstrap.js" />
/// <reference path="../jqplot/plugins/jqplot.pieRenderer.js" />


$().ready(function () {
    UI.Initialize();
    Terminal.Initialize();
    Editor.Initialize();
    Resources.Initialize();
    Documentation.Initialize();
    ModalViews.Initialize();

    window.setInterval(UI.LongPoll, 10);
});

var UI = {
    Initialize: function () {

        UI.UpdateDropboxMenuState();

        $('a[rel="tooltip"]').tooltip();
        $('a[data-toggle="tab"]').on('shown', UI.Handlers.Bootstrap.Tabs);
        $('a[data-toggle="tab"]:not("a[data-target=\'#T1\']")').on('shown', Terminal.Pause);

        $('#DropboxAuthModal_Authorize').click(UI.Dropbox.Authorize);
        $('#Nav_Account').click(UI.UpdateDropboxMenuState);
        $('#DropboxDeAuthModal_DeAuthorize').click(UI.Dropbox.Deauthorize);
        $('#DropboxInfoModal').on('show', UI.Handlers.Events.Button.OnDropboxInfoModalShow);
        $('#APIModal').on('show', UI.Handlers.Events.Modal.OnAPIModalShow);

        $('#DropboxInfoModal_Forget').click(function () {
            $('#DropboxDeAuthModal').modal('show');
        });

        $('#RevokeKeyModal_RevokeButton').click(UI.Handlers.Events.Button.OnRevokeButtonClick);

        $(window).resize(UI.Handlers.Events.Window.Resize);
        UI.Handlers.Events.Window.Resize();
    },

    Notify: function (title, text, sticky, time) {
        $.gritter.add({
            title: title,
            text: text,
            sticky: sticky,
            time: time
        });
    },

    isPolling: false,
    LongPoll: function () {
        if (!UI.isPolling) {
            $.ajax('/IDE/PushEvent', {
                timeout: 0,
                beforeSend: function () {
                    UI.isPolling = true;
                },
                success: function (response) {
                    if (response === '') return;
                    console.log("PushEvent:\t" + response);
                    eval(response);
                },
                error: function () {
                    console.error('There was an error polling the server');
                },
                complete: function () {
                    console.log('AJAX Completed');
                    UI.isPolling = false;
                }
            });
        }
    },

    Handlers: {
        Bootstrap: {
            Tabs: function (e) {
                if (e.target.innerText === "Terminal") {
                    Terminal.Resume();
                }
                if (e.target.innerText === "Editor") {
                    Editor.Refresh();
                }
                if (e.target.innerText === "Files") {
                    Resources.Refresh();
                }
            }
        },
        Events: {
            Window: {
                Resize: function () {
                    $('.tab-content-size').css('height', (window.innerHeight - 91) + "px");
                }
            },
            Button: {
                OnDropboxInfoModalShow: function (e) {
                    $.getJSON('/api/dropbox/info', function (model) {
                        var data = [['Used&nbsp;', model.quota_info.normal],
                                    ['Free ', model.quota_info.quota - model.quota_info.normal]];

                        $.jqplot('DropboxInfoModal_Graph', [data], {
                            grid: {
                                drawBorder: false,
                                drawGridlines: false,
                                background: '#ffffff',
                                shadow: false
                            },
                            seriesDefaults: {
                                renderer: jQuery.jqplot.PieRenderer,
                                rendererOptions: {
                                    showDataLabels: true,
                                    padding: 5
                                }
                            },
                            legend: {
                                show: true,
                                placement: 'outside',
                                rendererOptions: {
                                    numberRows: 1
                                },
                                location: 's',
                                marginTop: '10px'
                            }
                        });

                        $('#DropboxInfoModal_Table').html('')
                            .append('<tr><td>Name</td>   <td>' + model.display_name + '</td></tr>')
                            .append('<tr><td>Country</td><td>' + model.country + '</td></tr>')
                            .append('<tr><td>Email</td>  <td>' + model.email + '</td></tr>')
                            .append('<tr><td>ID</td>     <td>' + model.uid + '</td></tr>')
                            .append('<tr><td>Quota</td>  <td>' + UI.BytesToSize(model.quota_info.quota) + '</td></tr>');
                    });
                },
                OnRevokeButtonClick: function () {
                    alert('To do!');
                }
            },
            Modal: {
                OnAPIModalShow: function () {
                    $.getJSON('/IDE/APIInfo', function (model) {
                        $('#T_APIModal > tbody > tr').remove();
                        $('#T_APIModal').append('<tr><td><strong>API Key</strong></td><td style="font-family:monospace">'
                                                    + model.key +
                                                    ' <a class="label label-important" data-dismiss="modal" data-toggle="modal" data-target="#RevokeKeyModal" style="cursor:pointer">Revoke</a></td></tr>')
                                        .append('<tr><td><strong>API Credits</strong></td><td>' + model.credits + '</td></tr>');
                    });
                }
            }
        }
    },

    UpdateDropboxMenuState: function () {
        $.getJSON('/api/dropbox/isauthorized', function (result) {
            if (!result) {
                $('a[data-target="#DropboxInfoModal"]')
                        .attr('data-target', '#DropboxAuthModal')
                        .html('<i class="icon-refresh"></i> Sync with Dropbox');
            }
            else {
                $('a[data-target="#DropboxAuthModal"]')
                        .attr('data-target', '#DropboxInfoModal')
                        .html('<i class="icon-file"></i> My Dropbox &hellip;');
            }
        });
    },

    Dropbox: {
        Authorize: function () {
            $.getJSON('/api/dropbox/AuthorizeS1', function (response) {
                if (response === null) {
                    alert('Something went wrong');
                }
                else {
                    var childWindow = window.open(
                                response,
                                'Dropbox Authorization',
                                'resizable = yes, scrollbars = yes, status = no');
                    if (childWindow === null) {
                        window.location = response;
                    }
                }
            });
        },
        Deauthorize: function () {
            $.getJSON('/api/dropbox/deauthorize', function (result) {
                if (result) {
                    UI.Notify('Dropbox', 'Your Dropbox info was successfully forgotten. You will have to authorize again to access your dropbox', false, '');
                    $('a[data-target="#DropboxInfoModal"]')
                        .attr('data-target', '#DropboxAuthModal')
                        .html('<i class="icon-refresh"></i> Sync with Dropbox');
                }
                else {
                    UI.Notify('Dropbox', 'There was an error forgetting your information. Please contact support!', true, '');
                }
            });
        }
    },

    BytesToSize: function (bytes) {
        var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        if (bytes == 0) return 'n/a';
        var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
        return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
    }
};