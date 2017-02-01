
(function (win, $, abt) {

  var hasSessionStorage = abt.UTILS.hasSessionStorage;

  $(function () {

    var jsCalcHowMuchCanIAfford = {
      init: function () {
        $('#resultdiv_js').css("display", "none");
        $('#calc_form_div_js').css("display", "block");

        $('#btnCalculatePrice').on('click', function (e) {
          e.preventDefault();
          jsCalcHowMuchCanIAfford.calculatePrice();
        });

        if (hasSessionStorage) {
          var storedValues = JSON.parse(win.sessionStorage.getItem('jsCalcHowMuchCanIAfford'));
          if (storedValues !== null) {
            $("#HowMuchCanIAfford_downPayment").val(storedValues.downPayment);
            $("#HowMuchCanIAfford_monthlyPayment").val(storedValues.monthlyPayment);
            $("#HowMuchCanIAfford_interestRate").val(storedValues.interestRate);
            $("#HowMuchCanIAfford_termMonths").val(storedValues.termMonths);
          }
        }
        
      },
      
      calculatePrice: function () {
        var howMuchCanIAffordDownPayment = $('#HowMuchCanIAfford_downPayment').val().replace(/[^0-9.]+/g, "");
        var howMuchCanIAffordMonthlyPayment = $('#HowMuchCanIAfford_monthlyPayment').val().replace(/[^0-9.]+/g, "");
        var howMuchCanIAffordInterestRate = $('#HowMuchCanIAfford_interestRate').val().replace(/[^0-9.]+/g, "");
        var howMuchCanIAffordTermMonths = $('#HowMuchCanIAfford_termMonths').val().replace(/[^0-9.]+/g, "");

        var workingInterest = Number(howMuchCanIAffordInterestRate) / 100 / 12;
        var purchasePrice = Number(howMuchCanIAffordMonthlyPayment) / (workingInterest + (workingInterest / (Math.pow((1 + workingInterest), Number(howMuchCanIAffordTermMonths)) - 1)));
        purchasePrice = purchasePrice + Number(howMuchCanIAffordDownPayment);

        var purchasePriceLabel;

        if (isNaN(purchasePrice)) {
          purchasePriceLabel = "Please check your entries... Only numbers allowed!";
        } else if (Number(howMuchCanIAffordTermMonths) < 1) {
          purchasePriceLabel = "Term of loan must be at least 1 month";
        } else {
          purchasePriceLabel = "Total Purchase Price: $" + purchasePrice.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
          purchasePriceLabel = purchasePriceLabel.substring(0, purchasePriceLabel.length - 3);
        }

        $('#HowMuchCanIAfford_purchasePrice').text(purchasePriceLabel);
        $('#resultdiv_js').css("display", "block");
        $('#calc_form_div_js').css("display", "none");

        if (hasSessionStorage) {
          win.sessionStorage.setItem('jsCalcHowMuchCanIAfford', JSON.stringify({
            downPayment: howMuchCanIAffordDownPayment,
            monthlyPayment: howMuchCanIAffordMonthlyPayment,
            interestRate: howMuchCanIAffordInterestRate,
            termMonths: howMuchCanIAffordTermMonths
          }));
        }

      }
      
    };

    $(function () {
      jsCalcHowMuchCanIAfford.init();
    });

  });
})(window, jQuery, ABT)