/**
 *  This js module represents the tablet version of {controller}.{action} http://www.autobytel.com/{path}
 */


'use strict';

!function (win, $, abt, undefined) {
  //#region - Module level vars, minification enhancements, and DOM caching
  var utils = abt.UTILS
    , carsforsaleService = require('../../modules/abt-svc-c4s')
    , tmpls = require('./c4swidget_tmpls/desk/tmpls.js')
    , personaService = abt.PERSONA
    , extend = $.extend
    , true_bool = !0
    , false_bool = !true_bool
    , console = win.console
    , hasConsoleLogSupport = console && console.log
    , hash_symbol = '#'
    , dot_symbol = '.'
    , tilde_symbol = '~'
    , click_event = 'click'
    , submit_event = 'submit'
    , change_event = 'change'
    , length = 'length'
    , show = 'show'
    , hide = 'hide'
    , fadeIn = 'fadeIn'


  //#endregion


  //**** EXPOSE FOR DEBUGGING PURPOSES ONLY (should be commented out) ****
  //ABT.c4s = carsforsaleService



  var carsForSaleWidget = function (widgetPrefix) {
    //#region - Module level vars, minification enhancements, and DOM caching
    var criteria = carsforsaleService.criteria
      , $widgetContainer
      , $zipNotFound
      , $zipFound
      , $zipInput
      , $alterZipLink
      , $carsForSaleContainer
      , $closeAlterZipButton
      , $makeModelDisplays
      , $viewAllBtn
      , js_suffix = '_js'
      , zip_prefix = 'zip_'
      , form = 'form'
      , makeid_prop = 'makeid'
      , make_prop = 'make'
      , model_prop = 'model'
      , zip_prop = 'zip'
      , maxtoshow_prop = 'maxshow'

    //#endregion


    //#region - Private funcs

    var queryMetadata = function () {
      var zip = ''
        , makeId = ''
        , make = ''
        , model = ''
        , maxToShow = 3


      return {
        DEFAULT_MAX_TO_SHOW: 3,

        hasZip: function () {
          return utils.isZip(zip)
        },

        hasMake: function () {
          return utils.isInt(makeId) && makeId > 0 && !!make[length]
        },

        hasMakeAndModel: function () {
          return this.hasMake() && !!model[length]
        },

        set: function (prop, val) {

          if (prop === make_prop) {
            if (!utils.isInt(+val.id) || !val.name[length])
              return this

            makeId = +val.id
            make = val.name
            model = ''
          }

          else if (prop === model_prop) {
            model = this.hasMake() ? val : ''
          }

          else if (prop === zip_prop) {
            if (utils.isZip(val)) {
              zip = val
            }
          }

          else if (prop === maxtoshow_prop) {
            if (utils.isInt(+val)) {
              maxToShow = val
            }
          }

          return this
        },

        get: function (prop) {
          if (prop === makeid_prop) {
            return makeId
          }

          if (prop === make_prop) {
            return make
          }

          else if (prop === model_prop) {
            return model
          }

          else if (prop === zip_prop) {
            return zip
          }

          else if (prop === maxtoshow_prop) {
            return maxToShow
          }

          return undefined
        }
      }
    }()


    var _query = function () {

      // no make or zip, no render!
      if (!queryMetadata.hasMake() || !queryMetadata.hasZip()) {
        return
      }

      criteria.set(extend(true_bool, {}, criteria.default,
        {
          zipcode: queryMetadata.get(zip_prop),
          radius_miles: 100,

          page: {
            items_per_page: queryMetadata.get(maxtoshow_prop)
          },

          sort: {
            by: 'distance',
            dir: 'asc'
          },

          filters: {
            makes: !queryMetadata.hasMakeAndModel() ? queryMetadata.get(makeid_prop) : '',
            make_models: queryMetadata.hasMakeAndModel() ? queryMetadata.get(makeid_prop) + tilde_symbol + queryMetadata.get(model_prop) : ''
          }
        }))

      $.when(carsforsaleService.query())
        .then(function () {
          criteria.restore()
        })
    }


    var _init = function () {

      $widgetContainer.append($(tmpls.widget_wrap()))

      //#region - Cache DOM elements

      $zipNotFound = $widgetContainer.find(hash_symbol + widgetPrefix + zip_prefix + 'notfound' + js_suffix)
      $zipFound = $widgetContainer.find(hash_symbol + widgetPrefix + zip_prefix + 'found' + js_suffix)
      $zipInput = $zipNotFound.find(hash_symbol + widgetPrefix + zip_prefix + 'inp' + js_suffix)
      $carsForSaleContainer = $widgetContainer.find(hash_symbol + widgetPrefix + 'cars_wrap' + js_suffix)
      $alterZipLink = $widgetContainer.find(hash_symbol + widgetPrefix + zip_prefix + 'alter_lnk' + js_suffix)
      $closeAlterZipButton = $widgetContainer.find(hash_symbol + widgetPrefix + zip_prefix + 'close_btn' + js_suffix)
      $makeModelDisplays = $widgetContainer.find(dot_symbol + widgetPrefix + 'makemodel_display' + js_suffix)
      $viewAllBtn = $widgetContainer.find(hash_symbol + widgetPrefix + 'viewall_btn' + js_suffix)

      //#endregion


      //#region - Initialize some UI state

      queryMetadata.set(zip_prop, personaService.get().zipcode)

      if (queryMetadata.hasZip()) {
        $zipNotFound[hide]()
        $zipFound[show]()
      }
      else {
        $zipNotFound[show]()
        $zipFound[hide]()
      }

      $carsForSaleContainer[hide]()

      //#endregion


      //#region - Setup event handlers for both $wrapperFrag elements, and for carsforsaleService inventory change.

      // setup the |click-and-alter-the-zip link| event
      $alterZipLink.on(click_event, function (e) {
        e.preventDefault()

        $zipFound[hide]()
        $makeModelDisplays.text(function () {
          var makeModelBlurb = queryMetadata.get(make_prop) + (queryMetadata.hasMakeAndModel() ? ' ' + queryMetadata.get(model_prop) : '')
          //TODO: we need to find a better way to pluralize this (like, deriving from PVC tables)
          return makeModelBlurb + (makeModelBlurb.slice(-1) !== 's' ? 's' : 'es') // pluralize
        })
        $zipNotFound[fadeIn]()
        $zipInput.val('').focus()
      })

      // setup the |close-the-alter-the-zip box| event
      $closeAlterZipButton.on(click_event, function () {
        $zipNotFound[hide]()
        $zipFound[fadeIn]()
      })


      // setup the |submit-zip| form event
      $zipNotFound.find(form).on(submit_event, function (e) {
        e.preventDefault()

        var zip = $zipInput.val()

        if (utils.isZip(zip)) {
          queryMetadata.set(zip_prop, zip)
          personaService.set('zipcode', zip)

          $zipNotFound[hide]()
          $carsForSaleContainer[hide]().empty()
          $zipFound[fadeIn]()
          _show()
        }
        else {
          $zipInput.focus()
          return
        }
      })


      // setup the |view-all| event
      $viewAllBtn.on(click_event, function () {
        criteria.set(extend(true_bool, {}, criteria.default,
          {
            zipcode: queryMetadata.get(zip_prop),

            filters: {
              makes: !queryMetadata.hasMakeAndModel() ? queryMetadata.get(makeid_prop) : '',
              make_models: queryMetadata.hasMakeAndModel() ? queryMetadata.get(makeid_prop) + tilde_symbol + queryMetadata.get(model_prop) : ''
            }
          }))

        win.location.href = '/cars-for-sale/results/'
      })


      // setup the |inventory-changed| event
      carsforsaleService.inventory.on(change_event, function (e) {
        var data = e.args

        $makeModelDisplays.text(function () {
          var makeModelBlurb = queryMetadata.get(make_prop) + (queryMetadata.hasMakeAndModel() ? ' ' + queryMetadata.get(model_prop) : '')
          //TODO: we need to find a better way to pluralize this (like, deriving from PVC tables)
          return data.inventory_count + ' ' + makeModelBlurb + (makeModelBlurb.slice(-1) !== 's' ? 's' : 'es') // pluralize
        })

        $alterZipLink.text(queryMetadata.get(zip_prop))

        $carsForSaleContainer.empty()
          .append(tmpls.car_for_sale(data))[show]()
      })

      //#endregion
    }


    var _show = function () {

      $widgetContainer[show]();

      var zipcode = personaService.get().zipcode;

      if (utils.isZip(zipcode)) {

        carsforsaleService.ping({ "make": queryMetadata.make, "model": queryMetadata.model, "zipcode": zipcode })
          .then(function (data) {
            if (data.availability.new_cars.by_make.count > 0 || data.availability.used_cars.by_make.count) {
              if (data.availability.new_cars.by_make_model.count + data.availability.used_cars.by_make_model.count > 1) {
                queryMetadata
                  .set(make_prop, { id: queryMetadata.makeId, name: queryMetadata.make })
                  .set(model_prop, queryMetadata.model || '')
                  .set(maxtoshow_prop, (queryMetadata.DEFAULT_MAX_TO_SHOW))
              } else {
                queryMetadata
                  .set(make_prop, { id: queryMetadata.makeId, name: queryMetadata.make })
                  .set(maxtoshow_prop, (queryMetadata.DEFAULT_MAX_TO_SHOW))
              }
            }



            _query()
          });

      } else {

        _query()
      }


      //if (!queryMetadata.hasMake() || !queryMetadata.hasZip())
      //  return



    }

    //#endregion


    return {

      create: function () {

        abt.WDG.set({
          c4s: function () {
            return {
              init: function (config) {
                $.when(personaService.isReady(), carsforsaleService.isReady())
                  .then(function () {
                    var wrapper = config.wrapper

                    if (!wrapper) { // init by data-dash method
                      $widgetContainer = $('[data-widget^=c4s]')
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

                    // Make sure we at least have make or makeid.
                    if (!config.makeid && !config.make)
                      return


                    // resolve makeid and make
                    $.when(carsforsaleService.getFilterDomains())
                      .done(function (filterDomains) {

                        var makeObj = filterDomains.makes.filter(function (item) {
                          if (config.make) {
                            return item.descr === config.make
                          }
                          else if (config.makeid) {
                            return item.matchVal === config.makeid
                          }

                          return false_bool
                        })

                        // invalid make, no render!
                        if (!makeObj[length]) {
                          return
                        }
                        else {
                          config.makeid = makeObj[0].matchVal
                          config.make = makeObj[0].descr
                        }

                        _init()

                        queryMetadata.make = config.make;
                        queryMetadata.makeId = config.makeid;
                        queryMetadata.model = config.model;

                        _show()
                      })
                  })
              },

              render: function (config) {
                // TODO: This is stubbed here so we can implement it later.
                // This method will allow us to dynamically change the context, and re-render different content.
              }
            }
          }()
        })
      }
    }
  }('c4s_wdg_')

  carsForSaleWidget.create()

}(window, jQuery, ABT, []._)

//#region - Usage of API
/**

EX 1: (by passing in the _id_ of the wrapper element)

    $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4swidget'))))
      .then(function() {
        ABT.WDG.c4s.init({ wrapper: 'c4s_wdg_wrap', make: car.make, model: car.model, maxToShow: 4 })
      })

EX 2: (by passing in a jquery object of the wrapper element)

    $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4swidget'))))
      .then(function() {
        ABT.WDG.c4s.init({ wrapper: $('#c4s_wdg_wrap'), make: car.make, model: car.model, maxToShow: 4 })
      })

EX 3: (by implicitly assigning a data-widget="c4s" on wrapper element)

    $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4swidget'))))
      .then(function() {
        ABT.WDG.c4s.init({ make: car.make, model: car.model, maxToShow: 4 })
      })

 */
//#endregion