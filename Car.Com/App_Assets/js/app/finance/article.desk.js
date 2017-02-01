/**
 *	articlegallery.desk.js
 *	Autobytel (c)2014-2015
 *	@author: Marvin Fetalino
 */


"use strict";

(function (win, $) {
  var abt = ABT,
    env = abt.ENV,
    libsPath = env.libsPath,
    ads = abt.ADS,
    trk = abt.TRK,
    getJsFileNameByAlias = env.getJsFileNameByAlias;
        

    var articleGallery = function () {
      var $mainWrapper = $('#main-wrapper'),
        $pageText = $('.pagetext'),
        $media = $('.media'),
        $prevId = $('#prev'),
        $nextId = $('#next'),
        $prev = $('.prev'),
        $next = $('.next'),
        currentpage_str = 'currentpage',
        numpages_str = 'numpages',
        gotopage_str = 'gotopage',
        $endcapNav = $('#js_endcapNav'),
        $nextArticle = $('#js_nextArticle'),
        $viewMoreArticles = $('#js_viewMoreArticles'),
        $content = $('#content'),
        numRecords = 5; // number of related articles to retrieve

      var reloadAds = function (page) {
          ads.pageCtx.ads = abt.pageJson.articleAds[page]
          ads.reload()
        }

        var trackPageview = function (page) {
          var omni = trk.omni,
            ga = trk.ga,
            comscore = trk.comscore,
            pageview_str = 'pageview';

          trk.pageCtx.make = abt.pageJson.articleVehicles[page].Make;
          trk.pageCtx.superModel = abt.pageJson.articleVehicles[page].SuperModel;
          trk.pageCtx.model = abt.pageJson.articleVehicles[page].Model;
          trk.pageCtx.year = abt.pageJson.articleVehicles[page].Year;

          omni.track(pageview_str);
          ga.track(pageview_str);
          comscore.track(pageview_str);
        }

      return {
        init: function() {
          var numPages = $mainWrapper.data(numpages_str);

          if (numPages > 1) {
            $next.show();
          }

          articleGallery.loadUnslider();


          
          $viewMoreArticles.on('click', function (e) {
            e.preventDefault();

            var contentId = $content.data('id'),

              start = $content.data('relatedarticlestart') + numRecords;

            var url = '/api/article/relatedarticles/' + contentId + '?start=' + start + '&numRecords=' + numRecords;

            ads.reload();

            $.get(url).done(function (resp) {
              if (resp.data.relatedArticles.TotalRecords == numRecords) {
                for (var relatedArticleIndex = 0; relatedArticleIndex <= numRecords; relatedArticleIndex++) {
                  if (resp.data.relatedArticles.Articles[relatedArticleIndex] != null) {
                    $('#relatedArticle-' + relatedArticleIndex).find('img').attr('src', resp.data.relatedArticles.Articles[relatedArticleIndex].MainArticleImage);
                    $('#relatedArticle-' + relatedArticleIndex).find('h2').text(resp.data.relatedArticles.Articles[relatedArticleIndex].Title);
                    $('#relatedArticle-' + relatedArticleIndex).find('a').attr('href', resp.data.relatedArticles.Articles[relatedArticleIndex].Url);
                  }
                }
              } else {
                start = 1;
              }
              $content.data('relatedarticlestart', start);
            })
          }); 

          

        },

        changePage: function() {
          var currentPage = $mainWrapper.data(currentpage_str)
            , numPages = $mainWrapper.data(numpages_str)
            , prevPage, nextPage
        
        $pageText.hide();
        $('#page-' + currentPage).show();

        if (currentPage <= 1) {
          $prev.hide();
          }
          else {
            prevPage = $mainWrapper.data(currentpage_str) - 1;
            $prevId.data(gotopage_str, prevPage);
          $prev.show();
        }
       
        if ((numPages + 1) <= currentPage) {
          $next.hide();
          $endcapNav.show();
          $nextArticle.show();

          var $this = $('#js_fill_height');
          var $currentHeight = $this.prev('li').find('.img-container').outerHeight() + 'px';
          $this.css({ "height": $currentHeight });
        }
        else {
          nextPage = $mainWrapper.data(currentpage_str) + 1;
          $nextId.data(gotopage_str, nextPage);
          $endcapNav.hide();
          $next.show();
          $nextArticle.hide();
        }

        reloadAds(currentPage - 1);
        trackPageview(currentPage - 1);
      },

        loadUnslider: function() {

        var currentPage;
        var numPages;
      var unslider = $('.banner').unslider({
            speed: 500, //  The speed to animate each slide (in milliseconds)
            delay: false, //  The delay between slide animations (in milliseconds)
            complete: function() { articleGallery.changePage(); }, //  A function that gets called after every slide animation
            keys: false, //  Enable keyboard (left, right) arrow shortcuts
            dots: false, //  Display dot navigation
            fluid: true, //  Support responsive design. May break non-responsive designs
            scountplusone: true //  Display 1 of X+1 for Slide Count - needed because endcap adds one page to the end.
      });
          
      $prev.on("click", function() {
        $media.show();
        unslider.data('unslider').move($prevId.data('gotopage') - 1);
        $mainWrapper.data('currentpage', $prevId.data('gotopage'));
        $("html, body").animate({ scrollTop: 0 }, "slow");
      });

      $next.on("click", function () {

        if (($mainWrapper.data('currentpage') + 1) <= $mainWrapper.data('numpages')) {
          unslider.data('unslider').move($('#next').data('gotopage') - 1);
          $mainWrapper.data('currentpage', $nextId.data('gotopage'));
            }
            else {
          $mainWrapper.data('currentpage', $nextId.data('gotopage'));
          $media.css('height', 0).css('width', 0).hide();
              articleGallery.changePage();
        }
        $("html, body").animate({ scrollTop: 0 }, "slow");
      });
         
        var slideWrap = $('#articleSlider'),
          slides = slideWrap.find('.js_img_slide');

      document.documentElement.className = 'js';

      slides
      .on('movestart', function(e) {
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
      .on('move', function(e) {
        // Move slides with the finger
        if (e.distX < 0) {
          unslider.data('unslider').move($nextId.data('gotopage') - 1);
          $('#main-wrapper').data('currentpage', $nextId.data('gotopage'));
        }
        if (e.distX > 0) {
          unslider.data('unslider').move($prevId.data('gotopage') - 1);
          $('#main-wrapper').data('currentpage', $prevId.data('gotopage'));
        }
      })
      .on('moveend', function(e) {
        slideWrap.removeClass('notransition');
        $("html, body").animate({ scrollTop: 0 }, "slow");
      });


      }
      }
    }()


  $(function() {

      $.when(
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('picfill')),
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('slider')),
          $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('evmove'))

        /// TODO: Need to add function to back fill all images for slider after page init load. Will init page with
        )
      .then(function () {
        articleGallery.init();
      })


  })



})(window, jQuery)