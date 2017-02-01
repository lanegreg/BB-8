
(function (win, $, abt) {

  var hasSessionStorage = abt.UTILS.hasSessionStorage;

  $(function () {

    var env = abt.ENV,
        getJsFileNameByAlias = env.getJsFileNameByAlias,
        libsPath = env.libsPath,
        $body = $('body'),
        tmpls = require('./paymentestimate_tmpls/desk/tmpls.js'),
        $addChangeCarOverlay = $('#add_change_car_overlay_js'),
        $paymentEstimateModelSelect = '',
        $paymentEstimateTrimSelect = '',
        $paymentEstimateAddSelectedMake = '',
        $paymentEstimateAddSelectedModel = '',
        $paymentEstimateSelectedTrim = '',
        $paymentEstimateSelectedTrimId = '',
        $paymentEstimateSelectedTrimName = '',
        $paymentEstimateSelectedTrimYear = '',
        $paymentEstimateSelectedTrimLabel = '',
        $paymentEstimateSelectedTrimMsrpIdValue = '',
        $paymentEstimateSelectedTrimMsrpValue = '',
        $capitalizedMake = '',
        $capitalizedModel = '',
        $overlayCloseBtn = '';

    var jsCalcPaymentEstimate = {
      init: function () {
        $('#resultdiv_js').css("display", "none");
        $('#calc_form_div_js').css("display", "block");

        $('#btnCalculateMonthlyPayment').on('click', function (e) {
          e.preventDefault();
          jsCalcPaymentEstimate.calculateCarPayment();
        });

        $('#PaymentEstimate_purchasePrice').on('click', function (e) {
          $('#PaymentEstimate_purchasePrice').val("");
          $('#js_selected_trim_label').html("");
          $paymentEstimateSelectedTrimId = '';
          $paymentEstimateSelectedTrimLabel = '';
        });

        $('#js_add_compare_car').on('click', function (e) {
          e.preventDefault();
          $addChangeCarOverlay.popup('show');
        });

        if (hasSessionStorage) {
          var storedValues = JSON.parse(win.sessionStorage.getItem('jsCalcPaymentEstimate'));
          if (storedValues !== null) {
            $("#PaymentEstimate_purchasePrice").val(storedValues.purchasePrice);
            $("#PaymentEstimate_cashRebate").val(storedValues.cashRebate);
            $("#PaymentEstimate_tradeIn").val(storedValues.tradeIn);
            $("#PaymentEstimate_tradeInOwed").val(storedValues.tradeInOwed);
            $("#PaymentEstimate_downPayment").val(storedValues.downPayment);
            $("#PaymentEstimate_interestRate").val(storedValues.interestRate);
            $("#PaymentEstimate_termMonths").val(storedValues.termMonths);
            $paymentEstimateSelectedTrimId = storedValues.trimid;
            $paymentEstimateSelectedTrimLabel = storedValues.trimlabel;
          }

          if ($paymentEstimateSelectedTrimLabel.length > 0) {
            $('#js_selected_trim_label').html($paymentEstimateSelectedTrimLabel);
          }

        }
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
        var workingInterest = Number(paymentEstimateInterestRate) / 100 / 12;
        var monthlyPayment = netCost * (workingInterest + (workingInterest / (Math.pow((1 + workingInterest), Number(paymentEstimateTermMonths)) - 1)));

        var monthlyPaymentLabel;

        if (isNaN(monthlyPayment)) {
          monthlyPaymentLabel = "Please check your entries... Only numbers allowed!";
        } else if (Number(paymentEstimateTermMonths) < 1) {
          monthlyPaymentLabel = "Term of loan must be at least 1 month";
        } else {
          monthlyPaymentLabel = "$" + monthlyPayment.toFixed(2) + "/month for " + paymentEstimateTermMonths + " months";
        }

        $('#PaymentEstimate_monthlyPayment').text(monthlyPaymentLabel);
        $('#resultdiv_js').css("display", "block");
        $('#calc_form_div_js').css("display", "none");

        if (hasSessionStorage) {
          win.sessionStorage.setItem('jsCalcPaymentEstimate', JSON.stringify({
            purchasePrice: paymentEstimatePurchasePrice,
            cashRebate: paymentEstimateCashRebate,
            tradeIn: paymentEstimateTradeIn,
            tradeInOwed: paymentEstimateTradeInOwed,
            downPayment: paymentEstimateDownPayment,
            interestRate: paymentEstimateInterestRate,
            termMonths: paymentEstimateTermMonths,
            trimid: $paymentEstimateSelectedTrimId,
            trimlabel: $paymentEstimateSelectedTrimLabel
          }));
        }

        if ($paymentEstimateSelectedTrimId.length === 5) {
          jsCalcPaymentEstimate.showSimilarVehicles($paymentEstimateSelectedTrimId);
        } else {
          jsCalcPaymentEstimate.showSimilarVehiclesByPrice(paymentEstimatePurchasePrice);
        }

      },
      showSimilarVehicles: function (trimid) {

        var url = '/api/research/similarvehicles/',
          jsonString = "{trimid: '" + trimid + "'}",
          trimlist = '';

        $.post(url, { '': jsonString })
          .done(function(resp) {
            trimlist = resp.data;

            if (!(trimlist === null || trimlist.length < 1)) {
              $('#js_similar_vehicles_content')
                .empty()
                .prepend(tmpls.similar_vehicles({ trimlist: trimlist }));

              $('#js_similar_vehicles').show();
            }
          });
      },
      showSimilarVehiclesByPrice: function (price) {

        var url = '/api/research/similarvehiclesbyprice/',
          jsonString = "{price: '" + price + "'}",
          trimlist = '';

        $.post(url, { '': jsonString })
          .done(function (resp) {
            trimlist = resp.data;

            if (!(trimlist === null || trimlist.length < 1)) {
              $('#js_similar_vehicles_content')
                .empty()
                .prepend(tmpls.similar_vehicles({ trimlist: trimlist }));

              $('#js_similar_vehicles').show();
            }
          });
      },
      initAddChangeOverlay: function () {
        $.when(
          $.get('/api/compare/makes/').done(function (resp) {
            if (resp.data) {
              $body.append(tmpls.add_change_car_overlay({ makeArr: resp.data }));
            }
          })
        )
        .then(function () {
          $addChangeCarOverlay = $('#add_change_car_overlay_js'),
          $overlayCloseBtn = $('.overlay-close'),
          $overlayCloseBtn.on('click', function (e) {
            e.preventDefault();
            $addChangeCarOverlay.popup('hide');
          });
          $paymentEstimateModelSelect = $('#paymentEstimateModelSelect');
          $paymentEstimateTrimSelect = $('#paymentEstimateTrimSelect');

          $('#paymentEstimateMakeSelect').on('change', function (e) {
            $paymentEstimateAddSelectedMake = $('#paymentEstimateMakeSelect :selected').val();
            if ($paymentEstimateAddSelectedMake.length > 0) {
              jsCalcPaymentEstimate.updateCompareModelDropdown();
            }
            else {
              $paymentEstimateModelSelect.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
              $paymentEstimateTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $paymentEstimateModelSelect.on('change', function (e) {
            $paymentEstimateAddSelectedModel = $('#paymentEstimateModelSelect :selected').val();
            if ($paymentEstimateAddSelectedModel.length > 0) {
              jsCalcPaymentEstimate.updateCompareTrimDropdown();
            }
            else {
              $paymentEstimateTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $paymentEstimateTrimSelect.on('change', function (e) {

            $paymentEstimateSelectedTrim = $('#paymentEstimateTrimSelect :selected').text();
            $paymentEstimateSelectedTrimMsrpIdValue = $('#paymentEstimateTrimSelect :selected').val();
            
            $paymentEstimateSelectedTrimId = $paymentEstimateSelectedTrimMsrpIdValue.split("|")[1];
            $paymentEstimateSelectedTrimMsrpValue = $paymentEstimateSelectedTrimMsrpIdValue.split("|")[0];
            
            $paymentEstimateSelectedTrimYear = $paymentEstimateSelectedTrim.substring(0, 4);
            $paymentEstimateSelectedTrimName = $paymentEstimateSelectedTrim.substring(5);

            $capitalizedMake = $paymentEstimateAddSelectedMake.charAt(0).toUpperCase() + $paymentEstimateAddSelectedMake.substring(1);
            $capitalizedModel = $paymentEstimateAddSelectedModel.charAt(0).toUpperCase() + $paymentEstimateAddSelectedModel.substring(1);

            $paymentEstimateSelectedTrimLabel = $paymentEstimateSelectedTrimYear + ' ' + $capitalizedMake + ' ' + $capitalizedModel + ' ' + $paymentEstimateSelectedTrimName;
            
            if ($paymentEstimateSelectedTrimMsrpValue.length > 0) {
              $('#PaymentEstimate_purchasePrice').val($paymentEstimateSelectedTrimMsrpValue);
              $('#js_selected_trim_label').html($paymentEstimateSelectedTrimLabel);
              $addChangeCarOverlay.popup('hide');
            }
          });

        }); // End of then function

        return true;
      },
      updateCompareModelDropdown: function () {
        var url = '/api/compare/make/' + $paymentEstimateAddSelectedMake + '/super-models/';

        $.when(
          $.get(url).done(function (resp) {
            if (resp.data) {
              $paymentEstimateModelSelect.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $paymentEstimateTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          })
        )
        .then(function () {

        }); // End of then function

      },

      updateCompareTrimDropdown: function () {
        var url = '/api/compare/make/' + $paymentEstimateAddSelectedMake + '/super-model/' + $paymentEstimateAddSelectedModel + '/trims/';

        $.get(url).done(function (resp) {
          if (resp.data) {
            $paymentEstimateTrimSelect.empty().prepend(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);
          }
        });

      }
    };

    $(function () {
      jsCalcPaymentEstimate.init();
      jsCalcPaymentEstimate.initAddChangeOverlay();
    });

  });
})(window, jQuery, ABT)