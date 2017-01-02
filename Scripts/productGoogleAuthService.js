//Remove jquery-ui-1.12.1.js and jquery-ui-1.12.1.js from _references.js and intellisence works again.

$(document).ready
{
    //classes
    //---------------------------------------------------------------------------------

    //create base class for storing base service end point adresses
    var baseServiceEndPoints = function () { }

    //Add function to baseServiceEndPoints class for getting dev or prod base address.
    baseServiceEndPoints.prototype.getBaseAddress = function (isProduction) {
        return isProduction == false ? 'http://localhost:55749/' : 'http://localhost:8081/';
    }

    var productServiceEndPoints = function (isProduction) {

        var userInfo = 'api/account/userinfo';
        var registerExternal = 'api/account/registerExternal';

        //using inherited base function
        this.userInfoUrl = function () {
            return this.getBaseAddress(isProduction) + userInfo;
        }
        //using inherited base function
        this.registerExternalUserUrl = function () {
            return this.getBaseAddress(isProduction) + registerExternal;
        }
    };

    var googleAuthServiceEndPoints = function (isProduction) {
        var productionGoogleSignin = 'api/Account/ExternalLogin?provider=Google&response_type=token&client_id=self&redirect_uri=http%3A%2F%2Flocalhost%3A8081%2FRegistration_Login%2Flogin.html&state=S7xwr01yIjoEegE0hsi6sj9k5DIpkJcRHbhrE91TSNI1';
        var developmentGoogleSignin = 'api/Account/ExternalLogin?provider=Google&response_type=token&client_id=self&redirect_uri=http%3A%2F%2Flocalhost%3A55749%2FRegistration_Login%2Flogin.html&state=FXljII7DPwuTGoq_0m7EEXUdIr_dU8nlr5yF3dsUB3w1';
        var googleSignin = isProduction == true ? productionGoogleSignin : developmentGoogleSignin;
        this.googleSigninUrl = function () {
            //using inherited base function
            return this.getBaseAddress(isProduction) + googleSignin;
        }
    }

    //create a mechanism for implementing inheritence
    var inheritsFrom = function (child, parent) {
        child.prototype = Object.create(parent.prototype);
    };

    //Implement inheritence
    //Both of these classes will now inherit from baseServiceEndPoint
    //and can use base class getBaseAddress
    inheritsFrom(productServiceEndPoints, baseServiceEndPoints);
    inheritsFrom(googleAuthServiceEndPoints, baseServiceEndPoints);

    //events
    //---------------------------------------------------------------------------------------
    $('#btnGoogleLogin').click(function () {
        var uri = new googleAuthServiceEndPoints(window.IsProductionMode).googleSigninUrl();
        window.location.replace(uri);
    });

    //functions
    //-----------------------------------------------------------------------------------------
    //Parse the returned url from google.
    function getAccessToken() {
        //check if the url has a # hash in it.
        if (location.hash) {
            //split out url contained in location and get just the access token.
            if (location.hash.split('access_token=')) {
                var googleAccessToken = location.hash.split('access_token=')[1].split('&')[0];
            }
            //If the access token exists in url chech to see if the user is a registered product service user.
            if (googleAccessToken) {
                isUserRegistered(googleAccessToken);
            }
        }
    }

    //Check if google signin user is already a locally registered product service user.
    //If registered store token and identity a redirect to products by company display.
    //else register the new google signin user.
    function isUserRegistered(googleAccessToken) {
        var uri = new productServiceEndPoints(window.IsProductionMode).userInfoUrl();
        $.ajax({
            url: uri,
            method: 'GET',
            headers: {
                'content-type': 'application/json',
                'authorization': 'bearer ' + googleAccessToken
            },
            success: function (response) {
                if (response.HasRegistered) {
                    sessionStorage.setItem('accessToken', googleAccessToken)
                    sessionStorage.setItem('identity', response.Email)
                    window.location.href = '../htmlviews/ProductsByCompanyDisplay.html';
                }
                else {
                    signupExternalUser(googleAccessToken);
                }
            }
        });
    }

    //register the google signin user locally for product service access.
    function signupExternalUser(googleAccessToken) {
        var uri = new productServiceEndPoints(window.IsProductionMode).registerExternalUserUrl();
        $.ajax({
            url: uri,
            method: 'POST',
            headers: {
                'content-type': 'application/json',
                'authorization': 'bearer ' + googleAccessToken
            },
            success: function (response) {
                window.location.href = new googleAuthServiceEndPoints(window.IsProductionMode).googleSigninUrl();
            },
            error: function (jqXHR) {
                // API returns error in jQuery xml http request object.
                $('#errorMessage').text(jqXHR.responseText);
                $('#validationError').show('fade');
            }
        });
    }

    //function calls
    //-----------------------------------------------------------------------------------------
    getAccessToken();
}