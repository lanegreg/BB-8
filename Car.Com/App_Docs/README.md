# Carnado Environment Setup

1. Install Visual Studio 2013.

1. Download and install Node Tools for Visual Studio 2013 Version 1.0 (NTVS 1.0 VS 2013.msi) [https://nodejstools.codeplex.com/downloads/get/1440949](https://nodejstools.codeplex.com/downloads/get/1440949)

1. Install **NodeJS v0.10.24:** [https://nodejs.org/dist/v0.10.24/](https://nodejs.org/dist/v0.10.24/)

1. Install **Ruby 2.0.0-p353 (32bit):** 
[http://rubyinstaller.org/downloads/archives](http://rubyinstaller.org/downloads/archives)
[http://dl.bintray.com/oneclick/rubyinstaller/rubyinstaller-2.0.0-p353.exe](http://dl.bintray.com/oneclick/rubyinstaller/rubyinstaller-2.0.0-p353.exe)

1. Install **SASS and Compass:** Open the Ruby command prompt, then type: **gem install compass --pre**

1. Install **Git:** Go to [http://git-scm.com/download/win](http://git-scm.com/download/win) and make sure to select the right-click context-menu for git bash during the install.

1. Get latest of the **Carnado** solution from TFS, but do not open it just yet.

1. Navigate to the **\Carnado-Dev\Car.Com\App_Workflow** folder, right-click and open a git bash.

1. From the git bash, type this npm command at prompt: **npm install**

1. From the git bash, do a global **Gulp** install by typing at prompt: **npm install -g gulp**

1. Right click on the ***Workflow*** project node to get the context menu. Select ***Open Command Prompt Here...*** Then type > **npm install**

1. **OPTIONAL** Install Web Essentials 2013.5 thru VS2013 Extensions and Updates

1. **OPTIONAL** (*only if you have installed Web Essentials*): In Visual Studio 2013, from the Tools menu choose Options. In the Options dialog, select the Web Essentials node. In the Web Essentials node, select SASS, and update settings as follows: 
Compile files on build = **False**, 
Compile files on save = **False**, 
Create source map files = **False**

1. Run a full gulp.

1. Finally, do a complete build... Good Luck!
