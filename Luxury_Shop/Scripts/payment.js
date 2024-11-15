//----------------------------------------------------/
// Filter FAQ List
//----------------------------------------------------/
$(document).ready(function () {
    const input = $("#dynamic-filter-input");
    const clearButton = $("#clearButton");

    input.on("input", function () {
        if (input.val().trim() !== "") {
            clearButton.removeClass("d-none");
        } else {
            clearButton.addClass("d-none");
        }

        FilterListSection();
    });

    clearButton.on("click", function () {
        input.val("");
        clearButton.addClass("d-none");
        FilterListSection();
    });

    function FilterListSection() {
        var filter = input.val().toUpperCase();
        var ul = $("#dynamic-filter-list");
        var li = ul.find("li");

        li.each(function () {
            var p = $(this).find("p").eq(0);
            if (p.text().toUpperCase().indexOf(filter) > -1) {
                $(this).css("display", "");
            } else {
                $(this).css("display", "none");
            }
        });
    }
});
//----------------------------------------------------/
// dynamic filter system links
//----------------------------------------------------/
$(document).ready(function ($) {
    $("#dynamic-filter-list li").on("click", function () {
        $(this).addClass("filter-item-open");
        $(".dynamic-filter-list-container").addClass("dynamic-filter-min-height");
        $("#" + this.id + "-content")
            .delay(0)
            .fadeIn(50);
    });
    $(".filter-content-close,.filter-content-close-mobile ").on(
        "click",
        function () {
            $(this).addClass("filter-item-open");
            $(".dynamic-filter-list-container").removeClass(
                "dynamic-filter-min-height"
            );
            $(".filter-content-box").delay(0).fadeOut(50);
        }
    );
});

//--------------------------------------------------/
// Trigger Content Box
//--------------------------------------------------/
$(document).ready(function () {
    var card = $("ul#dynamic-filter-list li");

    card.on("click", function () {
        $(".dynamic-filter-list-container > p, ul#dynamic-filter-list")
            .delay(0)
            .fadeOut(0);
    });
    if (window.matchMedia("(max-width: 767.9px)").matches) {
        card.on("click", function () {
            $(".filter-content-close-mobile").css("display", "inline-block");
        });
    }
});

//--------------------------------------------------/
// Copy button
//--------------------------------------------------/
$(document).ready(function () {
    const buttonIds = [
        '#PhoneCopyBtn',
        '#textCopyBtn',
        '#priceCopyBtn',
    ];

    buttonIds.forEach(function (id) {
        new ClipboardJS(id);
    });

    $(buttonIds.join(", ")).on("click", function () {
        $(this).text("ÄĂ£ sao chĂ©p");
    });
});