/**  Research Pricing and Offers Tasks  **/

/* jshint node:true */
'use strict';

var gulp = require('gulp')
  , sass = require('gulp-ruby-sass')
  , compass = require('gulp-compass')
  , minifycss = require('gulp-minify-css')
  , rename = require('gulp-rename')
  , common = require('../common')
  , del = require('del')
  , runSequence = require('run-sequence')
  , livereload = require('gulp-livereload');

var controller = 'common'
  , action = 'offersystem'
  , filename = controller + '.' + action
  , templatesFolder = '../app_assets/js/app/' + controller + '/' + action + '_tmpls/';

///- JS tasks

gulp.task(filename + '-compiletmpls', function (done) {
  common.processTemplates(templatesFolder + 'mobi');
  common.processTemplates(templatesFolder + 'tabl');
  common.processTemplates(templatesFolder + 'desk');

  done();
});

gulp.task(filename + '-cleanjs', function (done) {
  del('../app_assets/dist/js/' + filename + '.*.min.js', { force: true }, done);
});

gulp.task(filename + '-js', [filename + '-cleanjs', filename + '-compiletmpls'], function () {
  return gulp
    .src('../app_assets/js/app/' + controller + '/' + action + '.*.js')
    .pipe(common.processJs())
    .pipe(rename({ prefix: controller + '.', suffix: '.min' }))
    .pipe(gulp.dest('../app_assets/dist/js/'));
});


///- Master task for this page.
gulp.task(filename + '-all', [filename + '-js'], function () {
  runSequence('create-buster-manifest');
});


///- Watch tasks for this page
gulp.task(filename + '-watch', function () {
  gulp
    .watch([
        '../app_assets/js/app/' + controller + '/' + action + '.*.js',
        '../app_assets/js/modules/*.js'
    ],
      [filename + '-all']);


  var server = livereload();
  gulp
    .watch([
      '../app_assets/dist/js/*.min.js'
    ]).on('change', function (file) {
      server.changed(file.path);
    });
});
