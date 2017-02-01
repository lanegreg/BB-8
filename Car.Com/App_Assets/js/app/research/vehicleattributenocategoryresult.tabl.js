
/**
 *	Autobytel (c)2014
 *	@author: Sam Schulte
 */

(function (win, $) {
  $(function () {

    var filtersModule = require('../../modules/abt-filters');

    var abt = ABT,
      env = abt.ENV,
      getJsFileNameByAlias = env.getJsFileNameByAlias,
      libsPath = env.libsPath;

    var jsVehicleAttributeNoCategoryResult = {

      init: function () {

        var hiddenTrimDivs = $("[id^='js-disablesupertrim']");
        for (var j = 0; j < hiddenTrimDivs.length; j++) {
          var hiddenTrimDivId = hiddenTrimDivs[j].id;
          jsVehicleAttributeNoCategoryResult.setShowHideTrimButtonClickHandler(hiddenTrimDivId);
        }

        var filterIcons = $("[id^='js-filter-icon']");
        for (j = 0; j < filterIcons.length; j++) {
          var filterIconId = filterIcons[j].id;
          jsVehicleAttributeNoCategoryResult.setFilterButtonClickHandler(filterIconId);
        }

        var showAllTrimsIds = $("[id^='js-show-all-trims']");
        for (j = 0; j < showAllTrimsIds.length; j++) {
          var showAllTrimsId = showAllTrimsIds[j].id;
          jsVehicleAttributeNoCategoryResult.setShowAllTrimsButtonClickHandler(showAllTrimsId);
        }

        var vehicleattributeseoname = ABT.ADS.pageCtx.vehicleattributeseoname;
        filtersModule.setVehicleAttributeNoCategoryFilterGroups(vehicleattributeseoname);
        filtersModule.setVehicleAttributeNoCategoryTrimFilterAttributes(vehicleattributeseoname);

        $("img.lazy").lazyload();
        
      },
      showHideTrimBlock: function (hiddenTrimButtonId) {

        var hiddenViewTrimsCollectionId = hiddenTrimButtonId.replace("js-disablesupertrim", "js-trimcollection");

        //if the supertrim is not grayed out and disabled from the filters.updateButtonCounts()...
        if (!$('#' + hiddenTrimButtonId).hasClass('disabled')) {

          //set-up the trims block visibility
          if ($('#' + hiddenViewTrimsCollectionId).css('display') == 'none') {
            $('#' + hiddenViewTrimsCollectionId).css('display', 'block');
          } else {
            $('#' + hiddenViewTrimsCollectionId).css('display', 'none');
          }

          //set-up and display the overlay containing the trims block
          var $hiddenTrimDivOverlay = $('#overlay' + hiddenViewTrimsCollectionId);
          $hiddenTrimDivOverlay.popup({
            blur: false
          });
          $hiddenTrimDivOverlay.popup('show');

          //set-up the click event to let the user close the overlay
          var $overlayCloseBtn = $('.overlay-close');
          $overlayCloseBtn.on('click', function (e) {
            $('#' + hiddenViewTrimsCollectionId).css('display', 'none');
            $hiddenTrimDivOverlay.popup('hide');
          });
        }

      },
      setShowAllTrimsButtonClickHandler: function (showAllTrimsId) {
        $('#' + showAllTrimsId).on('click', function (e) {
          filtersModule.showAllTrims();
        });
      },
      setShowHideTrimButtonClickHandler: function (hiddenTrimButtonId) {
        $('#' + hiddenTrimButtonId).on('click', function (e) {
          jsVehicleAttributeNoCategoryResult.showHideTrimBlock(this.id);
        });
      },
      setFilterButtonClickHandler: function (filterIconId) {
        $('#' + filterIconId).on('click', function (e) {
          filtersModule.applyFilters(this.id);
        });
      },
    };

    $(function () {
      $.when(
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup'))
        .then(function () {
          filtersModule.init();
          jsVehicleAttributeNoCategoryResult.init();
        }));
    });

  });
})(window, jQuery)