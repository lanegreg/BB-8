/*!
 * Autobytel JSLib v0.1.0
 * (Nasdaq: ABTL) - http://www.autobytel.com 
 * Copyright (c) 2014, Autobytel Inc. - All Rights Reserved.
 *
 * Team: http://www.autobytel.com/humans.txt
 */

/* window.ABT global object */
;(function($) {
	"use strict";
	
	var consolePolyfill = function() {
		var method
			, console = (window.console = window.console || {})
			, noop = function() {};

		var methods = [
			'assert', 'clear', 'count', 'debug', 'dir', 'dirxml',
			'error', 'exception', 'group', 'groupCollapsed', 'groupEnd',
			'info', 'log', 'markTimeline', 'profile', 'profileEnd',
			'table', 'time', 'timeEnd', 'timeStamp', 'trace', 'warn'
		];

		var length = methods.length;
		while (length--) {
			method = methods[length];

			// Only stub undefined methods. 
			if (!console[method]) {
				console[method] = noop;
			}
		}
	};

  // Easter Egg
  $(window).load(function() {
    try {
      if (true) { //Tabzilla.shouldShowEasterEgg()) {
        console.log('     __\n    /  \\             _          _               _           __\n   / /\\ \\  _    _ __| |__  ___ | |_  _     _ __| |__  ___  |  |  ___   ___  _   _ \n  / /__\\ \\| |  | |__   __|/ _ \\|    \\  \\ /  |__   __|/  _ \\|  | /  _\\ / _ \\| \\ / |\n /  ____  \\ \\__/ /  | |  | (_) | (_) |  v  /   | |  |   __/|  ||  (_ | (_) | |`| |\n/_/      \\_\\____/   |_|   \\___/|____/ \\   /    |_|   \\____/|__(_)___/ \\___/|_| |_|\n                                      /  /\n                                     /__/\n/* TEAM */\n\n    Name: Daly Yoo\n    Role: Creative Director\n    Location: Newport Beach, CA USA\n\n    Name: Don Linville\n    Role: Creative Design\n    Location: Newport Beach, CA USA\n\n    Name: Mioko Chaviro\n    Role: Lead Web Designer, Front-end Developer, Mother of Kai\n    Location: Newport Beach, CA USA\n\n    Name: Greg Lane\n    Role: Lead Full Stack Developer, SEO, Architect, Perf, Team Rebel Rouser (shutup already)\n    Location: Newport Beach, CA USA\n\n    Name: Bill Ray\n    Role: Full Stack Developer, Research, Team Linebacker (go-to man)\n    Location: Newport Beach, CA USA\n\n    Name: Marvin Fetalino\n    Role: Full Stack Developer, CMS, Team Food Advisor (try this)\n    Location: Newport Beach, CA USA\n\n    Name: Sam Schulte\n    Role: Full Stack Developer, New Kid on Block\n    Location: Newport Beach, CA USA\n\n    Name: Richard Bell\n    Role: Network Engineer, Infrastructure, Epic Guru (we\'re not worthy)\n    Location: Newport Beach, CA USA\n\n\n/* SITE */\n    Last update: 2014/11/21\n    Url: http://www.CAR.com\n    Language: English\n    Doctype: HTML5\n    Standards: HTML5, CSS3, ReST\n    Components: jQuery, Modernizr\n    Frameworks: Susy\n    Software: .NET4.5/MVC5, Redis, \n    Tools: Visual Studio 2013.3, Sass, Gulp, Browserify, Jenkins, Selenium, Mocha, NUnit\n\n\n/* THANKS */\n    To everyone else that supports this team!\n\n');
      }
    }
    catch (e) {}
  });

  window.ABT = {
    //ref: http://addyosmani.com/blog/essential-js-namespacing/
		extend: function ( ns, nsString ) {
			var parts = nsString.split('.')
				, parent = ns;

			if (parts[0] === "ABT") {
				parts = parts.slice(1);
			}
			var i;
			for (i = 0; i < parts.length; i++) {
				if (typeof parent[parts[i]] === 'undefined') {
					parent[parts[i]] = {};
				}
				parent = parent[parts[i]];
			}
			return parent;
		}
	};

	consolePolyfill();
}(jQuery));
