/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */

//var tmpls = require('./{templates_path}/{device_group}/tmpls.js')

(function (win, $) {
  $(function () {

    // Add page code here

    // Remove category block that received no data in DOM. We need to do this for nth-child rules to work. 
    $('.secondary-arts:hidden').remove();

    // Find tallest category/attr block in each row and append the height value to each block in the correspoding row. The results are equal height blocks in each row and looks neat.
    $(function () {
      var $list = $('.flex-grid-cont'),
          $items = $list.find('.secondary-arts'),
          setHeights = function () {
            $items.css('height', 'auto');

            var perRow = Math.floor($list.width() / $items.width());
            if (perRow == null || perRow < 2) return true;

            for (var i = 0, j = $items.length; i < j; i += perRow) {
              var maxHeight = 0,
                $row = $items.slice(i, i + perRow);

              $row.each(function () {
                var itemHeight = parseInt($(this).outerHeight());
                if (itemHeight > maxHeight) maxHeight = itemHeight;
              });
              $row.css('height', maxHeight);
            }
          };

      setHeights();
      $(window).on('resize', setHeights);
    });

    var attrArts = $('.secondary-arts[data-group="more"]');
    // Hide category blcoks that are in "More" category group. 
    attrArts.slideUp();

    // Click to display more category blocks and change the button text to "Less Category". Click again to display less category blocks and change the button text to "More Category". 
    $('#show_hide_more_cateogories_js').click(function () {
      var clicks = $(this).data('clicks');
      var el = $(this);

      if (!clicks) {
        $(attrArts).slideDown();
        el.text(el.data('text-swap'));
      } else {
        $(attrArts).slideUp();

        $('html, body').animate({
          scrollTop: $('#category_block').offset().top
      }, 500);

        el.text(el.data('text-original'));
      }
      $(this).data("clicks", !clicks);
    });
  });
})(window, jQuery)