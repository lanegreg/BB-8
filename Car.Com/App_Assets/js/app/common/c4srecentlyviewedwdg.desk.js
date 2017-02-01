/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */


'use strict';

!function (win, $, abt, undefined) {
  //#region - Module level vars, minification enhancements, and DOM caching
  var env = abt.ENV
    , utils = abt.UTILS
    , suggestedC4sService = require('../../modules/abt-svc-c4s-suggested')
    , trk = abt.TRK
    , tmpls = require('./c4srecentlyviewedwdg_tmpls/desk/tmpls.js')
    , personaService = abt.PERSONA
    , cacheKey = 'c4s_recvwd'
    //, maxToInitiallyShow = 5
    , cache = abt.CACHE.storage(cacheKey)
    , $widgetContainer
    , true_bool = !0
    , false_bool = !true_bool
    , empty_string = ''
    , pipe_symbol = '|'
    , tilde_symbol = '~'
    , console = win.console
    , hasConsoleLogSupport = console && console.log
    , hash_symbol = '#'
    , forwardSlash_symbol = '/'
    , suggestedVehicles_text = 'suggested-vehicles'
    , search_results_page_path = forwardSlash_symbol + suggestedVehicles_text + '/results/'
    , click_event = 'click'
    , length = 'length'
    , show = 'show'
    , hide = 'hide'
    , makeid_prop = 'makeid'
    , make_prop = 'make'
    , model_prop = 'model'
    , makemodel_prop = "make_model"
    , zip_prop = 'zip'
    , maxtoshow_prop = 'maxshow'

  //#endregion



  //**** EXPOSE FOR DEBUGGING PURPOSES ONLY (should be commented out) ****
  //ABT.c4s = suggestedC4sService

  //#region - Scoped Variables
  var $navigationContainers
    , querystringObj
    , criteria
    , inventory
    , originalStateOfCriteria
    , originalStateOfInventory
    , originalInventoryCount
    , currentInventoryPointer = -1
    , isEnabled = false

  //#endregion


  //#region Private funcs

  var wireupButtonEvents = function () {

  //  $('#vwrec_wdg_showall_btn_js').on(click_event, function (e) {
  //    e.preventDefault()

  //    $widgetContainer.find('#c4s_recvwd_wdg_cars_wrap_js .vwrec_card_js')[show]()
  //    $(this)[hide]()
  //  })

    $('#c4s_wdg_checkAll').on(click_event, function (e) {
          $('#c4s_varray').find(':checkbox').each(function () {
            if ($('#c4s_wdg_checkAll').prop("checked") === true) {
              this.checked = true;
            }
            else {
              this.checked = false;
            }
          });
      });

      $('#c4s_wdg_getpricing_btn_js').on(click_event, function (e) {
        e.preventDefault()
         
        var $b = $('input[type=checkbox]')
        if ($b.filter(':checked').length > 0) {

          var arr = []

          $('#c4s_varray').find(':checkbox').each(function () {

            //console.log("CHECKED:  tag = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(0).attr("value"))

            var validDealerPhone = $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(6).attr("value")
            var hasValidDealerPhone

            if (validDealerPhone.length > 6 && validDealerPhone !== "undefined") {
              hasValidDealerPhone = true
            } else {
              hasValidDealerPhone = false
            }

            if ($(this).is(':checked')) {

              console.log("CHECKED:  vin = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(0).attr("value"))
              console.log("CHECKED:  carId = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(1).attr("value"))
              console.log("CHECKED:  make = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(2).attr("value"))
              console.log("CHECKED:  model = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(3).attr("value"))
              console.log("CHECKED:  year = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(4).attr("value"))
              console.log("CHECKED:  dealerId = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(5).attr("value"))
              console.log("CHECKED:  dealerPhone = " + validDealerPhone)
              console.log("CHECKED:  hasValidDealerPhone = " + hasValidDealerPhone)

              arr.push({
                vin: $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(0).attr("value"),
                id: parseInt($(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(1).attr("value")),
                make: $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(2).attr("value"),
                model: $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(3).attr("value"),
                year: $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(4).attr("value"),
                dealerId: $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(5).attr("value"),
                dealerPhone: validDealerPhone,
                hasValidPhoneNum: hasValidDealerPhone,
                quoteButtonSelected: true
              });

            } else {

              console.log("UNCHECKED:  vin = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(0).attr("value"))
              console.log("UNCHECKED:  carId = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(1).attr("value"))
              console.log("UNCHECKED:  make = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(2).attr("value"))
              console.log("UNCHECKED:  model = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(3).attr("value"))
              console.log("UNCHECKED:  year = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(4).attr("value"))
              console.log("UNCHECKED:  dealerId = " + $(this).parent().parent().nextAll().eq(0).children().eq(1).children().eq(5).attr("value"))
              console.log("UNCHECKED:  dealerPhone = " + validDealerPhone)
              console.log("UNCHECKED:  hasValidDealerPhone = " + hasValidDealerPhone)

            }

          });


          ABT.OFR.offerPopup(arr, true)
          //alert("PO Form Submmission Feature Under Construction");

        } else {

          alert("Please select at least one vehicle");

        }

        //    $widgetContainer.find('#c4s_recvwd_wdg_cars_wrap_js .vwrec_card_js')[show]()
        //    $(this)[hide]()

      })

  }

  var weHaveInventoryToPageThrough = function (d) {
    return d.inventory_count > 0
  }

  var getValidatedRecentlyViewed = function() {
    var recentlyViewed = cache.get(cacheKey) || { sessionId: 0, cars: [] }
      , currentSessionId = trk.sessionCtx.session.id

    if (recentlyViewed.sessionId !== currentSessionId) {
      recentlyViewed.sessionId = currentSessionId
      recentlyViewed.cars = []
    }

    return recentlyViewed
  }

  var _postCar = function (car) {
    var recentlyViewed = getValidatedRecentlyViewed()

    var uniques = recentlyViewed.cars.filter(function (item) {
      return item.id !== car.id
    })

    if (uniques[length] < recentlyViewed.cars[length]) {
      recentlyViewed.cars = uniques
    }

    recentlyViewed.cars.unshift(car)
    recentlyViewed.cars = recentlyViewed.cars.slice(0, 5)
 
    cache.set(cacheKey, recentlyViewed)
  }

  var _render = function () {
    var recentlyViewed = getValidatedRecentlyViewed()
    //, showTheMoreCarsBtn = maxToInitiallyShow < recentlyViewed.cars[length]
    //  var recentlyViewed = getValidatedRecentlyViewed()
    //, showTheMoreCarsBtn = maxToInitiallyShow < recentlyViewed.cars[length]

    //$widgetContainer.find('#vwrec_wdg_showall_btn_js')[showTheMoreCarsBtn ? show : hide]()

    $widgetContainer
      .find('#c4s_recvwd_wdg_cars_wrap_js')
      .empty()
      .append(tmpls.car_for_sale({ recentsPerPage: recentlyViewed.cars.length, cars: recentlyViewed.cars }))
    //.append(tmpls.car_for_sale({ maxToShow: maxToInitiallyShow, cars: recentlyViewed.cars }))

    var recentDealers = recentlyViewed.cars.map(function (car) { return car.dealerId })
    var dealerExclusion = recentDealers.join("|");

    if (recentlyViewed.cars.length > 0) {
      $.when(suggestedC4sService.isReady())
        .then(function () {
          inventory = suggestedC4sService.inventory
          criteria = suggestedC4sService.criteria
        })

      // Criteria and Filters

      suggestedC4sService.criteria.set({ zipcode: personaService.get().zipcode })
      suggestedC4sService.criteria.set({ vdp_price: recentlyViewed.cars[0].price })
      suggestedC4sService.criteria.set({ vdp_mileage: recentlyViewed.cars[0].mileage })
      //suggestedC4sService.criteria.set({ page: { items_per_page: (4) } })
      suggestedC4sService.criteria.set({ exclusions: { dealers: dealerExclusion } })
      suggestedC4sService.criteria.set({ filters: { make_models: recentlyViewed.cars[0].makeId + tilde_symbol + recentlyViewed.cars[0].model } })

      if (parseInt(recentlyViewed.cars[0].year - 1) < 1) {
        suggestedC4sService.criteria.set({ filters: { year_range: parseInt(recentlyViewed.cars[0].year) + pipe_symbol + (parseInt(recentlyViewed.cars[0].year) + 1) } })
      } else {
        suggestedC4sService.criteria.set({ filters: { year_range: (parseInt(recentlyViewed.cars[0].year) - 1) + pipe_symbol + (parseInt(recentlyViewed.cars[0].year) + 1) } })
      }

      //if (parseInt(recentlyViewed.cars[0].price - 1) < 1) {
      //  suggestedC4sService.criteria.set({ filters: { price_range: parseInt(recentlyViewed.cars[0].price) + pipe_symbol + (parseInt(recentlyViewed.cars[0].price) + 1) } })
      //} else {
      //  suggestedC4sService.criteria.set({ filters: { price_range: (parseInt(recentlyViewed.cars[0].price) - 1) + pipe_symbol + (parseInt(recentlyViewed.cars[0].price) + 1) } })
      //}

      //if (parseInt(recentlyViewed.cars[0].mileage - 1) < 1) {
      //  suggestedC4sService.criteria.set({ filters: { mileage_range: parseInt(recentlyViewed.cars[0].mileage) + pipe_symbol + (parseInt(recentlyViewed.cars[0].mileage) + 1) } })
      //} else {
      //  suggestedC4sService.criteria.set({ filters: { mileage_range: (parseInt(recentlyViewed.cars[0].mileage) - 1) + pipe_symbol + (parseInt(recentlyViewed.cars[0].mileage) + 1) } })
      //}

      suggestedC4sService.criteria.set({ sort: { by: "suggested_ranking" } })

      // save original state of criteria, so we can restore it later
      originalStateOfCriteria = $.extend(true, {}, criteria.get())

      $.when(suggestedC4sService.query())
        .then(function (data) {
          if (weHaveInventoryToPageThrough(data)) {
            originalStateOfInventory = $.extend(true, {}, data)
            var wdgCars = originalStateOfInventory.cars_for_sale.map(function (car) {
              return {
                dealerId: car.dealer.id,
                vin: car.vin,
                id: car.id,
                url: win.document.location.href,
                imgUrl: car.primary_image_url,
                make: car.make,
                makeId: car.makeId,
                model: car.model,
                year: car.year,
                city: car.dealer.city,
                state: car.dealer.state,
                price: car.price,
                mileage: car.mileage
              }   //  end return
            })    //  end if

            var carsWithSignificantPriceAndMileage = wdgCars.filter(function (item) {
              return item.mileage !== "0" && item.mileage !== "" && item.price !== "0" && item.price !== ""
            })

            carsWithSignificantPriceAndMileage = carsWithSignificantPriceAndMileage.slice(0, 4)

            $widgetContainer
              .find('#c4s_recvwd_wdg_cars_wrap_js')
              .append(tmpls.car_for_sale({ recentsPerPage: 0, cars: carsWithSignificantPriceAndMileage }))
              //.append(tmpls.car_for_sale({ recentsPerPage: 0, cars: wdgCars }))
          } //  end if
        })  //  end when
    } //  end if

  } //  end _render


  var _init = function () {
    $widgetContainer
      .empty()
      .append(tmpls.widget_wrap())
  }

  var _show = function () {
    wireupButtonEvents()
    $widgetContainer.show()
  }

  //#endregion


  var carsForSaleRecentlyViewedWidget = function () {

    return {

      create: function () {

        abt.WDG.set({
          c4s_viewedrecently: function () {

            return {
              init: function (config) {
                var wrapper = (config || {}).wrapper

                if (!wrapper) { // init by data-dash method
                  $widgetContainer = $('[data-widget^=c4s-recvwd]')
                }
                else if (wrapper.jquery) { // init by jquery object method
                  $widgetContainer = wrapper
                }
                else if (typeof wrapper === 'string') { // init by element id method
                  $widgetContainer = $(hash_symbol + wrapper)
                }

                // Make sure we have been properly initialized.
                if (!$widgetContainer[0]) {
                  if (hasConsoleLogSupport) {
                    console.log(utils.WDG_ERR_MSG)
                  }

                  return
                }

                var recentlyViewedCars = getValidatedRecentlyViewed()

                if (recentlyViewedCars.cars.length === 0) {
                  return
                }

                _init()
                _render()
                _show()
              },

              render: function() {
                _render()

                return this
              },


              postCar: function (car) {
                _postCar(car)

                return this
              }
            }
          }()
        })
      }
    }
  }()

  carsForSaleRecentlyViewedWidget.create()

}(window, jQuery, ABT, []._)