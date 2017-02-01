
'use strict';

//#region - Variable Declarations
var config = require('./config')
  , gulp = require('gulp')
  , lazypipe = require('lazypipe')
  , sass = require('gulp-ruby-sass')
  , compass = require('gulp-compass')
  , minifycss = require('gulp-minify-css')
  , browserify = require('gulp-browserify')
  , uglify = require('gulp-uglify')
  , jshint = require('gulp-jshint')
  , stylish = require('jshint-stylish')
  , rename = require('gulp-rename')
  , header = require('gulp-header')
  , del = require('del')
  // NOT using ??
  , notify = require('gulp-notify')
  , fs = require('fs')
  , dot = require('dot');

//#endregion


//#region - Tasks to process and move files in the lib folder

gulp.task('process-modernizr', function () {
  return gulp
    .src('../app_assets/js/libs/modernizr-*.js')
    .pipe(uglify())
    .pipe(rename({ suffix: '.min' }))
    .pipe(gulp.dest('../assets/'));
});

gulp.task('move-minified-libs', function () {
  return gulp
    .src([
      '../app_assets/js/libs/omni.*.min.js',
      '../app_assets/js/libs/jquery-*.min.js',
      '../app_assets/js/libs/crypto-aes.min.js',
      '../app_assets/js/libs/crypto-sha1.min.js',
      '../app_assets/js/libs/crypto-lz.min.js',
      '../app_assets/js/libs/lodash-2.4.1.min.js',
      '../app_assets/js/libs/popup-1.7.4.min.js',
      '../app_assets/js/libs/picturefill-2.1.0.min.js',
      '../app_assets/js/libs/unslider-1.0.0-c1.0.min.js',
      '../app_assets/js/libs/jquery.event.move-1.3.6.min.js',
      '../app_assets/js/libs/svg4ie-1.0.0.min.js',
      '../app_assets/js/libs/jquery.lazyload.min.js',
      '../app_assets/js/libs/salvattore-1.0.8.min.js'
    ])
    .pipe(gulp.dest('../app_assets/dist/libs/'));
});

gulp.task('process-libs', [
    'process-modernizr',
    'move-minified-libs'
  ], function(done) {
    done();
  }
)

//#endregion


//#region - Tasks to process and move site-module files

gulp.task('bust-site-modules', function () {

})

gulp.task('process-site-modules', ['bust-site-modules'], function () {
  return gulp
    .src('../app_assets/js/modules/abt-trk.js')
    .pipe(uglify())
    .pipe(rename({ suffix: '.min' }))
    .pipe(gulp.dest('../assets/'));
})

gulp.task('site-modules', ['process-site-modules'], function(done) {
  done();
})

//#endregion


//#region - Tasks to minify and move the head.inline.js file

gulp.task('clean-inline-js', function (done) {
  del('../assets/head.inline.min.js', {
    force: true
  }, done);
});

gulp.task('process-inline-js', ['clean-inline-js'], function () {
  return gulp
    .src('../app_assets/js/app/head.inline.js')
    .pipe(browserify())
    //.pipe(gulp.dest('../assets/'))
    .pipe(jshint({
      asi: true,
      laxcomma: true,
      //lastsemic: true,
      //undef: true,
      //unused: true,
      //strict: true,
      //node: true,
      //jquery: true,
      //browser: true,
      //nonstandard: true,
      //predef: ['require', 'module', 'exports']
    }))
    .pipe(jshint.reporter(stylish))
    .pipe(uglify())
    .pipe(rename({ suffix: '.min' }))
    .pipe(gulp.dest('../assets/'));
});

//#endregion


//#region - Tasks to minify and move the global.js file

gulp.task('clean-global-js', function (done) {
  del('../app_assets/dist/libs/global.site.min.js', {
    force: true
  }, done);
});

gulp.task('process-global-js', ['clean-global-js'], function () {
  return gulp
    .src('../app_assets/js/app/global.site.js')
    .pipe(processJs())
    .pipe(jshint({
      asi: true,
      laxcomma: true,
      //lastsemic: true,
      //undef: true,
      //unused: true,
      //strict: true,
      //node: true,
      //jquery: true,
      //browser: true,
      //nonstandard: true,
      //predef: ['require', 'module', 'exports']
    }))
    .pipe(jshint.reporter(stylish))
    .pipe(rename({ suffix: '.min' }))
    .pipe(gulp.dest('../app_assets/dist/libs/'));
});

//#endregion


//#region - Establish some commonly used pipes

var processCss = lazypipe()
  .pipe(sass, { compass: true })
  .pipe(minifycss)
  .pipe(header, '/*! CSS v0.0.1 CAR.com (c)2015 - Team ref: http://www.car.com/humans.txt */');

var debugProcessJs = lazypipe()
  .pipe(browserify)
  .pipe(jshint, {
    asi: true,
    laxcomma: true,
    //lastsemic: true,
    //undef: true,
    //unused: true,
    //strict: true,
    //node: true,
    //jquery: true,
    //browser: true,
    //nonstandard: true,
    //predef: ['require', 'module', 'exports']
  })
  .pipe(jshint.reporter, stylish);

var processJs =
  config.isdebug
    ? debugProcessJs
    : debugProcessJs
    .pipe(uglify)
    .pipe(header, '/*! Javascript v0.0.1 CAR.com (c)2014 - Team:http://www.car.com/humans.txt */')


//#endregion


//#region - Process Templates

function processTemplates(path) {
  console.log('processTemplates', path)
  fs.exists(path, function (exists) {
    if (exists) {
      var tmpls = dot.process({ path: path, strip: true }),
          code = '';

      for (var prop in tmpls) {
        code += 'module.exports.' + prop.toString() + '=' + tmpls[prop].toString().replace('\ufeff', '') + ';';
      }

      fs.writeFileSync(path + '/tmpls.js', code);
    }
  });
}

//#endregion



module.exports = {
  processCss: processCss,
  processJs: processJs,
  processTemplates: processTemplates
};