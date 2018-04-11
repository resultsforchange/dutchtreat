var x = 0;
var s = "";
console.log("hello Vernon");

var theForm = $("#theForm");
theForm.hide();

var buyButton = $("#buyButton");
// add a listener to the event
buyButton.on("click", function () {
    console.log("Buying item");
});

var productInfo = $(".productProps li");
productInfo.on("click", function () {
    console.log("You clicked on " + $(this).text());
});

var $loginToggle = $("#loginToggle");
var $popupForm = $(".popup-form");

$loginToggle.on("click", function () {
    $popupForm.fadeToggle(1000);
});
