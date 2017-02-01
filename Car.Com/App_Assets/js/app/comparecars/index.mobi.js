/**
 *  This js module represents the mobile version of comparecars.index http://www.autobytel.com/{path}
 */

//var tmpls = require('./{templates_path}/{device_group}/tmpls.js')

(function (win, $) {
  $(function () {
    var abt = ABT,
        env = abt.ENV,
        getJsFileNameByAlias = env.getJsFileNameByAlias,
        libsPath = env.libsPath,
        $body = $('body'),
        tmpls = require('./index_tmpls/desk/tmpls.js'),
        personaService = abt.PERSONA,
        currentCompareList = '',
        $addChangeCarOverlay = $('#add_change_car_overlay_js'),
        $compareCarModelSelect = '',
        $compareCarTrimSelect = '',
        $compareCarAddSelectedMake = '',
        $compareCarAddSelectedModel = '',
        $compareCarAddSelectedTrim = '',
        $overlayCloseBtn = '';

    var jsCompareCarIndex = {

      initSelectedAndDefaultBtns: function () {
        var pData = personaService.get();

        currentCompareList = pData.compareList;
        if (currentCompareList && currentCompareList.length > 0) {
          jsCompareCarIndex.showSelectedComparItems();
        } else {
          jsCompareCarIndex.showDefaultComparItems();
        }

        jsCompareCarIndex.wireupCompareButtons();

        $('#popCompareBtn1').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "33210,34261" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn2').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "34198,32437" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn3').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "33802,34217" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn4').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "34078,34034" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn5').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "33023,32549" });
          window.location = '/tools/car-comparison/results/';
        });

        $('#popCompareBtn6').on('click', function (e) {
          e.preventDefault();
          personaService.set({ compareList: "32397,32718" });
          window.location = '/tools/car-comparison/results/';
        });

        return true;
      },

      wireupCompareButtons: function () {
        $('.js_add_compare_car').on('click', function (e) {
          e.preventDefault();
          $addChangeCarOverlay.popup('show');
        });

        $('.js_delete_compare_car').on('click', function (e) {
          e.preventDefault();
          var deleteId = $(this).data('trimid').toString();
          //alert(deleteId);
          jsCompareCarIndex.deleteSelectedCompareCar(deleteId);
        });

        $('.js_compare_results').on('click', function (e) {
          e.preventDefault();
          window.location = '/tools/car-comparison/results/';
        });
      },

      showSelectedComparItems: function () {
        if (currentCompareList && currentCompareList.length > 0) {
          var url = '/api/compare/index/trimidlist/' + currentCompareList,
            $selectedCompareTable = $('#selectedCompareTable'),
            comparCarArr = '';

          $.get(url).done(function (resp) {
            if (resp.data) {
              comparCarArr = resp.data;
              $selectedCompareTable
                .empty()
                .prepend(tmpls.compare_data({ carArr: comparCarArr }));

              jsCompareCarIndex.wireupCompareButtons();

            }

            return true;
          }); //end done
        }
        //else { personaService.set({ compareList: "33111,32999,33508" }); }
      },

      showDefaultComparItems: function () {
        var $selectedCompareTable = $('#selectedCompareTable');

        $selectedCompareTable
          .empty()
          .prepend(tmpls.default_compare_items({}));

        jsCompareCarIndex.wireupCompareButtons();

        return true;
      },

      deleteSelectedCompareCar: function (deleteId) {
        var currentCompareListArr = [];

        if (deleteId > 0) {

          if (currentCompareList && currentCompareList.length > 0) {
            currentCompareListArr = currentCompareList.split(',');
          }

          var delItemIndx = currentCompareListArr.indexOf(deleteId);
          if (delItemIndx >= 0) {
            currentCompareListArr.splice(delItemIndx, 1);
            currentCompareList = currentCompareListArr.join();
            personaService.set({ compareList: currentCompareList });
            if (currentCompareList.length > 0)
              jsCompareCarIndex.showSelectedComparItems();
            else {
              jsCompareCarIndex.showDefaultComparItems();
            }
          }
        }

      },

      initAddChangeOverlay: function () {
        $.when(
          $.get('/api/compare/makes/').done(function (resp) {
            if (resp.data) {
              $body.append(tmpls.add_change_car_overlay({ makeArr: resp.data }));
            }
          })
        )
        .then(function () {
          $addChangeCarOverlay = $('#add_change_car_overlay_js'),
          $overlayCloseBtn = $('.overlay-close'),
          $overlayCloseBtn.on('click', function (e) {
            e.preventDefault();
            $addChangeCarOverlay.popup('hide');
          });
          $compareCarModelSelect = $('#compareCarModelSelect');
          $compareCarTrimSelect = $('#compareCarTrimSelect');

          $('#compareCarMakeSelect').on('change', function (e) {
            $compareCarAddSelectedMake = $('#compareCarMakeSelect :selected').val();
            if ($compareCarAddSelectedMake.length > 0) {
              jsCompareCarIndex.updateCompareModelDropdown();
            }
            else {
              $compareCarModelSelect.empty().prepend('<option value="">Select a Model</option>').prop('disabled', true);
              $compareCarTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $compareCarModelSelect.on('change', function (e) {
            $compareCarAddSelectedModel = $('#compareCarModelSelect :selected').val();
            if ($compareCarAddSelectedModel.length > 0) {
              jsCompareCarIndex.updateCompareTrimDropdown();
            }
            else {
              $compareCarTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          });

          $compareCarTrimSelect.on('change', function (e) {
            var currentCompareListArr = [];

            $compareCarAddSelectedTrim = $('#compareCarTrimSelect :selected').val();
            if ($compareCarAddSelectedTrim.length > 0) {

              if (currentCompareList && currentCompareList.length > 0) {
                currentCompareListArr = currentCompareList.split(',');
              }

              if (currentCompareListArr.indexOf($compareCarAddSelectedTrim) < 0) {
                currentCompareListArr.push($compareCarAddSelectedTrim);
                currentCompareList = currentCompareListArr.join();
                personaService.set({ compareList: currentCompareList });
                $addChangeCarOverlay.popup('hide');
                jsCompareCarIndex.updateViewedRecentlyList($compareCarAddSelectedTrim);
                jsCompareCarIndex.showSelectedComparItems();
              }
            }
          });

        }); // End of then function

        return true;

      },

      updateViewedRecentlyList: function (addTrimId) {
        var pData = personaService.get(),
            currentViewedRecentlyList = pData.viewedRecentlyList,
            currentViewedRecentlyArr = [],
            maxVrNum = 10;

        if (currentViewedRecentlyList && currentViewedRecentlyList.length > 0) {
          currentViewedRecentlyArr = currentViewedRecentlyList.split(',');
        }

        if (currentViewedRecentlyArr.indexOf(addTrimId) < 0) {
          currentViewedRecentlyArr.push(addTrimId);
          if (currentViewedRecentlyArr.length > maxVrNum) {
            currentViewedRecentlyArr.shift();
          }
        }

        currentViewedRecentlyList = currentViewedRecentlyArr.join();
        personaService.set({ viewedRecentlyList: currentViewedRecentlyList });
      },

      updateCompareModelDropdown: function () {
        var url = '/api/compare/make/' + $compareCarAddSelectedMake + '/super-models/';

        $.when(
          $.get(url).done(function (resp) {
            if (resp.data) {
              $compareCarModelSelect.empty().prepend(tmpls.model_select_options({ modelArr: resp.data })).prop('disabled', false);
              $compareCarTrimSelect.empty().prepend('<option value="">Select a Vehicle</option>').prop('disabled', true);
            }
          })
        )
        .then(function () {

        }); // End of then function

      },

      updateCompareTrimDropdown: function () {
        var url = '/api/compare/make/' + $compareCarAddSelectedMake + '/super-model/' + $compareCarAddSelectedModel + '/trims/';

        $.get(url).done(function (resp) {
          if (resp.data) {
            $compareCarTrimSelect.empty().prepend(tmpls.trim_select_options({ trimArr: resp.data })).prop('disabled', false);
          }
        });

      },

      init: function () {
        $.when(
          jsCompareCarIndex.initSelectedAndDefaultBtns())
        .then(function () {
          jsCompareCarIndex.initAddChangeOverlay();
        })
      } // End init function
    };

    $(function () {
      $.when(
        $.getCachedScript(libsPath + '/' + getJsFileNameByAlias('popup')),
        personaService.isReady())
      .then(function () {
        jsCompareCarIndex.init();
      })
    });

  });
})(window, jQuery)