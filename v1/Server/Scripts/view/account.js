/// <reference path="bootbox.js"/>
/// <reference path="jquery.md5.js"/>
/// <reference path="jquery-1.7.1-vsdoc.js" />

var Account = {
    init: function () {
        this.ui_init();
        this.loadGraph();
    },
    ui_init: function () {
        $('#nav_account').addClass("active");

        $('a[data-toggle="tab"]').live('shown', Account.ui_onTabChange);
        $("#UseGravatar").on('click', Account.loadGravatar);
        $("#profileForm").submit(function () {
            Account.profile_submit();
            return false;
        });
        $('#revokeButton').click(function () {
            bootbox.dialog("<h3>Are you sure you want to revoke your API key?</h3><p>If you revoke your API key, you will be provided a new one. Your old one will be destroyed and cannot be recovered. Are you sure you want to do this?</p>",
            [
            {
                "label": "Revoke",
                "class": "danger",
                "callback": function () {
                    $.getJSON('/Account/_RevokeAPIKey', function (data) {
                        if (data.status) {
                            $('#apikeyVerbatim').text(data.key);
                            bootbox.alert("<h3>Success!</h3><p>Your API was successfully revoked. Your new API key is:</p><p><code>" + data.key + "</code></p>", function () {
                                bootbox.hideAll();
                            });
                        }
                        else {
                            bootbox.alert("<h3>Error</h3><p>There was an error revoking your API key. Please try revoking the key again, sign-out then in again, or let us know by submitting a new support ticket.</p><p>Again, sorry.</p>", function () {
                                bootbox.hideAll();
                            });
                        }
                    });
                    bootbox.dialog("<div style='text-align:center;'><img src='/Content/glyphs/spinner.gif' width='24px' /><h3>Revoking Key . . .</h3></div>");
                }
            },
            {
                "label": "Cancel",
                "class": " "
            }
            ], {
                "backdrop": "static",
                "keyboard": true
            });
        });
        $('#ticketButton').click(function () {
            bootbox.dialog("<h3>Lodge Support Ticket</h3><form id='ticketForm'><fieldset class='control-group'><label class='control-label' for='Title'>Title</label><div class='controls'><input class='span5' id='Title' name='Title' type='text' maxlength='50' required placeholder='Title of your support request (Required)' /></div></fieldset><fieldset class='control-group'><label class='control-label' for='Description'>Description</label><div class='controls'><textarea class='span5' rows='4' id='Description' name='Description' style='resize:none;overflow:visible' maxlength='4000' required placeholder='Please, in as much detail as you can, describe the issue you are having (bug, feature request etc). (Required)'></textarea></div></fieldset></form>", [
            {
                "label": "Submit",
                "class": "btn-success",
                "callback": function () {
                    bootbox.dialog("<div style='text-align:center;'><img src='/Content/glyphs/spinner.gif' width='24px' /><h3>Submitting Ticket ...</h3></div>");
                    $.ajax({
                        url: '/Account/_SubmitTicket',
                        data: $("#ticketForm").serialize(),
                        type: 'POST',
                        success: function (model) {
                            bootbox.hideAll();
                            if (model) {
                                var m = bootbox.dialog("<div style='text-align:center;'><h3>Submitted</h3></div>");
                                setTimeout(function () {
                                    m.modal('hide');
                                }, 2000);
                                Account.tickets_load();
                            }
                            else {
                                bootbox.alert("<h3>Error</h3><p>There was an error processing you submission. We'll get right on that.</p>");
                            }
                        }
                    });
                }
            },
            {
                "label": "Cancel",
                "class": " "
            }
            ], {
                "backdrop": "static",
                "keyboard": true
            });
        });

        $('#ticketsTable > tbody > tr').live('click', function (e) {
            var ticketid = $(this).children(":first").text();
            $.getJSON('/Account/_SupportTicketGet/' + ticketid, function (model) {
                console.log(model);
                _html = '<h3>Problem</h3><p>' + model.problem + '</p>';
                if (model.solved) {
                    _html += '<h3>Solution</h3><p>' + model.solution + '</p>';
                }
                else {
                    _html += "<h3>Solution</h3><p><em>There is no solution yet, but we're getting there!</em></p>";
                }
                bootbox.alert(_html);
            });
        });

        Account.loadGravatar();
    },
    ui_onTabChange: function (e) {
        switch (e.target.text) {
            case "Overview":
                Account.plot.replot();
                break;
            case "Profile":
                Account.profile_load();
                break;
            case "Support":
                Account.tickets_load();
                break;
        }
    },
    profile_load: function () {
        Account.loadGravatar();
    },
    profile_submit: function () {
        $("#profileSubmitButton").addClass('disabled');
        $("#profileSubmitMessage").html('Saving ...').removeClass();
        $.ajax({
            url: '/Account/_UpdateProfile',
            data: $("#profileForm").serialize(),
            type: 'POST',
            success: function (e) {

                $("#profileSubmitMessage").html('Saved').addClass();
            },
            error: function (e) {
                $("#profileSubmitMessage").html('There was an error saving your info.').addClass();
            },
            complete: function () {
                $("#profileSubmitButton").removeClass('disabled');
            }
        });
    },
    tickets_load: function () {
        $.getJSON('/Account/_SupportTicketGet', function (data) {
            $('#ticketsTable > tbody').html('');
            if (data.length === 0) {
                $('#ticketsTable > tfoot').html("<tr><td colspan='3' style='text-align:center;font-style:italic'>You don't have any support tickets. That's good, right?</td></tr>");
            }
            else {
                $('#ticketsTable > tfoot').html('');
                $.each(data, function (i, item) {
                    $("#ticketsTable").append('<tr title="click to view"><td>' + item.Id + '</td><td>' + item.Title + '</td><td>' + (item.WeSolvedIt ? '✔' : '✖') + '</td></tr>');
                });
            }
        });
    },
    loadGravatar: function () {
        if ($('#UseGravatar').is(':checked')) {
            $.getJSON('/Account/_Email', function (data) {
                var hash = $.md5(data);
                $("#gravatar").attr("src", "http://www.gravatar.com/avatar/" + hash + "?d=mm&r=pg");
            });
        }
        else {
            $("#gravatar").attr("src", "http://www.gravatar.com/avatar/?d=mm&f=y");
        }
    },
    loadGraph: function () {
        $('#usageGraph').html('');
        $.getJSON('/Account/_Quota', function (model) {
            var tData = [model.tools];
            var dData = [model.data];
            var range = 25;
            if (model.tools >= 25 || model.data >= 25) range = 50;
            if (model.tools >= 50 || model.data >= 50) range = 75;
            if (model.tools >= 75 || model.data >= 75) range = 100;

            Account.plot = $.jqplot('usageGraph', [dData, tData], {
                title: {
                    text:'Account Usage'
                },
                grid: {
                    shadow: false,
                },
                seriesDefaults: {
                    renderer: $.jqplot.BarRenderer
                },
                series: [
                    { label: 'Data' },
                    { label: 'Tools' }
                ],
                legend: {
                    show: true
                },
                axes: {
                    xaxis: {
                        renderer: $.jqplot.CategoryAxisRenderer,
                        label: '',
                        ticks: ['Component']
                    },
                    yaxis: {
                        pad: 1.10,
                        max: range,
                        min: 0,
                        label: 'Usage (%)',
                        angle: 90,
                        labelRenderer: $.jqplot.CanvasAxisLabelRenderer
                    }
                },
            });

            Account.plot.replot();
        });
    },

    plot: null
};

$(document).ready(function () {
    Account.init();
});