function ShowPassword()
{
    var x = document.getElementsByClassName("ShowPass")[0];
    if (x.type === "password") {
        x.type = "text";
        }
    else {
        x.type = "password";
        }
}



function DeleteAccount() {
    if (confirm("Are you sure you want to delete your account?"))
        window.location.href = "/Login/ConfirmDeletAccount/";
    else
        window.location.href = "/Home/Index/";
}
