/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */


'use strict';

!function (win, $, abt) {
  var env = abt.ENV
    , getJsFileNameByAlias = env.getJsFileNameByAlias
    , getScriptUriByName = env.getScriptUriByName
    , libsPath = env.libsPath
    , personaService = abt.PERSONA
    , zipRegex = /^[0-9]+$/
    , $body = $('body')

  var bootstrap = function () {
    var $sidepanelcontainer = $('#sidepanel_wrap_js')
      , tmpls = require('./offerbootstrap_tmpls/desk/tmpls.js')
      , sidePanelLeadFormInit = false
      , click_event = 'click'
      , relative_str = 'relative'
      , hidden_str = 'hidden'
      , wide_str = 'wide'
      , ppc_str = 'ppc'

    var displayC4SLeadForm = function () {
      if (!sidePanelLeadFormInit) {
        var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
        $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

        sidePanelLeadFormInit = true
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
    }

    var displayMultiC4SLeadForm = function () {
      if (!sidePanelLeadFormInit) {
        var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
        $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

        sidePanelLeadFormInit = true
      } else {

        // Display form
        $('#LeadForm_NewCar_Info').css("display", "none");
        $('#LeadForm_PersonalInfo').removeClass("col");
        $('#LeadForm_NewCar_Header').css("display", "none");
        $('#LeadForm_C4S_Header').text('Contact the Sellers');
        $('#LeadForm_C4S_Header').css("display", "block");
        $('#SubmitPostButton').text("Contact Dealers").attr('data-formtype', "multic4s");
        $('#ol_ofr_Thankyou').css("display", "none");
        $('#c4sPhoneNum').text('').css("display", "none");
        $('#ol_Leadform').css("display", "block");
        $('#leadform_overlay_js').popup('show');
      }
    }

    var wireupOfferPanel = function () {

      var $offerPanelContainer = $sidepanelcontainer.find('#offerpanel_wrap_js')
        , $sidenav = $('.navigation')

      $('#pno_btn_js').on(click_event, function (e) {
        e.preventDefault()

        if (!sidePanelLeadFormInit) {
          var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
          $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

          sidePanelLeadFormInit = true
        }

        $('.navigation').toggleClass(relative_str)
        $offerPanelContainer.parent().toggleClass(hidden_str)

        if ($('#sp_Special_Offers').html().length > 20) {
          $offerPanelContainer.toggleClass(wide_str)
        }
      })

      $offerPanelContainer
        .find('#offerpanel_close_btn_js').on(click_event, function (e) {
          e.preventDefault()

          $sidenav.toggleClass(relative_str)
          $offerPanelContainer.parent().toggleClass(hidden_str)
          $offerPanelContainer.removeClass(wide_str)
        })
    }

    var editNavZip = function () {
      //#region - Scoped Variables
      var $zipOverlay
        , $zipPopupBtn
        , $zip
        , strValidChars = "0123456789"

      //#endregion


      return {
        init: function () {
          $body.append(tmpls.nav_zip_overlay());

          //#region Cache DOM Elements
          $zipOverlay = $('#nav_zip_overlay_js');
          $zipPopupBtn = $('#js_sitenav_zipcode_btn');
          $zip = $zipOverlay.find('#nav_zip_js');
          //#endregion

          //#region - Setup UI event handlers

          //  setup zip input validation
          $zip.keyup(function (e) {
            var zipcodeVal = $(this).val();

            var zipLen = zipcodeVal.length;
            var zipLastChar = "";
            var newZipCodeVal = "";

            if (zipLen > 0) {
              zipLastChar = zipcodeVal.charAt(zipLen - 1);
              if (strValidChars.indexOf(zipLastChar) == -1) {
                newZipCodeVal = zipcodeVal.replace(/[^0-9]+/g, '');
                $(this).val(newZipCodeVal);
              }
            }
          });


          // overlay form submission event
          $zipOverlay.find('form')
            .on('submit', function (e) {
              var zip = $zip.val()

              e.preventDefault()

              // capture the user-provided zip in the site-wide persona storage

              if (zip != null && zip.length == 5 && zip.match(zipRegex)) {
                personaService.set('zipcode', zip);
                personaService.set({ zcverified: true });
                ABT.OFR.updateNavZip(zip);
              }

              $zipOverlay.popup('hide')
            })


          // click event that opens this overlay
          $zipPopupBtn
            .on(click_event, function () {
              $zip.val(personaService.get().zipcode);
              $zipOverlay.popup('show')
            })


          // click event that closes this overlay
          $zipOverlay.find('.close_btn_js')
            .on(click_event, function () {
              $zipOverlay.popup('hide')
            })

          //#endregion
        },

        popup: function () {
          $zip.val(personaService.get().zipcode);
          $zipOverlay.popup('show');
        }
      }
    }()

    var _updateNavZip = function (zipcode) {
      if (zipcode == null || zipcode.length != 5 || !zipcode.match(zipRegex)) {
        console.log('You must pass a valid zipcode to updateNavZip.');
        return;
      }

      $('#js_sitenav_zipcode').text(zipcode);
    }



    var _offerPopup = function (data, isC4S) {
      if ((!$.isArray(data)) && (!($.type(data) === "object"))) {
        console.log('You must pass an object or array of objects for first param to offerPopup().');
        return;
      }

      var pageJson = ABT.pageJson;
      if (!pageJson) {
        ABT.pageJson = {};
        pageJson = ABT.pageJson;
      }

      // Cars For Sale Offers Process
      if (isC4S) {
        // Cars For Sale with multiple cars selected
        if (($.isArray(data)) && data.length > 1) {
          //console.log('MultiC4S');
          pageJson.multiInventory = [];
          for (var x = 0; x < data.length; x++) {
            pageJson.multiInventory[x] = {
              id: data[x].id,
              vin: data[x].vin,
              make: data[x].make,
              model: data[x].model,
              year: data[x].year,
              dealerId: data[x].dealerId,
              dealerPhone: data[x].dealerPhone,
              hasValidPhoneNum: data[x].hasValidPhoneNum,
              quoteButtonSelected: true
            };
          } // End Loop through data
          displayMultiC4SLeadForm();
          // Cars For Sale with single car selected
        } else {
          //console.log('C4S');
          pageJson.inventory = {
            id: data[0].id,
            make: data[0].make,
            model: data[0].model,
            year: data[0].year,
            dealerId: data[0].dealerId,
            dealerPhone: data[0].dealerPhone,
            hasValidPhoneNum: data[0].hasValidPhoneNum,
            quoteButtonSelected: true
          };
          displayC4SLeadForm();
        }
        // New Car Offers Process
      } else {
        //console.log('New');
      }

    }

    var checkForSemHash = function () {
      var hashItems = location.hash.replace('#', '').split('&')
        , hashKeys = []
        , hashValues = []

      for (var hiIndex in hashItems) {
        var keyItems = hashItems[hiIndex].split('=')

        if (keyItems.length == 2) {
          hashKeys.push(keyItems[0])
          hashValues.push(keyItems[1])
        }
      }

      if (hashKeys.indexOf(ppc_str) >= 0) {
        if (hashValues[hashKeys.indexOf(ppc_str)] == 1) {
          if (!sidePanelLeadFormInit) {
            var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
            $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

            sidePanelLeadFormInit = true
          }
        }
      }
    }

    var showNavZipcode = function (currentZip) {
      //console.log('Showing Nav Zip');
      $('#js_sitenav_zipcode').text(currentZip);
      $('#js_sitenav_zipcode_btn').css('display', 'block');
    }

    var checkAndSetZipcode = function () {
      var pData = personaService.get(),
        currentZip = pData.zipcode;

      //console.log('current zip = ' + currentZip);
      if (currentZip == null || currentZip.length != 5 || !currentZip.match(zipRegex)) {
        var locationUrl = '/api/geolocation/ip/';
        $.get(locationUrl).done(function (resp) {
          if (resp.data) {
            //console.log(resp.data);
            if (resp.data.postalcode && resp.data.postalcode.length == 5 && resp.data.postalcode.match(zipRegex)) {
              currentZip = resp.data.postalcode;
              personaService.set({ zipcode: currentZip });
              showNavZipcode(currentZip);
            }
          }
        });
      } else {
        showNavZipcode(currentZip);
      }

    }


    return {
      init: function () {
        $sidepanelcontainer.prepend(tmpls.add_side_panel())
        wireupOfferPanel()
        checkForSemHash()
        checkAndSetZipcode()
        editNavZip.init()

        abt.UTILS.mergeObjs(abt.OFR, { offerPopup: function (data, isC4S) { _offerPopup(data, isC4S) } })
        abt.UTILS.mergeObjs(abt.OFR, { updateNavZip: function (zipcode) { _updateNavZip(zipcode) } })
      }
    }
  }()


  $.when(
      $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup')),
      personaService.isReady()
    )
    .then(function () {
      bootstrap.init()
    })
}(window, jQuery, ABT)