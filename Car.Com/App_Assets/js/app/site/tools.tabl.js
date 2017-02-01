/**
 *  This js module represents the tablet version of {controller}.{action} http://www.autobytel.com/{path}
 */


(function (win, $) {
  $(function () {
    var abt = ABT,
        personaService = abt.PERSONA;

    var jsSiteToolsIndex = {

      initSelectedAndDefaultBtns: function () {
        $('#popCompareBtn1').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "33210,34261" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn2').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "34198,32437" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn3').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "33802,34217" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn4').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "34078,34034" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn5').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "33023,32549" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn6').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "32397,32718" });
          window.location = '/tools/car-comparison/results/';
        });

        return true;
      },

      init: function () {
        jsSiteToolsIndex.initSelectedAndDefaultBtns();
      } // End init function
    };

    $(function () {
      $.when(
        personaService.isReady())
      .then(function () {
        jsSiteToolsIndex.init();
      })
    });

  });
})(window, jQuery)