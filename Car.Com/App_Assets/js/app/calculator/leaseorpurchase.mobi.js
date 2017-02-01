
(function (win, $, abt) {

  var hasSessionStorage = abt.UTILS.hasSessionStorage;

  $(function () {

    var env = abt.ENV,
        getJsFileNameByAlias = env.getJsFileNameByAlias,
        libsPath = env.libsPath,
        $body = $('body'),
        tmpls = require('./leaseorpurchase_tmpls/desk/tmpls.js'),
        $addChangeCarOverlay = $('#add_change_car_overlay_js'),
        $leaseOrPurchaseModelSelect = '',
        $leaseOrPurchaseTrimSelect = '',
        $leaseOrPurchaseAddSelectedMake = '',
        $leaseOrPurchaseAddSelectedModel = '',
        $leaseOrPurchaseSelectedTrim = '',
        $leaseOrPurchaseSelectedTrimId = '',
        $leaseOrPurchaseSelectedTrimName = '',
        $leaseOrPurchaseSelectedTrimYear = '',
        $leaseOrPurchaseSelectedTrimLabel = '',
        $leaseOrPurchaseSelectedTrimMsrpIdValue = '',
        $leaseOrPurchaseSelectedTrimMsrpValue = '',
        $capitalizedMake = '',
        $capitalizedModel = '',
        $overlayCloseBtn = '';

    var jsCalcLeaseOrPurchase = {
      init: function () {
        $('#resultdiv_js').css("display", "none");
        $('#calc_form_div_js').css("display", "block");

        $('#btnCalculateLeaseOrPurchase').on('click', function (e) {
          e.preventDefault();
          jsCalcLeaseOrPurchase.calculateLeaseOrPurchase();
        });

        $('#LeaseOrPurchase_purchasePrice').on('click', function (e) {
          $('#LeaseOrPurchase_purchasePrice').val("");
          $('#js_selected_trim_label').html("");
          $leaseOrPurchaseSelectedTrimId = '';
          $leaseOrPurchaseSelectedTrimLabel = '';
        });

        $('#js_add_compare_car').on('click', function (e) {
          e.preventDefault();
          $addChangeCarOverlay.popup('show');
        });

        if (hasSessionStorage) {
          var storedValues = JSON.parse(win.sessionStorage.getItem('jsCalcLeaseOrPurchase'));
          if (storedValues !== null) {

            $('#LeaseOrPurchase_downPayment').val(storedValues.downPayment);
            $('#LeaseOrPurchase_termMonths').val(storedValues.termMonths);
            $('#LeaseOrPurchase_loanRate').val(storedValues.loanRate);
            $('#LeaseOrPurchase_residualValue').val(storedValues.residualValue);

            $('#LeaseOrPurchase_leasePayment').val(storedValues.leasePayment);
            $('#LeaseOrPurchase_leaseFee').val(storedValues.leaseFee);

            $('#LeaseOrPurchase_purchasePrice').val(storedValues.purchasePrice);
            $('#LeaseOrPurchase_rebate').val(storedValues.rebate);

            $leaseOrPurchaseSelectedTrimId = storedValues.trimid;
            $leaseOrPurchaseSelectedTrimLabel = storedValues.trimlabel;

          }
        }

        if ($leaseOrPurchaseSelectedTrimLabel.length > 0) {
          $('#js_selected_trim_label').html($leaseOrPurchaseSelectedTrimLabel);
        }

      },
      calculateLeaseOrPurchase: function () {
        var leaseOrPurchaseDownPayment = $('#LeaseOrPurchase_downPayment').val().replace(/[^0-9.]+/g, "");
        var leaseOrPurchaseTermMonths = $('#LeaseOrPurchase_termMonths').val().replace(/[^0-9.]+/g, "");
        var leaseOrPurchaseLoanRate = $('#LeaseOrPurchase_loanRate').val().replace(/[^0-9.]+/g, "");
        var leaseOrPurchaseResidualValue = $('#LeaseOrPurchase_residualValue').val().replace(/[^0-9.]+/g, "");

        var leaseOrPurchaseLeasePayment = $('#LeaseOrPurchase_leasePayment').val().replace(/[^0-9.]+/g, "");
        var leaseOrPurchaseLeaseFee = $('#LeaseOrPurchase_leaseFee').val().replace(/[^0-9.]+/g, "");

        var leaseOrPurchasePurchasePrice = $('#LeaseOrPurchase_purchasePrice').val().replace(/[^0-9.]+/g, "");
        var leaseOrPurchaseRebate = $('#LeaseOrPurchase_rebate').val().replace(/[^0-9.]+/g, "");

        var totalLeaseCost = (Number(leaseOrPurchaseLeasePayment) * Number(leaseOrPurchaseTermMonths)) + Number(leaseOrPurchaseLeaseFee) + Number(leaseOrPurchaseDownPayment);
        var deductions = Number(leaseOrPurchaseDownPayment) + Number(leaseOrPurchaseRebate);
        var amtFinanced = Number(leaseOrPurchasePurchasePrice) - Number(deductions);
        var purchaseMonthlyPayment = (Number(amtFinanced) + (Number(amtFinanced) * Number(leaseOrPurchaseLoanRate) / 100)) / Number(leaseOrPurchaseTermMonths);
        var totalBuyCost = ((Number(purchaseMonthlyPayment) * Number(leaseOrPurchaseTermMonths)) + Number(leaseOrPurchaseDownPayment)) - Number(leaseOrPurchaseResidualValue);

        var leaseOrPurchaseLabel;

        if (isNaN(totalLeaseCost) || isNaN(totalBuyCost)) {
          leaseOrPurchaseLabel = "Please check your entries... Only numbers allowed!";
        } else if (Number(leaseOrPurchaseTermMonths) < 1) {
          leaseOrPurchaseLabel = "Term of loan must be at least 1 month";
        } else {
          if (Number(totalBuyCost) > Number(totalLeaseCost)) {
            var leaseSavings = Number(totalBuyCost) - Number(totalLeaseCost);
            leaseOrPurchaseLabel = "LEASE! You'll save $" + Math.round(leaseSavings) + "!";
          } else {
            var purchaseSavings = Number(totalLeaseCost) - Number(totalBuyCost);
            leaseOrPurchaseLabel = "PURCHASE! You'll save $" + Math.round(purchaseSavings) + "!";
          }
        }

        $('#LeaseOrPurchase_decision').text(leaseOrPurchaseLabel);
        $('#resultdiv_js').css("display", "block");
        $('#calc_form_div_js').css("display", "none");

        if (hasSessionStorage) {
          win.sessionStorage.setItem('jsCalcLeaseOrPurchase', JSON.stringify({

            downPayment: leaseOrPurchaseDownPayment,
            termMonths: leaseOrPurchaseTermMonths,
            loanRate: leaseOrPurchaseLoanRate,
            residualValue: leaseOrPurchaseResidualValue,

            leasePayment: leaseOrPurchaseLeasePayment,
            leaseFee: leaseOrPurchaseLeaseFee,

            purchasePrice: leaseOrPurchasePurchasePrice,
            rebate: leaseOrPurchaseRebate,

            trimid: $leaseOrPurchaseSelectedTrimId,
            trimlabel: $leaseOrPurchaseSelectedTrimLabel
          }));
        }

        if ($leaseOrPurchaseSelectedTrimId.length === 5) {
          jsCalcLeaseOrPurchase.showSimilarVehicles($leaseOrPurchaseSelectedTrimId);
        } else {
          jsCalcLeaseOrPurchase.showSimilarVehiclesByPrice(leaseOrPurchasePurchasePrice);
        }

      },
      showSimilarVehicles: function (trimid) {

        var url = '/api/research/similarvehicles/',
          jsonString = "{trimid: '" + trimid + "'}",
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
          $leaseOrPurchaseModelSelect = $('#leaseOrPurchaseModelSelect');
          $leaseOrPurchaseTrimSelect = $('#leaseOrPurchaseTrimSelect');

          $('#leaseOrPurchaseMakeSelect').on('change', function (e) {
            $leaseOrPurchaseAddSelectedMake = $('#leaseOrPurchaseMakeSelect :selected').val();
            if ($leaseOrPurchaseAddSelectedMake.length > 0) {
              jsCalcLeaseOrPurchase.updateCompareModelDropdown();
            }
            else {
              $leaseOrPurchaseModelSelect.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
              $leaseOrPurchaseTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $leaseOrPurchaseModelSelect.on('change', function (e) {
            $leaseOrPurchaseAddSelectedModel = $('#leaseOrPurchaseModelSelect :selected').val();
            if ($leaseOrPurchaseAddSelectedModel.length > 0) {
              jsCalcLeaseOrPurchase.updateCompareTrimDropdown();
            }
            else {
              $leaseOrPurchaseTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $leaseOrPurchaseTrimSelect.on('change', function (e) {

            $leaseOrPurchaseSelectedTrim = $('#leaseOrPurchaseTrimSelect :selected').text();
            $leaseOrPurchaseSelectedTrimMsrpIdValue = $('#leaseOrPurchaseTrimSelect :selected').val();

            $leaseOrPurchaseSelectedTrimId = $leaseOrPurchaseSelectedTrimMsrpIdValue.split("|")[1];
            $leaseOrPurchaseSelectedTrimMsrpValue = $leaseOrPurchaseSelectedTrimMsrpIdValue.split("|")[0];

            $leaseOrPurchaseSelectedTrimYear = $leaseOrPurchaseSelectedTrim.substring(0, 4);
            $leaseOrPurchaseSelectedTrimName = $leaseOrPurchaseSelectedTrim.substring(5);

            $capitalizedMake = $leaseOrPurchaseAddSelectedMake.charAt(0).toUpperCase() + $leaseOrPurchaseAddSelectedMake.substring(1);
            $capitalizedModel = $leaseOrPurchaseAddSelectedModel.charAt(0).toUpperCase() + $leaseOrPurchaseAddSelectedModel.substring(1);

            $leaseOrPurchaseSelectedTrimLabel = $leaseOrPurchaseSelectedTrimYear + ' ' + $capitalizedMake + ' ' + $capitalizedModel + ' ' + $leaseOrPurchaseSelectedTrimName;

            if ($leaseOrPurchaseSelectedTrimMsrpValue.length > 0) {
              $('#LeaseOrPurchase_purchasePrice').val($leaseOrPurchaseSelectedTrimMsrpValue);
              $('#js_selected_trim_label').html($leaseOrPurchaseSelectedTrimLabel);
              $addChangeCarOverlay.popup('hide');
            }
          });

        }); // End of then function

        return true;
      },
      updateCompareModelDropdown: function () {
        var url = '/api/compare/make/' + $leaseOrPurchaseAddSelectedMake + '/super-models/';

        $.when(
          $.get(url).done(function (resp) {
            if (resp.data) {
              $leaseOrPurchaseModelSelect.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $leaseOrPurchaseTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          })
        )
        .then(function () {

        }); // End of then function

      },
      updateCompareTrimDropdown: function () {
        var url = '/api/compare/make/' + $leaseOrPurchaseAddSelectedMake + '/super-model/' + $leaseOrPurchaseAddSelectedModel + '/trims/';

        $.get(url).done(function (resp) {
          if (resp.data) {
            $leaseOrPurchaseTrimSelect.empty().prepend(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);
          }
        });

      }

    };

    $(function () {
      jsCalcLeaseOrPurchase.init();
      jsCalcLeaseOrPurchase.initAddChangeOverlay();
    });

  });
})(window, jQuery, ABT)