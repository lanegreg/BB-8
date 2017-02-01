
(function (win, $, abt) {

  var hasSessionStorage = abt.UTILS.hasSessionStorage;

  $(function () {

    var jsCalcFuelEfficiency = {
      init: function () {
        $('#resultdiv_js').css("display", "none");
        $('#calc_form_div_js').css("display", "block");

        $('#btnCalculateFuelSavings').on('click', function (e) {
          e.preventDefault();
          jsCalcFuelEfficiency.calculateFuelSavings();
        });

        if (hasSessionStorage) {
          var storedValues = JSON.parse(win.sessionStorage.getItem('jsCalcFuelEfficiency'));
          if (storedValues !== null) {

            $('#FuelEfficiency_gasCostGallon').val(storedValues.gasCostGallon);
            $('#FuelEfficiency_milesDrivenDaily').val(storedValues.milesDrivenDaily);
            $('#FuelEfficiency_mpgOld').val(storedValues.mpgOld);
            $('#FuelEfficiency_mpgNew').val(storedValues.mpgNew);

            $('#FuelEfficiency_paymentOld').val(storedValues.paymentOld);
            $('#FuelEfficiency_termOld').val(storedValues.termOld);
            $('#FuelEfficiency_salesOld').val(storedValues.salesOld);

            $('#FuelEfficiency_termNew').val(storedValues.termNew);
            $('#FuelEfficiency_priceNew').val(storedValues.priceNew);
            $('#FuelEfficiency_downNew').val(storedValues.downNew);
            $('#FuelEfficiency_intRateNew').val(storedValues.intRateNew);
            $('#FuelEfficiency_feesNew').val(storedValues.feesNew);
            $('#FuelEfficiency_creditNew').val(storedValues.creditNew);

          }
        }

      },

      calculateFuelSavings: function () {
        var fuelEfficiencyGasCostGallon = $('#FuelEfficiency_gasCostGallon').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyMilesDrivenDaily = $('#FuelEfficiency_milesDrivenDaily').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyMpgOld = $('#FuelEfficiency_mpgOld').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyMpgNew = $('#FuelEfficiency_mpgNew').val().replace(/[^0-9.]+/g, "");

        var fuelEfficiencyPaymentOld = $('#FuelEfficiency_paymentOld').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyTermOld = $('#FuelEfficiency_termOld').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencySalesOld = $('#FuelEfficiency_salesOld').val().replace(/[^0-9.]+/g, "");

        var fuelEfficiencyTermNew = $('#FuelEfficiency_termNew').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyPriceNew = $('#FuelEfficiency_priceNew').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyDownNew = $('#FuelEfficiency_downNew').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyIntRateNew = $('#FuelEfficiency_intRateNew').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyFeesNew = $('#FuelEfficiency_feesNew').val().replace(/[^0-9.]+/g, "");
        var fuelEfficiencyCreditNew = $('#FuelEfficiency_creditNew').val().replace(/[^0-9.]+/g, "");

        var amtFinanced = (Number(fuelEfficiencyPriceNew) + Number(fuelEfficiencyFeesNew)) - (Number(fuelEfficiencyDownNew) + Number(fuelEfficiencyCreditNew));
        var purchasePmt = (Number(amtFinanced) + (Number(amtFinanced) * Number(fuelEfficiencyIntRateNew) / 100)) / Number(fuelEfficiencyTermNew);
        var oldCarNet = Number(fuelEfficiencySalesOld) - (Number(fuelEfficiencyPaymentOld) * Number(fuelEfficiencyTermOld));
        var newCarNet = (Number(purchasePmt) * Number(fuelEfficiencyTermNew)) + Number(fuelEfficiencyDownNew);
        var netCarChangeCost = Number(newCarNet) - Number(oldCarNet);
        var newGasPeryear = Number(fuelEfficiencyGasCostGallon) * (Number(fuelEfficiencyMilesDrivenDaily) / Number(fuelEfficiencyMpgNew)) * 365;
        var oldGasPeryear = Number(fuelEfficiencyGasCostGallon) * (Number(fuelEfficiencyMilesDrivenDaily) / Number(fuelEfficiencyMpgOld)) * 365;
        var gasSavingsPerYear = Number(oldGasPeryear) - Number(newGasPeryear);
        var gasSavingsPerMonth = Number(gasSavingsPerYear) / 12;
        var breakEvenMonths = Number(netCarChangeCost) / Number(gasSavingsPerMonth);

        var fuelEfficiencyLabel;

        if (isNaN(breakEvenMonths) || isNaN(fuelEfficiencyTermNew) || isNaN(gasSavingsPerYear)) {
          fuelEfficiencyLabel = "Please check your entries... Only numbers allowed!";
        } else {
          var gasSavingsString = gasSavingsPerYear.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
          gasSavingsString = gasSavingsString.substring(0, gasSavingsString.length - 3);

          if (Math.round(breakEvenMonths) <= Math.round(fuelEfficiencyTermNew)) {
            fuelEfficiencyLabel = "UPGRADE! You'll save $" + gasSavingsString + " per year after " + Math.round(breakEvenMonths) + " months.";
          } else {
            fuelEfficiencyLabel = "DON'T UPGRADE. You won't begin saving $" + gasSavingsString + " per year until after " + Math.round(breakEvenMonths) + " months.";
          }
        }

        $('#FuelEfficiency_decision').text(fuelEfficiencyLabel);
        $('#resultdiv_js').css("display", "block");
        $('#calc_form_div_js').css("display", "none");

        if (hasSessionStorage) {
          win.sessionStorage.setItem('jsCalcFuelEfficiency', JSON.stringify({

            gasCostGallon: fuelEfficiencyGasCostGallon,
            milesDrivenDaily: fuelEfficiencyMilesDrivenDaily,
            mpgOld: fuelEfficiencyMpgOld,
            mpgNew: fuelEfficiencyMpgNew,

            paymentOld: fuelEfficiencyPaymentOld,
            termOld: fuelEfficiencyTermOld,
            salesOld: fuelEfficiencySalesOld,

            termNew: fuelEfficiencyTermNew,
            priceNew: fuelEfficiencyPriceNew,
            downNew: fuelEfficiencyDownNew,
            intRateNew: fuelEfficiencyIntRateNew,
            feesNew: fuelEfficiencyFeesNew,
            creditNew: fuelEfficiencyCreditNew
          }));
        }

      }
    };

    $(function () {
      jsCalcFuelEfficiency.init();
    });

  });
})(window, jQuery, ABT)