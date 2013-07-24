/// <reference path="../jquery-1.7.1-vsdoc.js" />

var ModalViews = {
    Initialize: function () {
        $('a[data-purpose="purchase-credits"]').click(ModalViews.Events.Button.PurchaseCredits);
    },

    Events: {
        Button: {
            PurchaseCredits: function (e) {
                $.getJSON('/IDE/GetAPIPaymentLink/?sku=' + $(this).attr('data-sku'), function(link) {
                    var childWnd = window.open(link, 'Paymate', 'menubar=no,width=800');
                    if (childWnd === null) {
                        if (confirm('Cloudlab was prevented from opening the payment window. Would you like to redirect to the payments site? (You will be returned back to cloudlab afterwards)')) {
                            window.location = link;
                        }
                    }
                });
            }
        }
    }
};