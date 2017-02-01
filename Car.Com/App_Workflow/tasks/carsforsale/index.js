/**  Site Index (Home Page) Tasks  **/

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

var controller = 'carsforsale'
  , action = 'index'
  , filename = controller + '.' + action
  , templatesFolder = '../app_assets/js/app/' + controller + '/' + action + '_tmpls/';



///- CSS tasks
gulp.task(filename + '-cleancss', function (done) {
  del('../app_assets/dist/css/' + filename + '*.css', { force: true }, done);
});

gulp.task(filename + '-process-inline-css', [filename + '-cleancss'], function () {
  return gulp
    .src([
      '../app_assets/sass/app/' + controller + '/' + action + '.head.scss',
      '../app_assets/sass/libs/**/*.scss',
      '../app_assets/sass/modules/*.scss',
      '!../app_assets/sass/app/' + controller + '/' + action + '.mobi.scss',
      '!../app_assets/sass/app/' + controller + '/' + action + '.tabl.scss',
      '!../app_assets/sass/app/' + controller + '/' + action + '.desk.scss'
    ])
    .pipe(common.processCss())
    .pipe(rename({ prefix: controller + '.', suffix: '.min' }))
    .pipe(gulp.dest('../app_assets/tmp/'));
});

gulp.task(filename + '-processcss', [filename + '-process-inline-css'], function () {
  return gulp
    .src([
      '../app_assets/sass/app/' + controller + '/' + action + '.mobi.scss',
      '../app_assets/sass/app/' + controller + '/' + action + '.tabl.scss',
      '../app_assets/sass/app/' + controller + '/' + action + '.desk.scss',
      '../app_assets/sass/libs/**/*.scss',
      '../app_assets/sass/modules/*.scss',
      '!../app_assets/sass/app/' + controller + '/' + action + '.head.scss'
    ])
    .pipe(common.processCss())
    .pipe(rename({ prefix: controller + '.', suffix: '.min' }))
    .pipe(gulp.dest('../app_assets/tmp/'));
});

gulp.task(filename + '-movecss', [filename + '-processcss'], function () {
  return gulp
    .src([
      '../app_assets/tmp/' + filename + '*.css',
      '!../app_assets/tmp/' + filename + '.head.css',
      '!../app_assets/tmp/' + filename + '.head.min.css'
    ])
    .pipe(gulp.dest('../app_assets/dist/css/'));
});

gulp.task(filename + '-css', [filename + '-movecss'], function () {
  return gulp
    .src([
      '../app_assets/tmp/' + filename + '.head.css',
      '../app_assets/tmp/' + filename + '.head.min.css'
    ])
    .pipe(gulp.dest('../assets/above_the_fold/'));
});


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
gulp.task(filename + '-all', [filename + '-js', filename + '-css'], function () {
  runSequence('create-buster-manifest');
});


///- Watch tasks for this page
gulp.task(filename + '-watch', function () {
  gulp
    .watch([
        '../app_assets/js/app/' + controller + '/' + action + '.*.js',
        '../app_assets/js/modules/*.js',
        '../app_assets/sass/app/' + controller + '/' + action + '.*.scss',
        '../app_assets/sass/modules/*.scss'
    ],
      [filename + '-all']);


  var server = livereload();
  gulp
    .watch([
      '../app_assets/dist/js/*.min.js',
      '../app_assets/dist/css/*.min.css'
    ]).on('change', function (file) {
      server.changed(file.path);
    });
});
