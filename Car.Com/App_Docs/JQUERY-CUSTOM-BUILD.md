## To build a new custom version of jquery... ##

1. **Install Grunt:** Open NodeJS CLI, then type: > **npm install -g grunt-cli**

1. **Install git:** Go to [http://git-scm.com/download/win](http://git-scm.com/download/win) and install

1. Go to: **\Sharknado\Abt.Rwd\App_Assets\js\libs**, right click and select **git bash here**

1. From \Sharknado\Abt.Rwd\App_Assets\js\libs, in your git bash command window, execute the following 3 commands:

----------

 - git clone -b 1.11.0 https://github.com/jquery/jquery.git
 - cd jquery && npm install
 - grunt custom:-deprecated,-event/alias,-effects

----------

**Creates new jquery files in this folder:**
Sharknado\Abt.Rwd\App_Assets\js\libs\jquery\dist
