/**
 *  This js module represents the mobile version of site.index http://www.autobytel.com/
 */


(function (win, $) {
  $(function () {
    var abt = ABT,
        personaService = abt.PERSONA;

    var jsSiteHomeIndex = {

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
        jsSiteHomeIndex.initSelectedAndDefaultBtns();
      } // End init function
    };

    $(function () {
      $.when(
        personaService.isReady())
      .then(function () {
        jsSiteHomeIndex.init();
      })
    });

  });
})(window, jQuery)