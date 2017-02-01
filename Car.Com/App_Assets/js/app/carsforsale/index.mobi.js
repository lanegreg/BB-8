/**
 *  This js module represents the mobile version of CarsForSale.Index http://www.car.com/cars-for-sale/
 */


'use strict';

(function (win, $, abt) {

  var page = function () {
    //#region - Module level vars, minification enhancements, and DOM caching
    var
      carsforsaleService = require('../../modules/abt-svc-c4s'),
      personaService = abt.PERSONA,
      $body = $('body'),
      tmpls = require('./index_tmpls/desk/tmpls.js'),
      clearSelectionsOverlay = 'clear_selections_overlay_js',
      criteria = carsforsaleService.criteria,
      hash_char = '#',
      click_event = 'click',
      anchor_elem = 'a',
      li_elem = 'li',
      id_str = 'id',
      js_suffix = '_js',
      show_str = 'show',
      empty_str = '',
      selected_str = 'selected',
      selected_class = selected_str,
      undefined_str = 'undefined',
      allmakes = 'allmakes',
      carsForSaleMake = '.carsForSaleMake',
      carsForSaleCategory = '.carsForSaleCategory',
      btnChooseModels = 'btnChooseModels',
      btnChooseMakes = 'btnChooseMakes',
      btnShowAllMakes = 'btnShowAllMakes',
      btnShowAllCategories = 'btnShowAllCategories',
      clearSelectionsContainer = 'clear-selections-text',
      overlayClose = 'overlay-close',
      closeClearSelectionsContainer = 'close-clear-selections-container',
      continueClearSelectionsContainer = 'continue-clear-selections-container';

    //#endregion

    var updateButtonStateMakes = function ($btn, select) {

      // if select state was not provided, treat it like a toggle.
      if (typeof select === undefined_str) {
        select = !$btn.hasClass(selected_class);
      }

      if (select) {

        if ($(carsForSaleCategory).parent().parent('.' + selected_class).length >= 1) {
          $(hash_char + clearSelectionsContainer).html('Continue with Make choice and Clear Category Selections?');
          $(hash_char + continueClearSelectionsContainer).on('click', function () {
            criteria.set({ filters: { categories: '' } }, true);

            // undo selected categories
            $(carsForSaleCategory).parent().parent('.' + selected_class).removeClass(selected_class);
            $(hash_char + btnChooseMakes).hide();
            $(hash_char + btnShowAllCategories).show();

            $(hash_char + clearSelectionsOverlay).popup('hide');

            // finish selecting make
            $(hash_char + btnShowAllMakes).hide();
            $(hash_char + btnChooseModels).show();

            $btn.addClass(selected_class);

          });

          $(hash_char + clearSelectionsOverlay).popup('show');
        } else {
          $(hash_char + btnShowAllMakes).hide();
          $(hash_char + btnChooseModels).show();

          $btn.addClass(selected_class);
        }


      } else {
        $btn.removeClass(selected_class);

        if ($(carsForSaleMake + '.' + selected_class).length == 0) {
          $(hash_char + btnChooseModels).hide();
          $(hash_char + btnShowAllMakes).show();
        }
      }
    };

    var updateButtonStateCategories = function ($btn, select) {

      // if select state was not provided, treat it like a toggle.
      if (typeof select === undefined_str) {
        select = !$btn.hasClass(selected_class);
      }

      if (select) {

        if ($('.carsForSaleMake.selected').length >= 1) {

          $(hash_char + clearSelectionsContainer).html('Continue with Category choice and Clear Make Selections?');
          $(hash_char + continueClearSelectionsContainer).on('click', function () {
            criteria.set({ filters: { makes: '', make_models: '' } }, true);

            // undo selected makes
            $('.carsForSaleMake.selected').removeClass(selected_class);
            $(hash_char + btnChooseModels).hide();
            $(hash_char + btnShowAllMakes).show();

            $(hash_char + clearSelectionsOverlay).popup('hide');

            // finish selecting category
            $(hash_char + btnShowAllCategories).hide();
            $(hash_char + btnChooseMakes).show();

            $btn.addClass(selected_class);
          });

          $(hash_char + clearSelectionsOverlay).popup('show');
        } else {
          $(hash_char + btnShowAllCategories).hide();
          $(hash_char + btnChooseMakes).show();

          $btn.addClass(selected_class);
        }


      } else {
        $btn.removeClass(selected_class);

        if ($(carsForSaleCategory).parent().parent('.' + selected_class).length == 0) {
          $(hash_char + btnChooseMakes).hide();
          $(hash_char + btnShowAllCategories).show();
        }
      }
    };

    return {
      init: function () {
        var $categories = $(hash_char + 'categories' + js_suffix),
          $makes = $(hash_char + 'makes' + js_suffix),
          $showMoreMakesBtn = $(hash_char + show_str + '_more_makes_btn' + js_suffix),
          $showMore = $showMoreMakesBtn.find(hash_char + show_str + '_more' + js_suffix),
          $showLess = $showMoreMakesBtn.find(hash_char + show_str + '_less' + js_suffix);

        $body.append(tmpls.clear_selections_overlay());
        $('.' + overlayClose).on('click', function (e) {
          e.preventDefault();
          $(hash_char + clearSelectionsOverlay).popup('hide');
        });

        $(hash_char + closeClearSelectionsContainer).on('click', function (e) {
          e.preventDefault();
          $(hash_char + clearSelectionsOverlay).popup('hide');
        });


        $.when(carsforsaleService.isReady(), personaService.isReady())
          .then(function () {

            criteria.set({ filters: { makes: '', make_models: '', categories: '', category_makes: '' } }, true);


            $categories
            .on(click_event, carsForSaleCategory, function () {
              updateButtonStateCategories($(this).parent().parent());
            });

            $makes
            .on(click_event, carsForSaleMake, function () {
              updateButtonStateMakes($(this));
            });

            $(hash_char + btnShowAllMakes).on(click_event, function () {
              criteria.set({ filters: { makes: '', make_models: '', categories: '' } }, true);



              window.location = '/cars-for-sale/results/';

            });

            $(hash_char + btnShowAllCategories).on(click_event, function (e) {
              criteria.set({ filters: { makes: '', make_models: '', categories: '' } }, true);

              window.location = '/cars-for-sale/results/';
            });

            $(hash_char + btnChooseModels).on(click_event, function () {
              var selectedItems = $('button.selected');
              var makeIdArray = [];
              var makeIdList;

              window.location = '/cars-for-sale/selectmodels/';

              //if ($(carsForSaleCategory).parent().parent('.' + selected_class).length >= 1) {
              //  $(hash_char + clearSelectionsContainer).html('Continue with Make choice and Clear Category Selections?');
              //  $(hash_char + continueClearSelectionsContainer).on('click', function() {
              //    criteria.set({ filters: { categories: '' } }, true);
              //    window.location = '/cars-for-sale/selectmodels/';
              //  });

              //  $(hash_char + clearSelectionsOverlay).popup('show');
              //} else {
              //  window.location = '/cars-for-sale/selectmodels/';
              //}


              $.each(selectedItems, function (index, value) {
                makeIdArray.push(value.parentElement.dataset.id);
              });

              makeIdList = makeIdArray.join('|');

              criteria.set({ filters: { makes: makeIdList, make_models: '', categories: '' } }, true);

            });

            $(hash_char + btnChooseMakes).on(click_event, function () {
              var categoryArray = [];
              var categoryList;

              window.location = '/cars-for-sale/selectcategorymakes/';

              //if ($('.carsForSaleMake.selected').length >= 1) {

              //  $(hash_char + clearSelectionsContainer).html('Continue with Category choice and Clear Make Selections?');
              //  $(hash_char + continueClearSelectionsContainer).on('click', function() {
              //    criteria.set({ filters: { makes: '', make_models: '' } }, true);
              //    window.location = '/cars-for-sale/selectcategorymakes/';
              //  });

              //  $(hash_char + clearSelectionsOverlay).popup('show');
              //} else {
              //    window.location = '/cars-for-sale/selectcategorymakes/';
              //}

              $.each($('.carsForSaleCategory').parent().parent('.selected'), function () {

                categoryArray.push($(this).find('div').data('id'));
              });

              categoryList = categoryArray.join('|');

              criteria.set({ filters: { makes: '', make_models: '', categories: categoryList } }, true);

            });

            // Show... more or less :)
            var showMoreHandler = function () {
              $makes.find(li_elem).show()
              $showMore.hide()
              $showLess.show()
              $showMoreMakesBtn.one(click_event, showLessHandler)
            }

            var showLessHandler = function () {
              $makes.find(li_elem + '[data-hideable=true]').hide()
              $showLess.hide()
              $showMore.show()
              $showMoreMakesBtn.one(click_event, showMoreHandler)
            }

            $showMoreMakesBtn.one(click_event, showMoreHandler)
          })
      }
    }
  }()

  $(function () {
    page.init();

  })
})(window, jQuery, ABT)