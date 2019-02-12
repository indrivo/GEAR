# Deployment
---

### General Info

For CI / CO in gitlab, a .gitlab-ci.yml file must be configured in the source
1. Stages must be specified in this section. Example: `- test`

```yaml
stages:
  - test
  - deploy-core
  - deploy-crm
  - deploy-hrm
  - deploy-bsc
  - deploy-pm
```
2. Add new implementation for stage

```yaml
deploy_core_service:
  stage: deploy-core
  only:
    - master
  environment:
    name: st_core
    url: "http://bpmn-dev.cs1.soft-tehnica.com"
  before_script:
    - pushd "./src/BLL/ST.CORE"
    - dotnet restore
  script:
    - dotnet publish -c Release -o ./dist
    - >-
      lftp -c "open -u %FTP_USER%,%FTP_PASSWORD% %FTP_HOST%; mirror --only-newer
      --reverse --delete --verbose --no-perms --no-symlinks ./dist/ ./ST.CORE"
```
`%FTP_USER%,%FTP_PASSWORD% %FTP_HOST%` are the secrets stored in Gitlab and can be set only for `Mainteners`
Example: In Gitlab can be added as name: `FTP_USER` and value: `Administrator`

### Environment Configuration
In order to be deployed, they are set on each 3 env project on projects
1. `Release`
2. `Stage`
3. `Debug`

This project is deployed only for Stage and Release. According to this, there are 2 Branches. Respectively for Stage -> stage and Release -> master, in the configuration file is specified the target branch on which to deploy.
When a commit is added to the master, it will automatically be deployed for Release, and when it is done on the stage, it will automatically be done on Stage.
### Deploy on IIS

In order to be deployed, it is necessary to turn off the WebSite on which the source will change, because if it does not stop, the FTP data transfer will be canceled because the files are being used.
To stop this website in IIS, you must connect to the server `192.168.1.209` with the user credentials:` Administrator` and password: `AlphaOmega1` then open IIS and shut down ST.BPMN for Release and ST.CORE.Stage for Stage.