/**
 *  This js module represents the mobile version of CarsForSale.SelectCategoryMakes http://www.car.com/cars-for-sale/
 */


'use strict';

(function (win, $, abt) {
  var page = function () {
    //#region - Module level vars, minification enhancements, and DOM caching
    var carsforsaleService = require('../../modules/abt-svc-c4s'),
      personaService = abt.PERSONA,
      criteria = carsforsaleService.criteria,
      ads = abt.ADS,
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
      makes = 'makes',
      allmakes = 'allmakes',
      carsForSaleMakes = '.carsForSaleMakes',
      btnChooseMakes = 'btnChooseMakes',
      btnShowResults = 'btnShowResults',
      selectMakeTitle = 'selectMakeTitle';

    //#endregion
    // var vehicles = [{ makeId: 1, make: 'Acura', complete: 0 }, { makeId: 11, make: 'Dodge', complete: 0 }, { makeId: 8, make: 'Chevrolet', complete: 0 }];

    var vehicles = [];
    var makeIds = [];

    var updateButtonState = function ($btn, select) {
      // if select state was not provided, treat it like a toggle.
      if (typeof select === undefined_str) {
        select = !$btn.hasClass(selected_class);
      }


      if ($btn.attr('id') == allmakes) {
        $(carsForSaleMakes).removeClass(selected_class);
        $(hash_char + allmakes).addClass(selected_class);

      } else {
        if (select) {
          $(hash_char + allmakes).removeClass(selected_class);

          $btn.addClass(selected_class);
        } else {
          $btn.removeClass(selected_class);
        }
      }
    };

    var addToPersonaSearch = function ($btn, select) {

      var filters = criteria.get().filters;

      var currentCategoryId = $btn.parent().data('categoryid');
      var currentItem = $btn.parent().data('value');

      var categories;
      var categoryMake = [];

      if (filters.categories.length == 0)
        categories = [];
      else
        categories = filters.categories.split('|');

      var categoryMakes;

      if (filters.category_makes.length == 0)
        categoryMakes = [];
      else
        categoryMakes = filters.category_makes.split('|');

      var categoryExists = false;

      if (typeof select === undefined_str) {
        select = !$btn.hasClass(selected_class);
      }

      if ($btn.attr('id') == allmakes) {
        $(carsForSaleMakes).removeClass(selected_class);
        $(hash_char + allmakes).addClass(selected_class);

        categories.push(currentCategoryId);

        // now, remove make_models that are using this makeid
        for (var i = 0; i < categoryMakes.length; i++) {
          categoryMake = categoryMakes[i].split('~');

          if (categoryMake[0] == currentCategoryId) {
            categoryMakes.splice(i, 1);
            i--;
          }
        }

        criteria.set({ filters: { categories: categories.join('|'), category_makes: categoryMakes.join('|') } });

      } else {

        if (select) {
          // add to Persona Search
          // check if it exists, if not, then add to makeIds

          $btn.addClass(selected_class);
          $(hash_char + allmakes).removeClass(selected_class);

          if ($.inArray(currentItem, categoryMakes) == -1)
            categoryMakes.push(currentItem);

          // now, remove category from categories
          $.each(categories, function (index, value) {
            if (currentCategoryId == value) {
              categories.splice(index, 1);
              return false;
            }
          });

          criteria.set({ filters: { categories: categories.join('|'), category_makes: categoryMakes.join('|') } });

        } else {
          // remove from Persona Search
          // check if it exists, if yes, remove from makes
          // if no makes from make are in makes, add make to makes

          $btn.removeClass(selected_class);

          $.each(categoryMakes, function (index, value) {
            if (currentItem == value) {
              categoryMakes.splice(index, 1);
              return false;
            }
          });

          criteria.set({ filters: { category_makes: categoryMakes.join('|') } });


          categoryExists = false;
          $.each(categoryMakes, function (index, value) {
            categoryMake = value.split('~');
            if (categoryMake[0] == currentCategoryId) {
              categoryExists = true;
              return false;
            }
          });


          // make doesn't exist in makemodels, so add make back to makes object
          if (!categoryExists && currentCategoryId != 0) {
            categories.push(currentCategoryId);
            criteria.set({ filters: { categories: categories.join('|') } });
          }

        }

      }
    };

    var checkMakesLeft = function () {
      var makesFound = false;

      $.each(vehicles, function (index, value) {
        if (value.complete == 0) {
          makesFound = true;
        };
      });

      if (makesFound == false) {
        $(hash_char + btnChooseMakes).hide();
        $(hash_char + btnShowResults).show();
      }
    };

    var getMakes = function (categoryId) {
      // call the API to get models

      $(hash_char + makes + js_suffix).html('');
      $(hash_char + makes + js_suffix).append('<li><div data-id="all" data-value="0" data-categoryid="' + categoryId + '"><button type="button" id="allmakes" class="btn-mm carsForSaleMakes selected">All Makes</button></div></li>');

      $.when(carsforsaleService.getCategoryByCategoryId(categoryId))
        .then(function (data) {

          var categoryName = data.category;
          $(hash_char + selectMakeTitle).html('Choose ' + categoryName + ' Brands');

        });

      $.when(carsforsaleService.getMakesByCategoryId(categoryId))
        .then(function (data) {

          $.each(data.makes, function (index, value) {
            $(hash_char + makes + js_suffix).append('<li><div data-id="' + index + '" data-value="' + categoryId + '~' + value.matchVal + '" data-categoryid="' + categoryId + '"><button type="button" id="' + categoryId + "_" + value.matchVal + '_st" class="btn-mm carsForSaleMakes">' + value.descr + '</button></div></li>');
            index++;
          });

        });

      $.each(vehicles, function (index, value) {
        if (value.categoryId == categoryId) {
          value.complete = 1;
          return false;
        };
      });

      checkMakesLeft();

    }

    var getNextCategory = function () {
      var makesFound = false;

      $.each(vehicles, function (index, value) {
        if (value.complete == 0) {
          makesFound = true;
          getMakes(value.categoryId);
          return false;
        };
      });

      if (makesFound == false) {
        // window.location = '/cars-for-sale/results/';
      }

    };

    var addCategoriesFromCategoryMakes = function () {

      var filters = criteria.get().filters;
      var categories;
      var categoryMakes;

      if (filters.categories.length == 0)
        categories = [];
      else {
        categories = filters.categories.split('|');
      }

      if (filters.category_makes.length == 0)
        categoryMakes = [];
      else {
        categoryMakes = filters.category_makes.split('|');
      }

      var categoryMake = [];
      var categoryIdList = [];

      $.each(categoryMakes, function (index, value) {
        categoryMake = value.split('~');
        if ($.inArray(categoryMake[0], categoryIdList) == -1 && categoryMake[0] != '')
          categoryIdList.push(categoryMake[0]);
      });

      $.each(categoryIdList, function (index, value) {
        if ($.inArray(value, categories) == -1 && value != '') {
          categories.push(value);
        }
      });

      criteria.set({ filters: { category_makes: '', categories: categories.join('|') } });
    };

    var loadCategories = function (categoryIdArray) {

      var dfd = $.Deferred();

      $.each(categoryIdArray, function (index, value) {

        $.when(carsforsaleService.getCategoryByCategoryId(value))
          .then(function (data) {

            var categoryName = data.category;

            vehicles.push({
              category: categoryName, categoryId: value, complete: 0
            });

            if (vehicles.length == categoryIdArray.length)
              dfd.resolve();
          });
      });

      return dfd.promise();
    };

    return {
      init: function () {

        var $makes = $(hash_char + makes + js_suffix);



        $.when(carsforsaleService.isReady(), personaService.isReady())
          .then(function () {

            addCategoriesFromCategoryMakes();

            var categoryIds;

            if (criteria.get().filters.categories.length == 0)
              categoryIds = [];
            else
              categoryIds = criteria.get().filters.categories.split('|');

            if (criteria.get().filters.category_makes.length == 0 && criteria.get().filters.categories.length == 0) {
              window.location = '/cars-for-sale/';
            } else {

              $.when(loadCategories(categoryIds))
                .then(function () {
                  $(hash_char + 'selectedCategories').html('You\'ve selected ' + vehicles.map(function (elem) { return elem.category; }).join(", "));

                  getNextCategory();
                });

            }

            $makes
              .on(click_event, carsForSaleMakes, function () {
                addToPersonaSearch($(this));
              });

            $(hash_char + btnChooseMakes).on(click_event, function () {
              getNextCategory();

            });

            $(hash_char + btnShowResults).on(click_event, function () {
              window.location = '/cars-for-sale/results/';
            });


          })
      }
    }
  }()

  $(function () {
    page.init()
  })
})(window, jQuery, ABT)