
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

    var jsYear = {

      init: function () {

        var hiddenTrimDivs = $("[id^='js-disablesupertrim']");
        for (var j = 0; j < hiddenTrimDivs.length; j++) {
          var hiddenTrimDivId = hiddenTrimDivs[j].id;
          jsYear.setShowHideTrimButtonClickHandler(hiddenTrimDivId);
        }

        var filterIcons = $("[id^='js-filter-icon']");
        for (j = 0; j < filterIcons.length; j++) {
          var filterIconId = filterIcons[j].id;
          jsYear.setFilterButtonClickHandler(filterIconId);
        }

        var showAllTrimsIds = $("[id^='js-show-all-trims']");
        for (j = 0; j < showAllTrimsIds.length; j++) {
          var showAllTrimsId = showAllTrimsIds[j].id;
          jsYear.setShowAllTrimsButtonClickHandler(showAllTrimsId);
        }

        jsYear.setYearSelectorClickHandler();

        var makename = ABT.ADS.pageCtx.makeseoname;
        var supermodelname = ABT.ADS.pageCtx.supermodelseoname;
        var year = ABT.ADS.pageCtx.year;
        filtersModule.setSuperModelFilterGroupsByYear(makename, supermodelname, year);
        filtersModule.setSuperModelTrimFilterAttributesByYear(makename, supermodelname, year);

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
          jsYear.showHideTrimBlock(this.id);
        });
      },
      setFilterButtonClickHandler: function (filterIconId) {
        $('#' + filterIconId).on('click', function (e) {
          filtersModule.applyFilters(this.id);
        });
      },
      setYearSelectorClickHandler: function () {
        $('#js-yearsupermodel-year-select').on('change', function (e) {
          var makename = ABT.ADS.pageCtx.make;
          var supermodelname = ABT.ADS.pageCtx.supermodelseoname;
          var year = $('#js-yearsupermodel-year-select').val();
          var url = '/' + makename.toLowerCase() + '/' + supermodelname.toLowerCase() + '/' + year;
          location.assign(url);
        });
      }
    };

    $(function () {
      $.when(
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup'))
        .then(function () {
          filtersModule.init();
          jsYear.init();
        }));
    });

  });
})(window, jQuery)