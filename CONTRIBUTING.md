# CONTRIBUTING

The following guid will help you contribute to this project harass-free and productive. Bad practices of contribution should not be tolerated. 

Any change, addition to this project must be tested and documented. By `tested`, a unit testing should be deducted, as the unit tests are ran automatically by the pipeline, plus some pre-merge tests of the actual new feature to be added. When developing something new, you must make sure it doesn't break up the whole project, because the default branch `dev` is always deployable and other developers should be able to deploy this branch easily. Test it out before requesting a merge.

# Rules

Some rules should be taken into consideration when making a contribution to this project.

### Commit messages

* Commit messages should be descriptive about what are the changes included in this commit and the description should match with what is actually in this commit. **DO NOT** include some additional changes, besides those descripted in the commit message. It is very important as it will help the code reviewer to understand what has been changed/added in the commit.
* A really good practice is to append a prefix to the commit describing the type of contribution you are making which are enumerated as follows:
    * `feat:` A brand new feature introduced in the project that is completely new.
    * `doc:` A commit that represents a documentation contribution of some existing modules and services.
    * `refact:` A refactoring (modification) of the existing code that is supposed to improve performance or follow some convention rules that were not previously respected.
    * `fix:` When a bug has been reported by some testers, or it was just found during development, this prefix indicates that the commit is a fix for that bug. A description of the bug should be included with/without a 
    [Issue](http://gitlab.cs1.soft-tehnica.com/aodpi/PIGDMachina/issues) reference if an issue has been opened on gitlab.

All these prefixes are very useful as it can help filtering out the commits. For example if a supervisor of the project would want to see how many fixes were commited to the project they could simply filter out the commits with the `fix:` prefix, thus they are shown in the list easily accessible. 

Example commit messages:

* fix: Added missing configuration to the IdentityService (#15)
* refact: Split up the DatabaseModels for UserProfiles into separated tables
* doc: Documented Identity Service Permission Management related code.

Be short and descriptive about your actions, and keep your commits clean.


### Branches

When creating a new branch it should have a short descriptive name about the actual development process on this branch. We are creating a feature branch all the time, which is done according to the git flow. Please refer to [GitHub Flow](https://guides.github.com/introduction/flow/) for more information. Keep the branch names with lowercase letters separated by a hyphen, for example:

* feature-identity-service
* feature-user-permissions
* ui/ux-users-crud
* ui/ux-permissions-crud

Naming will help reviewers understand what feature has been developed, what changes were made to make the reviewing process easy and smooth.

### Issues

The GitLab Issues feature can be used to discuss an issue detected in the project, it can also be used as a task management system as there is a [Kanban Board](http://gitlab.cs1.soft-tehnica.com/aodpi/PIGDMachina/boards) were you can easily manage your tasks on the project. If you detect a problem on the project, open up an issue and apply respective labels to this issue. 

* **DO NOT** create new labels unless discussed with supervisors. 
* **DO NOT** close someone else's opened issue, they should close them.

The Git flow ensures a comfortable, standard workflow that will boost up the development process.

Keep your commits simple and specific, give descriptive names. Happy coding !!!