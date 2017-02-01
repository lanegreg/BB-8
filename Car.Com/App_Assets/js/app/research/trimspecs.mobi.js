/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */

//var tmpls = require('./{templates_path}/{device_group}/tmpls.js')

(function (win, $, abt) {
  $(function () {
    var env = abt.ENV,
      car = abt.pageJson.vehicleinfo,
      getJsFileNameByAlias = env.getJsFileNameByAlias,
      getScriptUriByName = env.getScriptUriByName;

    var jsTrimSpecsInit = {

      init: function () {
        jsTrimSpecsInit.initCarsForSaleWidget(),
        jsTrimSpecsInit.initViewedRecentlyWidget()
      },

      initCarsForSaleWidget: function () {
        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4swidget'))))
        .then(function () {
          abt.WDG.c4s.init({ make: car.make, model: car.model })
        })
      },

      initViewedRecentlyWidget: function () {
        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('viewedrecentlywidget'))))
        .then(function () {
          abt.WDG.viewedrecently.init({ currentTrim: car.trimid })
        })
      }

    } // end jsTrimOverviewInit

    $(function () {
      jsTrimSpecsInit.init();
    });

  });
})(window, jQuery, ABT)