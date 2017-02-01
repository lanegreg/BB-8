/**
 *  This js module represents the desktop version of Research.trimpicnvids http://www.autobytel.com/{path}
 */


"use strict";

(function (win, $) {
  $(function () {
    var abt = ABT
      , env = abt.ENV
      , trk = abt.TRK
      , car = abt.pageJson.vehicleinfo
      , libsPath = abt.ENV.libsPath
      , getJsFileNameByAlias = env.getJsFileNameByAlias
      , getScriptUriByName = env.getScriptUriByName;

    var jsResearchTrimPicsnVids = {

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

      reloadAdsandTrack: function () {
        abt.ADS.reload()

        var omni = trk.omni
          , ga = trk.ga
          , comscore = trk.comscore
          , pageview_str = 'pageview'

        omni.track(pageview_str)
        ga.track(pageview_str)
        comscore.track(pageview_str)
      },

      loadUnslider: function () {
        var unslider = $('.banner').unslider({
          speed: 500,                                                               //  The speed to animate each slide (in milliseconds)
          delay: 300000,                                                            //  The delay between slide animations (in milliseconds)
          complete: function () { jsResearchTrimPicsnVids.reloadAdsandTrack(); },   //  A function that gets called after every slide animation
          keys: true,                                                               //  Enable keyboard (left, right) arrow shortcuts
          dots: false,                                                               //  Display dot navigation
          fluid: true,                                                              //  Support responsive design. May break non-responsive designs
          scount: true                                                              //  Display 1 of X for Slide Count
        });

        $('#unsliderPrev').on("click", function () {
          unslider.data('unslider').prev();
        });
        $('#unsliderNext').on("click", function () {
          unslider.data('unslider').next();
        });

        var slideWrap = $('#picsnVidsSlider'),
            slides = slideWrap.find('.js_img_slide');

        document.documentElement.className = 'js';

        slides
        .on('movestart', function (e) {
          // If the movestart heads off in a upwards or downwards
          // direction, prevent it so that the browser scrolls normally.
          if ((e.distX > e.distY && e.distX < -e.distY) ||
					    (e.distX < e.distY && e.distX > -e.distY)) {
            e.preventDefault();
            return;
          }
          // To allow the slide to keep step with the finger,
          // temporarily disable transitions.
          slideWrap.addClass('notransition');
        })
        .on('move', function (e) {
          // Move slides with the finger
          if (e.distX < 0) {
            unslider.data('unslider').next();
          }
          if (e.distX > 0) {
            unslider.data('unslider').prev();
          }
        })
        .on('moveend', function (e) {
          slideWrap.removeClass('notransition');
        });


      }

    }

    $(function () {
      $.when(
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('picfill')),
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('slider')),
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('evmove'))
          // Need to add function to back fill all images for slider after page init load. Will init page with
          // first image and all other div's empty
          // jsResearchTrimPicsnVids.getUnloadedImage()
        )
        .then(function () {
          jsResearchTrimPicsnVids.loadUnslider();
          jsResearchTrimPicsnVids.initCarsForSaleWidget();
          jsResearchTrimPicsnVids.initViewedRecentlyWidget();
        });
    });

  });
})(window, jQuery)