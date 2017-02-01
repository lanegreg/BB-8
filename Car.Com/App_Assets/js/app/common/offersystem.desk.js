/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */


'use strict';

!function(win, $) {
  //#region - Module level vars and minification enhancements
  var abt = ABT
    , env = abt.ENV
    , omni = abt.TRK.omni
    , omniCallbacks = omni.callback
    , trackOmniture = omni.track
    , getJsFileNameByAlias = env.getJsFileNameByAlias
    , libsPath = env.libsPath
    , affiliateid = abt.TRK.sessionCtx.trackMeta.affiliate.id
    , carsforsaleService = require('../../modules/abt-svc-c4s')
    , tmpls = require('./offersystem_tmpls/desk/tmpls.js')
    , setDefaultMake = true
    , setDefaultModel = false
    , setDefaultTrim = false
    , setSemDefault = false
    , setSemModel = false
    , autoSubmitSem = false
    , setGetaQuote = false
    , skipSidePanel = false
    , autoSubmitGetaQuote = false
    , semKeys = ["make", "model", "year", "zipcode"]
    , semValues = ["", "", "", ""]
    , semSeoValues = ["", "", "", ""]
    , $body = $('body')
    , $spLeadFormContainer = $('#sp_NewCar_Lead')
    , $spC4SLinkContainer = $('#sp_c4s_Lead')
    , $spC4SOfferLinks = $('#sp_c4s_offer_links')
    , $spOffersZipForm = $('#sp_OfferZip')
    , $overlayLeadFormContainer = ''
    , $overlayLeadFormNewCarInfo = ''
    , $overlayLeadFormPersonalInfo = ''
    , $overlayLeadFormNewCarHeader = ''
    , $overlayLeadFormC4SHeader = ''
    , $overlayLeadFormC4SPhone = ''
    , $submitLeadBtn = ''
    , $overlayThankyouContainer = ''
    , $overlayTyAutowebContainer = ''
    , $overlayTyLeadInfoContainer = ''
    , $overlayTySimilarContainer = ''
    , $overlayTySimilarContent = ''
    , overlayLeadFormInit = false
    , sidePanelAutowebLoaded = false
    , $leadformOverlay = ''
    , $DealerList = ''
    , personaService = abt.PERSONA
    , currentZip = ''
    , zipcodeVerified = false
    , displayedOlAutoweb = false
    , $ncLeadModelSelect = ''
    , $ncLeadModelSelect2 = ''
    , $ncLeadTrimSelect = ''
    , $ncLeadTrimSelect2 = ''
    , $ncLeadAddSelectedMake = ''
    , $ncLeadAddSelectedModel = ''
    , $ncLeadSelectedTrim = ''
    , $sidePanel = ''
    , $overlayCloseBtn = ''
    , $pnlGetPricingBtn
    , $step1GetPricingBtn
    , zipRegex = /^[0-9]+$/
    , cssDisplay = "display"
    , cssDisplayNone = "none"
    , cssDisplayBlock = "block"
    , action_str = 'action'

  //#endregion


  var offersystem = {

    init: function() {
      //#region - Handlers for user-action omni-tracking
      var colon_str = ':',
          clicked_str = '_clicked'

      omniCallbacks.trackDealers = function(e) {
        var checkboxes = e.$this.closest('li').siblings().find('[name^=dealerchkbox]'),
            message = e.page + colon_str + e.target + clicked_str + '(dlrs-showing:' + checkboxes.length + ')'
        trackOmniture(action_str, message)
      }

      omniCallbacks.trackAutoweb = function(e) {
        var url = e.$this.find('.aw_url_js').text(),
            message = e.page + colon_str + e.target + clicked_str + '(ad-url:' + url + ')'
        trackOmniture(action_str, message)
      }

      omniCallbacks.trackLeadSubmission = function(e) {
        var selected = $('[name^=dealerchkbox]:checked'),
            message = e.page + colon_str + e.target + clicked_str + '(dlrs-selected:' + selected.length + ')'
        trackOmniture(action_str, message)
      }

      //#endregion

      var pData = personaService.get();
      currentZip = pData.zipcode;
      zipcodeVerified = personaService.get().zcverified;

      $spLeadFormContainer.empty().prepend(tmpls.sp_mmy_form({}));
      offersystem.wireupSidePanelZipform();

      if (!overlayLeadFormInit) {
        $body.append(tmpls.add_offers_overlay({}));
        overlayLeadFormInit = true;
      }
      // Wireup inputs and id's for SidePanel and Overlay Forms
      offersystem.wireupLeadformOverlay();
      offersystem.wireupNewCarLeadForm(),
        offersystem.wireupZipInputs();
      // Side Panel Offers use Page Context (Make, Model, Zip, ....)
      offersystem.LoadSPOffers();
      // Check for entry by Inventory Lead Form Request
      offersystem.checkC4SQuoteSelect();
      // Check for entry by MultiSelect C4S Lead Form Request
      offersystem.checkMultiC4SQuoteSelect();
      // Check for SEM Parameters
      offersystem.checkSemLink();
      // Check for entry by Get a Quote Submit Button
      offersystem.checkGetAQuoteSelect();

      $.when(
          $.get('/api/leadengine/makes/').done(function(leadMakelistResp) {
            if (leadMakelistResp.data) {
              $('#ncLeadMakeSelect').empty().prepend(tmpls.make_select_options({ makeArr: leadMakelistResp.data }));
              $('#ncLeadMakeSelect2').empty().prepend(tmpls.make_select_options({ makeArr: leadMakelistResp.data }));
            }
            else {
              // Need To Error Log Make List Error
            }
          })
        )
        .then(function() {
          // New Car Lead Default Submit must follow completion of make list population
          offersystem.checkAutoShowNewCarLeadForm();
        })
        .then(function() {
          // Will Display Auto New Car Lead Form Overlay for SEM Entry pages and/or Get A Quote Button Submit
          offersystem.displayAutoNewCarLeadForm();
        }); // End of when/then function

    }, // End init function

    wireupSidePanelZipform: function() {
      $('#SetOffersZip').on('click', function(e) {
        e.preventDefault();

        var zipcode = $("#offersZipcode").val();
        if (zipcode.length < 5) {
          $('#offersZipErrorMessage').css(cssDisplay, cssDisplayBlock);
        }
        else {
          currentZip = zipcode;
          $spOffersZipForm.css(cssDisplay, cssDisplayNone);
          offersystem.LoadSPOffers();
        }
      });

    },

    wireupLeadformOverlay: function() {
      $leadformOverlay = $('#leadform_overlay_js');
      $leadformOverlay.popup({ blur: false, escape: false });

      // Wireup Lead Form Overlay Close Button to show Autoweb Ads
      $overlayCloseBtn = $('#close_ovl_btn_js'),
        $overlayCloseBtn.on('click', function(e) {
          e.preventDefault();
          // The first time the Lead Form is closed, we want to display Autoweb Ads, if available
          if (currentZip !== null && currentZip.length == 5 && currentZip.match(zipRegex)) {
            offersystem.ShowAutowebAds();
          }
          else {
            $leadformOverlay.popup('hide');
          }
        });

      // Wireup Lead Thankyou page similar lead dealer list close button
      $('#js_close_lead_thankyou_dealerlist').on('click', function (e) {
        e.preventDefault();
        $('.sel-dlr-ovl-outer').css(cssDisplay, cssDisplayNone);
      });

      // Wireup Section containers on Leadform Overlay
      $overlayLeadFormContainer = $('#ol_Leadform');
      $overlayThankyouContainer = $('#ol_ofr_Thankyou');
      $overlayTyAutowebContainer = $('#ol_ofr_TY_AutowebAds');
      $overlayTyLeadInfoContainer = $('#ol_ofr_TY_LeadInfo');
      $overlayTySimilarContainer = $('#ol_ofr_TY_Similar');
      $overlayTySimilarContent = $('#js_lead_similar_vehicles_content');

      $overlayLeadFormNewCarInfo = $('#LeadForm_NewCar_Info');
      $overlayLeadFormPersonalInfo = $('#LeadForm_PersonalInfo');
      $overlayLeadFormNewCarHeader = $('#LeadForm_NewCar_Header');
      $overlayLeadFormC4SHeader = $('#LeadForm_C4S_Header');
      $overlayLeadFormC4SPhone = $('#c4sPhoneNum');
      $submitLeadBtn = $('#submit_lead_btn_js');

      $('#HParea').keyup(function(e) {
        offersystem.validatePhoneArea();
      });

      $('#HPpre').keyup(function(e) {
        offersystem.validatePhonePrefix();
      });

      $('#HPphone').keyup(function(e) {
        offersystem.validatePhoneUCNumber();
      });

      if (!zipcodeVerified) {
        $('#zipcodeConfirmMessage').css(cssDisplay, cssDisplayBlock);
        $('#zipcodeConfirmMessage2').css(cssDisplay, cssDisplayBlock);
      }


    },

    wireupNewCarLeadForm: function() {
      var $zipcode = $('#ncLeadZipcode'),
          $zipcode2 = $('#ncLeadZipcode2');

      $sidePanel = $('#js_SidePanel');
      $pnlGetPricingBtn = $('#get_pricing_btn_js');
      $step1GetPricingBtn = $('#stp1_get_pricing_btn_js');

      if (currentZip !== null && currentZip.length == 5 && currentZip.match(zipRegex)) {
        $zipcode.val(currentZip);
        $zipcode2.val(currentZip);
      }

      $ncLeadModelSelect = $('#ncLeadModelSelect');
      $ncLeadModelSelect2 = $('#ncLeadModelSelect2');
      $ncLeadTrimSelect = $('#ncLeadTrimSelect');
      $ncLeadTrimSelect2 = $('#ncLeadTrimSelect2');

      $('#ncLeadMakeSelect').on('change', function(e) {
        offersystem.updateAllModelDropdowns();
      });

      $('#ncLeadMakeSelect2').on('change', function(e) {
        var newMakeVal = $('#ncLeadMakeSelect2').val();
        $("#ncLeadMakeSelect option[value='" + newMakeVal + "']").prop('selected', true);
        offersystem.updateAllModelDropdowns();
      });

      $ncLeadModelSelect.on('change', function(e) {
        offersystem.updateAllTrimDropdowns();
      });

      $ncLeadModelSelect2.on('change', function(e) {
        var newModelVal = $ncLeadModelSelect2.val();
        $("#ncLeadModelSelect option[value='" + newModelVal + "']").prop('selected', true);
        offersystem.updateAllTrimDropdowns();
      });

      $ncLeadTrimSelect.on('change', function(e) {
        var newTrimVal = $ncLeadTrimSelect.val();
        $("#ncLeadTrimSelect2 option[value='" + newTrimVal + "']").prop('selected', true);
      });

      $ncLeadTrimSelect2.on('change', function(e) {
        var newTrimVal = $ncLeadTrimSelect2.val();
        $("#ncLeadTrimSelect option[value='" + newTrimVal + "']").prop('selected', true);
      });

      $pnlGetPricingBtn.on('click', function(e) {
        e.preventDefault();
        offersystem.getDealers();
      });

      $step1GetPricingBtn.on('click', function(e) {
        e.preventDefault();
        $('#ncLeadZipcode').val($('#ncLeadZipcode2').val());
        offersystem.getDealers();
      });

      $DealerList = $('#DealerList'),
        $submitLeadBtn.on('click', function(e) {
          e.preventDefault();
          offersystem.PostToQuoteSubmitted($(this));
        });

    },

    wireupZipInputs: function() {
      var $zipcode = $('#ncLeadZipcode'),
          $zipcode2 = $('#ncLeadZipcode2'),
          $zipcode3 = $('#offersZipcode'),
          strValidChars = "0123456789";

      $('.js_offers_zipcode').each(function() {

        $(this).keyup(function(e) {
          var zipcodeVal = $(this).val();

          var zipLen = zipcodeVal.length;
          var zipLastChar = "";
          var newZipCodeVal = "";

          if (zipLen > 0) {
            zipLastChar = zipcodeVal.charAt(zipLen - 1);
            if (strValidChars.indexOf(zipLastChar) == -1) {
              newZipCodeVal = zipcodeVal.replace(/[^0-9]+/g, '');
              zipLen = newZipCodeVal.length;
              $(this).val(newZipCodeVal);
            }
          }
          zipcodeVal = $(this).val();

          if (zipLen == 5) {
            $zipcode.val(zipcodeVal);
            $zipcode2.val(zipcodeVal);
            $zipcode3.val(zipcodeVal);
            personaService.set({ zipcode: zipcodeVal });
          }
        });
      });
    },

    LoadSPOffers: function() {
      var selectedMake = abt.OFR.pageCtx.make ? abt.OFR.pageCtx.make : "",
          selectedModel = abt.OFR.pageCtx.model ? abt.OFR.pageCtx.model : "",
          selectedZip = currentZip;

      // If we are on Cars for Sale/ Vehicle Details page, we show Contact Dealer Button instead of New Car Lead Form
      if (location.href.indexOf("cars-for-sale/vehicle-details") > -1) {
        $spLeadFormContainer.css(cssDisplay, cssDisplayNone);
        offersystem.wireupSPC4SDealerButton();
        $spC4SOfferLinks.empty();
        offersystem.addSPC4SFinancLink(selectedMake, selectedModel, selectedZip);
        offersystem.addSPC4SCreditKarmaLink();
        offersystem.addSPC4SAutoCheckLink();
        $spC4SLinkContainer.css(cssDisplay, cssDisplayBlock);
      }

      if (selectedMake.length > 0 && selectedZip.length == 5) {
        // If we have Make and Zip we will get Cars for Sale Vehicles
        offersystem.LoadSPc4sInventoryLink(selectedMake, selectedModel, selectedZip);

        // If we have Make,Model and Zip we will get Autoweb Ads for the Side Panel
        if (selectedMake.length > 0 && selectedModel.length > 0 && selectedZip.length == 5) {
          offersystem.LoadSPAutowebAds(selectedMake, selectedModel, selectedZip);
          sidePanelAutowebLoaded = true;
        }
        // If we have Make and no Zip, we can display Zipcode form to refresh Cars for Sale and Autoweb Ads on update
      }
      else if (selectedMake.length > 0) {
        $spOffersZipForm.css(cssDisplay, cssDisplayBlock);
      }
    },

    addSPC4SFinancLink: function(selectedMake, selectedModel, selectedZip) {
      $spC4SOfferLinks.append('<p><a href="http://finance.car.com/?src=3072089&zipcode=' + selectedZip + '" class="btn-primary lg" style="width: 225px;" target="_blank">Get Easy Financing on ' + selectedMake + ' ' + selectedModel + '</a></p>');
    },

    addSPC4SCreditKarmaLink: function() {
      $spC4SOfferLinks.append('<p><a href="https://www.creditkarma.com/partner?id=Car&ovmtc=car&ovkey=none" class="btn-primary lg" style="width: 225px;" target="_blank">Get Your Free Credit Score</a></p>');
    },

    addSPC4SAutoCheckLink: function() {
      if (ABT.pageJson && ABT.pageJson.inventory && ABT.pageJson.inventory.vin) {
        if (ABT.pageJson.inventory.hasAutocheck) {
          $spC4SOfferLinks.append('<p><a title="See The Free Vehicle History Report" data-vin="' + ABT.pageJson.inventory.vin + '" href="#" class="btn-primary lg js_free_vin_report" style="width: 225px;">See The Free Vehicle History Report</a></p>');
        }
        else {
          $spC4SOfferLinks.append('<p><a title="Get The Vehicle History Report" data-vin="' + ABT.pageJson.inventory.vin + '" href="#" class="btn-primary lg js_vin_report" style="width: 225px;">Get The Vehicle History Report</a></p>');
        }
        // Wireup the buttons
        $('.js_vin_report').click(function(e) {
          e.preventDefault();
          var $this = $(this),
              vin = $this.data('vin');
          window.open('http://www.autocheck.com/consumers/gatewayAction.do?WT.mc_id=1826&nf=3&nf=4&siteID=1826&lpID=1826&vin=' + vin,
            'usedVehicleHistory',
            'toolbar=1, location=0, directories=0, status=1, menubar=0, scrollbars=1, resizable=1, copyhistory=0, width=1024, height=500, screenX=50, screenY=100, left=150, top=100');
        });

        $('.js_free_vin_report').click(function(e) {
          e.preventDefault();
          offersystem.LoadSPAutoCheck(ABT.pageJson.inventory.id, ABT.pageJson.inventory.dealerId);
        });
      }
    },

    wireupSPC4SDealerButton: function() {
      $('.js_sp_contact_dealer_btn').click(function(e) {
        e.preventDefault();
        offersystem.displayC4SLeadForm();
      });
    },

    LoadSPAutoCheck: function(vId, dId) {
      if (vId > 0 && dId > 0) {
        try {
          var url = '/api/research/freeautocheck/';
          var jsonString = "{dealerId: '" + dId + "', inventoryId: '" + vId + "'}";
          $.post(url, { '': jsonString }).done(function(resp) {
            if (resp) {
              var autoCheckWindow = window.open('', 'freeUsedVehicleHistory',
                'toolbar=1, location=0, directories=0, status=1, menubar=0, scrollbars=1, resizable=1, copyhistory=0, width=1024, height=500, screenX=50, screenY=100, left=150, top=100');
              $(autoCheckWindow.document.body).ready(function() {
                $(autoCheckWindow.document.body).append(resp.data.autocheckdata);
              });
            }
          });
        }
        catch (e) {
          //
        }
      }

    },

    LoadSPc4sInventoryLink: function(selectedMake, selectedModel, selectedZip) {
      var carData = { makeModel: '', count: '', zipcode: selectedZip },
          carButtonData = { makeId: '', makeModel: '' };
      // We will not show the Cars 4 Sale link on the C4S Vehicle Detail page   
      if (selectedMake.length > 0 && selectedZip.length == 5 && location.href.indexOf("cars-for-sale/vehicle-details") < 0) {
        try {
          $.when(
              carsforsaleService.ping({ "make": selectedMake, "model": selectedModel, "zipcode": selectedZip }))
            .then(function(data) {
              //console.log(data);
              //ABT.pageJson.getaquote.quoteButtonSelected = data;
              if (data.availability.new_cars.by_make.count > 0 || data.availability.used_cars.by_make.count) {
                if (data.availability.new_cars.by_make_model.count + data.availability.used_cars.by_make_model.count > 1) {
                  carData.makeModel = selectedMake + ' ' + data.ping_ctx.model;
                  carData.count = data.availability.new_cars.by_make_model.count + data.availability.used_cars.by_make_model.count;
                  carButtonData.makeId = '';
                  carButtonData.makeModel = data.ping_ctx.make_id + '~' + data.ping_ctx.model;
                }
                else {
                  carData.makeModel = selectedMake;
                  carData.count = data.availability.new_cars.by_make.count + data.availability.used_cars.by_make.count;
                  carButtonData.makeId = data.ping_ctx.make_id;
                  carButtonData.makeModel = '';
                }
                if (carData.makeModel.slice(-1) != 's' && carData.makeModel.slice(-1) != 'z') {
                  carData.makeModel = carData.makeModel + "'s";
                }
                $('#sp_c4s_inventory_link').empty().prepend(tmpls.sp_c4s({
                  carData: carData
                })).css(cssDisplay, cssDisplayBlock);
                //console.log(carButtonData);
                offersystem.wireupSPC4sButton(carButtonData);
              }
            });

        }
        catch (e) {
          //
        }
      }
    },

    wireupSPC4sButton: function(carButtonData) {
      $('#js_sp_c4s_btn').on('click', function(e) {
        e.preventDefault();
        carsforsaleService.criteria.set({
          filters: {
            makes: carButtonData.makeId,
            make_models: carButtonData.makeModel
          }
        });
        window.location = "/cars-for-sale/results/";
      });
    },

    LoadSPAutowebAds: function(selectedMake, selectedModel, selectedZip) {
      // Get Autoweb for Side Panel if we have make model and zip and not on the cars for sales pages
      var adData = { Make: selectedMake, Model: selectedModel, Location: '', adList: '' };

      if (selectedMake.length > 0 && selectedModel.length > 0 && selectedZip.length == 5 && location.href.indexOf("cars-for-sale") < 0) {
        try {
          var locationUrl = '/api/leadengine/citystate/' + selectedZip + '/';
          $.get(locationUrl).done(function(resp) {
            if (resp.data) {
              adData.Location = resp.data[0].city + ', ' + resp.data[0].state_abb;
            }
          });
          var url = '/api/research/getautowebads/';
          var jsonString = "{Make: '" + selectedMake + "', Model: '" + selectedModel + "', Zip: '" + selectedZip + "', PublisherCampaignId: '" + '191' + "'}";
          $.post(url, { '': jsonString }).done(function(resp) {
            if (resp.data && resp.data.autowebadjson) {
              var adInfo = JSON.parse(resp.data.autowebadjson);
              if (adInfo.Listings && adInfo.Listings.length > 0) {
                adData.adList = adInfo;
                $('#sp_Special_Offers').empty().prepend(tmpls.sp_autoweb({ adData: adData })).css(cssDisplay, cssDisplayBlock);
                if (!skipSidePanel) {
                  skipSidePanel = false;
                  $sidePanel.addClass('wide');
                }
              }
            }
          });
        }
        catch (e) {
          //
        }
      }
    },

    checkC4SQuoteSelect: function() {
      if (ABT.pageJson && ABT.pageJson.inventory && ABT.pageJson.inventory.quoteButtonSelected && ABT.pageJson.inventory.quoteButtonSelected == true) {
        ABT.pageJson.inventory.quoteButtonSelected = false;
        offersystem.displayC4SLeadForm();
      }
    },

    displayC4SLeadForm: function() {
      var year = ABT.pageJson.inventory.year,
          make = ABT.pageJson.inventory.make,
          model = ABT.pageJson.inventory.model;
      
      offersystem.setC4STrimIdforSimilar(make, model, year);
      // Display form
      $overlayLeadFormNewCarInfo.css(cssDisplay, cssDisplayNone);
      offersystem.updateLeadFormCityState();
      $overlayLeadFormPersonalInfo.removeClass("col");
      $overlayLeadFormNewCarHeader.css(cssDisplay, cssDisplayNone);
      $overlayLeadFormC4SHeader.css(cssDisplay, cssDisplayBlock);
      $submitLeadBtn.text("Contact this Dealer").data('formtype', "c4s");
      $overlayThankyouContainer.css(cssDisplay, cssDisplayNone);
      if (ABT.pageJson.inventory.hasValidPhoneNum && ABT.pageJson.inventory.dealerPhone && ABT.pageJson.inventory.dealerPhone.length == 10) {
        var qfPhone = ABT.pageJson.inventory.dealerPhone;
        var qfPhoneSt = '(' + qfPhone.substr(0, 3) + ') ' + qfPhone.substr(3, 3) + '-' + qfPhone.substr(6, 4);
        $overlayLeadFormC4SPhone.text('Call ' + qfPhoneSt + ' or Fill out the form.').css(cssDisplay, cssDisplayBlock);
      }
      else {
        $overlayLeadFormC4SPhone.text('').css(cssDisplay, cssDisplayNone);
      }
      $overlayLeadFormContainer.css(cssDisplay, cssDisplayBlock);
      $leadformOverlay.popup('show');
      // Load Overlay Autoweb Ads if we have a make and model
      if (ABT.pageJson.inventory.make && ABT.pageJson.inventory.make.length > 0 && ABT.pageJson.inventory.model && ABT.pageJson.inventory.model.length) {
        offersystem.LoadOLAutowebAds(ABT.pageJson.inventory.make, ABT.pageJson.inventory.model);
      }
      else {
        $overlayTyAutowebContainer.css(cssDisplay, cssDisplayNone);
        $overlayThankyouContainer.addClass("single");
        offersystem.RemoveAutowebFromCloseButton();
      }
    },

    checkMultiC4SQuoteSelect: function () {
      if (ABT.pageJson && ABT.pageJson.multiInventory && ($.isArray(ABT.pageJson.multiInventory)) && ABT.pageJson.multiInventory[0].quoteButtonSelected && ABT.pageJson.multiInventory[0].quoteButtonSelected == true) {
        ABT.pageJson.multiInventory[0].quoteButtonSelected = false;
        offersystem.displayMultiC4SLeadForm();
      }
    },

    displayMultiC4SLeadForm: function () {
      // We need to get the vehicle from the most recent year for similar vehicle 
      var mostRecentIndx = 0,
          mostRecentYear = 1900;
      for (var x = 0; x < ABT.pageJson.multiInventory.length; x++) {
        if (ABT.pageJson.multiInventory[x].year > mostRecentYear) {
          mostRecentYear = ABT.pageJson.multiInventory[x].year;
          mostRecentIndx = x;
        }
      }

      var year = ABT.pageJson.multiInventory[mostRecentIndx].year,
          make = ABT.pageJson.multiInventory[mostRecentIndx].make,
          model = ABT.pageJson.multiInventory[mostRecentIndx].model;
      
      offersystem.setC4STrimIdforSimilar(make, model, year);
      // Display form
      $overlayLeadFormNewCarInfo.css(cssDisplay, cssDisplayNone);
      offersystem.updateLeadFormCityState();
      $overlayLeadFormPersonalInfo.removeClass("col");
      $overlayLeadFormNewCarHeader.css(cssDisplay, cssDisplayNone);
      $overlayLeadFormC4SHeader.text('Contact the Sellers');
      $overlayLeadFormC4SHeader.css(cssDisplay, cssDisplayBlock);
      $submitLeadBtn.text("Contact Dealers").data('formtype', "multic4s");
      $overlayThankyouContainer.css(cssDisplay, cssDisplayNone);
      $overlayLeadFormC4SPhone.text('').css(cssDisplay, cssDisplayNone);
      $overlayLeadFormContainer.css(cssDisplay, cssDisplayBlock);
      $leadformOverlay.popup('show');
      // Load Overlay Autoweb Ads if we have a make and model, Autoweb Ads will be on popup post thankyou close for multi c4s
      $overlayThankyouContainer.addClass("single");

      if (ABT.pageJson.multiInventory[mostRecentIndx].make && ABT.pageJson.multiInventory[mostRecentIndx].make.length > 0 && ABT.pageJson.multiInventory[mostRecentIndx].model && ABT.pageJson.multiInventory[mostRecentIndx].model.length) {
        offersystem.LoadOLAutowebAds(ABT.pageJson.multiInventory[mostRecentIndx].make, ABT.pageJson.multiInventory[mostRecentIndx].model);
      }
      else {
        $overlayTyAutowebContainer.css(cssDisplay, cssDisplayNone);
        offersystem.RemoveAutowebFromCloseButton();
      }
    },


    checkSemLink: function() {
      var hashItems = location.hash.replace('#', '').split('&'),
          keyItems = [],
          hashKeys = [],
          hashValues = [],
          find = ' ',
          re = new RegExp(find, 'g');

      for (var hiIndex in hashItems) {
        keyItems = hashItems[hiIndex].split('=');
        if (keyItems.length == 2) {
          hashKeys.push(keyItems[0].toLowerCase());
          hashValues.push(keyItems[1]);
        }
      }

      if (hashKeys.indexOf("ppc") >= 0) {
        if (hashValues[hashKeys.indexOf("ppc")] == 1) {
          skipSidePanel = true;
          setSemDefault = true;
          setDefaultMake = false;

          for (var kIndex in semKeys) {
            if (hashKeys.indexOf(semKeys[kIndex]) >= 0) {
              semValues[kIndex] = hashValues[hashKeys.indexOf(semKeys[kIndex])];
              semSeoValues[kIndex] = hashValues[hashKeys.indexOf(semKeys[kIndex])].toLowerCase().replace(re, '-');
            }
          }
        }
      }


    },

    checkGetAQuoteSelect: function() {
      if (ABT.pageJson && ABT.pageJson.getaquote && ABT.pageJson.getaquote.quoteButtonSelected && ABT.pageJson.getaquote.quoteButtonSelected == true) {
        setGetaQuote = true;
        skipSidePanel = true;
        autoSubmitGetaQuote = true;
        //ABT.pageJson.getaquote.quoteButtonSelected = false;
      }
    },

    checkAutoShowNewCarLeadForm: function() {
      var find = ' ',
          re = new RegExp(find, 'g'),
          $zipcode = $('#ncLeadZipcode'),
          $zipcode2 = $('#ncLeadZipcode2');

      if (setSemDefault && semValues[3] && semValues[3].length == 5 && semValues[3].match(zipRegex)) {
        $zipcode.val(semValues[3]);
        $zipcode2.val(semValues[3]);
        personaService.set({ zipcode: semValues[3] });
      }

      if (autoSubmitGetaQuote && ABT.pageJson.getaquote.make) {
        $('#ncLeadMakeSelect').val(ABT.pageJson.getaquote.make).change();
      }
      else if (setDefaultMake && abt.OFR.pageCtx.make) {
        var defaultMake = abt.OFR.pageCtx.make.toLowerCase().replace(re, '-');

        setDefaultMake = false;
        setDefaultModel = true;
        $('#ncLeadMakeSelect').val(defaultMake).change();
      }
      else if (setSemDefault && semSeoValues[0]) {
        //$('#ncLeadMakeSelect option:contains(' + 'Ch' + ')').prop({selected: true}).change();
        setSemModel = true;
        $('#ncLeadMakeSelect').val(semSeoValues[0]).change();
      }

    },

    updateAllModelDropdowns: function() {
      $ncLeadAddSelectedMake = $('#ncLeadMakeSelect :selected').val();
      if ($ncLeadAddSelectedMake.length > 0) {
        $('#ncLeadMakeSelect2').val($ncLeadAddSelectedMake);
        offersystem.updateNCLeadEngineModelDropdown();
        $('#makeErrorMessage').css(cssDisplay, cssDisplayNone);
        $('#makeErrorMessage2').css(cssDisplay, cssDisplayNone);
      }
      else {
        $('#trimErrorMessage').css(cssDisplay, cssDisplayNone);
        $('#trimErrorMessage2').css(cssDisplay, cssDisplayNone);
        $pnlGetPricingBtn.removeClass('disabled').prop('disabled', false);
        $step1GetPricingBtn.removeClass('disabled').prop('disabled', false);
        $ncLeadModelSelect.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
        $ncLeadModelSelect2.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
        $ncLeadTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
        $ncLeadTrimSelect2.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
      }
    },

    updateAllTrimDropdowns: function() {
      $ncLeadAddSelectedModel = $('#ncLeadModelSelect :selected').val();
      if ($ncLeadAddSelectedModel.length > 0) {
        $('#ncLeadModelSelect2').val($ncLeadAddSelectedModel);
        offersystem.updateNCLeadEngineTrimDropdown();
        $('#modelErrorMessage').css(cssDisplay, cssDisplayNone);
        $('#modelErrorMessage2').css(cssDisplay, cssDisplayNone);
      }
      else {
        $ncLeadTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
        $ncLeadTrimSelect2.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
      }
    },

    RemoveAutowebFromCloseButton: function() {
      $overlayCloseBtn.unbind('click');
      $overlayCloseBtn.on('click', function(e) {
        e.preventDefault();
        $leadformOverlay.popup('hide');
        ABT.OFR.setBygLockout(false);
      });
    },

    RemoveHideFromCloseButton: function() {
      $overlayCloseBtn.unbind('click');
      $overlayCloseBtn.on('click', function(e) {
        e.preventDefault();
        offersystem.ShowAutowebAds();
      });
    },

    LoadOLAutowebAds: function(selectedMake, selectedModel) {
      var adData = { Make: selectedMake, Model: selectedModel, Location: '', adList: '' },
          pData = personaService.get(),
          selectedZip = pData.zipcode;

      ABT.OFR.updateBygMakeModel(selectedMake, selectedModel);

      try {
        var locationUrl = '/api/leadengine/citystate/' + selectedZip + '/';
        $.get(locationUrl).done(function(resp) {
          if (resp.data) {
            adData.Location = resp.data[0].city + ', ' + resp.data[0].state_abb;
          }
        });

        var url = '/api/research/getautowebads/';
        var jsonString = "{Make: '" + selectedMake + "', Model: '" + selectedModel + "', Zip: '" + selectedZip + "', PublisherCampaignId: '" + '190' + "'}";
        $.post(url, { '': jsonString }).done(function(resp) {
          if (resp.data && resp.data.autowebadjson) {
            var adInfo = JSON.parse(resp.data.autowebadjson);
            if (adInfo.Listings && adInfo.Listings.length > 0) {
              adData.adList = adInfo;
              $overlayTyAutowebContainer.empty().prepend(tmpls.overlay_lead_autoweb({ adData: adData }));
              $overlayThankyouContainer.removeClass("single");
              if (!displayedOlAutoweb) {
                offersystem.RemoveHideFromCloseButton();
              }
              //if (!sidePanelAutowebLoaded && location.href.indexOf("cars-for-sale") < 0) {
              //  sidePanelAutowebLoaded = true;
              //  $('#sp_Special_Offers').empty().prepend(tmpls.sp_autoweb({ adData: adData })).css(cssDisplay, cssDisplayBlock);
              //  $sidePanel.addClass('wide');
              //}
            }
          }
          else {
            $overlayTyAutowebContainer.css(cssDisplay, cssDisplayNone);
            $overlayThankyouContainer.addClass("single");
            offersystem.RemoveAutowebFromCloseButton();
          }
        });
      }
      catch (e) {
        $overlayTyAutowebContainer.css(cssDisplay, cssDisplayNone);
        $overlayThankyouContainer.addClass("single");
        offersystem.RemoveAutowebFromCloseButton();
      }

    },

    ShowAutowebAds: function() {
      displayedOlAutoweb = true;
      $overlayThankyouContainer.addClass("single");
      $overlayLeadFormContainer.css(cssDisplay, cssDisplayNone);
      $overlayThankyouContainer.css(cssDisplay, cssDisplayBlock);
      offersystem.RemoveAutowebFromCloseButton();
      ABT.OFR.setBygLockout(true);
    },

    displayAutoNewCarLeadForm: function() {
      if (setSemDefault || setGetaQuote) {
        setGetaQuote = false;
        $('#ol_display_ymm').hide();
        $('#ol_personalinfo_ymm').hide();
        $('#ol_form_ymm').show();
        $overlayLeadFormContainer.css(cssDisplay, cssDisplayBlock);
        $overlayLeadFormNewCarInfo.css(cssDisplay, cssDisplayBlock);
        $overlayLeadFormPersonalInfo.addClass("col");
        $overlayLeadFormNewCarHeader.css(cssDisplay, cssDisplayBlock);
        $overlayLeadFormC4SHeader.css(cssDisplay, cssDisplayNone);
        $submitLeadBtn.text("Submit and Get Pricing").data('formtype', "new");
        $overlayThankyouContainer.css(cssDisplay, cssDisplayNone);
        $leadformOverlay.popup('show');
      }
    },

    getDealers: function() {
      $pnlGetPricingBtn.addClass('disabled').prop('disabled', true);
      $step1GetPricingBtn.addClass('disabled').prop('disabled', true);
      if (offersystem.validateGetDealersReady(true)) {
        personaService.set({ zcverified: true });
        zipcodeVerified = true;
        $('#zipcodeConfirmMessage').css(cssDisplay, cssDisplayNone);
        $('#zipcodeConfirmMessage2').css(cssDisplay, cssDisplayNone);
        // Start Processes to update LeadForm Overlay Data
        offersystem.updateLeadFormCityState();
        offersystem.updateLeadFormH1DescMsrp();
        offersystem.updateLeadFormSelectedImage();
        offersystem.updateLeadFormPingDealers();
        // Set Submit Lead button to enabled
        $submitLeadBtn.removeClass('disabled').prop('disabled', false);
        // Close Side Pane and Open Overlay
        $('.navigation').removeClass("relative");
        $sidePanel.parent().addClass('hidden');
        $sidePanel.removeClass('wide');
        $('#ol_display_ymm').show();
        $('#ol_personalinfo_ymm').show();
        $('#ol_form_ymm').hide();
        $overlayLeadFormContainer.css(cssDisplay, cssDisplayBlock);
        $overlayLeadFormNewCarInfo.css(cssDisplay, cssDisplayBlock);
        $overlayLeadFormPersonalInfo.addClass("col");
        $overlayLeadFormNewCarHeader.css(cssDisplay, cssDisplayBlock);
        $overlayLeadFormC4SHeader.css(cssDisplay, cssDisplayNone);
        $submitLeadBtn.text("Submit and Get Pricing").data('formtype', "new");
        $overlayThankyouContainer.css(cssDisplay, cssDisplayNone);
        $leadformOverlay.popup('show');
        // Set Get Dealers Button to enabled
        $pnlGetPricingBtn.removeClass('disabled').prop('disabled', false);
        $step1GetPricingBtn.removeClass('disabled').prop('disabled', false);

        // Wireup Lead Form Buttons and actions
        $('#displaySidePanel').on('click', function(e) {
          e.preventDefault();
          //$('.navigation').toggleClass("relative");
          //$sidePanel.parent().removeClass('hidden');
          //$leadformOverlay.popup('hide');
          $('#ol_display_ymm').hide();
          $('#ol_personalinfo_ymm').hide();
          $('#ol_form_ymm').show();
        });

        // Load Autoweb Ads
        offersystem.LoadOLAutowebAds($('#ncLeadMakeSelect :selected').text(), $('#ncLeadTrimSelect option:selected').attr('data-model'));

      }
      else {
        $pnlGetPricingBtn.removeClass('disabled').prop('disabled', false);
        $step1GetPricingBtn.removeClass('disabled').prop('disabled', false);
      }
    }, // end getDealers()

    setC4STrimIdforSimilar: function (make, model, year) {
      var url = '/api/leadengine/make/' + make + '/super-model/' + model + '/trims/',
          trimId = 0;
      $.get(url).done(function (resp) {
        if (resp.data && resp.data.length > 0) {
          for (var i = 0; i < resp.data.length; i++) {
            if (resp.data[i].year == year) {
              trimId = resp.data[i].id;
              break;
            }
          }
          $('#c4sSimTrimId').val(trimId);
        }
      });
      return;
    },

    PostToQuoteSubmitted: function($elem) {
      var formType = $elem.data('formtype'),
          errorStatus = false,
          errorMessage = '',
          year = '',
          make = '',
          model = '',
          trim = '',
          vehicleId = 0,
          vehicleList = '[',
          trimId = 0,
          fName = $('#fName').val(),
          lName = $('#lName').val(),
          address = $("#strAddr").val(),
          zip = '',
          phone = $('#HPfull').val(),
          email = $('#email').val(),
          message = encodeURIComponent($('#leadMessage').val()),
          contacttime = $('#ContactTime option:selected').val(),
          timeframe = $('#TimeFrame option:selected').val(),
          dealerCheckedCount = 0;

      if (formType == 'c4s') { // Inventory Lead Form Data
        year = ABT.pageJson.inventory.year;
        make = ABT.pageJson.inventory.make;
        model = ABT.pageJson.inventory.model;
        vehicleId = ABT.pageJson.inventory.id;
        zip = currentZip;
        contacttime = '';
        timeframe = '';
        trimId = $('#c4sSimTrimId').val();
      }
      else if (formType == 'multic4s') { // Multiple Inventory Lead Form Data
        for (var x = 0; x < ABT.pageJson.multiInventory.length; x++) {
          vehicleList = vehicleList + '{year: "' + ABT.pageJson.multiInventory[x].year + '",';
          vehicleList = vehicleList + 'make: "' + ABT.pageJson.multiInventory[x].make + '",';
          vehicleList = vehicleList + 'model: "' + ABT.pageJson.multiInventory[x].model + '",';
          vehicleList = vehicleList + 'vin: "' +ABT.pageJson.multiInventory[x].vin + '",';
          vehicleList = vehicleList + 'vehicleid: ' + ABT.pageJson.multiInventory[x].id + ',';
          vehicleList = vehicleList + 'dealerid: ' + ABT.pageJson.multiInventory[x].dealerId + '}';
          if (x + 1 < ABT.pageJson.multiInventory.length) {
            vehicleList = vehicleList + ',';
          }
        }
        vehicleList = vehicleList + ']';
        //console.log(vehicleList);

        zip = currentZip;
        contacttime = '';
        timeframe = '';
        trimId = $('#c4sSimTrimId').val();
      }
      else { // New Car Lead Form Data
        year = $('#ncLeadTrimSelect option:selected').attr('data-year');
        make = $('#ncLeadMakeSelect option:selected').text();
        model = $('#ncLeadTrimSelect option:selected').attr('data-model');
        trimId = $('#ncLeadTrimSelect option:selected').attr('data-id');
        trim = $ncLeadSelectedTrim;
        if (trim.indexOf("'") >= 0) {
          trim = trim.replace("'", " ");
          message = "Trim: " + trim + " " + message;
        }
        zip = $("input#ncLeadZipcode").val();
        // We need to make sure a dealer was selected if there was at least one available
        $('input[name="dealerchkbox[]"]:checked').each(
          function() {
            dealerCheckedCount++;
          }
        );
        if (dealerCheckedCount < 1 && $('#CoverageCount').val() > 0) {
          errorStatus = true;
          $('#DealerErrorMessage').css(cssDisplay, cssDisplayBlock);
        }
        else {
          $('#DealerErrorMessage').css(cssDisplay, cssDisplayNone);
        }
      }

      $('#LeadPostErrorMessage').css('display', 'none');
      $submitLeadBtn.addClass('disabled').prop('disabled', true);

      if (fName.trim().length < 1 || lName.trim().length < 1 || address.trim().length < 1 || email.trim().length < 1) {
        errorStatus = true;
        if (fName.trim().length < 1) {
          errorMessage = "First Name is blank."
        }
        else if (lName.trim().length < 1) {
          errorMessage = "Last Name is blank."
        }
        else if (address.trim().length < 1) {
          errorMessage = "Address is blank."
        }
        else if (email.trim().length < 1) {
          errorMessage = "Email is blank."
        }
        $('#LeadPostErrorMessage').html(errorMessage).css('display', '');
      }

      if (!errorStatus) {
        var optin = 0;
        var optMkgBox = 0;
        $(":input").each(function(i) {
          if (this.name == 'optInAll') {
            if (this.checked === true) {
              optin = 1;
            }
          }
          if (this.name == 'optMkgBox') {
            if (this.checked === true) {
              optMkgBox = 1;
            }
          }
        });

        var dealers = '[';
        $('input[name="dealerchkbox[]"]:checked').each(
          function() {
            dealers = dealers + "{dealer:'" + this.id + "'},";
          }
        );
        dealers = dealers + "]";

        var jsonString = "",
            url = "";
        if (formType == 'multic4s') {
          jsonString = "{firstname: '" + fName + "', lastname: '" + lName + "', streetaddress: '" + address + "', zipcode: '" + zip + "', phonenumber: '" + phone +
            "', emailaddress: '" + email + "', comments: '" + message + "', contacttime: '" + contacttime + "', timeframe: '" + timeframe + "', trim: '" + trim +
            "', optin: '" + optin + "', optMkgBox: '" + optMkgBox + "', vehiclelist: " + vehicleList + ", affiliateid: '" + affiliateid + "', dealers: " + dealers + "}";

          url = '/api/leadengine/post-multi-lead/';
        } else {
          jsonString = "{firstname: '" + fName + "', lastname: '" + lName + "', streetaddress: '" +address + "', zipcode: '" + zip + "', phonenumber: '" +phone +
            "', emailaddress: '" +email + "', comments: '" + message + "', contacttime: '" +contacttime + "', timeframe: '" + timeframe + "', year: '" + year +
            "', make: '" + make + "', model: '" + model + "', trim: '" + trim + "', optin: '" + optin + "', optMkgBox: '" + optMkgBox + "', vehicleid: '" + vehicleId +
            "', affiliateid: '" +affiliateid + "', dealers: " + dealers + "}";

          url = '/api/leadengine/post-lead/';
        }

        $.post(url, { '': jsonString })
          .done(function(resp) {
            var result = resp.data;
            //console.log(result);
            if (result.status == '0') {
              offersystem.displayLeadSubmitThankyou(make, model, year, result.leadId, fName, trimId);
              trackOmniture(action_str, $elem);
            }
            else if (result.status == '1') {
              offersystem.displayLeadSubmitThankyou(make, model, year, result.leadId, fName, trimId);
              trackOmniture(action_str, $elem);
            }
            else if (result.status == '2') {
              var prNumArr = [];
              for (var x = 0; x < result.postresults.length; x++) {
                if (result.postresults[x].leadId.length > 0) {
                  prNumArr.push(result.postresults[x].leadId);
                }
              }
              offersystem.displayLeadSubmitThankyouMulti(prNumArr, fName, trimId);
              trackOmniture(action_str, $elem);
            }
            else {
              $('#LeadPostErrorMessage').html(result.message).css('display', '');
              $submitLeadBtn.removeClass('disabled').prop('disabled', false);
            }
          });
      }
      else {
        $submitLeadBtn.removeClass('disabled').prop('disabled', false);
      }
    }, // End PostToQuoteSubmitted Function

    displayLeadSubmitThankyou: function (make, model, year, prnumber, fName, trimId) {
      var prVal = 0;
      if (prnumber.length > 0)
        prVal = prnumber;
      var url = '/api/leadengine/leadprinfo/' + make + '/' + model + '/' + year + '/' + prVal + '/' + fName + '/';
      $.get(url).done(function(resp) {
        if (resp.data) {
          var prinfo = resp.data;
          $overlayTyLeadInfoContainer.empty().prepend(tmpls.overlay_lead_thankyou({ prinfo: prinfo }));
          $overlayLeadFormContainer.css(cssDisplay, cssDisplayNone);
          $('#awAbandonMessage').css(cssDisplay, cssDisplayNone);
          $('#awThankyouMessage').css(cssDisplay, cssDisplayBlock);
          $overlayTyLeadInfoContainer.css(cssDisplay, cssDisplayBlock);
          $overlayThankyouContainer.css(cssDisplay, cssDisplayBlock);
          offersystem.RemoveAutowebFromCloseButton();
        }
      });

      var similarUrl = '/api/research/similarvehicles/',
          jsonString = "{trimid: '" + trimId + "'}",
          trimlist = '';

      $.post(similarUrl, { '': jsonString })
        .done(function (resp) {
          trimlist = resp.data
          if (!(trimlist === null || trimlist.length < 1)) {
            $overlayTySimilarContent
              .empty()
              .prepend(tmpls.overlay_lead_similar_vehicles({ trimlist: trimlist }))
            
            offersystem.wireupTYSimilarVehicleLead()
            $overlayTySimilarContainer.show()
          }
        })
    },

    displayLeadSubmitThankyouMulti: function (prNumArr, fName, trimId) {
      var url = '/api/leadengine/leadmultiprinfo/' + prNumArr.toString() + '/' + fName + '/';
      $.get(url).done(function(resp) {
        if (resp.data) {
          var prinfo = resp.data;
          //console.log(prinfo);
          $overlayTyLeadInfoContainer.empty().prepend(tmpls.overlay_multilead_thankyou({ prinfo: prinfo }));
          $overlayLeadFormContainer.css(cssDisplay, cssDisplayNone);
          $('#awAbandonMessage').css(cssDisplay, cssDisplayNone);
          $('#awThankyouMessage').css(cssDisplay, cssDisplayBlock);
          $overlayTyLeadInfoContainer.css(cssDisplay, cssDisplayBlock);
          $overlayThankyouContainer.css(cssDisplay, cssDisplayBlock);
          //offersystem.RemoveAutowebFromCloseButton();
        }
      });

      var similarUrl = '/api/research/similarvehicles/',
          jsonString = "{trimid: '" + trimId + "'}",
          trimlist = '';

      $.post(similarUrl, { '': jsonString })
        .done(function (resp) {
          trimlist = resp.data
          if (!(trimlist === null || trimlist.length < 1)) {
            $overlayTySimilarContent
              .empty()
              .prepend(tmpls.overlay_lead_similar_vehicles({ trimlist: trimlist }))
            
            offersystem.wireupTYSimilarVehicleLead()
            $overlayTySimilarContainer.show()
          }
        })
    },

    wireupTYSimilarVehicleLead: function () {

      $('.js_ty_similar_lead_gp').on('click', function (e) {
        e.preventDefault();
        var $this = $(this),
            similarMake = $this.attr('data-make'),
            similarModel = $this.attr('data-model'),
            similarYear = $this.attr('data-year'),
            similarTrim = $this.attr('data-trim'),
            similarTrimId = $this.attr('data-id'),
            similarItem = $this.attr('data-itemnum');

        offersystem.pingSimilarVehicle(similarMake, similarModel, similarYear, similarTrim, similarTrimId, similarItem);

        $('.sel-dlr-ovl-outer').css(cssDisplay, cssDisplayBlock);
      });

    },

    pingSimilarVehicle: function (similarMake, similarModel, similarYear, similarTrim, similarTrimId, similarItem) {

      var zipcode = $("input#ncLeadZipcode").val(),
          pingUrl = '',
          removeFeedbackStatus = false;

      $DealerList.empty(); // Make sure original dealer list is cleared
      $('#SimilarDealerErrorMessage').css(cssDisplay, cssDisplayNone);
      $('#js_post_similar_lead').hide();
      $('#Ty_Similar_DealerList').empty().append('One moment, loading dealer list...');

      try {
        $('[id$=js_suggested_item_' + similarItem + ']').addClass('feedback').empty();

        pingUrl = '/api/leadengine/ping-dealers/?year=' + similarYear + '&make=' + similarMake + '&model=' + similarModel + '&trim=' + similarTrim + '&zipcode=' + zipcode + '&affiliateid=' + affiliateid;
        $.get(pingUrl).done(function(resp) {
          if (resp.data) {
            if (!resp.data.coverage) {
              $('[id$=js_suggested_item_' + similarItem + ']').addClass('feedback').empty().append('<span class="msg msg-no-inv">Inventory not available</span>');
              removeFeedbackStatus = false;
            } else {
              $('#js_similarMake').val(similarMake);
              $('#js_similarModel').val(similarModel);
              $('#js_similarYear').val(similarYear);
              $('#js_similarTrim').val(similarTrim);
              $('#js_similarTrimId').val(similarTrimId);
              $('#js_similarItem').val(similarItem);
              $('#js_post_similar_lead').removeClass('disabled').prop('disabled', false).show();
              removeFeedbackStatus = true;
            }
            $.when(
                $('#Ty_Similar_DealerList').empty().append(tmpls.dealerlist_similar_ping_data({ dealerData: resp.data }))
              )
              .then(function () {
                offersystem.wireupSimilarDealerOverlay(similarItem, removeFeedbackStatus),
                offersystem.wireupDealerCustomMessages(),
                offersystem.wireupSelectAllDealers()
              })
          }
          else {
            $('#Ty_Similar_DealerList').empty().append('Service unavailable, please try again')
          }
        });

      }
      catch (e) {
        //Need to add error catch log;
      }
    },

    wireupSimilarDealerOverlay: function (similarItem, removeFeedbackStatus) {

      // first need to unbind in case we have clicked more than once
      $('#js_close_lead_thankyou_dealerlist').unbind('click');
      // Wireup Lead Thankyou page similar lead dealer list close button
      $('#js_close_lead_thankyou_dealerlist').on('click', function (e) {
        e.preventDefault();
        $('.sel-dlr-ovl-outer').css(cssDisplay, cssDisplayNone);
        if (removeFeedbackStatus) {
          $('[id$=js_suggested_item_' + similarItem + ']').removeClass('feedback').empty();
        }
      });

      $('#js_post_similar_lead').unbind('click');
      $('#js_post_similar_lead').on('click', function (e) {
        e.preventDefault();
        offersystem.PostToQuoteSubmittedSimilar($(this));
      });


    },

    PostToQuoteSubmittedSimilar: function($elem) {
      var formType = $elem.data('formtype'),
          errorStatus = false,
          errorMessage = '',
          year = $('#js_similarYear').val(),
          make = $('#js_similarMake').val(),
          model = $('#js_similarModel').val(),
          trim = $('#js_similarTrim').val(),
          vehicleId = 0,
          trimId = $('#js_similarTrimId').val(),
          similarItem = $('#js_similarItem').val(),
          fName = $('#fName').val(),
          lName = $('#lName').val(),
          address = $("#strAddr").val(),
          zip = '',
          phone = $('#HPfull').val(),
          email = $('#email').val(),
          message = encodeURIComponent($('#leadMessage').val()),
          contacttime = $('#ContactTime option:selected').val(),
          timeframe = $('#TimeFrame option:selected').val(),
          dealerCheckedCount = 0;
      
      $('[id$=js_suggested_item_' + similarItem + ']').addClass('feedback').empty().append('<span class=""><div class="spinner">Loading...</div></span>');

      if (formType == 'c4s') { // Inventory Lead Form Data
        vehicleId = $('#js_similarVehicleId').val();
        zip = currentZip;
        contacttime = '';
        timeframe = '';
      }
      else { // New Car Lead Form Data
        if (trim.indexOf("'") >= 0) {
          trim = trim.replace("'", " ");
        }
        message = "Trim: " + trim + " " + message;
        zip = $("input#ncLeadZipcode").val();
        // We need to make sure a dealer was selected if there was at least one available
        $('input[name="dealerchkbox[]"]:checked').each(
          function() {
            dealerCheckedCount++;
          }
        );
        if (dealerCheckedCount < 1 && $('#CoverageCount').val() > 0) {
          errorStatus = true;
          $('#SimilarDealerErrorMessage').css(cssDisplay, cssDisplayBlock);
        }
        else {
          $('#SimilarDealerErrorMessage').css(cssDisplay, cssDisplayNone);
        }
      }

      $('#js_post_similar_lead').addClass('disabled').prop('disabled', true);

      if (fName.trim().length < 1 || lName.trim().length < 1 || address.trim().length < 1 || email.trim().length < 1) {
        errorStatus = true;
        if (fName.trim().length < 1) {
          errorMessage = "First Name is blank."
        }
        else if (lName.trim().length < 1) {
          errorMessage = "Last Name is blank."
        }
        else if (address.trim().length < 1) {
          errorMessage = "Address is blank."
        }
        else if (email.trim().length < 1) {
          errorMessage = "Email is blank."
        }
        $('#SimilarDealerErrorMessage').html(errorMessage).css(cssDisplay, cssDisplayBlock);
      }

      if (!errorStatus) {

        var optin = 0;
        var optMkgBox = 0;
        $(":input").each(function(i) {
          if (this.name == 'optInAll') {
            if (this.checked === true) {
              optin = 1;
            }
          }
          if (this.name == 'optMkgBox') {
            if (this.checked === true) {
              optMkgBox = 1;
            }
          }
        });

        var dealers = '[';
        $('input[name="dealerchkbox[]"]:checked').each(
          function() {
            dealers = dealers + "{dealer:'" + this.id + "'},";
          }
        );
        dealers = dealers + "]";

        var jsonString = "{firstname: '" + fName + "', lastname: '" + lName + "', streetaddress: '" + address + "', zipcode: '" + zip + "', phonenumber: '" + phone +
          "', emailaddress: '" + email + "', comments: '" + message + "', contacttime: '" + contacttime + "', timeframe: '" + timeframe + "', year: '" + year +
          "', make: '" + make + "', model: '" + model + "', trim: '" + trim + "', optin: '" + optin + "', optMkgBox: '" + optMkgBox + "', vehicleid: '" + vehicleId +
          "', affiliateid: '" + affiliateid + "', dealers: " + dealers + "}";

        var url = '/api/leadengine/post-lead/';

        $.post(url, { '': jsonString })
          .done(function(resp) {
            var result = resp.data;
            if (result.status == '0') {
              $('#js_post_similar_lead').hide();
              $('.sel-dlr-ovl-outer').css(cssDisplay, cssDisplayNone);
              $('#Ty_Similar_DealerList').empty();
              $('[id$=js_suggested_item_' + similarItem + ']').addClass('feedback').empty().append('<span class="msg">Confirmation#: ' + result.leadId + '</span>');
              //trackOmniture(action_str, $elem);
            }
            else if (result.status == '1') {
              $('#js_post_similar_lead').hide();
              $('.sel-dlr-ovl-outer').css(cssDisplay, cssDisplayNone);
              $('#Ty_Similar_DealerList').empty();
              $('[id$=js_suggested_item_' + similarItem + ']').addClass('feedback').empty().append('<span class="msg msg-no-inv">Inventory not available</span>');
              //trackOmniture(action_str, $elem);
            }
            else {
              $('#SimilarDealerErrorMessage').html(result.message).css(cssDisplay, cssDisplayBlock);
              $('#js_post_similar_lead').removeClass('disabled').prop('disabled', false);
            }
          });

      }
      else {
        $('[id$=js_suggested_item_' + similarItem + ']').addClass('feedback').empty();
        $('#js_post_similar_lead').removeClass('disabled').prop('disabled', false);
      }
    }, // End PostToQuoteSubmittedSimilar Function

    validateGetDealersReady: function(displayError) {

      var make = $('#ncLeadMakeSelect').val(),
          model = $('#ncLeadModelSelect').val(),
          zipcode = $("input#ncLeadZipcode").val().replace(/[^0-9]+/g, ''),
          ready = true;

      if (make.length < 1) {
        ready = false;
        if (displayError) {
          $('#makeErrorMessage').css(cssDisplay, cssDisplayBlock);
          $('#makeErrorMessage2').css(cssDisplay, cssDisplayBlock);
        }
      }
      else {
        $('#makeErrorMessage').css(cssDisplay, cssDisplayNone);
        $('#makeErrorMessage2').css(cssDisplay, cssDisplayNone);
      }
      if (model.length < 1) {
        ready = false;
        if (displayError) {
          $('#modelErrorMessage').css(cssDisplay, cssDisplayBlock);
          $('#modelErrorMessage2').css(cssDisplay, cssDisplayBlock);
        }
      }
      else {
        $('#modelErrorMessage').css(cssDisplay, cssDisplayNone);
        $('#modelErrorMessage2').css(cssDisplay, cssDisplayNone);
      }
      if (zipcode.length < 5) {
        ready = false;
        if (displayError) {
          $('#zipcodeErrorMessage').css(cssDisplay, cssDisplayBlock);
          $('#zipcodeErrorMessage2').css(cssDisplay, cssDisplayBlock);
        }
      }
      else {
        $('#zipcodeErrorMessage').css(cssDisplay, cssDisplayNone);
        $('#zipcodeErrorMessage2').css(cssDisplay, cssDisplayNone);
      }

      return ready;
    },

    updateNCLeadEngineModelDropdown: function() {
      var url = '/api/leadengine/make/' + $ncLeadAddSelectedMake + '/super-models/';

      $.when(
          $.get(url).done(function(resp) {
            if (resp.data) {
              $('#trimErrorMessage').css(cssDisplay, cssDisplayNone);
              $('#trimErrorMessage2').css(cssDisplay, cssDisplayNone);
              $pnlGetPricingBtn.removeClass('disabled').prop('disabled', false);
              $step1GetPricingBtn.removeClass('disabled').prop('disabled', false);
              $ncLeadModelSelect.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $ncLeadModelSelect2.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $ncLeadTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
              $ncLeadTrimSelect2.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
            else {
              $('#trimErrorMessage').css(cssDisplay, cssDisplayBlock);
              $('#trimErrorMessage2').css(cssDisplay, cssDisplayBlock);
              $pnlGetPricingBtn.addClass('disabled').prop('disabled', true);
              $step1GetPricingBtn.addClass('disabled').prop('disabled', true);
            }
          })
        )
        .then(function() {
          if (setDefaultModel && abt.OFR.pageCtx.model) {
            var find = ' ';
            var re = new RegExp(find, 'g');
            var defaultModel = abt.OFR.pageCtx.model.toLowerCase().replace(re, '-');
            setDefaultTrim = true;
            setDefaultModel = false;
            $('#ncLeadModelSelect').val(defaultModel);
            $('#ncLeadModelSelect2').val(defaultModel);
            $('#ncLeadModelSelect').val(defaultModel).change();
          }
          else if (setSemModel && semSeoValues[1]) {
            setSemModel = false;
            autoSubmitSem = true;
            $('#ncLeadModelSelect').val(semSeoValues[1]);
            $('#ncLeadModelSelect2').val(semSeoValues[1]);
            $('#ncLeadModelSelect').val(semSeoValues[1]).change();
          }
          else if (ABT.pageJson && ABT.pageJson.getaquote && ABT.pageJson.getaquote.quoteButtonSelected && ABT.pageJson.getaquote.quoteButtonSelected == true) {
            if (ABT.pageJson.getaquote.supermodel && ABT.pageJson.getaquote.supermodel.length > 0) {
              autoSubmitGetaQuote = true;
              ABT.pageJson.getaquote.quoteButtonSelected = false;
              $('#ncLeadModelSelect').val(ABT.pageJson.getaquote.supermodel);
              $('#ncLeadModelSelect2').val(ABT.pageJson.getaquote.supermodel);
              $('#ncLeadModelSelect').val(ABT.pageJson.getaquote.supermodel).change();
            }
          }

        }); // End of then function

    },

    updateNCLeadEngineTrimDropdown: function() {
      var url = '/api/leadengine/make/' + $ncLeadAddSelectedMake + '/super-model/' + $ncLeadAddSelectedModel + '/trims/';

      $.get(url).done(function(resp) {
        if (resp.data) {
          var find = "[ \./]";
          var re = new RegExp(find, 'g');
          $ncLeadTrimSelect.empty()
            .prepend($("<option />").val('Select a Trim').text('Select a Trim').attr('data-model', resp.data[0].model).attr('data-year', resp.data[0].year)
              .attr('data-msrp', resp.data[0].msrp).attr('data-trim', '').attr('data-acode', '').attr('data-id', resp.data[0].id))
            .append(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);

          $ncLeadTrimSelect2.empty()
            .prepend($("<option />").val('Select a Trim').text('Select a Trim').attr('data-model', resp.data[0].model).attr('data-year', resp.data[0].year)
              .attr('data-msrp', resp.data[0].msrp).attr('data-trim', '').attr('data-acode', '').attr('data-id', resp.data[0].id))
            .append(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);

          if (setDefaultTrim && abt.OFR.pageCtx.trim && abt.OFR.pageCtx.year) {
            var defaultTrim = abt.OFR.pageCtx.trim.toLowerCase().replace(re, '-');
            var defaultYear = abt.OFR.pageCtx.year;
            var defaultYearTrim = defaultYear + '-' + defaultTrim;
            setDefaultTrim = false;
            $("#ncLeadTrimSelect").find("option[value='" + defaultYearTrim + "']").each(function() {
              if (jQuery(this).attr('data-year') == defaultYear) {
                jQuery(this).attr("selected", "selected");
              }
            });
            $("#ncLeadTrimSelect2").find("option[value='" + defaultYearTrim + "']").each(function() {
              if (jQuery(this).attr('data-year') == defaultYear) {
                jQuery(this).attr("selected", "selected");
              }
            });
          }
          else if (autoSubmitGetaQuote && ABT.pageJson && ABT.pageJson.getaquote) {
            if (ABT.pageJson.getaquote.year && ABT.pageJson.getaquote.trim.length > 0) {
              var getquoteTrim = ABT.pageJson.getaquote.trim.toLowerCase().replace(re, '-');
              var getquoteYear = ABT.pageJson.getaquote.year;
              var getquoteYearTrim = getquoteYear + '-' + getquoteTrim;
              $("#ncLeadTrimSelect").find("option[value='" + getquoteYearTrim + "']").each(function() {
                if (jQuery(this).attr('data-year') == getquoteYear) {
                  jQuery(this).attr("selected", "selected");
                }
              });
              $("#ncLeadTrimSelect2").find("option[value='" + getquoteYearTrim + "']").each(function() {
                if (jQuery(this).attr('data-year') == getquoteYear) {
                  jQuery(this).attr("selected", "selected");
                }
              });
            }
          }
          if (autoSubmitSem) {
            autoSubmitSem = false;
            offersystem.getDealers();
          }
          else if (autoSubmitGetaQuote) {
            autoSubmitGetaQuote = false;
            ABT.pageJson.getaquote.quoteButtonSelected = false;
            if (zipcodeVerified) {
              offersystem.getDealers();
            }
          }

        }
      });

    },

    updateLeadFormCityState: function() {
      var zipcode = $("input#ncLeadZipcode").val(),
          url = '/api/leadengine/citystate/' + zipcode + '/';

      $.get(url).done(function(resp) {
        if (resp.data) {
          $('#leadFormCityState').empty().prepend(resp.data[0].city + ', ' + resp.data[0].state_abb + ' ' + zipcode);
        }
      });

    },

    updateLeadFormH1DescMsrp: function() {
      var selectedYear = $('#ncLeadTrimSelect option:selected').attr('data-year'),
          selectedMake = $('#ncLeadMakeSelect option:selected').text(),
          selectedModel = $('#ncLeadTrimSelect option:selected').attr('data-model'),
          selectedMsrp = $('#ncLeadTrimSelect option:selected').attr('data-msrp');

      $('#leadFormH1').empty().prepend(selectedYear + ' ' + selectedMake + ' ' + selectedModel + '  Price Quote');
      if (selectedMsrp.length < 2) {
        $('#leadFormCarDesc').empty().prepend(selectedYear + ' ' + selectedMake + ' ' + selectedModel);
        $('#leadFormCarMsrp').empty();
      }
      else {
        $('#leadFormCarDesc').empty().prepend(selectedYear + ' ' + selectedMake + ' ' + selectedModel + '  MSRP');
        $('#leadFormCarMsrp').empty().prepend('<strike>' + selectedMsrp + '<strike>');
      }
    },

    updateLeadFormSelectedImage: function() {
      var trimId = $('#ncLeadTrimSelect option:selected').attr('data-id'),
          url = '/api/leadengine/selectedtrimimage/' + trimId + '/',
          imageUrl = '';

      $.get(url).done(function(resp) {
        if (resp.data) {
          imageUrl = resp.data.url_path_prefix + '_320x.png';
          $('#leadFormSelectedImage').attr('src', imageUrl);
        }
      });

    },

    wireupDealerCustomMessages: function() {
      $('.js_dealer_message_init').on('click', function(e) {
        e.preventDefault();

        var $this = $(this),
            dealerInfo = $this.data('json-values');

        $('[id$=dealer-message-display' + dealerInfo.DealerID + ']').toggle();
        $('[id$=dealer-message-short' + dealerInfo.DealerID + ']').toggle();

        if ($this.text() == '+') {
          $this.text('-');
        }
        else {
          $this.text('+');
        }
      });
    },

    checkNewsletterBoxStatus: function(status) {
      $('#autobytelNewsletterBox').attr('checked', status);
    },

    wireupSelectAllDealers: function() {
      $('#select_all_dealers_js').on('click', function() {
        var allInputs = $(":input");
        var checkedUnchecked = 1;

        allInputs.each(function() {
          if (this.name == 'selectAllDealers') {
            if (this.checked === true) {
              checkedUnchecked = 1;
            }
            else {
              checkedUnchecked = 0;
            }
          }
        });

        allInputs.each(function() {
          if (this.name == 'dealerchkbox[]') {
            this.checked = checkedUnchecked;
          }
        });
      });

    },

    validatePhoneArea: function() {
      var strValidChars = "0123456789",
          areaVal = $('#HParea').val(),
          areaLen = areaVal.length,
          areaLastChar = "",
          HPfullVal = ""

      if (areaLen > 0) {
        areaLastChar = areaVal.charAt(areaLen - 1);
        if (strValidChars.indexOf(areaLastChar) == -1) {
          areaVal = areaVal.replace(/[^0-9]+/g, '');
          areaLen = areaVal.length;
          $("#HParea").val(areaVal);
        }
      }

      HPfullVal = $("#HParea").val() + $("#HPpre").val() + $("#HPphone").val();
      $("#HPfull").val(HPfullVal);

      if (areaVal.length > 2) {
        $("#HPpre").focus();
      }
      return false;
    },

    validatePhonePrefix: function() {
      var strValidChars = "0123456789",
          prefixVal = $('#HPpre').val(),
          HPfullVal = "",
          prefixLen = prefixVal.length,
          prefixLastChar = "";

      if (prefixLen > 0) {
        prefixLastChar = prefixVal.charAt(prefixLen - 1);
        if (strValidChars.indexOf(prefixLastChar) == -1) {
          prefixVal = prefixVal.replace(/[^0-9]+/g, '');
          prefixLen = prefixVal.length;
          $("#HPpre").val(prefixVal);
        }
      }

      HPfullVal = $("#HParea").val() + $("#HPpre").val() + $("#HPphone").val();
      $("#HPfull").val(HPfullVal);

      if (prefixVal.length > 2) {
        $("#HPphone").focus();
      }
      return false;
    },

    validatePhoneUCNumber: function() {
      var strValidChars = "0123456789",
          ucnumberVal = $('#HPphone').val(),
          HPfullVal = "",
          ucnumberLen = ucnumberVal.length,
          ucnumberLastChar = "",
          newucnumberVal = "";

      if (ucnumberLen > 0) {
        newucnumberVal = ucnumberVal;
        ucnumberLastChar = ucnumberVal.charAt(ucnumberLen - 1);
        if (strValidChars.indexOf(ucnumberLastChar) == -1) {
          newucnumberVal = ucnumberVal;
          newucnumberVal = ucnumberVal.replace(/[^0-9]+/g, '');
          ucnumberLen = newucnumberVal.length;
          $("#HPphone").val(newucnumberVal);
        }
      }

      HPfullVal = $("#HParea").val() + $("#HPpre").val() + $("#HPphone").val();
      $("#HPfull").val(HPfullVal);

      return false;
    },

    updateLeadFormPingDealers: function() {
      var selectedYear = $('#ncLeadTrimSelect option:selected').attr('data-year'),
          selectedMake = $('#ncLeadMakeSelect option:selected').text(),
          selectedModel = $('#ncLeadTrimSelect option:selected').attr('data-model'),
          zipcode = $("input#ncLeadZipcode").val(),
          pingUrl = '',
          selectedAcode = $('#ncLeadTrimSelect option:selected').attr('data-acode'),
          abtTrimUrl = '';

      $ncLeadSelectedTrim = $('#ncLeadTrimSelect option:selected').attr('data-trim');

      $DealerList.empty().append('One moment, loading dealer list...');

      try {

        $('#leadMessage').val('');

        if (selectedAcode && selectedAcode.length > 1)
          abtTrimUrl = '/api/leadengine/acode/' + selectedAcode + '/trim';
        else
          abtTrimUrl = '/api/leadengine/acode/xxxxx/trim';

        $.get(abtTrimUrl).done(function(resp) {
          if (resp.data && resp.data.abt_name && resp.data.abt_name.length > 0) {
            $ncLeadSelectedTrim = resp.data.abt_name;
          }
          else {
            if (selectedAcode && selectedAcode.length > 1) {
              $('#leadMessage').val($('#ncLeadTrimSelect option:selected').attr('data-trim'));
            }
          }

          //alert(selectedYear + ' | ' + selectedMake + ' | ' + selectedModel + ' | ' + $ncLeadSelectedTrim + ' | ' + zipcode + ' | ' + affiliateid); 
          pingUrl = '/api/leadengine/ping-dealers/?year=' + selectedYear + '&make=' + selectedMake + '&model=' + selectedModel + '&trim=' + $ncLeadSelectedTrim + '&zipcode=' + zipcode + '&affiliateid=' + affiliateid;

          $.get(pingUrl).done(function(resp) {
            if (resp.data) {
              $.when(
                  $DealerList.empty().append(tmpls.dealerlist_ping_data({ dealerData: resp.data }))
                )
                .then(function() {
                  offersystem.wireupDealerCustomMessages(),
                    offersystem.wireupSelectAllDealers(),
                    offersystem.checkNewsletterBoxStatus(resp.data.CheckAutobytelNewletterBox)
                })
            }
            else {
              $DealerList.empty().append('Service unavailable, please try again')
            }
          });
        });

      }
      catch (e) {
        //Need to add error catch log;
      }
    },

  }


  $.when(
      $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup')),
      personaService.isReady(),
      carsforsaleService.isReady()
    )
    .then(function() {
      offersystem.init();
    })
}(window, jQuery)