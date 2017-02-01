
'use strict';

(function (win, $, abt, undefined) {

  var env = abt.ENV,
    libsPath = env.libsPath,
    getJsFileNameByAlias = env.getJsFileNameByAlias;

  var page = (function () {
    //#region - Module level vars, minification enhancements, and DOM caching
    var mergeObjs = abt.UTILS.mergeObjs
      , inventory = abt.pageJson.inventory
      , location = win.location
      , personaService = abt.PERSONA
      , carsforsaleService = require('../../modules/abt-svc-c4s')
      , Spinify = require('../../modules/abt-spinify')
      , spinner = Spinify.createSpinner()
      , getScriptUriByName = env.getScriptUriByName
      , tmpls = require('./vehicledetails_tmpls/desk/tmpls.js')
      , click_event = 'click'
      , change_event = 'change'
      , move_event = 'move'
      , movestart_event = move_event + 'start'
      , moveend_event = move_event + 'end'
      , empty_string = ''
      , space = ' '
      , hash_symbol = '#'
      , dot_symbol = '.'
      , pipe_symbol = '|'
      , forwardSlash_symbol = '/'
      , length = 'length'
      , ul_elem = 'ul'
      , js_suffix = '_js'
      , wrap_suffix = '_wrap'
      , btn_suffix = '_btn'
      , button_elem = 'button'
      , slideto_data = 'slide-to'
      , notransition_text = 'notransition'
      , disabled_text = 'disabled'
      , carsForSale_text = 'cars-for-sale'
      , search_results_page_path = forwardSlash_symbol + carsForSale_text + '/results/'
      , vehicle_details_page_path = forwardSlash_symbol + carsForSale_text + '/vehicle-details/'
      , c4sViewedRecentlyWidget

    //#endregion

    //#region - Element Aliases

    var page_text = 'page'
      , wrapper_text = 'wrapper'
      , dash_btn = '-btn'
      , left_btn_text = 'left' + dash_btn
      , right_btn_text = 'right' + dash_btn
      , photo_slider_text = 'photo-slider'
      , inventory_nav_text = 'inventory-nav'
      , lead_btn_text = 'lead' + dash_btn
      , back_to_search_btn = 'back-to-search' + dash_btn
      , prev_text = 'prev'
      , next_text = 'next'

    //#endregion


    //**** EXPOSE FOR DEBUGGING ONLY (should be commented out) ****
    //ABT.c4s = carsforsaleService



    //#region Private funcs

    function formatNumber(num) {
      return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,")
    }

    //#endregion



    var pageComponents = (function () {

      //#region - Private funcs

      var weHaveAValidQuerystring = function (arr) {
        var expectedNumOfPartsInQuerystring = 5
        return arr && arr[0] !== empty_string && arr[length] === expectedNumOfPartsInQuerystring
      }

      var parseQuerystring = function (q) {
        return q.slice(3).split(pipe_symbol)
      }

      //#endregion


      var backToSearchButton = (function (componentName) {
        var $backToSearchBtn


        return {
          init: function () {
            $backToSearchBtn = $(hash_symbol + componentName + btn_suffix + js_suffix)

            //$backToSearchBtn.on(click_event, function(e) {
            //  //e.preventDefault()

            //})
          },

          get: function (propname) {
            if (propname === back_to_search_btn) {
              return $backToSearchBtn
            }

            return undefined
          }
        }
      })('back_2_search')


      var photoSlider = (function (componentName) {
        //#region - Scoped Variables
        var $sliderContainer
          , $sliderBtns
          , sliderFuncs

        //#endregion


        //#region - Private funcs

        var moveHandlerForLazyLoadImages = function (e) {
          var src_text = 'src'
            , image = $sliderContainer.find('img')[e + 1]

          if (image) {
            var $image = $(image)
              , src = $image.attr(src_text)

            if (src === empty_string) {
              $image.attr(src_text, $image.data('lazy' + src_text))
            }
          }
        }

        //#endregion

        return {

          init: function () {
            $sliderContainer = $(hash_symbol + componentName + js_suffix)
            $sliderBtns = $(hash_symbol + componentName + '_btns' + js_suffix)

            $sliderContainer.unslider({
              delay: false,
              dots: false,
              complete: moveHandlerForLazyLoadImages,
              fluid: true,
              scount: false //  enable the (1 of x) display feature
            })

            sliderFuncs = $sliderContainer.data('un' + componentName)
            $sliderContainer.show()

            $sliderBtns.on(click_event, button_elem, function () {
              var $this = $(this)
                , slideTo = $this.data(slideto_data)

              sliderFuncs[slideTo]()
            })

            var $framesWrapper = $sliderContainer.find('ul')
              , frames = $sliderContainer.find('.frame_js')

            frames
              .on(movestart_event, function (e) {
                // If the movestart heads off in a upwards or downwards
                // direction, prevent it so that the browser scrolls normally.
                if ((e.distX > e.distY && e.distX < -e.distY) ||
                  (e.distX < e.distY && e.distX > -e.distY)) {
                  e.preventDefault();

                  return;
                }

                // To allow the slide to keep step with the finger,
                // temporarily disable transitions.
                $framesWrapper.addClass(notransition_text);
              })
              .on(move_event, function (e) {
                // Move slides with the finger
                if (e.distX < 0) {
                  sliderFuncs.next();
                }

                if (e.distX > 0) {
                  sliderFuncs.prev();
                }
              })
              .on(moveend_event, function () {
                $framesWrapper.removeClass(notransition_text);
              })

            $('#js_sitenav_zipcode_btn').unbind('click');
          },

          kill: function () {
            $sliderContainer.data('un' + componentName, null)
          },

          get: function (propname) {
            if (propname === wrapper_text) {
              return $sliderContainer
            } else if (propname === left_btn_text) {
              return $sliderBtns.find(dot_symbol + prev_text)
            } else if (propname === right_btn_text) {
              return $sliderBtns.find(dot_symbol + next_text)
            }

            return undefined
          }
        }
      })('slider')


      var inventoryNavigation = (function (componentName, dealerBox, featureGrid, messaging) {
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


        //#region - Private funcs

        var weHaveInventoryToPageThrough = function (d) {
          return d.inventory_count > 0
        }

        var pointerIsValidForThisContext = function (pointer) {
          return pointer > 0 && pointer <= originalInventoryCount
        }

        var calculateInventoryPointer = function (page, currItem) {
          return page.items_per_page * (page.current - 1) + (currItem.idx + 1)
        }

        var resetActiveStateOfButtons = function () {
          var $buttons = $navigationContainers.find(button_elem)

          var getFilteredButtonsBy = function (gototext) {
            return $buttons.filter(function () { return $(this).data("goto") === gototext })
          }

          $buttons.removeClass(disabled_text)
          getFilteredButtonsBy('prev').show()
          getFilteredButtonsBy('next').show()

          if (!pointerIsValidForThisContext(currentInventoryPointer - 1)) {
            getFilteredButtonsBy('prev').addClass(disabled_text)
            getFilteredButtonsBy('prev').hide()
          }

          if (!pointerIsValidForThisContext(currentInventoryPointer + 1)) {
            getFilteredButtonsBy('next').addClass(disabled_text)
            getFilteredButtonsBy('next').hide()
          }
        }

        var updatePageContent = function (car) {
          var $h1YearMakeModel = $(hash_symbol + 'h1_ymm' + js_suffix)
            , $h1Price = $(hash_symbol + 'h1_price' + js_suffix)
            , $dealerBoxContainer = $(hash_symbol + dealerBox + js_suffix)
            , $featureGridContainer = $(hash_symbol + featureGrid + js_suffix + space + ul_elem)
            , $messaging = $(hash_symbol + messaging + js_suffix)
            , features = []
            , notAvailable_text = 'N/A'
            , tmpl_replace_class = '.tmpl_replace_js'


          // add a little additional functionality
          mergeObjs(car, {
            hasDetails: function () {
              return !!this.details[length]
            },
            hasNotes: function () {
              return !!this.seller_notes[length]
            },
            hasDlrMsg: function () {
              return !!this.dealer.message[length]
            }
          })

          //#region - Update H1 Year|Make|Model and Price

          $h1YearMakeModel.text(car.year + space + car.make + space + car.model)
          $h1Price.text('$' + car.price)

          //#endregion


          //#region - Update slider images

          photoSlider.kill()
          photoSlider.get(wrapper_text).find(ul_elem).replaceWith(tmpls.slider_frames(car))
          photoSlider.init()

          //#endregion

          /*
           ========================== AUTONATION  ===================================
           NOTES: These Autonation firstload/not firstload DIVs are required 
                  so that when a user's first selection/click from search results 
                  is not for an Autonation vehicle and the VehicleDetails view 
                  is invoked rather than VehicleDetailsBranded, if they paginate 
                  through the results in the VDP and encounter an Autonation vehicle,
                  the appropriate decorations and badging will be made visible.
           ==========================================================================
           */

          //#region - Update dealer-box

          $dealerBoxContainer.find(tmpl_replace_class + ':first').replaceWith(tmpls.dealer_identifier(car))
          $dealerBoxContainer.find(tmpl_replace_class + ':last').replaceWith(tmpls.dealer_car_contact(car))
          $('#autonation_dealerbox_firstload_js').css('display', 'none');

          //#endregion


          //#region - Update autonation divs 

          $('#autonation_navdecorator_firstload_js').css('display', 'none');
          $('#autonation_dealerhours_firstload_js').css('display', 'none');
          $('#autonation_recallfreebadge_firstload_js').css('display', 'none');

          if (car.dealer.autonationdealer) {

            if ($('#autonation_navdecorator_js').css('display') === 'none') {
              $('#autonation_navdecorator_js').css('display', 'block');
            }

            if ($('#autonation_dealerhours_js').css('display') === 'none') {
              $('#autonation_dealerhours_js').css('display', 'block');
              $('#tmpl_replace_dealerhours_js').empty().append(tmpls.dealer_hours(car));
            }

            if ($('#autonation_recallfreebadge_js').css('display') === 'none') {
              $('#autonation_recallfreebadge_js').css('display', 'block');
            }

          } else {
            $('#autonation_navdecorator_js').css('display', 'none');
            $('#autonation_dealerhours_js').css('display', 'none');
            $('#autonation_recallfreebadge_js').css('display', 'none');
          }

          //#endregion


          //#region - Update dealer-map

          $('#tmpl_replace_dealermap_js').empty().append(tmpls.dealer_map(car));
          var refreshGoogleMaps = function () {
            var dealer = $('#js_dealer_map');
            var map = new google.maps.Map($('#map-canvas').get(0), {
              zoom: 15,
              center: new google.maps.LatLng(dealer.data("latitude"), dealer.data("longitude")),
              mapTypeControl: false,
              panControl: false,
              zoomControl: false,
              scaleControl: true,
              streetViewControl: false,
              mapTypeId: google.maps.MapTypeId.ROADMAP
            });
            setTimeout(function () {
              var marker = new google.maps.Marker({
                position: new google.maps.LatLng(dealer.data("latitude"), dealer.data("longitude")),
                map: map,
                draggable: false,
                animation: google.maps.Animation.DROP
              });
            }, 100);
          };
          refreshGoogleMaps();

          //#endregion


          //#region - Update feature-grid

          var price = car.price
            , mileage = car.mileage
            , cityMpg = car.city_mpg
            , highwayMpg = car.highway_mpg
            , trim = space
            , interiorColor = space
            , exteriorColor = car.ext_color
            , cylinders = car.cylinders
            , trannyType = car.tranny_type
            , vin = car.vin


          var addFeature = function (arr, label, value) {
            arr.push({ label: label, value: value })
          }

          addFeature(features, 'Price:', (price[length] ? '$' + price : notAvailable_text))
          addFeature(features, 'Mileage:', (mileage[length] ? mileage + ' miles' : notAvailable_text))
          addFeature(features, 'MPG City / Hwy:',
            (cityMpg > 0 ? cityMpg : notAvailable_text) + ' / ' +
            (highwayMpg > 0 ? highwayMpg : notAvailable_text))
          addFeature(features, 'Trim:', (trim[length] ? trim : notAvailable_text))
          addFeature(features, 'Color:',
            (exteriorColor[length] ? 'Ext: ' + exteriorColor : notAvailable_text) + ' / Int: ' +
            (interiorColor[length] ? interiorColor : notAvailable_text))
          addFeature(features, 'Engine:', (cylinders[length] ? cylinders + ' Cylinders' : notAvailable_text))
          addFeature(features, 'Transmission:',
            (trannyType === 'AUTO' ? 'Automatic' : trannyType === 'MAN' ? 'Manual' : notAvailable_text))
          addFeature(features, 'VIN:', (vin[length] ? vin : notAvailable_text))

          $featureGridContainer.empty().append(tmpls.feature_grid(features))

          //#endregion


          //#region - Update the details|notes|message

          $messaging.empty().append(tmpls.dlr_car_messaging(car))

          //#endregion

        }

        var queryCarByPointer = function (pointer) {
          criteria.set({
            page: {
              current: pointer,
              items_per_page: 1
            }
          })

          spinner.start()

          $.when(carsforsaleService.query())
          .then(function () {
            criteria.set(originalStateOfCriteria)
          })
        }

        var triggerTrackingAndAds = function (car) {
          var ads = abt.ADS
            , trk = abt.TRK
            , extend = $.extend
            , obj = { make: car.make, year: car.year }
            , pageview = 'pageview'

          ads.pageCtx.set(extend(obj, { model: car.model }))
          ads.reload()

          trk.pageCtx.set(extend(obj, { superModel: car.model }))
          trk.omni.track(pageview)
          trk.ga.track(pageview)
          trk.comscore.track(pageview)
        }

        //#endregion


        return {
          init: function () {
            $navigationContainers = $(dot_symbol + componentName + wrap_suffix + js_suffix)

            var queryParts = parseQuerystring(location.search)

            if (weHaveAValidQuerystring(queryParts)) {
              querystringObj = {
                dlrId: +queryParts[0],
                invId: +queryParts[1],
                zip: queryParts[2],
                makeId: +queryParts[3],
                model: queryParts[4]
              }
            }
            else { // if something is screwed up on the querystring, redirect back to the results page
              location.replace(search_results_page_path)
            }


            // if the browser does NOT support the history API, do not use progressive enhancement for this feature
            if (env.browser.supportsHistoryApi) {

              $.when(carsforsaleService.isReady())
                .then(function () {
                  inventory = carsforsaleService.inventory
                  criteria = carsforsaleService.criteria

                  // save original state of criteria, so we can restore it later
                  originalStateOfCriteria = $.extend(true, {}, criteria.get())

                  $.when(carsforsaleService.query())
                    .then(function (data) {
                      if (weHaveInventoryToPageThrough(data)) {
                        originalStateOfInventory = $.extend(true, {}, data)

                        var currentItem = originalStateOfInventory.cars_for_sale
                          .map(function (item, idx) { return { id: item.id, idx: idx } })
                          .filter(function (item) { return item.id === querystringObj.invId })[0]

                        // if we did NOT find the querystring inventory_id in the search results collection, 
                        // then we did not arrive here via a click-thru from the /cars-for-sale/results/ page, so we will 
                        // not create the prev|next nav buttons b/c there is not a search results context to paginate through.
                        // We also need to manage the "back to search" button accordingly
                        var backBtn = backToSearchButton.get(back_to_search_btn)
                        if (!currentItem) {
                          // set "back-to-search" button to new verbiage
                          backBtn.text('All Inventory').show()
                          return
                        }
                        else {
                          backBtn.text('Back to Search Results').show()
                        }

                        currentInventoryPointer = calculateInventoryPointer(originalStateOfCriteria.page, currentItem)
                        originalInventoryCount = originalStateOfInventory.inventory_count

                        // check if the inventory pointer falls within the valid range for this context
                        if (pointerIsValidForThisContext(currentInventoryPointer)) {

                          // seems like all is good, so let's create prev|next nav buttons and wireup the click event handlers
                          $navigationContainers
                            .append(tmpls.inventory_nav())
                            .find(button_elem).on(click_event, function (e) {
                              e.preventDefault()

                              var gotoDirection = $(this).data('goto')

                              if (gotoDirection === next_text) {
                                if (pointerIsValidForThisContext(currentInventoryPointer + 1)) {
                                  queryCarByPointer(currentInventoryPointer += 1)
                                }
                              } else if (gotoDirection === prev_text) {
                                if (pointerIsValidForThisContext(currentInventoryPointer - 1)) {
                                  queryCarByPointer(currentInventoryPointer -= 1)
                                }
                              }

                              resetActiveStateOfButtons()
                            })


                          setTimeout(function () {
                            inventory.on(change_event, function (e) {
                              var carsForSale = e.args.cars_for_sale

                              if (carsForSale[length]) {
                                var car = carsForSale[0]
                                updatePageContent(car)

                                var urlpath = vehicle_details_page_path + '?q=' +
                                  car.dealer.id + pipe_symbol +
                                  car.id + pipe_symbol +
                                  querystringObj.zip + pipe_symbol +
                                  car.make_id + pipe_symbol +
                                  car.model

                                win.history.replaceState({}, win.document.title, urlpath)

                                c4sViewedRecentlyWidget
                                  .postCar({
                                    dealerId: car.dealer.id,
                                    vin: car.vin,
                                    id: car.id,
                                    url: win.document.location.href,
                                    imgUrl: car.primary_image_url,
                                    make: car.make,
                                    makeId: car.make_id,
                                    model: car.model,
                                    year: car.year,
                                    city: car.dealer.city,
                                    state: car.dealer.state,
                                    phone: car.dealer.phone,
                                    price: car.price,
                                    mileage: car.mileage
                                  })
                                  .render()

                                offerOverlay.setContext(car)
                                triggerTrackingAndAds(car)
                              }

                              spinner.stop()
                              $('html,body').scrollTop(0)
                            })
                          }, 500)

                          isEnabled = true
                          resetActiveStateOfButtons()
                        }
                      }
                    })
                })
            }
          },

          get: function (propname) {
            if (propname === left_btn_text) {
              return $navigationContainers.find(dot_symbol + prev_text)
            } else if (propname === right_btn_text) {
              return $navigationContainers.find(dot_symbol + next_text)
            }

            return undefined
          }
        }
      })('inv_nav', 'dlr_box', 'feature_grid', 'messaging')


      var offerOverlay = (function () {
        var $leadOverlayBtn = $('.leadoverlay_pop_btn_js')
          , inventoryLeadFormInit = !1 // false
          , hasBeenInitialized = !1 // false

        var updateBygData = function () {
          if (inventory.make && inventory.model && inventory.make.length > 2 && inventory.model.length > 0) {
            ABT.OFR.updateBygMakeModel(inventory.make, inventory.model);
          }
        }

        var wireupInvContactDealerBtn = function () {
          $leadOverlayBtn.on(click_event, function (e) {
            e.preventDefault()

            if (!inventoryLeadFormInit) {
              inventory.quoteButtonSelected = !0 // true

              var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
              $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

              inventoryLeadFormInit = !0
            }
            else {

              // Display form
              var lead_form_prefix = '#LeadForm_'
                , $phoneNum = $('#c4sPhoneNum')

              $(lead_form_prefix + 'NewCar_Info').hide()
              $(lead_form_prefix + 'PersonalInfo').removeClass("col")
              $(lead_form_prefix + 'NewCar_Header').hide()
              $(lead_form_prefix + 'NewCar_Header').text('Contact the Seller')
              $(lead_form_prefix + 'C4S_Header').show()
              $('#SubmitPostButton').text("Contact this Dealer").data('formtype', "c4s")
              $('#ol_ofr_Thankyou').hide()

              if (inventory.hasValidPhoneNum && inventory.dealerPhone && inventory.dealerPhone[length] == 10) {
                var qfPhone = inventory.dealerPhone,
                    qfPhoneSt = '(' + qfPhone.substr(0, 3) + ') ' + qfPhone.substr(3, 3) + '-' + qfPhone.substr(6, 4)

                $phoneNum.text('Call ' + qfPhoneSt + ' or Fill out the form.').show()
              }
              else {
                $phoneNum.text('').hide()
              }

              $('#ol_Leadform').show()
              $('#leadform_overlay_js').popup('show')
            }
          })
        }


        return {
          init: function () {
            if (inventory) {
              wireupInvContactDealerBtn()
              hasBeenInitialized = !0 // true
              updateBygData()
            }
          },

          get: function (propname) {
            if (propname === lead_btn_text) {
              return $leadOverlayBtn
            }

            return undefined
          },

          setContext: function (car) {
            if (!hasBeenInitialized) return

            var dealer = car.dealer
            inventory.id = car.id
            inventory.dealerId = dealer.id
            inventory.vin = car.vin
            inventory.hasAutocheck = !1 // false
            inventory.year = car.year
            inventory.dealerPhone = dealer.phone
            inventory.hasValidPhoneNum = !!dealer.phone[length]
            inventory.make = car.make
            inventory.model = car.model
          }
        }
      })()


      return {
        initAll: function () {
          backToSearchButton.init()
          photoSlider.init()
          inventoryNavigation.init()
          offerOverlay.init()
        },

        //#region /* we are exposing actionable dom elements here for testability */
        get: function (propname) {
          var parts = propname.split(space)
            , component = parts[0]
            , element = parts[1]

          if (component === photo_slider_text) {
            if (element === left_btn_text) {
              return photoSlider.get(left_btn_text)[0]
            } else if (element === right_btn_text) {
              return photoSlider.get(right_btn_text)[0]
            }
          } else if (component === inventory_nav_text) {
            if (element === left_btn_text) {
              return inventoryNavigation.get(left_btn_text)[0]
            } else if (element === right_btn_text) {
              return inventoryNavigation.get(right_btn_text)[0]
            }
          } else if (component === page_text) {
            if (element === lead_btn_text) {
              return offerOverlay.get(lead_btn_text)[0]
            } else if (element === back_to_search_btn) {
              return backToSearchButton.get(back_to_search_btn)[0]
            }
          }

          return undefined
        }
        //#endregion
      }
    })()

    return {
      init: function () {

        $.when(
            $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('slider')),
            $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('evmove')),
            personaService.isReady()
          )
          .then(function () {
            pageComponents.initAll()
          })

        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4svwrecwdg'))))
          .then(function () {
            var car = abt.pageJson.inventory
            c4sViewedRecentlyWidget = abt.WDG.c4s_viewedrecently

            c4sViewedRecentlyWidget
              .postCar({
                dealerId: car.dealerId,
                vin: car.vin,
                id: car.id,
                url: win.document.location.href,
                imgUrl: car.primaryImage,
                make: car.make,
                makeId: car.makeId,
                model: car.model,
                year: car.year,
                city: car.city,
                state: car.state,
                phone: car.dealerPhone,
                price: formatNumber(car.price),
                mileage: formatNumber(car.mileage)
              })
              .init()
          })

        abt.PAGE.element = {
          get: pageComponents.get
        }
      }



    }
  })()

  var setupGoogleMaps = function () {
    var dealer = $('#js_dealer_map');
    var map = new google.maps.Map($('#map-canvas').get(0), {
      zoom: 15,
      center: new google.maps.LatLng(dealer.data("latitude"), dealer.data("longitude")),
      mapTypeControl: false,
      panControl: false,
      zoomControl: false,
      scaleControl: true,
      streetViewControl: false,
      mapTypeId: google.maps.MapTypeId.ROADMAP
    });
    setTimeout(function () {
      var marker = new google.maps.Marker({
        position: new google.maps.LatLng(dealer.data("latitude"), dealer.data("longitude")),
        map: map,
        draggable: false,
        animation: google.maps.Animation.DROP
      });
    }, 100);
  };

  $(function () {
    $.when(
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup'))
        .then(function () {
          page.init();
          setupGoogleMaps();
        }));
  })
})(window, jQuery, ABT, []._)