#Abt.Rwd/App_Workflow

- Firstly, this folder is only used for managing workflow aspects (like Gulp tasks for preprocessing, bundling, minifying, cache busting, etc.), and helps produce artifacts that get placed under the /app_assets/dist/* folder.

- Secondly, the generated artifacts under the /app_assets/dist/* folder should NEVER get committed to TFS. They are only meant to be the distributable assets for deployment.
