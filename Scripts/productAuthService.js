//Remove jquery-ui-1.12.1.js and jquery-ui-1.12.1.js from _references.js and intellisence works again.
$(document).ready()
{
    //classes
    //---------------------------------------------------------------------------------
    function serviceEndPoints(isProduction) {

        var baseDev = 'http://localhost:55749/';
        var baseProd = 'http://localhost:8081/';
        var token = 'token';
        var base = isProduction == true ? baseProd : baseDev;
        
        this.loginUrl = function () {
            return base + token;
        }
    };

    //events
    //----------------------------------------------------------------------------------
    $('#linkCollapseValidationError').click(function () {
        $('#validationError').hide('fade');
    });

    // pass offered credenials for login attempt
    $('#btnLogin').click(function () {
        var uri = new serviceEndPoints(window.IsProductionMode).loginUrl();
        $.ajax({
            method: 'POST',
            url: uri,
            contentType: 'application/json',
            data: {
                username: $('#txtUsername').val(),
                password: $('#txtPassword').val(),
                grant_type: 'password'
            },
            success: function (response) {
                // api returns token and username in response object and we save in session.
                sessionStorage.setItem('accessToken', response.access_token);
                sessionStorage.setItem('identity', response.userName);
                window.location.href = '../HtmlViews/ProductsByCompanyDisplay.html';
            },
            error: function (jqxhr) {
                // api returns error in jquery xml http request object.
                $('#errorMessage').text(jqxhr.responseText);
                $('#validationError').show('fade');
            }
        });
    });

  
}