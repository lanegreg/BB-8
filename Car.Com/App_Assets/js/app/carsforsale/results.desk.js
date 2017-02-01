
'use strict';

(function (win, $, abt, undefined) {
  var page = function () {
    //#region - Module level vars, minification enhancements, and DOM caching
    var env = abt.ENV
      , utils = abt.UTILS
      , trk = abt.TRK
      , ads = abt.ADS
      , getJsFileNameByAlias = env.getJsFileNameByAlias
      , getScriptUriByName = env.getScriptUriByName
      , pageJson = abt.pageJson
      , libsPath = env.libsPath
      , isInt = utils.isInt
      , isZip = utils.isZip
      , zipify = utils.zipify
      , tmpls = require('./results_tmpls/desk/tmpls.js')
      , carsforsaleService = require('../../modules/abt-svc-c4s')

      , masonry = require('../../libs/salvattore-1.0.8.min.js')
      //salvattore-1.0.8.min.js - library throws a bunch of errors in gulp: "Expected an assignment or function call..." 

      , packify = require('../../modules/abt-packify')
      , personaService = abt.PERSONA
      , Eventify = abt.Eventify
      , criteria = carsforsaleService.criteria
      , inventory = carsforsaleService.inventory
      , isArray = Array.isArray
      , setTimeout = win.setTimeout
      , location = win.location
      , extend = $.extend
      , JSON = win.JSON
      , jsonStringify = JSON.stringify
      , jsonParse = JSON.parse
      , noop = abt.noop
      , lzCompress = noop
      , lzDecompress = noop
      , true_bool = !0
      , false_bool = !true_bool
      , undefined_str = 'undefined'
      , change_event = 'change'
      //, ready_event = 'ready'
      , click_event = 'click'
      , focus_event = 'focus'
      , blur_event = 'blur'
      , submit_event = 'submit'
      , dot_validate = '.validate'
      , input_modified_events = 'keyup' + dot_validate + ' mouseup' + dot_validate + ' input' + dot_validate + ' paste' + dot_validate
      , form_elem = 'form'
      , svg_elem = 'svg'
      , button_elem = 'button'
      , ul_elem = 'ul'
      , li_elem = 'li'
      , span_elem = 'span'
      , div_tag = '<div>'
      , selected_str = 'selected'
      , selected_class = selected_str
      , show = 'show'
      , hide = 'hide'
      , checked = 'checked'
      , disabled = 'disabled'
      , category_type = 'cat'
      , make_type = 'ma'
      , makeModel_type = 'mamo'
      , categoryMake_type = 'cama'
      , selected_buttons = 'li[class=' + selected_class + ']'
      , id_text = 'id'
      , type_text = 'type'
      , value_text = 'value'
      , tab_text = 'tab'
      , all_text = 'All'
      , empty_str = ''
      , space = ' '
      , length_str = 'length'
      , pipeSeparator = '|'
      , tildeSeparator = '~'
      , underscore = '_'
      , dot_symbol = '.'
      , hash_symbol = '#'
      , js_suffix = '_js'
      , lbl_suffix = '_lbl'
      , btn_suffix = '_btn'
      , close_btn_class = '.close' + btn_suffix + js_suffix
      , pop_btn_suffix = '_pop' + btn_suffix
      , tab_lbl_suffix = '_tab' + lbl_suffix
      , overlay_suffix = '_overlay'
      , wrap_suffix = '_wrap'
      , range_suffix = '_range'
      , $body = $('body')
      , paginatorTF = false

    //#endregion


    //**** EXPOSE FOR DEBUGGING PURPOSES ONLY (should be commented out) ****
    //ABT.c4s = carsforsaleService


    //#region - Private funcs


    var updateButtonState = function ($btn, select) {
      // if select state was not provided, treat it like a toggle.
      if (typeof select === undefined_str) {
        select = !$btn.hasClass(selected_class)
      }

      if (select) {
        $btn.addClass(selected_class)
      }
      else {
        $btn.removeClass(selected_class)
      }
    }

    var syncAllButtonStatesWithModel = function ($container, arr) {
      arr.forEach(function (item) {
        var $btn = $container.find('[data-id=' + item.id + ']')
        updateButtonState($btn, item[selected_str])
      })
    }

    //#endregion


    //#region - Common Models

    var commonModels = function () {

      var defaultModelItem = {
        selected: false_bool,
        toggleState: function () {
          this[selected_str] = !this[selected_str]
        }
      }

      //#region - Private funcs

      var syncSingleValueModelToPipedRange = function (arr, pipedRange) {
        var splitends = (pipedRange || empty_str).split(pipeSeparator)
          , lowend = +splitends[0]
          , highend = +splitends[1]

        arr.forEach(function (item) {
          item[selected_str] = (item.matchVal >= lowend && item.matchVal <= highend)
        })
      }

      var syncPipedValueModelToPipedRange = function (arr, pipedRange) {
        var splitends = (pipedRange || empty_str).split(pipeSeparator)
          , lowend = +splitends[0]
          , highend = +splitends[1]

        arr.forEach(function (item) {
          var lo2Hi = item.matchVal.split(pipeSeparator)
          item[selected_str] = (+lo2Hi[0] >= lowend && +lo2Hi[1] <= highend)
        })
      }

      var syncModelToPipedList = function (arr, pipedList) {
        var matchValues = (pipedList || empty_str).split(pipeSeparator)

        if (matchValues[0] === empty_str) {
          arr.forEach(function (item) {
            item[selected_str] = false_bool
          })
        }
        else {
          arr.forEach(function (item) {
            item[selected_str] = ($.inArray(item.matchVal, matchValues) > -1)
          })
        }
      }

      var stringifyModelToRange = function (model) {
        var selecteds = model.filter(function (item) {
          return item[selected_str]
        })

        var startValue = selecteds.slice(0, 1)[0].matchVal,
            endValue = selecteds.slice(-1)[0].matchVal

        var matchValue = empty_str.concat(
          (startValue.indexOf(pipeSeparator) === -1) ? startValue : startValue.split(pipeSeparator)[0],
          pipeSeparator,
          (endValue.indexOf(pipeSeparator) === -1) ? endValue : endValue.split(pipeSeparator)[1]
        )

        return matchValue
      }

      var stringifyModelToList = function (model) {
        var selecteds = model.filter(function (item) {
          return item[selected_str]
        })

        if (!selecteds[length_str]) {
          return empty_str
        }

        var matchValues =
          selecteds
            .map(function (item) {
              return item.matchVal
            })
            .join(pipeSeparator)

        return matchValues
      }

      //#endregion


      return {

        init: function (filterDomains) {
          this.priceRange.init(filterDomains.priceRanges)
          this.mileageRange.init(filterDomains.mileageRanges)
          this.yearRange.init(filterDomains.years)
          this.fuelTypes.init(filterDomains.fuelTypes)
          this.options.init(filterDomains.options)
        },

        priceRange: function () {
          var changeDelegate = new Eventify()
            , model


          return {

            init: function (priceRanges) {
              model = priceRanges
                .map(function (obj, idx) {
                  var item = extend({}, defaultModelItem, { id: idx })
                  item[selected_str] = true_bool

                  return extend({}, obj, item)
                })

              return this
            },

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return stringifyModelToRange(model)
            },

            sync: function (pipedRange) {
              syncPipedValueModelToPipedRange(model, pipedRange)
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        mileageRange: function () {
          var changeDelegate = new Eventify()
            , model


          return {

            init: function (mileageRanges) {
              model = mileageRanges
                .map(function (obj, idx) {
                  var item = extend({}, defaultModelItem, { id: idx })
                  item[selected_str] = true_bool

                  return extend({}, obj, item)
                })

              return this
            },

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return stringifyModelToRange(model)
            },

            sync: function (pipedRange) {
              syncPipedValueModelToPipedRange(model, pipedRange)
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        yearRange: function () {
          var changeDelegate = new Eventify()
            , model


          return {

            init: function (years) {
              model = years
                .map(function (obj, idx) {
                  var item = extend({}, defaultModelItem, { id: idx })
                  item[selected_str] = true_bool

                  return extend({}, obj, item)
                })

              return this
            },

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return stringifyModelToRange(model)
            },

            sync: function (pipedRange) {
              syncSingleValueModelToPipedRange(model, pipedRange)
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        fuelTypes: function () {
          var changeDelegate = new Eventify()
            , model


          return {

            init: function (fuelTypes) {
              model = fuelTypes
                .map(function (obj, idx) {
                  var item = extend({}, defaultModelItem, { id: idx })

                  return extend({}, obj, item)
                })

              return this
            },

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return stringifyModelToList(model)
            },

            sync: function (pipedList) {
              syncModelToPipedList(model, pipedList)
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        options: function () {
          var changeDelegate = new Eventify()
            , model


          return {

            init: function (options) {
              model = options
                .map(function (obj, idx) {
                  var item = extend({}, defaultModelItem, { id: idx })

                  return extend({}, obj, item)
                })

              return this
            },

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return stringifyModelToList(model)
            },

            sync: function (pipedList) {
              syncModelToPipedList(model, pipedList)
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        categories: function () {
          var changeDelegate = new Eventify()
            , model = []


          return {

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return model.join(pipeSeparator)
            },

            sync: function (pipedList) {
              model = pipedList[length_str] ? pipedList.split(pipeSeparator) : []
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        categoryMakes: function () {
          var changeDelegate = new Eventify()
            , model = []


          return {

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return model.join(pipeSeparator)
            },

            sync: function (pipedList) {
              model = pipedList[length_str] ? pipedList.split(pipeSeparator) : []
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        makes: function () {
          var changeDelegate = new Eventify()
            , model = []


          return {

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return model.join(pipeSeparator)
            },

            sync: function (pipedList) {
              model = pipedList[length_str] ? pipedList.split(pipeSeparator) : []
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }(),

        makeModels: function () {
          var changeDelegate = new Eventify()
            , model = []


          return {

            get: function () {
              return model
            },

            set: function (arr) {
              if (isArray(arr)) {
                model = arr
                changeDelegate.trigger(model)
              }
            },

            stringify: function () {
              return model.join(pipeSeparator)
            },

            sync: function (pipedList) {
              model = pipedList[length_str] ? pipedList.split(pipeSeparator) : []
              changeDelegate.trigger(model)
            },

            on: function (event, callback) {
              if (event === change_event)
                changeDelegate.register(callback)
            }
          }
        }()
      }
    }()

    //#endregion


    //#region - Page Components 

    var pageComponents = function () {
      //#region - Scoped Variables
      var filterDomains
        , $priceRange
        , $mileageRange
        , $yearRange
        , display_class = '.display'

      //#endregion


      return {
        initAll: function (domains) {
          filterDomains = domains
          this.priceRange.init()
          this.mileageRange.init()
          this.yearRange.init()
          this.sortBy.init()
          this.carsForSale.init()
          this.paginator.init()
          this.tagLister.init()
        },

        priceRange: function (componentName, model) {

          return {
            init: function () {

              $('#price_menu_js').click(function () {
                if ($(this).parent().next('.filter-pane').is(':visible')) {

                  $('.menu-label').removeClass('selected');
                  $(this).parent().next('.filter-pane').slideUp("fast");

                } else {

                  $('.filter-pane').hide();
                  $('.menu-label').removeClass('selected');
                  $(this).toggleClass('selected');
                  $(this).parent().next('.filter-pane').slideDown("fast");

                }
              });

              $('.prices_js').click(function () {

                $('#prices_min_textbox_js').val("");
                $('#prices_max_textbox_js').val("");

                $('.prices_js').prop('checked', false);
                $(this).prop('checked', true);

                var selectedCheckboxLabelText = $('#label_' + this.id).text();

                //strip off (qty) string for filterbar label
                var positionOfLeftParens = selectedCheckboxLabelText.indexOf("(");
                if (positionOfLeftParens > 0) {
                  selectedCheckboxLabelText = selectedCheckboxLabelText.slice(0, positionOfLeftParens);
                }

                $('#price_menu_selected_value_js').text(selectedCheckboxLabelText);

              });

              $('#button_prices_apply_js').click(function () {
                var pipeDelimitedRangePrice = "0|0";
                var minTextboxValuePrice = $('#prices_min_textbox_js').val();
                var maxTextboxValuePrice = $('#prices_max_textbox_js').val();
                var pricesCheckboxes = $('.prices_js');

                var minSearchValueCleanedPrice = minTextboxValuePrice.replace(/[^\d]+/g, "");
                var maxSearchValueCleanedPrice = maxTextboxValuePrice.replace(/[^\d]+/g, "");
                if (minSearchValueCleanedPrice.length > 3) {
                  minSearchValueCleanedPrice = Math.round(minSearchValueCleanedPrice / 1000).toString();
                }
                if (maxSearchValueCleanedPrice.length > 3) {
                  maxSearchValueCleanedPrice = Math.round(maxSearchValueCleanedPrice / 1000).toString();
                }

                //did the user enter a value into either of the textboxes?
                if (minTextboxValuePrice.length > 0 || maxTextboxValuePrice.length > 0) {
                  if (minSearchValueCleanedPrice.length > 0 && maxSearchValueCleanedPrice.length > 0) {
                    pipeDelimitedRangePrice = minSearchValueCleanedPrice + "|" + maxSearchValueCleanedPrice;
                  } else if (minSearchValueCleanedPrice.length === 0 && maxSearchValueCleanedPrice.length > 0) {
                    pipeDelimitedRangePrice = "0|" + maxSearchValueCleanedPrice;
                  } else if (minSearchValueCleanedPrice.length > 0 && maxSearchValueCleanedPrice.length === 0) {
                    //if they didn't enter a high value, fetch the high value from the "All Prices" value string
                    var highValueForRange = pricesCheckboxes[0].value.split("|")[1];
                    pipeDelimitedRangePrice = minSearchValueCleanedPrice + "|" + highValueForRange;
                  }
                  var priceMenuSelectedValue = "$" + pipeDelimitedRangePrice.split("|")[0] + "K - $" + pipeDelimitedRangePrice.split("|")[1] + "K";
                  $('#price_menu_selected_value_js').text(priceMenuSelectedValue);
                  $('.prices_js').prop('checked', false);
                }

                if (pipeDelimitedRangePrice === "0|0") {
                  for (var i = 0; i < pricesCheckboxes.length; i++) {
                    if (pricesCheckboxes[i].checked === true) {
                      pipeDelimitedRangePrice = pricesCheckboxes[i].value;
                    }
                  }
                }

                $('.filter-pane').slideUp("fast");

                var arrItem = {};
                arrItem.descr = "selected price range";
                arrItem.id = 0;
                arrItem.matchVal = pipeDelimitedRangePrice;
                arrItem.selected = true;
                var modelArray = new Array(arrItem);
                model.set(modelArray);

                var maxMileage = $('#results_first_load_js').data("maxmileage");
                var maxPrice = $('#results_first_load_js').data("maxprice");
                criteria.set({ max_mileage: maxMileage }, true_bool);
                criteria.set({ max_price: maxPrice }, true_bool);
                criteria.set({ filters: { price_range: pipeDelimitedRangePrice } }, true_bool);
                carsforsaleService.query();

              });

            }
          }
        }('price', commonModels.priceRange),

        mileageRange: function (componentName, model) {

          return {
            init: function () {

              $('#mileage_menu_js').click(function () {
                if ($(this).parent().next('.filter-pane').is(':visible')) {

                  $('.menu-label').removeClass('selected');
                  $(this).parent().next('.filter-pane').slideUp("fast");

                } else {

                  $('.filter-pane').hide();
                  $('.menu-label').removeClass('selected');
                  $(this).toggleClass('selected');
                  $(this).parent().next('.filter-pane').slideDown("fast");

                }
              });

              $('.mileage_js').click(function () {

                $('#mileage_min_textbox_js').val("");
                $('#mileage_max_textbox_js').val("");

                $('.mileage_js').prop('checked', false);
                $(this).prop('checked', true);

                var selectedCheckboxLabelText = $('#label_' + this.id).text();

                //strip off (qty) for filterbar
                var positionOfLeftParens = selectedCheckboxLabelText.indexOf("(");
                if (positionOfLeftParens > 0) {
                  selectedCheckboxLabelText = selectedCheckboxLabelText.slice(0, positionOfLeftParens);
                }

                $('#mileage_menu_selected_value_js').text(selectedCheckboxLabelText);

              });

              $('#button_mileage_apply_js').click(function () {

                var pipeDelimitedRangeMileage = "0|0";
                var minTextboxValueMileage = $('#mileage_min_textbox_js').val();
                var maxTextboxValueMileage = $('#mileage_max_textbox_js').val();
                var mileageCheckboxes = $('.mileage_js');

                var minSearchValueCleanedMileage = minTextboxValueMileage.replace(/[^\d]+/g, "");
                var maxSearchValueCleanedMileage = maxTextboxValueMileage.replace(/[^\d]+/g, "");
                if (minSearchValueCleanedMileage.length > 3) {
                  minSearchValueCleanedMileage = Math.round(minSearchValueCleanedMileage / 1000).toString();
                }
                if (maxSearchValueCleanedMileage.length > 3) {
                  maxSearchValueCleanedMileage = Math.round(maxSearchValueCleanedMileage / 1000).toString();
                }

                //did the user enter a value into either of the textboxes?
                if (minTextboxValueMileage.length > 0 || maxTextboxValueMileage.length > 0) {
                  if (minSearchValueCleanedMileage.length > 0 && maxSearchValueCleanedMileage.length > 0) {
                    pipeDelimitedRangeMileage = minSearchValueCleanedMileage + "|" + maxSearchValueCleanedMileage;
                  } else if (minSearchValueCleanedMileage.length === 0 && maxSearchValueCleanedMileage.length > 0) {
                    pipeDelimitedRangeMileage = "0|" + maxSearchValueCleanedMileage;
                  } else if (minSearchValueCleanedMileage.length > 0 && maxSearchValueCleanedMileage.length === 0) {
                    //if they didn't enter a high value, fetch the high value from the "Any Mileages" value string
                    var highValueForRange = mileageCheckboxes[0].value.split("|")[1];
                    pipeDelimitedRangeMileage = minSearchValueCleanedMileage + "|" + highValueForRange;
                  }
                  var priceMileageSelectedValue = pipeDelimitedRangeMileage.split("|")[0] + "K - " + pipeDelimitedRangeMileage.split("|")[1] + "K miles";
                  $('#mileage_menu_selected_value_js').text(priceMileageSelectedValue);
                  $('.mileage_js').prop('checked', false);
                }

                if (pipeDelimitedRangeMileage === "0|0") {
                  for (var i = 0; i < mileageCheckboxes.length; i++) {
                    if (mileageCheckboxes[i].checked === true) {
                      pipeDelimitedRangeMileage = mileageCheckboxes[i].value;
                    }
                  }
                }

                $('.filter-pane').slideUp("fast");

                var arrItem = {};
                arrItem.descr = "selected mileage range";
                arrItem.id = 0;
                arrItem.matchVal = pipeDelimitedRangeMileage;
                arrItem.selected = true;
                var modelArray = new Array(arrItem);
                model.set(modelArray);

                var maxMileage = $('#results_first_load_js').data("maxmileage");
                var maxPrice = $('#results_first_load_js').data("maxprice");
                criteria.set({ max_mileage: maxMileage }, true_bool);
                criteria.set({ max_price: maxPrice }, true_bool);
                criteria.set({ filters: { mileage_range: pipeDelimitedRangeMileage } }, true_bool);
                carsforsaleService.query();

              });

            }
          }
        }('mileage', commonModels.mileageRange),

        yearRange: function (componentName, model) {

          //#region - Private funcs

          var syncModelWithView = function (loValue, hiValue) {
            var years = model.get()

            years.forEach(function (elem) {
              var matchValue = elem.matchVal
              elem[selected_str] = matchValue >= loValue && matchValue <= hiValue
            })

            model.set(years)
          }

          var syncViewWithModel = function () {
            var arr = model.get()
              , selecteds = arr.filter(function (elem) {
                return elem[selected_str]
              })

            selecteds = !selecteds[length_str] ? arr : selecteds

            var yearFrom = selecteds.slice(0, 1)[0].descr;
            var yearTo = selecteds.slice(-1)[0].descr;

            $('#year_from_ddl_js').val(yearFrom);
            $('#year_to_ddl_js').val(yearTo);

            $('#year_menu_selected_value_js').text(yearFrom + ' - ' + yearTo);

          }

          //#endregion

          return {
            init: function () {

              syncViewWithModel()

              //#region - Setup event handlers for component

              var $yearMenuFrom = $('#year_from_ddl_js')
              var $yearMenuTo = $('#year_to_ddl_js')

              $yearMenuFrom.on(change_event, function () {
                syncModelWithView($yearMenuFrom.val(), $yearMenuTo.val())
                criteria.set({ filters: { year_range: model.stringify() } }, true_bool)
                carsforsaleService.query()
                $('.filter-pane').slideUp("fast")
              })

              $yearMenuTo.on(change_event, function () {
                syncModelWithView($yearMenuFrom.val(), $yearMenuTo.val())
                criteria.set({ filters: { year_range: model.stringify() } }, true_bool)
                carsforsaleService.query()
                $('.filter-pane').slideUp("fast")
              })

              //#endregion

              model.on(change_event, function () {
                syncViewWithModel()
              })

              $('#year_menu_js').click(function () {
                if ($(this).parent().next('.filter-pane').is(':visible')) {
                  $('.menu-label').removeClass('selected');
                  $(this).parent().next('.filter-pane').slideUp("fast");
                } else {
                  $('.filter-pane').hide();
                  $('.menu-label').removeClass('selected');
                  $(this).toggleClass('selected');
                  $(this).parent().next('.filter-pane').slideDown("fast");
                }
              });

            }
          }
        }('year', commonModels.yearRange),

        sortBy: function (componentName) {
          var $sortby = $(hash_symbol + componentName + '_dd' + js_suffix)

          return {
            init: function () {
              criteria.on(change_event, function (e) {
                var sort = e.args.sort
                $sortby.val(sort.by + pipeSeparator + sort.dir)
              })

              $sortby.on(change_event, function () {
                var sortparts = $(this).val().split('|')
                criteria.set({
                  sort: {
                    by: sortparts[0],
                    dir: sortparts[1]
                  }
                })

                carsforsaleService.query()
              })
            }
          }
        }('sortby'),

        tagLister: function (componentName, models) {
          var $tagLister

          //#region - Private funcs

          var mapValuesToFilterDomain = function (modelValues, filterDomain, type) {
            return modelValues.map(function (value) {
              var obj = filterDomain
                .filter(function (item) {
                  return item.matchVal == value
                })[0]

              return utils.mergeObjs(obj, { type: type })
            })
          }

          var buildTagsFromModels = function () {
            var tags = []
              , filterMakes = filterDomains.makes
              , filterCategories = filterDomains.categories
              , categories = mapValuesToFilterDomain(models.categories.get(), filterCategories, category_type)
              , makes = mapValuesToFilterDomain(models.makes.get(), filterMakes, make_type)
              , makeModels = models.makeModels.get()
                .map(function (item) {
                  var parts = item.split(tildeSeparator)

                  return {
                    matchVal: item,
                    descr: filterMakes.filter(function (make) { return make.matchVal === parts[0] })[0].descr + ' ' + parts[1],
                    type: makeModel_type
                  }
                })
              , categoryMakes = models.categoryMakes.get()
                .map(function (item) {
                  var parts = item.split(tildeSeparator)

                  return {
                    matchVal: item,
                    descr: parts[1] + ' ' + filterCategories.filter(function (category) { return category.matchVal === parts[0] })[0].descr,
                    type: categoryMake_type
                  }
                })

            tags = tags
              .concat(categories)
              .concat(categoryMakes)
              .concat(makes)
              .concat(makeModels)
              .sort(function (a, b) {
                if (a.descr < b.descr)
                  return -1
                else if (a.descr > b.descr)
                  return 1

                return 0
              })

            return tags
          }

          var setFilterOnCriteria = function (filter) {
            criteria.set({ filters: filter }, true_bool)
          }

          var modelChangedHandler = function () {
            var tags = buildTagsFromModels()

            $tagLister
              .empty()
              .append(tmpls.tag_btn(tags))
          }

          var removeFromArrayByValue = function (arr, value) {
            return arr.filter(function (item) {
              return item != value
            })
          }

          //#endregion


          return {
            init: function () {
              $tagLister = $(hash_symbol + componentName + js_suffix)


              //#region - UC1: 
              // This needs to listen to changes on the make, model, 
              // category data models and update the tags list on the UI

              models.categories.on(change_event, modelChangedHandler)

              models.categoryMakes.on(change_event, modelChangedHandler)

              models.makes.on(change_event, modelChangedHandler)

              models.makeModels.on(change_event, modelChangedHandler)

              //#endregion


              //#region - UC2: 
              // We also need to set the state of the appropriate models, 
              // plus set the criteria model, 
              // if the user deletes a tag, then re-run the query

              $tagLister.on(click_event, 'button', function () {
                var $tagBtn = $(this)
                  , type = $tagBtn.data(type_text)
                  , value = $tagBtn.data(value_text)

                switch (type) {
                  case category_type:
                    {
                      var categories = models.categories

                      categories.set(removeFromArrayByValue(categories.get(), value))
                      setFilterOnCriteria({ categories: categories.stringify() })
                      carsforsaleService.query()

                      break
                    }

                  case make_type:
                    {
                      var makes = models.makes

                      makes.set(removeFromArrayByValue(makes.get(), value))
                      setFilterOnCriteria({ makes: makes.stringify() })
                      carsforsaleService.query()

                      break
                    }

                  case makeModel_type:
                    {
                      var makeModels = models.makeModels

                      makeModels.set(removeFromArrayByValue(makeModels.get(), value))
                      setFilterOnCriteria({ make_models: makeModels.stringify() })
                      carsforsaleService.query()

                      break
                    }

                  case categoryMake_type:
                    {
                      var categoryMakes = models.categoryMakes

                      categoryMakes.set(removeFromArrayByValue(categoryMakes.get(), value))
                      setFilterOnCriteria({ category_makes: categoryMakes.stringify() })
                      carsforsaleService.query()

                      break
                    }
                }
              })

              //#endregion
            }
          }
        }('tag_lister', commonModels),

        paginator: function (componentName) {
          var $paginator = $(hash_symbol + componentName + js_suffix)

          //#region - Private funcs

          var createPaginatorMarkup = function (data) {

            var paginationItems = [];

            var page = data.page,
              currentPageNum = page.current,
              totalNumOfPages = page.total_pages,
              numOfAdjacentLinksToDisplay = 2;

            //----------------------------------------------------------------------
            //the following code adapted from car.com\common\pagination\paginator.cs
            //----------------------------------------------------------------------

            var leftShowableLink = currentPageNum - numOfAdjacentLinksToDisplay;
            var rightShowableLink = currentPageNum + numOfAdjacentLinksToDisplay;

            // possible 'Previous' link
            if (currentPageNum > 1) {
              paginationItems.push(createPreviousLink((currentPageNum - 1)));
            }

            // first page link
            paginationItems.push(createPageLink(1, (currentPageNum == 1)));

            // possible left-side ellipse
            if ((leftShowableLink - 1) > 1) {
              paginationItems.push("<li class=\"disabled\"><span>...</span></li>");
            }

            // links
            for (var pageNum = leftShowableLink < 2 ? 2 : leftShowableLink;
              pageNum <= rightShowableLink && pageNum < totalNumOfPages;
              pageNum++) {
              paginationItems.push(createPageLink(pageNum, (currentPageNum == pageNum)));
            }

            // possible right-side ellipse
            if ((totalNumOfPages - rightShowableLink) > 1) {
              paginationItems.push("<li class=\"disabled\"><span>...</span></li>");
            }

            // last page link
            if (totalNumOfPages > 1) {
              paginationItems.push(createPageLink(totalNumOfPages, (currentPageNum == totalNumOfPages)));
            }

            // possible 'Next' link
            if (currentPageNum < totalNumOfPages) {
              paginationItems.push(createNextLink(currentPageNum + 1));
            }

            var paginatorHtml = "";
            for (var i = 0; i < paginationItems.length; i++) {
              paginatorHtml += paginationItems[i];
            }

            return (totalNumOfPages > 1) ?
              tmpls.paginator({
                display: paginatorHtml
              })
              : empty_str;
          }

          var createPreviousLink = function (pageNum) {
            var previousLinkHtml = "<li><button data-go-page=\"" + pageNum + "\" id=\"pg_prev_js\" title=\"Page " + pageNum + "\"><span class=\"icon inline\"><svg><use xlink:href=\"/assets/svg/global-sprite.svg#i_arrow3_l\"></use></svg></span> Prev</button></li>";
            return previousLinkHtml;
          }

          var createPageLink = function (pageNum, isActive) {
            var pageLinkHtml = "<li><button data-go-page=\"" + pageNum + "\" id=\"pg_link_js\" title=\"Page " + pageNum + "\">" + pageNum + "</button></li>";
            if (isActive) {
              pageLinkHtml = "<li class=\"active\"><span title=\"Page " + pageNum + "\">" + pageNum + "</span></li>";
            }
            return pageLinkHtml;
          }

          var createNextLink = function (pageNum) {
            var nextLinkHtml = "<li><button data-go-page=\"" + pageNum + "\" id=\"pg_next_js\" title=\"Page " + pageNum + "\">Next <span class=\"icon inline\"><svg><use xlink:href=\"/assets/svg/global-sprite.svg#i_arrow3_r\"></use></svg></span></button></li>";
            return nextLinkHtml;
          }

          //#endregion

          return {
            init: function () {
              paginatorTF = false
              // register handler to listen for any changes to the inventory model
              inventory.on(change_event, function (e) {
                var markup = createPaginatorMarkup(e.args)
                $paginator
                  .empty()
                  .prepend(markup)
              })

              $paginator.on(click_event, button_elem, function () {
                paginatorTF = true
                var $this = $(this)
                  , gotoPage = $this.data('go-page')

                if (gotoPage === 0)
                  return

                criteria.set({ page: { current: gotoPage } })
                carsforsaleService.query()
                $('html,body').scrollTop(0)
              })
            }
          }
        }('paginator'),

        carsForSale: function (componentName) {
          //#region - Cache DOM Elements
          var $carsForSaleWrapper = $(hash_symbol + componentName + '_wrap' + js_suffix)
            , $carsForSaleCount = $(hash_symbol + componentName + '_count' + js_suffix)
            , $carsForSale = $carsForSaleWrapper.find(hash_symbol + componentName + '_list' + js_suffix)

          //#endregion


          //#region - Private Funcs

          var getFromArrayById = function (arr, id) {
            if (!id)
              return empty_str

            return arr.filter(function (item) {
              return id === item.matchVal
            })[0].descr
          }

          var getMakeModelCategorySingles = function () {
            var filters = criteria.get().filters
              , categories = filters.categories
              , makes = filters.makes
              , make_models = filters.make_models
              , categoryId = !~categories.indexOf(pipeSeparator) ? categories : empty_str
              , makeId = !~makes.indexOf(pipeSeparator) ? makes : empty_str
              , makeIdModel = !~make_models.indexOf(pipeSeparator) ? make_models : empty_str
              , make = getFromArrayById(filterDomains.makes, makeId)
              , category = getFromArrayById(filterDomains.categories, categoryId)
              , makeModel = empty_str

            if (!!makeIdModel) {
              var parts = makeIdModel.split(tildeSeparator)
              makeModel = getFromArrayById(filterDomains.makes, parts[0]) + tildeSeparator + parts[1]
            }


            return {
              make: make,
              hasMake: !!make[length_str],
              category: category,
              hasCategory: !!category[length_str],
              makeModel: makeModel,
              hasMakeModel: !!makeModel[length_str]
            }
          }

          var getContextObject = function (defaultPageCtx, obj) {
            return utils.mergeObjs(defaultPageCtx, obj)
          }

          var callTrackers = function (singles) {
            var omni = trk.omni
              , ga = trk.ga
              , comscore = trk.comscore
              , pageview_str = 'pageview'
              , pageCtx = trk.pageCtx
              , defaultPageCtx = {
                make: empty_str,
                superModel: empty_str,
                category: empty_str,
                delayPageviewTracking: false_bool
              }


            if (singles.hasMakeModel) {
              var parts = singles.makeModel.split(tildeSeparator)

              pageCtx.set(getContextObject(defaultPageCtx, {
                make: parts[0],
                superModel: parts[1]
              }))
            }
            else if (singles.hasMake) {
              pageCtx.set(getContextObject(defaultPageCtx, { make: singles.make }))
            }
            else if (singles.hasCategory) {
              pageCtx.set(getContextObject(defaultPageCtx, { category: singles.category }))
            }
            else {
              pageCtx.set(defaultPageCtx)
            }

            omni.track(pageview_str)
            ga.track(pageview_str)
            comscore.track(pageview_str)
          }

          var reloadAds = function (singles) {
            var pageCtx = ads.pageCtx
              , defaultPageCtx = {
                make: empty_str,
                superModel: empty_str,
                category: empty_str,
                delayAdsLoading: false_bool
              }

            if (singles.hasMakeModel) {
              var parts = singles.makeModel.split(tildeSeparator)

              pageCtx.set(getContextObject(defaultPageCtx, {
                make: parts[0],
                superModel: parts[1]
              }))
            }
            else if (singles.hasMake) {
              pageCtx.set(getContextObject(defaultPageCtx, { make: singles.make }))
            }
            else if (singles.hasCategory) {
              pageCtx.set(getContextObject(defaultPageCtx, { category: singles.category }))
            }
            else {
              pageCtx.set(defaultPageCtx)
            }

            if (paginatorTF) {
              ads.reloadC4SResults()
            } else {
              ads.loadC4SResults()
            }
            
          }

          var triggerTrackingAndAds = function () {
            var singles = getMakeModelCategorySingles()
            callTrackers(singles)
            reloadAds(singles)
          }

          var updateDisplay = function (inv) {
            $carsForSale.empty()

            if (inv.cars_for_sale[length_str]) {
              $carsForSale.prepend(tmpls.car_for_sale(inv))
            }
            else {
              $carsForSale.prepend(tmpls.no_results())
            }

            var firstLoadStatus = $('#results_first_load_js').data("status");

            var priceGroupQuantities = inv.price_group_quantities.split("|");
            var mileageGroupQuantities = inv.mileage_group_quantities.split("|");

            var mileageGroup20PctQty = mileageGroupQuantities[0];
            var mileageGroup40PctQty = mileageGroupQuantities[1];
            var mileageGroup60PctQty = mileageGroupQuantities[2];
            var mileageGroup80PctQty = mileageGroupQuantities[3];
            var mileageGroup100PctQty = mileageGroupQuantities[4];

            var priceGroup20PctQty = priceGroupQuantities[0];
            var priceGroup40PctQty = priceGroupQuantities[1];
            var priceGroup60PctQty = priceGroupQuantities[2];
            var priceGroup80PctQty = priceGroupQuantities[3];
            var priceGroup100PctQty = priceGroupQuantities[4];

            if (firstLoadStatus === "True") {

              var maxMileage = inv.max_mileage;
              var maxPrice = inv.max_price;
              var minMileage = inv.min_mileage;
              var minPrice = inv.min_price;

              $('#results_first_load_js').data("maxmileage", maxMileage);
              $('#results_first_load_js').data("maxprice", maxPrice);

              //COMPUTE 20% STEPS
              var mileage20Percent = maxMileage * 0.20;
              var mileage40Percent = maxMileage * 0.40;
              var mileage60Percent = maxMileage * 0.60;
              var mileage80Percent = maxMileage * 0.80;
              var mileage100Percent = maxMileage;

              var price20Percent = maxPrice * 0.20;
              var price40Percent = maxPrice * 0.40;
              var price60Percent = maxPrice * 0.60;
              var price80Percent = maxPrice * 0.80;
              var price100Percent = maxPrice;

              //ROUND VALUES
              mileage20Percent = Math.round(mileage20Percent / 1000);
              mileage40Percent = Math.round(mileage40Percent / 1000);
              mileage60Percent = Math.round(mileage60Percent / 1000);
              mileage80Percent = Math.round(mileage80Percent / 1000);
              mileage100Percent = Math.round(mileage100Percent / 1000) + 1; //+1k to make sure we catch the top

              price20Percent = Math.round(price20Percent / 1000);
              price40Percent = Math.round(price40Percent / 1000);
              price60Percent = Math.round(price60Percent / 1000);
              price80Percent = Math.round(price80Percent / 1000);
              price100Percent = Math.round(price100Percent / 1000) + 1; //+1k to make sure we catch the top

              //RENDER THEM IN THE VIEW
              $('#mileage_0-100Percent_js').val("0|" + mileage100Percent);
              $('#mileage_0-20Percent_js').val("0|" + mileage20Percent);
              $('#mileage_20-40Percent_js').val(mileage20Percent + "|" + mileage40Percent);
              $('#mileage_40-60Percent_js').val(mileage40Percent + "|" + mileage60Percent);
              $('#mileage_60-80Percent_js').val(mileage60Percent + "|" + mileage80Percent);
              $('#mileage_80-100Percent_js').val(mileage80Percent + "|" + mileage100Percent);

              $('#prices_0-100Percent_js').val("0|" + price100Percent);
              $('#prices_0-20Percent_js').val("0|" + price20Percent);
              $('#prices_20-40Percent_js').val(price20Percent + "|" + price40Percent);
              $('#prices_40-60Percent_js').val(price40Percent + "|" + price60Percent);
              $('#prices_60-80Percent_js').val(price60Percent + "|" + price80Percent);
              $('#prices_80-100Percent_js').val(price80Percent + "|" + price100Percent);

              $('#results_first_load_js').data("status", "False");

              $('#results_first_load_js').data("m20", mileage20Percent);
              $('#results_first_load_js').data("m40", mileage40Percent);
              $('#results_first_load_js').data("m60", mileage60Percent);
              $('#results_first_load_js').data("m80", mileage80Percent);
              $('#results_first_load_js').data("m100", mileage100Percent);

              $('#results_first_load_js').data("p20", price20Percent);
              $('#results_first_load_js').data("p40", price40Percent);
              $('#results_first_load_js').data("p60", price60Percent);
              $('#results_first_load_js').data("p80", price80Percent);
              $('#results_first_load_js').data("p100", price100Percent);

            }

            var mileage20PercentText = $('#results_first_load_js').data("m20");
            var mileage40PercentText = $('#results_first_load_js').data("m40");
            var mileage60PercentText = $('#results_first_load_js').data("m60");
            var mileage80PercentText = $('#results_first_load_js').data("m80");
            var mileage100PercentText = $('#results_first_load_js').data("m100");

            var price20PercentText = $('#results_first_load_js').data("p20");
            var price40PercentText = $('#results_first_load_js').data("p40");
            var price60PercentText = $('#results_first_load_js').data("p60");
            var price80PercentText = $('#results_first_load_js').data("p80");
            var price100PercentText = $('#results_first_load_js').data("p100");

            $('#label_mileage_0-20Percent_js').text("Up to " + mileage20PercentText + "K miles (" + mileageGroup20PctQty + ")");
            $('#label_mileage_20-40Percent_js').text(mileage20PercentText + "K - " + mileage40PercentText + "K miles (" + mileageGroup40PctQty + ")");
            $('#label_mileage_40-60Percent_js').text(mileage40PercentText + "K - " + mileage60PercentText + "K miles (" + mileageGroup60PctQty + ")");
            $('#label_mileage_60-80Percent_js').text(mileage60PercentText + "K - " + mileage80PercentText + "K miles (" + mileageGroup80PctQty + ")");
            $('#label_mileage_80-100Percent_js').text(mileage80PercentText + "K - " + mileage100PercentText + "K miles (" + mileageGroup100PctQty + ")");

            if (mileageGroup20PctQty === "0") { $('#mileage_0-20Percent_js').prop('disabled', true); } else { $('#mileage_0-20Percent_js').prop('disabled', false); }
            if (mileageGroup40PctQty === "0") { $('#mileage_20-40Percent_js').prop('disabled', true); } else { $('#mileage_20-40Percent_js').prop('disabled', false); }
            if (mileageGroup60PctQty === "0") { $('#mileage_40-60Percent_js').prop('disabled', true); } else { $('#mileage_40-60Percent_js').prop('disabled', false); }
            if (mileageGroup80PctQty === "0") { $('#mileage_60-80Percent_js').prop('disabled', true); } else { $('#mileage_60-80Percent_js').prop('disabled', false); }
            if (mileageGroup100PctQty === "0") { $('#mileage_80-100Percent_js').prop('disabled', true); } else { $('#mileage_80-100Percent_js').prop('disabled', false); }

            $('#label_prices_0-20Percent_js').text("Up to $" + price20PercentText + "K (" + priceGroup20PctQty + ")");
            $('#label_prices_20-40Percent_js').text("$" + price20PercentText + "K - $" + price40PercentText + "K (" + priceGroup40PctQty + ")");
            $('#label_prices_40-60Percent_js').text("$" + price40PercentText + "K - $" + price60PercentText + "K (" + priceGroup60PctQty + ")");
            $('#label_prices_60-80Percent_js').text("$" + price60PercentText + "K - $" + price80PercentText + "K (" + priceGroup80PctQty + ")");
            $('#label_prices_80-100Percent_js').text("$" + price80PercentText + "K - $" + price100PercentText + "K (" + priceGroup100PctQty + ")");

            if (priceGroup20PctQty === "0") { $('#prices_0-20Percent_js').prop('disabled', true); } else { $('#prices_0-20Percent_js').prop('disabled', false); }
            if (priceGroup40PctQty === "0") { $('#prices_20-40Percent_js').prop('disabled', true); } else { $('#prices_20-40Percent_js').prop('disabled', false); }
            if (priceGroup60PctQty === "0") { $('#prices_40-60Percent_js').prop('disabled', true); } else { $('#prices_40-60Percent_js').prop('disabled', false); }
            if (priceGroup80PctQty === "0") { $('#prices_60-80Percent_js').prop('disabled', true); } else { $('#prices_60-80Percent_js').prop('disabled', false); }
            if (priceGroup100PctQty === "0") { $('#prices_80-100Percent_js').prop('disabled', true); } else { $('#prices_80-100Percent_js').prop('disabled', false); }

            $('#cars_found_js').text(inv.inventory_count.toLocaleString());
            $('#cars_found_h1_js').text(inv.inventory_count.toLocaleString() + " Cars Found");

          }

          //#endregion


          return {
            init: function () {
              $carsForSale.on(click_event, '.lead' + pop_btn_suffix + js_suffix, function (e) {
                e.preventDefault()

                var inventoryId = $(this).closest(li_elem).data(id_text),
                    carforsale = inventory.get().cars_for_sale.filter(function (item) {
                      return item.id === inventoryId
                    })[0]

                var inventoryLeadFormInit = false,
                    hasValidePhoneNum = false;

                if (carforsale.dealer.phone[length_str] > 6) {
                  hasValidePhoneNum = true;
                }

                pageJson.inventory = {
                  id: carforsale.id,
                  dealerId: carforsale.dealer.id,
                  year: carforsale.year,
                  dealerPhone: carforsale.dealer.phone,
                  hasValidPhoneNum: hasValidePhoneNum,
                  make: carforsale.make,
                  model: carforsale.model,
                  quoteButtonSelected: true
                };

                if (!inventoryLeadFormInit) {
                  var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
                  $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

                  inventoryLeadFormInit = true;
                } else {

                  // Display form
                  $('#LeadForm_NewCar_Info').css("display", "none");
                  $('#LeadForm_PersonalInfo').removeClass("col");
                  $('#LeadForm_NewCar_Header').css("display", "none");
                  $('#LeadForm_C4S_Header').text('Contact the Seller');
                  $('#LeadForm_C4S_Header').css("display", "block");
                  $('#SubmitPostButton').text("Contact this Dealer").attr('data-formtype', "c4s");
                  $('#ol_ofr_Thankyou').css("display", "none");
                  if (ABT.pageJson.inventory.hasValidPhoneNum && ABT.pageJson.inventory.dealerPhone && ABT.pageJson.inventory.dealerPhone.length == 10) {
                    var qfPhone = ABT.pageJson.inventory.dealerPhone;
                    var qfPhoneSt = '(' + qfPhone.substr(0, 3) + ') ' + qfPhone.substr(3, 3) + '-' + qfPhone.substr(6, 4);
                    $('#c4sPhoneNum').text('Call ' + qfPhoneSt + ' or Fill out the form.').css("display", "block");
                  } else {
                    $('#c4sPhoneNum').text('').css("display", "none");
                  }
                  $('#ol_Leadform').css("display", "block");
                  $('#leadform_overlay_js').popup('show');
                }
                //#endregion

              })

              // register handler to listen for any changes to the inventory model
              inventory.on(change_event, function (e) {
                updateDisplay(e.args)
                masonry.rescanMediaQueries()
                triggerTrackingAndAds()
              })

              $('#toggle_filters_js').click(function () {
                $('.search-bar-cont').slideToggle();
              });

              $('#btn_filters_done_js').click(function () {
                $('.search-bar-cont').slideToggle();
              });

            }
          }
        }('c4s')
      }
    }()

    //#endregion



    //#region - Overlays

    var missingCarOverlay = function (componentName) {
      var $missingCarOverlay


      return {
        init: function () {
          $body.append(tmpls.missing_car_overlay())

          $missingCarOverlay = $(hash_symbol + componentName + overlay_suffix + js_suffix)

          //#region - Setup UI event handlers

          // overlay form submission event
          $missingCarOverlay.find(form_elem)
            .on(submit_event, function (e) {
              e.preventDefault()
              $missingCarOverlay.popup(hide)
            })


          // click event that closes this overlay
          $missingCarOverlay.find(close_btn_class)
            .on(click_event, function () {
              $missingCarOverlay.popup(hide)
            })

          //#endregion
        },

        popup: function () {
          $missingCarOverlay.popup(show)
        }
      }
    }('missing_car')

    var zipVerifyOverlay = function (componentName) {

      var $zipVerifyOverlay

      return {
        init: function () {
          $body.append(tmpls.zip_verify_overlay())

          $zipVerifyOverlay = $(hash_symbol + componentName + overlay_suffix + js_suffix)

          //#region - Setup UI event handlers
 
          // overlay form submission event
          $zipVerifyOverlay.find(form_elem)
            .on(submit_event, function (e) {

              e.preventDefault()

              personaService.set({ zcverified: true });

              $.when(carsforsaleService.query())
                .done(function () {
                  // when query is finished, close this overlay
                  $zipVerifyOverlay.popup(hide)
                })
            })

          // change zipcode click event
          $zipVerifyOverlay.find('#verify_zipcode_changezip_js')
            .on(click_event, function () {
              $zipVerifyOverlay.popup(hide)
              zipRadiusOverlay.popup()
            })

          // click event that closes this overlay
          $zipVerifyOverlay.find(close_btn_class)
            .on(click_event, function () {
              $zipVerifyOverlay.popup(hide)
            })


          //#endregion
        },

        popup: function () {
          $zipVerifyOverlay.popup(show)
        }
      }
    }('zip_verify')


    var zipRadiusOverlay = function (componentName, radius) {
      //#region - Scoped Variables
      var $zipOverlay
        , $zipPopupBtn
        , $zipPopupBtn2
        , $zipPopupBtn3
        , $radius
        , $zip

      //#endregion


      return {
        init: function () {
          $body.append(tmpls.zip_overlay())

          //#region Cache DOM Elements
          $zipOverlay = $(hash_symbol + componentName + overlay_suffix + js_suffix)
          $zipPopupBtn = $(hash_symbol + componentName + pop_btn_suffix + js_suffix)
          $zipPopupBtn2 = $('#zip_pop_btn2_js')
          $zipPopupBtn3 = $('#js_sitenav_zipcode_btn');
          $radius = $zipOverlay.find(hash_symbol + radius + js_suffix)
          $zip = $zipOverlay.find(hash_symbol + componentName + js_suffix)

          //#endregion

          //#region - Setup UI event handlers

          //  setup zip input validation
          $zip
            .on(focus_event, function (e) {
              e.target.select()

              setTimeout(function () {
                $zip.on(input_modified_events, function () {
                  $(this).val(function (idx, val) {
                    return zipify(val)
                  })
                })
              }, 200)
            })
            .on(blur_event, function () {
              $zip.off(dot_validate)
            })


          // overlay form submission event
          $zipOverlay.find(form_elem)
            .on(submit_event, function (e) {
              var zip = $zip.val()

              e.preventDefault()

              // capture the user-provided zip in the site-wide persona storage
              if (isZip(zip)) {
                personaService.set({ zipcode: zip})
                personaService.set({ zcverified: true })
                ABT.OFR.updateNavZip(zip);
                // update the criteria model with user-input
                criteria.set({
                  zipcode: zip,
                  radius_miles: +$radius.find('option:' + selected_str).val()
                }, true_bool)
              }

              $.when(carsforsaleService.query())
                .done(function () {
                  // when query is finished, close this overlay
                  $zipOverlay.popup(hide)
                })
            })


          // click event that opens this overlay
          $zipPopupBtn
            .on(click_event, function () {
              $zipOverlay.popup(show)
            })
          $zipPopupBtn2
            .on(click_event, function () {
              $zipOverlay.popup(show)
            })
          $zipPopupBtn3.unbind('click');
          $zipPopupBtn3
            .on(click_event, function () {
              $zipOverlay.popup(show)
            })


          // click event that closes this overlay
          $zipOverlay.find(close_btn_class)
            .on(click_event, function () {
              $zipOverlay.popup(hide)
            })


          // register handler to listen for any changes to the criteria model, and update UI
          criteria
            .on(change_event, function (e) {
              $zipPopupBtn.find(dot_symbol + componentName + js_suffix).text(e.args.zipcode)
              $zipPopupBtn2.find(dot_symbol + componentName + js_suffix).text(e.args.zipcode)
              $zipPopupBtn.find(dot_symbol + radius + js_suffix).text(e.args.radius_miles)
              $zip.val(e.args.zipcode)
              $radius.val(e.args.radius_miles)
            })

          //#endregion
        },

        popup: function () {
          $zipOverlay.popup(show)
        }
      }
    }('zip', 'radius')


    var filtersOverlay = function (componentName, models) {
      //#region - Scoped Variables
      var filterDomains
        , $filtersPopupBtn
        , $filtersOverlay
        , $fuelTypes
        , $options

      //#endregion


      //#region - Private funcs


      //#endregion


      return {
        init: function (domains) {
          filterDomains = domains
          $body.append(tmpls.filters_overlay(commonModels))

          //#region - Cached DOM Elements
          $filtersPopupBtn = $(hash_symbol + componentName + pop_btn_suffix + js_suffix)
          $filtersOverlay = $(hash_symbol + componentName + overlay_suffix + js_suffix)
          $fuelTypes = $filtersOverlay.find(hash_symbol + 'fuel_types' + js_suffix)
          $options = $filtersOverlay.find(hash_symbol + 'options' + js_suffix)

          //#endregion


          //#region - Setup UI Click Event Handlers

          $fuelTypes
            .on(click_event, li_elem, function () {
              updateButtonState($(this))
            })

          $options
            .on(click_event, li_elem, function () {
              updateButtonState($(this))
            })

          //#endregion


          //#region - Setup Overlay Event Handlers

          // Form submission event for overlay
          $filtersOverlay.find(form_elem)
            .on(submit_event, function (e) {
              e.preventDefault()

              var fuelTypesModel = models.fuelTypes
                , fuelTypes = fuelTypesModel.get()
                , optionsModel = models.options
                , options = optionsModel.get()


              fuelTypes.forEach(function (item) {
                item[selected_str] = false_bool
              })

              $fuelTypes
                .find(selected_buttons)
                .each(function () {
                  fuelTypes[$(this).data(id_text)][selected_str] = true_bool
                })

              fuelTypesModel.set(fuelTypes)


              options.forEach(function (item) {
                item[selected_str] = false_bool
              })

              $options
                .find(selected_buttons)
                .each(function () {
                  options[$(this).data(id_text)][selected_str] = true_bool
                })

              optionsModel.set(options)

              criteria.set({
                filters: {
                  fuel_types: fuelTypesModel.stringify(),
                  options: optionsModel.stringify()
                }
              }, true_bool)

              $.when(carsforsaleService.query())
                .done(function () {
                  // when query is finished, close this overlay
                  $filtersOverlay.popup(hide)
                })
            })


          // click event that opens this overlay
          $filtersPopupBtn
            .on(click_event, function () {
              $filtersOverlay.popup(show)
            })

          // click event that closes this overlay
          $filtersOverlay.find(close_btn_class)
            .on(click_event, function () {
              $filtersOverlay.popup(hide)
            })

          //#endregion


          //#region - Register Model Event Handlers
          // We are listening for any changes to the models, 
          // then updating any associated UI components.

          models.fuelTypes
            .on(change_event, function (e) {
              syncAllButtonStatesWithModel($fuelTypes, e.args)
            })

          models.options
            .on(change_event, function (e) {
              syncAllButtonStatesWithModel($options, e.args)
            })

          //#endregion
        }
      }
    }('filters', commonModels)


    var makeTagsOverlay = function (componentName, models) {
      //#region - Scoped Variables
      var filterDomains
        , $makeTagPopupBtn
        , $makeTagOverlay
        , $categories
        , $makes
        , $modelsWrapper
        , $makesWrapper
        , $tabs
        , $tab1
        , $tab2
        , $byMakeTabLbl
        , $byCategoryTabLbl
        , $makeAlert
        , $catAlert
        , $pageOne
        , $makeModelPageTwo
        , $catMakePageTwo
        , maketag_prefix = 'mt_'
        , mamo_text = makeModel_type
        , cama_text = categoryMake_type
        , cat_text = 'cat'
        , pageone_text = 'pg1'
        , pagetwo_text = 'pg2'
        , alert_text = 'alert'

      //#endregion


      //#region private funcs

      var disableTabLabel = function ($obj, disable) {
        if (disable) {
          $obj.addClass(disabled)
        }
        else {
          $obj.removeClass(disabled)
        }
      }

      var setInitialStateOfOverlay = function () {
        var categories = filterDomains.categories
            .map(function (item, idx) {
              item.id = idx
              return item
            })
          , makes = filterDomains.makes
            .map(function (item, idx) {
              item.id = idx
              return item
            })

        $categories
          .empty()
          .append(tmpls.make_tag_btn(categories))

        $makes
          .empty()
          .append(tmpls.make_tag_btn(makes))

        $modelsWrapper.empty()
        $makesWrapper.empty()

        $pageOne.show()
        $makeModelPageTwo.hide()
        $catMakePageTwo.hide()

        $tab1.prop(checked, true_bool)
        disableTabLabel($byMakeTabLbl, false_bool)
        disableTabLabel($byCategoryTabLbl, false_bool)

        $catAlert.hide()
        $makeAlert.hide()
      }

      var updateTheAllButtonOnButtonStateChange = function ($btn) {
        var $ul = $btn.parent()
          , $allBtn = $ul.find(li_elem + ':first')
          , selectTheAllButton = true_bool

        $ul.find(selected_buttons)
          .each(function () {
            if (!!$(this).data(id_text)) { // this means any model button, excluding the 'All' button
              selectTheAllButton = false_bool
            }
          })

        updateButtonState($allBtn, selectTheAllButton)
      }

      var valueIsAlreadyInArray = function (arr, value, leftsideMatch) {
        if (typeof (leftsideMatch) === 'undefined') {
          return arr.some(function (item) {
            return item == value
          })
        }
        else if (leftsideMatch) {
          return arr.some(function (item) {
            return !!~item.indexOf(value)
          })
        }
        else {
          return arr.some(function (item) {
            var revValue = value.split(empty_str).reverse().join(empty_str)
              , revItem = item.split(empty_str).reverse().join(empty_str)

            return !!~revItem.indexOf(revValue)
          })
        }
      }

      var setTagRelatedCriteriaFilters = function () {
        criteria.set({
          filters: {
            categories: models.categories.stringify(),
            category_makes: models.categoryMakes.stringify(),
            makes: models.makes.stringify(),
            make_models: models.makeModels.stringify()
          }
        }, true_bool)
      }

      var alertContinueHandler = function () {
        var $this = $(this)
          , type = $this.parent().data(type_text)

        if (type === mamo_text) {       // Make alert
          disableTabLabel($byCategoryTabLbl, false_bool)
          $makeAlert.hide()
          $makes.find(li_elem).each(function () { $(this).removeClass(selected_class) })
          $tab2.prop(checked, true_bool)
        } else if (type === cat_text) { // Category alert
          disableTabLabel($byMakeTabLbl, false_bool)
          $catAlert.hide()
          $categories.find(li_elem).each(function () { $(this).removeClass(selected_class) })
          $tab1.prop(checked, true_bool)
        }
      }

      var alertCancelHandler = function () {
        var $this = $(this)
          , type = $this.parent().data(type_text)

        if (type === mamo_text) {       // Make alert
          $makeAlert.hide()
        } else if (type === cat_text) { // Category alert
          $catAlert.hide()
        }
      }

      //#endregion


      return {
        init: function (domains) {
          filterDomains = domains

          //#region - Make and append overlay

          $makeTagOverlay = $(tmpls.make_tag_overlay())

          $makeTagOverlay
            .find(hash_symbol + mamo_text + underscore + alert_text + js_suffix)
            .prepend(tmpls.make_tag_alert({ type: mamo_text, descr1: 'Make', descr2: 'Category' }))

          $makeTagOverlay
            .find(hash_symbol + cama_text + underscore + alert_text + js_suffix)
            .prepend(tmpls.make_tag_alert({ type: cat_text, descr1: 'Category', descr2: 'Make' }))

          $body.append($makeTagOverlay)

          //#endregion


          //#region - Cached DOM Elements

          $makeTagPopupBtn = $(hash_symbol + componentName + pop_btn_suffix + js_suffix)


          $makes = $makeTagOverlay.find(hash_symbol + mamo_text + underscore + 'makes' + js_suffix)

          $modelsWrapper = $makeTagOverlay.find(hash_symbol + mamo_text + underscore + 'models' + wrap_suffix + js_suffix)

          $categories = $makeTagOverlay.find(hash_symbol + cama_text + underscore + 'cats' + js_suffix)

          $makesWrapper = $makeTagOverlay.find(hash_symbol + cama_text + underscore + 'makes' + wrap_suffix + js_suffix)


          $tabs = $makeTagOverlay.find(hash_symbol + maketag_prefix + 'tabs' + js_suffix + ' [type=radio]')

          $tab1 = $makeTagOverlay.find(hash_symbol + tab_text + 1)
          $tab2 = $makeTagOverlay.find(hash_symbol + tab_text + 2)

          $byMakeTabLbl = $makeTagOverlay.find(hash_symbol + mamo_text + underscore + tab_lbl_suffix + js_suffix)

          $byCategoryTabLbl = $makeTagOverlay.find(hash_symbol + cama_text + underscore + tab_lbl_suffix + js_suffix)

          $makeAlert = $makeTagOverlay.find(hash_symbol + mamo_text + underscore + alert_text + js_suffix + space + dot_symbol + alert_text + js_suffix)

          $catAlert = $makeTagOverlay.find(hash_symbol + cama_text + underscore + alert_text + js_suffix + space + dot_symbol + alert_text + js_suffix)

          $pageOne = $makeTagOverlay.find(hash_symbol + maketag_prefix + pageone_text + js_suffix)

          $makeModelPageTwo = $makeTagOverlay.find(hash_symbol + mamo_text + underscore + pagetwo_text + js_suffix)

          $catMakePageTwo = $makeTagOverlay.find(hash_symbol + cama_text + underscore + pagetwo_text + js_suffix)


          var $gotoMakeModelPageTwoBtn = $makeTagOverlay.find(hash_symbol + 'goto_' + mamo_text + underscore + pagetwo_text + btn_suffix + js_suffix)
            , $backtoMakeModelPageOneBtn = $makeTagOverlay.find(hash_symbol + 'backto_' + mamo_text + underscore + pageone_text + btn_suffix + js_suffix)
            , $gotoCatMakePageTwoBtn = $makeTagOverlay.find(hash_symbol + 'goto_' + cama_text + underscore + pagetwo_text + btn_suffix + js_suffix)
            , $backtoCatMakePageOneBtn = $makeTagOverlay.find(hash_symbol + 'backto_' + cama_text + underscore + pageone_text + btn_suffix + js_suffix)

          //#endregion


          //#region - Setup overlay user events

          $tabs.on(click_event, function (e) {
            var $this = $(this)
              , $activeTab = $tabs.filter(':not(:' + checked + ')')

            if (!$activeTab.is($this)) {
              var activeId = $activeTab.attr(id_text)

              if (activeId === tab_text + 1) { // By Make tab
                if ($makes.find(selected_buttons)[length_str]) {
                  e.preventDefault()
                  $makeAlert.show()
                  $tab1.prop(checked, true_bool)
                }
              } else if (activeId === tab_text + 2) { // By Category tab
                if ($categories.find(selected_buttons)[length_str]) {
                  e.preventDefault()
                  $catAlert.show()
                  $tab2.prop(checked, true_bool)
                }
              }
            }
          })

          var continueBtnClass = '.continue' + btn_suffix + js_suffix
            , cancelBtnClass = '.cancel' + btn_suffix + js_suffix

          $makeAlert.find(continueBtnClass)
            .add($catAlert.find(continueBtnClass))
            .on(click_event, alertContinueHandler)

          $makeAlert.find(cancelBtnClass)
            .add($catAlert.find(cancelBtnClass))
            .on(click_event, alertCancelHandler)


          $categories
            .on(click_event, li_elem, function () {
              updateButtonState($(this))
              disableTabLabel($byMakeTabLbl, !!$categories.find(selected_buttons)[length_str])
            })

          $makes
            .on(click_event, li_elem, function () {
              updateButtonState($(this))
              disableTabLabel($byCategoryTabLbl, !!$makes.find(selected_buttons)[length_str])
            })


          $modelsWrapper
            .on(click_event, li_elem, function () {
              var $this = $(this)

              if (!!$this.data(id_text)) { // this means any model button, excluding the 'All' button
                updateButtonState($this)
                updateTheAllButtonOnButtonStateChange($this)
              }
            })

          $makesWrapper
            .on(click_event, li_elem, function () {
              var $this = $(this)

              if (!!$this.data(id_text)) { // this means any make button, excluding the 'All' button
                updateButtonState($this)
                updateTheAllButtonOnButtonStateChange($this)
              }
            })


          $gotoMakeModelPageTwoBtn.on(click_event, function () {
            var makes = []
              , $selectedMakes = $makes.find(selected_buttons)
              , pendingResponseCount = $selectedMakes[length_str]

            $selectedMakes.each(function () {
              var $this = $(this)
                , value = $this.data(value_text)
                , name = $this.text().trim()

              makes.push({ name: name, value: value })

              $.when(carsforsaleService.getModelsByMakeId(value))
                .then(function (data) {
                  var make = makes.filter(function (item) {
                    return item.value == data.make_id
                  })[0]

                  make.wrap = tmpls.make_tag_btn_group({ name: make.name, value: make.value, descr: 'Models' })

                  data.models.forEach(function (item, idx) {
                    item.id = idx + 1
                    item[selected_str] = false_bool
                    item.matchVal = make.value + tildeSeparator + item.matchVal
                  })

                  data.models.unshift({ id: 0, matchVal: make.value, descr: all_text, selected: true_bool })
                  make.models = tmpls.make_tag_btn(data.models)

                  if (!--pendingResponseCount) {
                    var $frag = $(div_tag)

                    makes.forEach(function (item) {
                      var $wrap = $(div_tag).append(item.wrap)

                      $wrap.find(ul_elem).append(item.models)
                      $frag.append($wrap.html())
                    })

                    $modelsWrapper
                      .empty()
                      .append($frag.html())
                  }
                })
            })

            $pageOne.hide()
            $makeModelPageTwo.show()
          })

          $backtoMakeModelPageOneBtn.on(click_event, function () {
            $pageOne.show()
            $modelsWrapper.empty()
            $makeModelPageTwo.hide()
          })


          $gotoCatMakePageTwoBtn.on(click_event, function () {
            var categories = []
              , $selectedCategories = $categories.find(selected_buttons)
              , pendingResponseCount = $selectedCategories[length_str]

            $selectedCategories.each(function () {
              var $this = $(this)
                , value = $this.data(value_text)
                , name = $this.text().trim()

              categories.push({ name: name, value: value })

              $.when(carsforsaleService.getMakesByCategoryId(value))
                .then(function (data) {
                  var category = categories.filter(function (item) {
                    return item.value == data.category_id
                  })[0]

                  category.wrap = tmpls.make_tag_btn_group({ name: category.name, value: category.value, descr: 'Makes' })

                  data.makes.forEach(function (item, idx) {
                    item.id = idx + 1
                    item[selected_str] = false_bool
                    item.matchVal = category.value + tildeSeparator + item.matchVal
                  })

                  data.makes.unshift({ id: 0, matchVal: category.value, descr: all_text, selected: true_bool })
                  category.makes = tmpls.make_tag_btn(data.makes)

                  if (!--pendingResponseCount) {
                    var $frag = $(div_tag)

                    categories.forEach(function (item) {
                      var $wrap = $(div_tag).append(item.wrap)

                      $wrap.find(ul_elem).append(item.makes)
                      $frag.append($wrap.html())
                    })

                    $makesWrapper
                      .empty()
                      .append($frag.html())
                  }
                })
            })

            $pageOne.hide()
            $catMakePageTwo.show()
          })

          $backtoCatMakePageOneBtn.on(click_event, function () {
            $pageOne.show()
            $makesWrapper.empty()
            $catMakePageTwo.hide()
          })

          //#endregion


          //#region - Set up open/close overlay events

          $makeTagPopupBtn
            .on(click_event, function () {
              setInitialStateOfOverlay()
              $makeTagOverlay.popup(show)
            })

          $makeTagOverlay.find(close_btn_class)
            .on(click_event, function () {
              $makeTagOverlay.popup(hide)
            })

          //#endregion


          //#region - Form Submission

          // form submission event for Make/Model page 2 overlay
          $makeModelPageTwo.find(form_elem)
            .on(submit_event, function (e) {
              e.preventDefault()

              var makesModel = models.makes
                , makes = makesModel.get()
                , makeModelsModel = models.makeModels
                , makeModels = makeModelsModel.get()


              $modelsWrapper.find(ul_elem)
                  .each(function () {
                    $(this).find(selected_buttons)
                        .each(function () {
                          var $this = $(this)
                            , id = $this.data(id_text)
                            , value = $this.data(value_text)


                          if (!id) { // set make only (the 'All' models button is selected)
                            if (!valueIsAlreadyInArray(makes, value)) {
                              makes.push(value)
                              makesModel.set(makes)

                              // remove any existing make_model tags that have this make
                              if (valueIsAlreadyInArray(makeModels, value, true_bool)) {
                                makeModelsModel.set(makeModels.filter(function (item) { return (!~item.indexOf(value + tildeSeparator)) }))
                              }

                              setTagRelatedCriteriaFilters()
                            }
                          }
                          else { // set make & make_model (the 'All' models button is *NOT* selected)
                            var parts = value.split(tildeSeparator) // e.g. '14~Acadia' => ['14','Acadia']

                            if (!valueIsAlreadyInArray(makeModels, value)) {
                              makeModels.push(value)
                              makeModelsModel.set(makeModels)

                              // remove any existing make_model tags that have this make
                              if (valueIsAlreadyInArray(makes, parts[0])) {
                                makesModel.set(makes.filter(function (item) { return item != parts[0] }))
                              }

                              setTagRelatedCriteriaFilters()
                            }
                          }
                        })
                  })

              $.when(carsforsaleService.query())
                  .done(function () {
                    // when query is finished, close this overlay
                    $makeTagOverlay.popup(hide)
                  })
            })


          // form submission event for Category/Make page 2 overlay
          $catMakePageTwo.find(form_elem)
            .on(submit_event, function (e) {
              e.preventDefault()

              var categoriesModel = models.categories
                , categories = categoriesModel.get()
                , categoryMakesModel = models.categoryMakes
                , categoryMakes = categoryMakesModel.get()


              $makesWrapper.find(ul_elem)
                .each(function () {
                  $(this).find(selected_buttons)
                    .each(function () {
                      var $this = $(this)
                        , id = $this.data(id_text)
                        , value = $this.data(value_text)


                      if (!id) { // set category only (the 'All' makes button is selected)
                        if (!valueIsAlreadyInArray(categories, value)) {
                          categories.push(value)
                          categoriesModel.set(categories)

                          // remove any existing category_make tags that have this category
                          if (valueIsAlreadyInArray(categoryMakes, value, true_bool)) {
                            categoryMakesModel.set(categoryMakes.filter(function (item) { return (!~item.indexOf(value + tildeSeparator)) }))
                          }

                          setTagRelatedCriteriaFilters()
                        }
                      }
                      else { // set category & category_make (the 'All' makes button is *NOT* selected)
                        var parts = value.split(tildeSeparator) // e.g. '7~Ford' => ['7','Ford']

                        if (!valueIsAlreadyInArray(categoryMakes, value)) {
                          categoryMakes.push(value)
                          categoryMakesModel.set(categoryMakes)

                          // remove any existing category_make tags that have this category
                          if (valueIsAlreadyInArray(categories, parts[0])) {
                            categoriesModel.set(categories.filter(function (item) { return item != parts[0] }))
                          }

                          setTagRelatedCriteriaFilters()
                        }
                      }
                    })
                })

              $.when(carsforsaleService.query())
                .done(function () {
                  // when query is finished, close this overlay
                  $makeTagOverlay.popup(hide)
                })
            })

          //#endregion
        }
      }
    }('make_tag', commonModels)

    //#endregion


    return {
      init: function () {

        //#region - Private funcs

        var setUrlHashToCriteria = function (crit, loc) {
          var packed = packify.pack(crit.get(), crit.packTemplate)
            , hashValue = lzCompress(jsonStringify(packed))

          loc.hash = hash_symbol + hashValue
        }

        var getCriteriaFromUrlHash = function (crit, loc) /*(packTempl)*/ {
          var hashValue = loc.hash

          if (!hashValue[length_str]) return undefined

          try {
            var jsonStr = lzDecompress(hashValue.substr(1)),
                unpacked = packify.unpack(jsonParse(jsonStr), crit.packTemplate)

            if (crit.isValidStruct(unpacked)) {
              return unpacked
            }

            return undefined
          }
          catch (e) {
            return undefined
          }
        }

        var syncComponentModelsToCriteria = function (criteriaObj) {
          var filters = criteriaObj.filters

          commonModels.priceRange.sync(filters.price_range)
          commonModels.mileageRange.sync(filters.mileage_range)
          commonModels.yearRange.sync(filters.year_range)
          commonModels.fuelTypes.sync(filters.fuel_types)
          commonModels.options.sync(filters.options)
          commonModels.categories.sync(filters.categories)
          commonModels.categoryMakes.sync(filters.category_makes)
          commonModels.makes.sync(filters.makes)
          commonModels.makeModels.sync(filters.make_models)
        }

        var getRedirectPartsFromQuerystring = function (qs) {
          // querystring should contain something like => "5-digit-zipcode|make-id|model-name"
          var parts = (qs || empty_str).split(pipeSeparator)

          if (parts[length_str] !== 3 ||  // we should have 3 parts
              !isZip(parts[0]) ||         // should be a valid zip
              !isInt(+parts[1]) ||        // second part should be an intger (make_id)
              !parts[2][length_str]       // third part should be a string w/ min length of 1
          ) {
            return undefined
          }

          return parts
        }

        //#endregion



        //#region - Manage asynchronous dependencies

        // Because we have some asynchronous dependencies, we will use the *promise* pattern so we can still make things happen in the proper order.
        $.when(
            $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup')),
            $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('masonry')),
            personaService.isReady()
          )

          .then(function () {
            return carsforsaleService.isReady()
          })

          .then(function () {
            return carsforsaleService.getFilterDomains()
          })

          .then(function (filterDomains) {
            // Init all our models and UI components.
            commonModels.init(filterDomains)
            pageComponents.initAll(filterDomains)
            filtersOverlay.init(filterDomains)
            makeTagsOverlay.init(filterDomains)
            zipVerifyOverlay.init()
            zipRadiusOverlay.init()
            missingCarOverlay.init()
          })

          .then(function () {
            var LZString = win.LZString
            lzCompress = LZString.compressToBase64
            lzDecompress = LZString.decompressFromBase64

            setTimeout(function () {
              //#region - Broad Business Rules
              /**
               *  First, inspect the URL for either a hash of the criteria, 
               *  or a querystring with zip|make-id|model-name which got here as a redirect from the details page.
               *  If valid, use it to set the criteria and query the inventory.
               *  
               *  If there is NO valid criteria that can be derived from the URL, 
               *  look for local-cache saved criteria, and if valid, use it to query inventory.
               *
               *  If there is NO criteria in local-cache, then we need to get a zipcode and set defaults on the criteria filters.
               *  
               *  For the zipcode, we will first try the Persona local-cache, but if not found, 
               *  open the Zip/Radius overlay to get a zipcode from the user.
               */
              //#endregion

              var validCriteriaFromUrlHash = getCriteriaFromUrlHash(criteria, location)

              //#region - First, check if we have a hash on the url ...

              // If url hash is valid, attempt to restore the state of the criteria from it.
              if (validCriteriaFromUrlHash) {
                criteria.set(validCriteriaFromUrlHash, true_bool)

                $.when(carsforsaleService.query())
                  .then(function () {
                    // Adjust page number back to "1", if server-side says that their is no longer inventory for this page.
                    if (criteria.get().page.current !== validCriteriaFromUrlHash.page.current) {
                      criteria.set({
                        page: {
                          current: 1
                        }
                      })

                      setUrlHashToCriteria(criteria, location)
                    }
                  })
              }

                //#endregion

              else { // If NO hash on the url ...

                //#region - Secondly, check if we have a querystring on the url ...

                // We may have a redirect from the details page (IOW, the specific piece of inventory no longer exists),
                // so lets check for the existence of a querstring and then verify that it is valid for our use-case.
                var validParts = getRedirectPartsFromQuerystring($.getQuerystringValueByKey('q'))

                if (validParts) {
                  criteria
                    .reset()
                    .set({
                      zipcode: validParts[0],
                      filters: {
                        make_models: validParts[1] + tildeSeparator + validParts[2]
                      }
                    })

                  carsforsaleService.query()
                  win.history.replaceState(null, empty_str, location.href.replace(location.search, empty_str))
                  missingCarOverlay.popup()
                }

                  //#endregion

                else { // If NO querystring on the url ...

                  //#region - Restore state of query criteria from a saved version in local-cache, or the default criteria ...

                  // If we have an invalid zip in our criteria, 
                  // If no zip can be resolved, ask the user to provide one.
                  var zipcodeVerified = personaService.get().zcverified
                  //console.log(zipcodeVerified)
                  if (isZip(criteria.get().zipcode) && zipcodeVerified) {
                    //console.log('c4s zip = ' + criteria.get().zipcode)
                    criteria.set({}, true_bool)
                    carsforsaleService.query()
                  }
                  else {
                    // Check for a zip in our persona local-cache.
                    var zipcode = personaService.get().zipcode

                    if (zipcode && zipcodeVerified) {
                      criteria.set({ zipcode: zipcode }, true_bool)
                      carsforsaleService.query()
                    }
                    else if (zipcode) {
                      criteria.set({ zipcode: zipcode }, true_bool)
                      $('#verify_zipcode_js').text(zipcode)
                      zipVerifyOverlay.popup()
                    }
                    else {
                      zipRadiusOverlay.popup()
                    }
                  }
                }
                //#endregion
              }

              syncComponentModelsToCriteria(criteria.get())
            }, 100)


            // register handlers to listen for any changes to the inventory model
            inventory.on(change_event, function () {
              setUrlHashToCriteria(criteria, location)
            })
          })

        //#endregion


        //#region - Recently Viewed Cars Widget

        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4svwrecwdg'))))
          .then(function () {
            abt.WDG.c4s_viewedrecently.init()
          })

        //#endregion
      },

      models: commonModels
    }
  }()

  $(function () {
    page.init()
  })
})(window, jQuery, ABT, []._)