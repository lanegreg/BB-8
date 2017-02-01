/**
 *  This js module represents the desktop version of carvalue.index http://www.autobytel.com/used-car-values/
 */


!function (win, $, abt) {
  var page = function() {
    var click_event = 'click',
      change_event = 'change',
      button_elem = 'button';


    var mapArrayToJson = function(arr) {
      var obj = {}
      arr.forEach(function(el) {
        obj[el.name] = el.value;
      });
      return obj;
    }

    var step1 = function() {
      var $form = $('#step_1 form'),
        name_prop = 'name',
        $evalType = $('#evalType'),
        $evalType2 = $('#evalType2'),
        $evalTypeDesc = $('.evalTypeDesc'),
        $evalTypeDesc2 = $('.evalTypeDesc2'),
        $year = $form.find('[' + name_prop + '=year]'),
        $make = $form.find('[' + name_prop + '=make]'),
        $modelTrim = $form.find('[' + name_prop + '=model_trim]'),
        $mileage = $('#mileage'),
        $zipCode = $('#zipCode'),
        $nextBtn = $('#btnDetails'),
        $submitBtn = $('#btnSubmit'),
        $newSearch = $('.newSearch'),
        $searchCarsBtn = $('#btnSearchCars'),
        $drives = $('#drives'),
        $transmissions = $('#transmissions'),
        $engines = $('#engines'),
        $vehicleoptions = $('#vehicleoptions'),
        $zipCodeVal = $('#zipCodeVal'),
        $mileageVal = $('#mileageVal'),
        $drivesVal = $('#drivesVal'),
        $transmissionsVal = $('#transmissionsVal'),
        $enginesVal = $('#enginesVal'),
        $vehicleoptionsVal = $('#vehicleoptionsVal'),
        $dollarSign = $('#dollarSign'),
        $vehicleValue = $('#vehicleValue'),
        $requiredTextSection1 = $('#requiredTextSection1'),
        $requiredTextSection2 = $('#requiredTextSection2'),
        $conditionExcellent = $('#conditionExcellent'),
        $conditionExcellent2 = $('#conditionExcellent2'),
        $conditionExcellentDesc = $('#conditionExcellentDesc'),
        $conditionExcellentDesc2 = $('#conditionExcellentDesc2'),
        $conditionGood = $('#conditionGood'),
        $conditionGood2 = $('#conditionGood2'),
        $conditionGoodDesc = $('#conditionGoodDesc'),
        $conditionGoodDesc2 = $('#conditionGoodDesc2'),
        $conditionFair = $('#conditionFair'),
        $conditionFair2 = $('#conditionFair2'),
        $conditionFairDesc = $('#conditionFairDesc'),
        $conditionFairDesc2 = $('#conditionFairDesc2'),
        $conditionPoor = $('#conditionPoor'),
        $conditionPoor2 = $('#conditionPoor2'),
        $conditionPoorDesc = $('#conditionPoorDesc'),
        $conditionPoorDesc2 = $('#conditionPoorDesc2'),
        $conditionDesc = $('.conditionDesc'),
        $section1 = $('#section1'),
        $section2 = $('#section2'),
        $section3 = $('#section3'),
        $section4 = $('#section4'),
        $sectionAboutKBB = $('#sectionAboutKBB'),
        $sectionKBBcopyright = $('#sectionKBBcopyright'),
        $tradeInDesc = $('#tradeInDesc'),
        $tradeInDesc2 = $('#tradeInDesc2'),
        $privatePartyDesc = $('#privatePartyDesc'),
        $privatePartyDesc2 = $('#privatePartyDesc2'),
        $suggestedRetailDesc = $('#suggestedRetailDesc'),
        $suggestedRetailDesc2 = $('#suggestedRetailDesc2'),
        select_elem = 'select',
        selected_attr = 'selected',
        option_elem = 'option';


      return {
        init: function() {
          $form.on(click_event, select_elem, function() {
            var $this = $(this),
              value = $this.val(),
              promptValue = $this.find('.prompt');

            if (value !== promptValue) {
              if (!value) {
                $this
                  .find(option_elem + ':' + selected_attr)
                  .prop(selected_attr, false)
                  .next(option_elem)
                  .prop(selected_attr, true)
                  .change();
              }

              $this.removeClass('hint-mode');

              var $next = $form.find(select_elem + ':nth-of-type(' + ($this.data('ord') + 1) + ')');
              $next.prop('disabled', false);
            }
          });

          $evalType.on(change_event, (function() {
            $('.type-cont').css('visibility', 'visible');
            $evalTypeDesc.hide();
            $evalTypeDesc2.hide();
            if ($(this).val() == "TI") {
              $tradeInDesc.show();
              $tradeInDesc2.show();
            } else if ($(this).val() == "PP") {
              $privatePartyDesc.show();
              $privatePartyDesc2.show();
            } else if ($(this).val() == "R") {
              $suggestedRetailDesc.show();
              $suggestedRetailDesc2.show();
            }
            $evalType2.val($(this).val());

          }));

          $evalType2.on(change_event, (function() {
            $evalTypeDesc2.hide();
            $evalTypeDesc.hide();
            if ($(this).val() == "TI") {
              $tradeInDesc.show();
              $tradeInDesc2.show();
            } else if ($(this).val() == "PP") {
              $privatePartyDesc.show();
              $privatePartyDesc2.show();
            } else if ($(this).val() == "R") {
              $suggestedRetailDesc.show();
              $suggestedRetailDesc2.show();
            }
            $evalType.val($(this).val());
            $submitBtn.click();
          }));

          $year.on(change_event, function() {
            var formParams = mapArrayToJson($form.serializeArray());

            $.post('/api/car-value-param/makes/', {
                '': JSON.stringify({
                  "eval_type": formParams.evalType,
                  "year": formParams.year
                })
              },
              function(resp) {
                $make
                  .empty()
                  .append('<option disabled selected class=\'prompt\'>Make</option>');

                var options = []
                $.each(resp.data.makes, function(idx, item) {
                  options.push('<option value=\'' + item.key + '\'>' + item.value + '</option>');
                });
                $make.append(options.join(''));
              });
          });

          $make.on(change_event, function() {
            var formParams = mapArrayToJson($form.serializeArray());

            $.post('/api/car-value-param/model-trims/', {
                '': JSON.stringify({
                  "eval_type": formParams.evalType,
                  "year": formParams.year,
                  "make": formParams.make
                })
              },
              function(resp) {
                $modelTrim
                  .empty()
                  .append('<option disabled selected class=\'prompt\'>Model</option>');

                var options = []
                $.each(resp.data.model_trims, function(idx, item) {
                  options.push('<option value=\'' + item.key + '\'>' + item.value + '</option>');
                });
                $modelTrim.append(options.join(''));
              });
          });

          $nextBtn.on(click_event, function(e) {
            e.preventDefault();

            $evalType.css('border-color', '#CCCCCC');
            $year.css('border-color', '#CCCCCC');
            $make.css('border-color', '#CCCCCC');
            $modelTrim.css('border-color', '#CCCCCC');

            $requiredTextSection1.hide();

            if ($evalType.val() && $year.val() && $make.val() && $modelTrim.val()) {

              $.post('/api/car-value-param/features/', {
                  '': JSON.stringify({
                    "trim": $('#model_trim').val()
                  })
                },
                function(resp) {

                  $('#step2_cardesc').html($('#year option:selected').text() + ' ' + $('#make option:selected').text() + ' ' + $('#model option:selected').text() + ' ' + $('#model_trim option:selected').text());
                  $('#step3_cardesc').html($('#year option:selected').text() + ' ' + $('#make option:selected').text() + ' ' + $('#model option:selected').text() + ' ' + $('#model_trim option:selected').text());

                  $drives.html('');

                  var drives = [];
                  $.each(resp.data.drives, function(idx, item) {
                    if (item.preselect) {
                      drives.push('<div class="radio"><input type="radio" id="radio' + item.key + '" name="drives" value=\'' + item.key + '\' class="drives" checked /><label for="radio' + item.key + '"><span></span>' + item.value + '</label></div>');
                    } else {
                      drives.push('<div class="radio"><input type="radio" id="radio' + item.key + '" name="drives" value=\'' + item.key + '\' class="drives" /><label for="radio' + item.key + '"><span></span>' + item.value + '</label></div>');
                    }
                  });
                  $drives.append(drives.join(''));

                  $transmissions.html('');

                  var transmissions = [];
                  $.each(resp.data.transmissions, function(idx, item) {
                    if (item.preselect) {
                      transmissions.push('<div class="radio"><input type="radio" id="radio' + item.key + '" name="transmissions" value=\'' + item.key + '\' class="transmissions" checked /><label for="radio' + item.key + '"><span></span>' + item.value + '</label></div>');
                    } else {
                      transmissions.push('<div class="radio"><input type="radio" id="radio' + item.key + '" name="transmissions" value=\'' + item.key + '\' class="transmissions" /><label for="radio' + item.key + '"><span></span>' + item.value + '</label></div>');
                    }
                  });
                  $transmissions.append(transmissions.join(''));

                  $engines.html('');

                  var engines = [];
                  $.each(resp.data.engines, function(idx, item) {
                    if (item.preselect) {
                      engines.push('<div class="radio"><input type="radio" id="radio' + item.key + '" name="engines" value="' + item.key + '" class="engines" checked /><label for="radio' + item.key + '"><span></span>' + item.value + '</label></div>');
                    } else {
                      engines.push('<div class="radio"><input type="radio" id="radio' + item.key + '" name="engines" value="' + item.key + '" class="engines"><label for="radio' + item.key + '"><span></span>' + item.value + '</label></div>');
                    }
                  });
                  $engines.append(engines.join(''));

                  $vehicleoptions.html('');

                  var vehicleoptions = [];
                  $.each(resp.data.vehicleoptions, function(idx, item) {
                    if (item.preselect) {
                      vehicleoptions.push('<li><input type="checkbox" id="cb' + item.key + '" value="' + item.key + '" class="vehicleoptions" checked /><label for="cb' + item.key + '">' + item.value + '</label></li>');
                    } else {
                      vehicleoptions.push('<li><input type="checkbox" id="cb' + item.key + '" value="' + item.key + '" class="vehicleoptions"><label for="cb' + item.key + '">' + item.value + '</label></li>');
                    }
                  });

                  $vehicleoptions.append(vehicleoptions.join(''));

                  $vehicleoptions.append('');

                  $section1.hide();
                  $section3.hide();
                  $section4.hide();
                  $sectionAboutKBB.hide();
                  $sectionKBBcopyright.hide();
                  $section2.show();
                });

              abt.ADS.reload();

            } else {
              var errorText = '';
              var errorItems = 0;

              if (!$evalType.val()) {
                errorText = errorText + 'Type';
                errorItems++;

                $evalType.css('border-color', 'red');
              }


              if (!$year.val()) {
                if (errorItems >= 1)
                  errorText = errorText + ', ';
                errorText = errorText + 'Year';
                errorItems++;

                $year.css('border-color', 'red');
              }


              if (!$make.val()) {
                if (errorItems >= 1)
                  errorText = errorText + ', ';
                errorText = errorText + 'Make';
                errorItems++;

                $make.css('border-color', 'red');
              }

              if (!$modelTrim.val()) {
                if (errorItems >= 1)
                  errorText = errorText + ', ';
                errorText = errorText + 'Model';
                errorItems++;

                $modelTrim.css('border-color', 'red');
              }

              if (errorItems == 1) {
                errorText = errorText + ' is ';
              } else {
                errorText = errorText + ' are ';
              }

              errorText = errorText + 'required.';

              $requiredTextSection1.show().html(errorText).css('color', 'red');
            }
          })

          $submitBtn.on(click_event, function(e) {
            e.preventDefault();

            $mileage.css('border-color', '#CCCCCC');
            $zipCode.css('border-color', '#CCCCCC');

            if ($mileage.val() && $zipCode.val() && $('input:radio[name=condition]:checked').val()) {
              $requiredTextSection2.hide();

              $vehicleValue.html('');
              $vehicleoptionsVal.html('');


              $zipCodeVal.html($zipCode.val());
              $mileageVal.html($mileage.val());

              $('.drives:checked + label').each(function() {
                $drivesVal.html($(this).text());
              });

              $('.transmissions:checked + label').each(function() {
                $transmissionsVal.html($(this).text());
              });

              $('.engines:checked + label').each(function() {
                $enginesVal.html($(this).text());
              });

              $('.vehicleoptions:checked + label').each(function() {
                $vehicleoptionsVal.append('<li>' + $(this).text() + '</li>');
              });

              var chkArray = [];

              var equipList;

              $('.drives:checked').each(function() {
                chkArray.push($(this).val());
              });

              $('.transmissions:checked').each(function() {
                chkArray.push($(this).val());
              });

              $('.engines:checked').each(function() {
                chkArray.push($(this).val());
              });

              $('.vehicleoptions:checked').each(function() {
                chkArray.push($(this).val());
              });

              equipList = chkArray.join(',');

              $.post('/api/car-value-param/vehiclevalue/', {
                  '': JSON.stringify({
                    "trim": $modelTrim.val(),
                    "eval_type": $evalType.val(),
                    "mileage": $mileage.val(),
                    "postalcode": $zipCode.val(),
                    "condition_type": $('input:radio[name=condition]:checked').val(),
                    "equipment_list": equipList
                  })
                },
                function(resp) {

                  if (resp.data.vehicleValue == '') {
                    $dollarSign.hide();
                    $vehicleValue.html('We are not able to estimate the current value of this vehicle.');
                  } else {
                    $dollarSign.show();
                    $vehicleValue.html(resp.data.vehicleValue.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                  }

                  window.scrollTo(0,0);
                  $section1.hide();
                  $section2.hide();
                  $sectionAboutKBB.hide();
                  $section3.show();
                  $section4.show();
                  $sectionKBBcopyright.show();
                });

              abt.ADS.reload();
            } else {

              var errorText = '';
              var errorItems = 0;

              if (!$.trim($mileage.val())) {
                errorText = errorText + 'Mileage';
                errorItems++;

                $mileage.css('border-color', 'red');
              }


              if (!$.trim($zipCode.val())) {
                if (errorItems >= 1)
                  errorText = errorText + ', ';
                errorText = errorText + 'Zip Code';
                errorItems++;

                $zipCode.css('border-color', 'red');
              }


              if (!$('input:radio[name=condition]:checked').val()) {
                if (errorItems >= 1)
                  errorText = errorText + ', ';
                errorText = errorText + 'Condition';
                errorItems++;
              }

              if (errorItems == 1) {
                errorText = errorText + ' is ';
              } else {
                errorText = errorText + ' are ';
              }

              errorText = errorText + 'required.';

              $requiredTextSection2.show().html(errorText).css('color', 'red');
            }
          });

          $newSearch.on(click_event, function(e) {
            abt.ADS.reload();

            window.scrollTo(0, 0);
            $section1.show();
            $sectionAboutKBB.show();
            $section2.hide();
            $section3.hide();
            $section4.hide();
            $sectionKBBcopyright.hide();
          });

          $conditionExcellent.on(click_event, function() {
            $conditionDesc.hide();
            $conditionExcellentDesc.show();
            $conditionExcellent2.prop('checked', true);
            $conditionExcellentDesc2.show();
          });

          $conditionGood.on(click_event, function() {
            $conditionDesc.hide();
            $conditionGoodDesc.show();
            $conditionGood2.prop('checked', true);
            $conditionGoodDesc2.show();
          });

          $conditionFair.on(click_event, function() {
            $conditionDesc.hide();
            $conditionFairDesc.show();
            $conditionFair2.prop('checked', true);
            $conditionFairDesc2.show();
          });

          $conditionPoor.on(click_event, function() {
            $conditionDesc.hide();
            $conditionPoorDesc.show();
            $conditionPoor2.prop('checked', true);
            $conditionPoorDesc2.show();
          });


          $conditionExcellent2.on(click_event, function() {
            $conditionDesc.hide();
            $conditionExcellentDesc2.show();
            $conditionExcellent.prop('checked', true);
            $conditionExcellentDesc.show();
            $submitBtn.click();
          });

          $conditionGood2.on(click_event, function() {
            $conditionDesc.hide();
            $conditionGoodDesc2.show();
            $conditionGood.prop('checked', true);
            $conditionGoodDesc.show();
            $submitBtn.click();
          });

          $conditionFair2.on(click_event, function() {
            $conditionDesc.hide();
            $conditionFairDesc2.show();
            $conditionFair.prop('checked', true);
            $conditionFairDesc.show();
            $submitBtn.click();
          });

          $conditionPoor2.on(click_event, function() {
            $conditionDesc.hide();
            $conditionPoorDesc2.show();
            $conditionPoor.prop('checked', true);
            $conditionPoorDesc.show();
            $submitBtn.click();
          });

        }
      }
    }();


    return {
      init: function() {
        step1.init();
      }
    }
  }();

  $(function() {
    page.init();
  });
}(window, jQuery, ABT)