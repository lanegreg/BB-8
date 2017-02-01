/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */

//var tmpls = require('./{templates_path}/{device_group}/tmpls.js')

(function (win, $) {
  $(function () {
    // Detect vehicle category article block height
    var $this = $('#js_fill_height');
    var $currentHeight = $this.prev('li').find('.img-container').outerHeight() + 'px';
    $this.css({ "height": $currentHeight });
  });
})(window, jQuery)