

'use strict';

(function (win, $, abt) {
  var click_event = 'click'
    , hasSessionStorage = abt.UTILS.hasSessionStorage

  $(function () {
    var env = abt.ENV,
        car = abt.pageJson.vehicleinfo,
        getJsFileNameByAlias = env.getJsFileNameByAlias,
        getScriptUriByName = env.getScriptUriByName,
        libsPath = abt.ENV.libsPath,
        $leadformOverlay = '',
        sidePanelLeadFormInit = false,
        tmpls = require('./trimoverview_tmpls/desk/tmpls.js'),
        $displayTrimList = $('#display_trim_list'),
        personaService = abt.PERSONA


    var trimOverview = {

      init: function () {
        trimOverview.updateViewedRecentlyList()

        $.when(trimOverview.wireupGetQuoteButtons(), trimOverview.initCarsForSaleWidget(), trimOverview.initViewedRecentlyWidget())
          .then(function () {
            trimOverview.wireupTrimListOverlayLinks()
            trimOverview.showSimilarVehicles()
            trimOverview.wireupCalculator()
            trimOverview.wireupDisclaimer()
            trimOverview.updateBygData()
          })
      },

      updateViewedRecentlyList: function () {
        var pData = personaService.get(),
            currentViewedRecentlyList = pData.viewedRecentlyList,
            currentViewedRecentlyArr = [],
            currentTrimId = car.trimid.toString(),
            maxVrNum = 10;

        if (currentViewedRecentlyList && currentViewedRecentlyList.length > 0) {
          currentViewedRecentlyArr = currentViewedRecentlyList.split(',');
        }

        if (currentViewedRecentlyArr.indexOf(currentTrimId) < 0) {
          currentViewedRecentlyArr.push(currentTrimId);
          if (currentViewedRecentlyArr.length > maxVrNum) {
            currentViewedRecentlyArr.shift();
          }
        } else {
          var delItemIndx = currentViewedRecentlyArr.indexOf(currentTrimId);
          if (delItemIndx >= 0) {
            currentViewedRecentlyArr.splice(delItemIndx, 1);
            currentViewedRecentlyArr.push(currentTrimId);
          }
        }

        currentViewedRecentlyList = currentViewedRecentlyArr.join();
        personaService.set({ viewedRecentlyList: currentViewedRecentlyList });
      },

      initCarsForSaleWidget: function () {
        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4swidget'))))
        .then(function() {
          abt.WDG.c4s.init({ make: car.make, model: car.model })
        })
      },

      initViewedRecentlyWidget: function () {
        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('viewedrecentlywidget'))))
        .then(function () {
          abt.WDG.viewedrecently.init({ currentTrim: car.trimid })
        })
      },

      updateBygData: function () {
        if (car.make && car.model && car.make.length > 2 && car.model.length > 0) {
          ABT.OFR.updateBygMakeModel(car.make, car.model);
        }
      },

      showSimilarVehicles: function () {

        var trimid = car.trimid,
            url = '/api/research/similarvehicles/',
            jsonString = "{trimid: '" + trimid + "'}",
            trimlist = ''

        $.post(url, { '': jsonString })
          .done(function (resp) {
            trimlist = resp.data

            if (!(trimlist === null || trimlist.length < 1)) {
              $('#js_similar_vehicles_content')
                .empty()
                .prepend(tmpls.similar_vehicles({ trimlist: trimlist }))

              $('#js_similar_vehicles').show()
            }
          })
      },


      showSelectedTrimList: function (supertrim) {

        var make = ABT.pageJson.getaquote.make,
            supermodel = ABT.pageJson.getaquote.supermodel,
            year = ABT.pageJson.getaquote.year,
            url = '/api/research/supertrim/trimlist/',
            jsonString = "{make: '" + make + "', supermodel: '" + supermodel + "', year: '" + year + "', supertrim: '" + supertrim + "'}",
            trimlist = ''

        $('#js_ol_supertrim').text(supertrim)

        $.post(url, { '': jsonString })
          .done(function (resp) {
            trimlist = resp.data

            if (trimlist === null || trimlist.length < 1) {
              $displayTrimList.popup('show')
            }
            else {
              $('#js_ol_trimlist')
                .empty()
                .prepend(tmpls.trimlist_by_supertrim({ trimlist: trimlist }))

              $displayTrimList.popup('show')
            }
          })

        $displayTrimList.popup('show')
      },

      calculateCarPayment: function () {
        var paymentEstimatePurchasePrice = $('#PaymentEstimate_purchasePrice').val().replace(/[^0-9.]+/g, "");
        var paymentEstimateCashRebate = $('#PaymentEstimate_cashRebate').val().replace(/[^0-9.]+/g, "");
        var paymentEstimateTradeIn = $('#PaymentEstimate_tradeIn').val().replace(/[^0-9.]+/g, "");
        var paymentEstimateTradeInOwed = $('#PaymentEstimate_tradeInOwed').val().replace(/[^0-9.]+/g, "");
        var paymentEstimateDownPayment = $('#PaymentEstimate_downPayment').val().replace(/[^0-9.]+/g, "");
        var paymentEstimateInterestRate = $('#PaymentEstimate_interestRate').val().replace(/[^0-9.]+/g, "");
        var paymentEstimateTermMonths = $('#PaymentEstimate_termMonths').val().replace(/[^0-9.]+/g, "");

        var netTradeIn = Number(paymentEstimateTradeIn) - Number(paymentEstimateTradeInOwed);
        var downAndRebate = Number(paymentEstimateDownPayment) + Number(paymentEstimateCashRebate);
        var tradeAndDown = netTradeIn + downAndRebate;
        var netCost = Number(paymentEstimatePurchasePrice) - tradeAndDown;
        var amtInterest = netCost * Number(paymentEstimateInterestRate) / 100;
        var loanAmount = netCost + amtInterest;
        var monthlyPayment = loanAmount / Number(paymentEstimateTermMonths);

        var monthlyPaymentLabel;

        if (isNaN(monthlyPayment)) {
          monthlyPaymentLabel = "Please check your entries... Only numbers allowed!";
        } else if (Number(paymentEstimateTermMonths) < 1) {
          monthlyPaymentLabel = "Term of loan must be at least 1 month";
        } else {
          monthlyPaymentLabel = "$" + monthlyPayment.toFixed(2) + "/month for " + paymentEstimateTermMonths + " months";
        }

        $('#PaymentEstimate_monthlyPayment').text(monthlyPaymentLabel);
        $('#resultDiv').css("display", "block");

          if (hasSessionStorage) {
              win.sessionStorage.setItem('calc-payment', JSON.stringify({
                  rebate: paymentEstimateCashRebate,
                  tradeIn: paymentEstimateTradeIn,
                  owedOnTradeIn: paymentEstimateTradeInOwed,
                  downPayment: paymentEstimateDownPayment,
                  interestRate: paymentEstimateInterestRate,
                  moTerms: paymentEstimateTermMonths
              }))
          }
      },


      wireupTrimListOverlayLinks: function () {

        $('.js_trimoverlay').on(click_event, function (e) {
          e.preventDefault()
          trimOverview.showSelectedTrimList($(this).data('supertrim').toString())
        })

        $('.overlay-close').on(click_event, function (e) {
          e.preventDefault()
          $displayTrimList.popup('hide')
        })
      },

      wireupCalculator: function () {

        $('#open_calculator_overlay_js').on(click_event, function (e) {
            e.preventDefault()
            if (hasSessionStorage) {
                var calcPmt = JSON.parse(win.sessionStorage.getItem('calc-payment'))
                if (calcPmt !== null) {
                    $("#PaymentEstimate_cashRebate").val(calcPmt["rebate"])
                    $("#PaymentEstimate_tradeIn").val(calcPmt["tradeIn"])
                    $("#PaymentEstimate_tradeInOwed").val(calcPmt["owedOnTradeIn"])
                    $("#PaymentEstimate_downPayment").val(calcPmt["downPayment"])
                    $("#PaymentEstimate_interestRate").val(calcPmt["interestRate"])
                    $("#PaymentEstimate_termMonths").val(calcPmt["moTerms"])
                }
            }

          $('#overlay_calculate_car_payment_js').popup('show')
        })

        $('#close_calculator_overlay_js').on(click_event, function (e) {
          e.preventDefault()
          $('#overlay_calculate_car_payment_js').popup('hide')
        })

        $('#calculator_reset_js').on(click_event, function (e) {
          $('#resultDiv').css("display", "none");
        })

        $('#btnCalculateMonthlyPayment').on(click_event, function (e) {
          e.preventDefault()
          trimOverview.calculateCarPayment()
        })

      },

      wireupDisclaimer: function() {
        // We are caching the *disclaimer* element here so we do not 
        // re-query the DOM each time the click event fires! :)
        var $disclaimer = $('#disclaimer_js')

        $('#disclaimer_btn_js').on(click_event, function(e) {
          e.preventDefault()
          $disclaimer.slideToggle()
        })
      },

      wireupGetQuoteButtons: function () {

        $('.js_get_quote_btn').on(click_event, function (e) {
          e.preventDefault()

          if (!sidePanelLeadFormInit) {

            if (ABT.pageJson && ABT.pageJson.getaquote) {
              ABT.pageJson.getaquote.quoteButtonSelected = true
            }

            var offerbootstrapFilename = getJsFileNameByAlias('ofrsystem')
            $.getCachedScript(getScriptUriByName(offerbootstrapFilename))

            sidePanelLeadFormInit = true

          } else {

            if (ABT.pageJson && ABT.pageJson.getaquote) {
              ABT.pageJson.getaquote.quoteButtonSelected = true

              if (ABT.pageJson.getaquote.make && ABT.pageJson.getaquote.make.length > 0) {
                $('#ncLeadMakeSelect').val(ABT.pageJson.getaquote.make).change()
              }
            }

            $('#ol_display_ymm').hide()
            $('#ol_personalinfo_ymm').hide()
            $('#ol_form_ymm').show()

            $('#ol_Leadform').css("display", "block")
            $('#ol_ofr_Thankyou').css("display", "none")
            $leadformOverlay = $('#leadform_overlay_js')
            $leadformOverlay.popup('show')
          }
        })
      }
    }

    $(function () {
      $.when(
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup')),
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('picfill')),
          personaService.isReady()
        )
        .then(function () {
          trimOverview.init()
        })
    })

  })
})(window, jQuery, ABT)