
(function (win, $, abt) {

  var hasSessionStorage = abt.UTILS.hasSessionStorage;

  $(function () {

    var jsCalcAcceleratedPayoff = {
      init: function () {
        $('#resultdiv_js').css("display", "none");
        $('#calc_form_div_js').css("display", "block");

        $('#btnCalculateAcceleratedPayoff').on('click', function (e) {
          e.preventDefault();
          jsCalcAcceleratedPayoff.calculateAccelPayoffMonths();
        });

        if (hasSessionStorage) {
          var storedValues = JSON.parse(win.sessionStorage.getItem('jsCalcAcceleratedPayoff'));
          if (storedValues !== null) {
            $("#AcceleratedPayoff_loanAmount").val(storedValues.loanAmount);
            $("#AcceleratedPayoff_interestRate").val(storedValues.interestRate);
            $("#AcceleratedPayoff_termMonths").val(storedValues.termMonths);
            $("#AcceleratedPayoff_paymentsMade").val(storedValues.paymentsMade);
            $("#AcceleratedPayoff_additionalPayment").val(storedValues.additionalPayment);
          }
        }

      },

      calculateAccelPayoffMonths: function () {
        var acceleratedPayoffLoanAmount = $('#AcceleratedPayoff_loanAmount').val().replace(/[^0-9.]+/g, "");
        var acceleratedPayoffInterestRate = $('#AcceleratedPayoff_interestRate').val().replace(/[^0-9.]+/g, "");
        var acceleratedPayoffTermMonths = $('#AcceleratedPayoff_termMonths').val().replace(/[^0-9.]+/g, "");
        var acceleratedPayoffPaymentsMade = $('#AcceleratedPayoff_paymentsMade').val().replace(/[^0-9.]+/g, "");
        var acceleratedPayoffAdditionalPayment = $('#AcceleratedPayoff_additionalPayment').val().replace(/[^0-9.]+/g, "");

        var currentMonthlyPayment = (Number(acceleratedPayoffLoanAmount) + (Number(acceleratedPayoffLoanAmount) * (Number(acceleratedPayoffInterestRate) / 100))) / Number(acceleratedPayoffTermMonths);
        var currentRemainingBalance = Number(acceleratedPayoffLoanAmount) - (Number(currentMonthlyPayment) * Number(acceleratedPayoffPaymentsMade));
        var oldMonthsLeft = Number(acceleratedPayoffTermMonths) - Number(acceleratedPayoffPaymentsMade);
        var acceleratedPayoffMonths = Number(currentRemainingBalance) / (Number(currentMonthlyPayment) + Number(acceleratedPayoffAdditionalPayment));
        var differenceInMonths = Number(oldMonthsLeft) - Number(acceleratedPayoffMonths);

        var acceleratedPayoffLabel;

        if (isNaN(acceleratedPayoffMonths)) {
          acceleratedPayoffLabel = "Please check your entries... Only numbers allowed!";
        } else if (Number(acceleratedPayoffAdditionalPayment) < 1) {
          acceleratedPayoffLabel = "Please enter a proposed additional monthly payment...";
        } else if (Number(acceleratedPayoffTermMonths) < 1) {
          acceleratedPayoffLabel = "Term of loan must be at least 1 month";
        } else {
          acceleratedPayoffLabel = "An extra $" + acceleratedPayoffAdditionalPayment + " per month will pay off your loan " + Math.round(differenceInMonths) + " months early!";
        }

        $('#AcceleratedPayoff_earlyPayoffResult').text(acceleratedPayoffLabel);
        $('#resultdiv_js').css("display", "block");
        $('#calc_form_div_js').css("display", "none");

        if (hasSessionStorage) {
          win.sessionStorage.setItem('jsCalcAcceleratedPayoff', JSON.stringify({
            loanAmount: acceleratedPayoffLoanAmount,
            interestRate: acceleratedPayoffInterestRate,
            termMonths: acceleratedPayoffTermMonths,
            paymentsMade: acceleratedPayoffPaymentsMade,
            additionalPayment: acceleratedPayoffAdditionalPayment
          }));
        }

      }

    };

    $(function () {
      jsCalcAcceleratedPayoff.init();
    });

  });
})(window, jQuery, ABT)