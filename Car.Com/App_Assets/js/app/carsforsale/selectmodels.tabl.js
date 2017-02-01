/**
 *  This js module represents the tablet version of CarsForSale.SelectModels http://www.car.com/cars-for-sale/
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
      models = 'models',
      allmodels = 'allmodels',
      carsForSaleModels = '.carsForSaleModels',
      btnChooseModels = 'btnChooseModels',
      btnShowResults = 'btnShowResults',
      selectModelTitle = 'selectModelTitle';

    //#endregion
    // var vehicles = [{ makeId: 1, make: 'Acura', complete: 0 }, { makeId: 11, make: 'Dodge', complete: 0 }, { makeId: 8, make: 'Chevrolet', complete: 0 }];

    var vehicles = [];
    var makeIds;

    var addToPersonaSearch = function ($btn, select) {

      var filters = criteria.get().filters;

      var currentMakeId = $btn.parent().data('makeid');
      var currentItem = $btn.parent().data('value');

      var makes;
      var makeModel = [];

      if (filters.makes.length == 0)
        makes = [];
      else
        makes = filters.makes.split('|');

      var makeModels;

      if (filters.make_models.length == 0)
        makeModels = [];
      else
        makeModels = filters.make_models.split('|');

      var makeExists = false;

      if (typeof select === undefined_str) {
        select = !$btn.hasClass(selected_class);
      }

      // if they clicked 'all models'
      if ($btn.attr('id') == allmodels) {
        $(carsForSaleModels).removeClass(selected_class);
        $(hash_char + allmodels).addClass(selected_class);

        makes.push(currentMakeId);

        // now, remove make_models that are using this makeid
        for (var i = 0; i < makeModels.length; i++) {
          makeModel = makeModels[i].split('~');

          if (makeModel[0] == currentMakeId) {
            makeModels.splice(i, 1);
            i--;
          }
        }

        criteria.set({ filters: { makes: makes.join('|'), make_models: makeModels.join('|') } });

      } else {

        if (select) {
          // add to Persona Search
          // check if it exists, if not, then add to make_models

          $btn.addClass(selected_class);
          $(hash_char + allmodels).removeClass(selected_class);

          if ($.inArray(currentItem, makeModels) == -1)
            makeModels.push(currentItem);

          // now, remove make from makes
          $.each(makes, function (index, value) {
            if (currentMakeId == value) {
              makes.splice(index, 1);
              return false;
            }
          });

          criteria.set({ filters: { makes: makes.join('|'), make_models: makeModels.join('|') } });

        } else {
          // remove from Persona Search
          // check if it exists, if yes, remove from make_models
          // if no more models from make are in make_models, add make to makes

          $btn.removeClass(selected_class);

          $.each(makeModels, function (index, value) {
            if (currentItem == value) {
              makeModels.splice(index, 1);
              return false;
            }
          });

          criteria.set({ filters: { make_models: makeModels.join('|') } });

          // add makeid back to make if there are no more models with this makeid

          makeExists = false;
          $.each(makeModels, function (index, value) {
            makeModel = value.split('~');
            if (makeModel[0] == currentMakeId) {
              makeExists = true;
              return false;
            }
          });

          // make doesn't exist in makemodels, so add make back to makes object
          if (!makeExists && currentMakeId != 0) {
            makes.push(currentMakeId);
            criteria.set({ filters: { makes: makes.join('|') } });
          }

        }
      }
    };

    var checkModelsLeft = function () {
      var modelsFound = false;

      $.each(vehicles, function (index, value) {
        if (value.complete == 0) {
          modelsFound = true;
        };
      });

      if (modelsFound == false) {
        $(hash_char + btnChooseModels).hide();
        $(hash_char + btnShowResults).show();
      }
    };

    var getModels = function (makeId) {
      // call the API to get models

      $(hash_char + models + js_suffix).html('');
      $(hash_char + models + js_suffix).append('<li><div data-id="all" data-value="0" data-makeid="' + makeId + '"><button type="button" id="allmodels" class="btn-mm carsForSaleModels selected">All Models</button></div></li>');

      $.when(carsforsaleService.getMakeByMakeId(makeId))
        .then(function (data) {

          var makeName = data.make;
          $(hash_char + selectModelTitle).html('Choose ' + makeName + ' Models');

        });

      $.when(carsforsaleService.getModelsByMakeId(makeId))
        .then(function (data) {

          $.each(data.models, function(index, value) {
            $(hash_char + models + js_suffix).append('<li><div data-id="' + index + '" data-value="' + makeId + '~' + value.matchVal + '" data-makeid="' + makeId + '"><button type="button" id="' + value.descr + '_st" class="btn-mm carsForSaleModels">' + value.descr + '</button></div></li>');
            index++;
          });

        });

      $.each(vehicles, function (index, value) {
        if (value.makeId == makeId) {
          value.complete = 1;
          return false;
        };
      });

      checkModelsLeft();

    }

    var getNextModel = function () {
      var modelsFound = false;

      $.each(vehicles, function (index, value) {
        if (value.complete == 0) {
          modelsFound = true;
          getModels(value.makeId, value.make);
          return false;
        };
      });

      if (modelsFound == false) {
        window.location = '/cars-for-sale/results/';
      }

    };

    var addMakesFromMakeModels = function () {

      var filters = criteria.get().filters;
      var makes;
      var makeModels;

      if (filters.makes.length == 0)
        makes = [];
      else {
        makes = filters.makes.split('|');
      }

      if (filters.make_models.length == 0)
        makeModels = [];
      else {
        makeModels = filters.make_models.split('|');
      }

      var makeModel = [];
      var makeIdList = [];

      $.each(makeModels, function (index, value) {
        makeModel = value.split('~');
        if ($.inArray(makeModel[0], makeIdList) == -1 && makeModel[0] != '')
          makeIdList.push(makeModel[0]);
      });

      $.each(makeIdList, function (index, value) {
        if ($.inArray(value, makes) == -1 && value != '') {
          makes.push(value);
        }
      });

      criteria.set({ filters: { makes: makes.join('|'), make_models: '' } });
    };

    var loadMakes = function (makeIdArray) {

      var dfd = $.Deferred();

      $.each(makeIdArray, function (index, value) {

        $.when(carsforsaleService.getMakeByMakeId(value))
          .then(function (data) {

            var makeName = data.make;

            vehicles.push({
              make: makeName, makeId: value, complete: 0
            });

            if (vehicles.length == makeIdArray.length)
              dfd.resolve();

          });

      });

      return dfd.promise();
    };

    return {
      init: function () {

        var $models = $(hash_char + models + js_suffix);

        $.when(carsforsaleService.isReady(), personaService.isReady())
          .done(function () {

            addMakesFromMakeModels();

            if (criteria.get().filters.makes.length == 0)
              makeIds = [];
            else
              makeIds = criteria.get().filters.makes.split('|');

            if (criteria.get().filters.makes.length == 0 && criteria.get().filters.make_models.length == 0) {
              window.location = '/cars-for-sale/';
            } else {

              $.when(loadMakes(makeIds))
                .then(function () {
                  $(hash_char + 'selectedMakes').html('You\'ve selected ' + vehicles.map(function (elem) { return elem.make; }).join(", "));

                  getNextModel();
                });


            }

            $models
              .on(click_event, carsForSaleModels, function () {
                addToPersonaSearch($(this));
              });

            $(hash_char + btnChooseModels).on(click_event, function () {
              getNextModel();

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