# dotNet5784_1982_5912

Todo:
- add comments to everything (chatGPT) in the style of ///
- change deliverable to non-nullable and default false











Done:
- add project start and end date
- add resetting those dates
- comment outfunctions in task that change the dates
- add functions to set those dates in config
- Add to test suite 4 - Config and make it that you can choose b/w the 3 functions in config
- initialize project start date and end date
- match all data types to the data source structure: https://dbdiagram.io/d/153007-5784-DO-64d8f90302bd1c4a5eb3b8a5
- maybe get rid of not needed data in dependencies
- add dependency as an XElement xml stuff (stage 3 part 5) add as XElement
- implement the project stuff (maybe in the config xml) the the project has a start and end date (or implement in both task and dependency, but that's dumb)
	- add project stuff in data-config.xml
	- fix reset stuff from inititialization in dalTest and put it in the config stuff
- refactor Engineer cost to be non-nullable this sucks and is a lot of work for their mistake
- change projected start date to be the same as scheduled start date for task everywhere
- add logic for calculated fields
- implement checks in Business layer found on page 16 of the general overview