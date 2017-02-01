/**
 *  This js module represents the mobile version of CarsForSale.Make http://www.car.com/cars-for-sale/{make}/
 */


'use strict';

(function (win, $) {
  var page = function () {
    //#region - Module level vars, minification enhancements, and DOM caching
    var carsforsaleService = require('../../modules/abt-svc-c4s')
      , criteria = carsforsaleService.criteria
      , click_event = 'click'
      , anchor_elem = 'a'
      , id_text = 'id'
      , empty = ''

    //#endregion


    return {
      init: function () {

        $.when(carsforsaleService.isReady())
          .then(function () {
            $('#models_js').on(click_event, anchor_elem, function () {
              criteria.set({
                filters: {
                  makes: empty,
                  make_models: $(this).data(id_text) + empty
                }
              })
            })
          })
      }
    }
  }()


  $(function () {
    page.init()
  })
})(window, jQuery)