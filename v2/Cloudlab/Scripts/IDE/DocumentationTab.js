/// <reference path="../jquery-1.7.1-vsdoc.js" />
/// <reference path="Index.js" />

var Documentation = {
    Initialize: function () {
        Documentation.LoadIndex();
        $('#doc-search-form').submit(Documentation.Handlers.Events.OnSearchSubmit);
    },
    LoadIndex: function () {
        $.getJSON('/IDE/DocList', function (model) {
            $('#DocumentationIndex > li:nth-child(2)').remove();
            $.each(model, function (i, item) {
                Documentation.AddIndexEntry(i, item);
            });

            $('a[data-doc-id]').on('click', Documentation.Handlers.Events.OnEntityMemberClick);
            $('#DocumentationIndex > li > a[data-toggle="collapse"]').click(Documentation.Handlers.Events.OnEntityToggle);
        });
    },
    AddIndexEntry: function (category, entity) {
        $('#DocumentationIndex').append('<li class="nav-header">' + category + '</li>');

        $.each(entity, function (i, item) {
            $('#DocumentationIndex').append('<li><a class="accordion-toggle" data-toggle="collapse" href="#' + (i + 'Collapse') + '"><i class="icon-chevron-right"></i> ' + i + '</li>');
            var collapseContainer = '<li id=' + (i + 'Collapse') + ' class="collapse">';
            collapseContainer += '<ul class="nav nav-list">';

            $.each(item, function (j, elem) {
                collapseContainer += '<li><a data-doc-id="' + elem.id + '">' + elem.name;
                if (elem.type === 'function') {
                    collapseContainer += '()';
                }
                collapseContainer + '</a></li>';
            });

            collapseContainer += '</ul></li>';
            $('#DocumentationIndex').append(collapseContainer);
        });
    },
    Handlers: {
        Events: {
            OnEntityToggle: function (e) {
                var icon = $(this).children(':first');
                if (icon.hasClass('icon-chevron-right')) {
                    icon.addClass('icon-chevron-down').removeClass('icon-chevron-right');
                }
                else {
                    icon.addClass('icon-chevron-right').removeClass('icon-chevron-down');
                }
            },
            OnEntityMemberClick: function (e) {
                var _this = $(this);
                console.log(_this);
                $.ajax('/IDE/Doc/' + _this.attr('data-doc-id'), {
                    success: function (result) {
                        $('li.active > a[data-doc-id]').parent().removeClass('active');
                        _this.parent().addClass('active');

                        var mdd = new MarkdownDeep.Markdown();
                        mdd.ExtraMode = true;
                        mdd.MarkdownInHtml = true;
                        $('#DocumentationBody').html(mdd.Transform(result));
                        $('#DocumentationBody').find('table')
                                               .addClass('table')
                                               .addClass('table-striped')
                                               .addClass('table-bordered');
                        $('#DocumentationBody').find('div.alert > h4')
                                               .css('text-align', 'center');
                    }
                });
            },
            OnSearchSubmit: function (e) {
                e.preventDefault();
                Documentation.PerformSearch($(this).children(':first').val());
                return false;
            }
        }
    },
    PerformSearch: function (e) {
        // Empty the existing content and any sidebar highlighting
        $('#DocumentationBody').html('');
        $('li.active > a[data-doc-id]').parent().removeClass('active');

        //
        $.getJSON('/IDE/DocSearch/?q=' + e, function (model) {
            if (model.length > 0) {

                var table = '<table class="table table-condensed">' +
                            '<thead><tr><th>Category</th><th>Object</th><th>Name</th><th>Type</th></tr></thead>' +
                            '<tbody>';

                $.each(model, function (i, item) {
                    table += '<tr data-doc-id="' + item.id + '"><td>' + item.category + '</td><td>' + item.entity + '</td><td>' + item.name + '</td><td>' + item.type + '</td></tr>';
                });

                table += '</tbody></table>';
                $('#DocumentationBody').html(table);
                $('tr[data-doc-id]').on('click', Documentation.Handlers.Events.OnEntityMemberClick);
            }
            else {
                $('#DocumentationBody').html('<p style="text-align:center;margin-top:50px;"><i class="icon-exclamation-sign"></i> No results found for "' + e + '"');
            }
        });
    }
    // Internal Vars

};