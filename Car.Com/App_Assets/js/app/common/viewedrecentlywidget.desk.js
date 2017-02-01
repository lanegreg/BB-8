/**
 *  This js module represents the desktop version of {controller}.{action} http://www.autobytel.com/{path}
 */


'use strict';

!function (win, $, abt, undefined) {
  //#region - Module level vars, minification enhancements, and DOM caching
  var utils = abt.UTILS
    , hash_symbol = '#'
    , tmpls = require('./viewedrecentlywidget_tmpls/desk/tmpls.js')
    , personaService = abt.PERSONA
    , show = 'show'
    , console = win.console
    , hasConsoleLogSupport = console && console.log

  //#endregion

  var viewedRecentlyWidget = function (widgetPrefix) {
    //#region - Module level vars, minification enhancements, and DOM caching

    var $widgetContainer

    //#endregion


    //#region - Private funcs

    var wireupViewedRecentlyButtons = function () {
      $('#viewedrecently_wdg_compareall_btn_js').on('click', function (e) {
        e.preventDefault();
        var pData = personaService.get(),
            currentViewedRecentlyList = pData.viewedRecentlyList;

        personaService.set({ compareList: currentViewedRecentlyList });
        window.location = '/tools/car-comparison/results/';
      });

      $('#viewedrecently_wdg_showall_btn_js').on('click', function (e) {
        e.preventDefault();
        $('#viewedrecently_wdg_loadhidden_btn_js').show();
        $('#viewedrecently_wdg_showall_btn_js').hide();
      });

    }

    var showViewedRecently = function (viewedRecentlyList) {
      var url = '/api/research/viewedrecently/trimidlist/' + viewedRecentlyList,
          vRCarArr = '';
      
      $.get(url).done(function (resp) {
        if (resp.data) {
          vRCarArr = resp.data.reverse();
          if (vRCarArr.length > 1) {
            $widgetContainer
              .empty()
              .prepend(tmpls.viewed_recently({ carArr: vRCarArr }));

            wireupViewedRecentlyButtons();

            $widgetContainer[show]();
          }
        }

        return true;
      }); //end done

    }
    
    var getViewedRecentlyList = function (currentTrim) {
      var pData = personaService.get(),
          currentViewedRecentlyList = pData.viewedRecentlyList,
          currentViewedRecentlyArr = [];
      
      if (currentViewedRecentlyList && currentViewedRecentlyList.length > 0) {
        currentViewedRecentlyArr = currentViewedRecentlyList.split(',');
      }

      //var delItemIndx = currentViewedRecentlyArr.indexOf(currentTrim.toString());
      //if (delItemIndx >= 0) {
      //  currentViewedRecentlyArr.splice(delItemIndx, 1);
      //  currentViewedRecentlyList = currentViewedRecentlyArr.join();
      //}

      return (currentViewedRecentlyList);
    }
    
    //#endregion


    return {

      create: function () {

        abt.WDG.set({
          viewedrecently: function () {
            return {
              init: function (config) {
                var currentTrim = '',
                    viewedRecentlyList = '';

                $.when(personaService.isReady())
                  .then(function() {
                    
                    var wrapper = config.wrapper

                    if (!wrapper) { // init by data-dash method
                      $widgetContainer = $('[data-widget^=viewed-recently]')
                    }
                    else if (wrapper.jquery) { // init by jquery object method
                      $widgetContainer = wrapper
                    }
                    else if (typeof wrapper === 'string') { // init by element id method
                      $widgetContainer = $(hash_symbol + wrapper)
                    }

                    // Make sure we have been properly initialized.
                    if (!$widgetContainer[0]) {
                      if (hasConsoleLogSupport) {
                        console.log(utils.WDG_ERR_MSG)
                      }
                      return
                    }

                    if (config.currentTrim)
                      currentTrim = config.currentTrim;

                    viewedRecentlyList = getViewedRecentlyList(currentTrim);

                    if (viewedRecentlyList.length)
                      showViewedRecently(viewedRecentlyList);

                  })
              },

              render: function(config) {
                // TODO: This is stubbed here so we can implement it later.
                // This method will allow us to dynamically change the context, and re-render different content.
              }
            }
          }()
        })
      }
    }
  }('viewedrecently_wdg_')

  viewedRecentlyWidget.create()

}(window, jQuery, ABT, []._)

