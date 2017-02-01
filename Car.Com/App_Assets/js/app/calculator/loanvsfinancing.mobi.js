
(function (win, $, abt) {

  var hasSessionStorage = abt.UTILS.hasSessionStorage;

  $(function () {

    var env = abt.ENV,
        getJsFileNameByAlias = env.getJsFileNameByAlias,
        libsPath = env.libsPath,
        $body = $('body'),
        tmpls = require('./loanvsfinancing_tmpls/desk/tmpls.js'),
        $addChangeCarOverlay = $('#add_change_car_overlay_js'),
        $rebateVsFinancingModelSelect = '',
        $rebateVsFinancingTrimSelect = '',
        $rebateVsFinancingAddSelectedMake = '',
        $rebateVsFinancingAddSelectedModel = '',
        $rebateVsFinancingSelectedTrim = '',
        $rebateVsFinancingSelectedTrimId = '',
        $rebateVsFinancingSelectedTrimName = '',
        $rebateVsFinancingSelectedTrimYear = '',
        $rebateVsFinancingSelectedTrimLabel = '',
        $rebateVsFinancingSelectedTrimMsrpIdValue = '',
        $rebateVsFinancingSelectedTrimMsrpValue = '',
        $capitalizedMake = '',
        $capitalizedModel = '',
        $overlayCloseBtn = '';

    var jsCalcLoanVsFinancing = {
      init: function () {
        $('#resultdiv_js').css("display", "none");
        $('#calc_form_div_js').css("display", "block");

        $('#btnCalculateRebateVsFinancing').on('click', function (e) {
          e.preventDefault();
          jsCalcLoanVsFinancing.calculateRebateVsFinancing();
        });

        $('#RebateVsFinancing_salesPrice').on('click', function (e) {
          $('#RebateVsFinancing_salesPrice').val("");
          $('#js_selected_trim_label').html("");
          $rebateVsFinancingSelectedTrimId = '';
          $rebateVsFinancingSelectedTrimLabel = '';
        });

        $('#js_add_compare_car').on('click', function (e) {
          e.preventDefault();
          $addChangeCarOverlay.popup('show');
        });

        if (hasSessionStorage) {
          var storedValues = JSON.parse(win.sessionStorage.getItem('jsCalcLoanVsFinancing'));
          if (storedValues !== null) {
            $('#RebateVsFinancing_salesPrice').val(storedValues.salesPrice);
            $('#RebateVsFinancing_rebate').val(storedValues.rebate);
            $('#RebateVsFinancing_interestRate').val(storedValues.interestRate);
            $('#RebateVsFinancing_termYears').val(storedValues.termYears);
            $rebateVsFinancingSelectedTrimId = storedValues.trimid;
            $rebateVsFinancingSelectedTrimLabel = storedValues.trimlabel;
          }
        }

        if ($rebateVsFinancingSelectedTrimLabel.length > 0) {
          $('#js_selected_trim_label').html($rebateVsFinancingSelectedTrimLabel);
        }

      },

      calculateRebateVsFinancing: function () {
        var rebateVsFinancingSalesPrice = $('#RebateVsFinancing_salesPrice').val().replace(/[^0-9.]+/g, "");
        var rebateVsFinancingRebate = $('#RebateVsFinancing_rebate').val().replace(/[^0-9.]+/g, "");
        var rebateVsFinancingInterestRate = $('#RebateVsFinancing_interestRate').val().replace(/[^0-9.]+/g, "");
        var rebateVsFinancingTermYears = $('#RebateVsFinancing_termYears').val().replace(/[^0-9.]+/g, "");

        var amtFinanced = Number(rebateVsFinancingSalesPrice) - Number(rebateVsFinancingRebate);
        var interestConsideration = Number(amtFinanced) * Number(rebateVsFinancingInterestRate) / 100;
        var amtPaid = Number(amtFinanced) + Number(interestConsideration);
        var rebateVsFinancingLabel;

        if (isNaN(amtPaid)) {
          rebateVsFinancingLabel = "Please check your entries... Only numbers allowed!";
        } else if (Number(rebateVsFinancingRebate) < 1) {
          rebateVsFinancingLabel = "Please enter a rebate amount...";
        } else {
          if (Number(amtPaid) > Number(rebateVsFinancingSalesPrice)) {
            rebateVsFinancingLabel = "Take the 0% financing!";
          } else {
            rebateVsFinancingLabel = "Take the rebate!";
          }
        }

        $('#RebateVsFinancing_decision').text(rebateVsFinancingLabel);
        $('#resultdiv_js').css("display", "block");
        $('#calc_form_div_js').css("display", "none");

        if (hasSessionStorage) {
          win.sessionStorage.setItem('jsCalcLoanVsFinancing', JSON.stringify({
            salesPrice: rebateVsFinancingSalesPrice,
            rebate: rebateVsFinancingRebate,
            interestRate: rebateVsFinancingInterestRate,
            termYears: rebateVsFinancingTermYears,
            trimid: $rebateVsFinancingSelectedTrimId,
            trimlabel: $rebateVsFinancingSelectedTrimLabel
          }));
        }

        if ($rebateVsFinancingSelectedTrimId.length === 5) {
          jsCalcLoanVsFinancing.showSimilarVehicles($rebateVsFinancingSelectedTrimId);
        } else {
          jsCalcLoanVsFinancing.showSimilarVehiclesByPrice(rebateVsFinancingSalesPrice);
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
          $rebateVsFinancingModelSelect = $('#rebateVsFinancingModelSelect');
          $rebateVsFinancingTrimSelect = $('#rebateVsFinancingTrimSelect');

          $('#rebateVsFinancingMakeSelect').on('change', function (e) {
            $rebateVsFinancingAddSelectedMake = $('#rebateVsFinancingMakeSelect :selected').val();
            if ($rebateVsFinancingAddSelectedMake.length > 0) {
              jsCalcLoanVsFinancing.updateCompareModelDropdown();
            }
            else {
              $rebateVsFinancingModelSelect.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
              $rebateVsFinancingTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $rebateVsFinancingModelSelect.on('change', function (e) {
            $rebateVsFinancingAddSelectedModel = $('#rebateVsFinancingModelSelect :selected').val();
            if ($rebateVsFinancingAddSelectedModel.length > 0) {
              jsCalcLoanVsFinancing.updateCompareTrimDropdown();
            }
            else {
              $rebateVsFinancingTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $rebateVsFinancingTrimSelect.on('change', function (e) {

            $rebateVsFinancingSelectedTrim = $('#rebateVsFinancingTrimSelect :selected').text();
            $rebateVsFinancingSelectedTrimMsrpIdValue = $('#rebateVsFinancingTrimSelect :selected').val();

            $rebateVsFinancingSelectedTrimId = $rebateVsFinancingSelectedTrimMsrpIdValue.split("|")[1];
            $rebateVsFinancingSelectedTrimMsrpValue = $rebateVsFinancingSelectedTrimMsrpIdValue.split("|")[0];

            $rebateVsFinancingSelectedTrimYear = $rebateVsFinancingSelectedTrim.substring(0, 4);
            $rebateVsFinancingSelectedTrimName = $rebateVsFinancingSelectedTrim.substring(5);

            $capitalizedMake = $rebateVsFinancingAddSelectedMake.charAt(0).toUpperCase() + $rebateVsFinancingAddSelectedMake.substring(1);
            $capitalizedModel = $rebateVsFinancingAddSelectedModel.charAt(0).toUpperCase() + $rebateVsFinancingAddSelectedModel.substring(1);

            $rebateVsFinancingSelectedTrimLabel = $rebateVsFinancingSelectedTrimYear + ' ' + $capitalizedMake + ' ' + $capitalizedModel + ' ' + $rebateVsFinancingSelectedTrimName;

            if ($rebateVsFinancingSelectedTrimMsrpValue.length > 0) {
              $('#RebateVsFinancing_salesPrice').val($rebateVsFinancingSelectedTrimMsrpValue);
              $('#js_selected_trim_label').html($rebateVsFinancingSelectedTrimLabel);
              $addChangeCarOverlay.popup('hide');
            }
          });

        }); // End of then function

        return true;
      },
      updateCompareModelDropdown: function () {
        var url = '/api/compare/make/' + $rebateVsFinancingAddSelectedMake + '/super-models/';

        $.when(
          $.get(url).done(function (resp) {
            if (resp.data) {
              $rebateVsFinancingModelSelect.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $rebateVsFinancingTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          })
        )
        .then(function () {

        }); // End of then function

      },

      updateCompareTrimDropdown: function () {
        var url = '/api/compare/make/' + $rebateVsFinancingAddSelectedMake + '/super-model/' + $rebateVsFinancingAddSelectedModel + '/trims/';

        $.get(url).done(function (resp) {
          if (resp.data) {
            $rebateVsFinancingTrimSelect.empty().prepend(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);
          }
        });

      }
    };

    $(function () {
      jsCalcLoanVsFinancing.init();
      jsCalcLoanVsFinancing.initAddChangeOverlay();
    });

  });
})(window, jQuery, ABT)