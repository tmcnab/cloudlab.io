$('#NavBar_AuthButton').click(function () {
    navigator.id.getVerifiedEmail(function (assertion) {
        if (assertion !== null) {
            $.ajax({
                type: 'POST',
                url: '/Main/Authenticate',
                data: {
                    assertion: assertion
                },
                success: function (res, status, xhr) {
                    if (res != null) {
                        window.location = '/';
                    }
                },
                error: function (res, status, xhr) {
                    
                }
            });
        }
    });
});