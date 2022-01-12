$(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar, #content').toggleClass('active');
        var text = $("#toggleMessage").text();

        if (text == "Show menu") {
            $("#toggleMessage").text("Hide menu");
        }
        else {
            $("#toggleMessage").text("Show menu");
        }
    });
});
