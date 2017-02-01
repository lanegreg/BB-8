/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */

(function (win, $) {
  $(function () {
    var abt = ABT
      , env = abt.ENV
      , car = abt.pageJson.vehicleinfo
      , tmpls = require('./trimincentives_tmpls/desk/tmpls.js')
      , personaService = ABT.PERSONA
      , getJsFileNameByAlias = env.getJsFileNameByAlias
      , getScriptUriByName = env.getScriptUriByName;

    var jsResearchTrimIncentive = {

      initCarsForSaleWidget: function () {
        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('c4swidget'))))
        .then(function () {
          abt.WDG.c4s.init({ make: car.make, model: car.model })
        })
      },

      initViewedRecentlyWidget: function () {
        $.when($.getCachedScript(getScriptUriByName(getJsFileNameByAlias('viewedrecentlywidget'))))
        .then(function () {
          abt.WDG.viewedrecently.init({ currentTrim: car.trimid })
        })
      },

      showZipIncentivesData: function (trimid, zipcode) {
        var url = '/api/research/trimincentives/',
            jsonString = "{trimid: '" + trimid + "', zip: '" + zipcode + "'}",
            $IncentivesNotAvailableDiv = $('#IncentivesNotAvailableDiv'),
            $IncentivesWithZipDiv = $('#IncentivesWithZipDiv'),
            trimincentives = '';

        $.post(url, { '': jsonString })
          .done(function (resp) {

            trimincentives = resp.data.trimincentives;

            if (trimincentives === null || trimincentives.length < 1) {
              $IncentivesNotAvailableDiv.show();
            }
            else {
              for (var td = 0; td < trimincentives.length; td++) {
                if (trimincentives[td].expires.length > 10) {
                  trimincentives[td].expires = trimincentives[td].expires.substr(5, 2) + '/' + trimincentives[td].expires.substr(8, 2) + '/' + trimincentives[td].expires.substr(0, 4);
                }
                if (trimincentives[td].groupdesc == "Public") { trimincentives[td].groupdesc = "Standard"; }
                trimincentives[td].catdesc = trimincentives[td].catdesc.replace("Stand Alone Retail", "").replace("OEM Finance", "")
              }

              $IncentivesNotAvailableDiv.hide();

              $IncentivesWithZipDiv
              .empty()
              .prepend(tmpls.incentives_by_zip({ incentives: trimincentives }))
              .show();
            }


            return true;
          }); //end done
      },
      
      init: function () {
        var $zipUpdateBtn = $('#VDRebateZipform'),
            $IncentivesNotAvailableDiv = $('#IncentivesDefaultDiv'),
            $IncentivesZipForm = $('#IncentivesZipForm'),
            trimid = $("#TrimId").val(),
            pData = personaService.get(),
            currentZip = pData.zipcode,
            regex = /^[0-9]+$/;

        if (currentZip !== null && currentZip.length == 5 && currentZip.match(regex)) {
          jsResearchTrimIncentive.showZipIncentivesData(trimid, currentZip);
        } else {
          $IncentivesZipForm.show();
        }

        $zipUpdateBtn.on('click', function(e) {
          e.preventDefault();
          var zipcode = $('#zipcode').val();
          var zipLen = zipcode.length;

          if (zipLen != 5 || !zipcode.match(regex)) {
            $('#zipErrorMessage').show();
            $('#zipcode').addClass('input-validation-error');
            return false;
          }

          $('#zipErrorMessage').hide();
          personaService.set({ zipcode: zipcode });

          $IncentivesZipForm.hide();
          jsResearchTrimIncentive.showZipIncentivesData(trimid, zipcode);

          return false;
        });

        jsResearchTrimIncentive.initCarsForSaleWidget();
        jsResearchTrimIncentive.initViewedRecentlyWidget();

      }
    };

    $(function () {
      $.when(personaService.isReady())
      .then(function() {
        jsResearchTrimIncentive.init();
      })
    });

    });
})(window, jQuery)