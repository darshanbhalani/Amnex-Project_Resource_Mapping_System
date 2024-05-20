var spinnerVisible = false;
function setRecaptchaResponse(response) {
    // Store the reCAPTCHA response in the hidden input field
    $('#recaptcha-response2').val(response);
}
$(document).ready(function () {

    $(document).ready(function () {
        $('#togglePassword').click(function () {
            var passwordInput = $('#Password');
            var passwordFieldType = passwordInput.attr('type');
            var newPasswordFieldType = (passwordFieldType === 'password') ? 'text' : 'password';
            passwordInput.attr('type', newPasswordFieldType);
        });
    });

    $("#loginBtnClick").click(function () {
        $("#loginModal").modal('show');
    });
    function setRecaptchaResponse(response) {
        $('#recaptcha-response2').val(response);
    }
    $("#submitBTN").click(function (e) {
        e.preventDefault();
        var userName = $("#userName").val();
        var email = $("#email").val();
        var recaptchaResponse = $('#recaptcha-response2').val();

        if (!userName || !email) {
            $("#errorMessage").text("Enter User Name and Email");
            return;
        }
        console.log("Captcha : " + recaptchaResponse);

        if (!recaptchaResponse) {
            $("#errorMessage").text("Please complete the reCAPTCHA");
            return;
        }
        if ($("#userName").val().length < 1 & $("#email").val().length < 1) {
            $("#errorMessage").text("Enter User Name and Email");
            return;
        }
        if ($("#userName").val().length < 1) {
            $("#errorMessage").text("Enter User Name");
            return;
        }
        if ($("#email").val().length < 1) {
            $("#errorMessage").text("Enter Email");
            return;
        }

        loading();
        $.ajax({
            type: 'POST',
            url: '/Account/ForgotPassword',
            data: {
                EmployeeUserName: userName,
                Email: email,
                recaptchaResponse: recaptchaResponse
            },
            success: function (res) {
                if (res.success) {
                    loading();
                    $("#userName").val('');
                    $("#email").val('');
                    $("#forgotPassword").modal('hide');
                    $('.modal-backdrop').remove();
                    showSnackbar("true", "Password successfuly sended");
                }
                else {
                    loading();
                    $('#errorMessage').text(res.message);
                    grecaptcha.reset();
                }
            },
            error: function (res) {
                loading();
            }
        });
    });

    $("#forgot-password-btn").click(function () {
        $("#loginModal").modal('hide');
        $('.modal-backdrop').remove();
        $("#forgotPassword").modal('show');
    });

    $('#loginbtn').click(function (e) {
        e.preventDefault();
        var username = $('#Username').val();
        var password = $('#Password').val();
        var recaptchaResponse = $('#g-recaptcha-response').val(); // Assuming you have a hidden input field with id 'g-recaptcha-response' to store the CAPTCHA response
        if (username.length < 1 & password.length < 1) {
            $('#Errormsg').text("Enter Username and password");
            return;
        }
        if (password.length < 1) {
            $('#Errormsg').text("Enter password");
            return;
        }
        if (username.length < 1) {
            $('#Errormsg').text("Enter Username");
            return;
        }

        // Perform basic client-side validation
        if (!username.trim() || !password.trim()) {
            $('#Username').next('.text-danger').text('Please enter a username.');
            $('#Password').next('.text-danger').text('Please enter a Password.'); return;
        }

        // Send AJAX request
        loading();
        $.ajax({
            type: 'POST',
            url: '/Account/Login',
            data: {
                username: username,
                password: password,
                recaptchaResponse: recaptchaResponse
            },
            success: function (response) {
                loading();
                if (response.success) {
                    window.location.href = '/Home/Dashboard';
                } else {
                    $('#Errormsg').text(response.message);
                    grecaptcha.reset();
                    return;
                }
            },
            error: function () {
                loading();
                alert('An error occurred. Please try again.');
            }
        });
    });

    function showSnackbar(status, message) {
        console.log("snackbar");
        if (status === "true") {
            $('#snackbar').addClass('bg-success');
        } else {
            $('#snackbar').addClass('bg-danger');
        }
        var snackbar = document.getElementById("snackbar");
        snackbar.textContent = message;
        snackbar.style.display = "block";
        setTimeout(function () { snackbar.style.display = "none"; }, 3000);
    };

    function loading() {
        if (!spinnerVisible) {
            $('#loadingModal').modal('show');
            spinnerVisible = true;
        } else {
            $('#loadingModal').modal('hide');
            spinnerVisible = false;
        }
    };


});
