# Trippit
[![Build status](https://ci.appveyor.com/api/projects/status/lkk8wtd9gochr8fs/branch/master?svg=true)](https://ci.appveyor.com/project/pingzing/digi-transit-10/branch/master)

A UWP HSL Reittiopas app for the greater Helsinki region. Runs on Windows 10 for Desktops and Mobile.

## Building
Requirements
* [Resw Code File Generator](https://reswcodegen.codeplex.com/) (only works with VS2015 at the moment)

## Feature Epics

### Completed

- [x] Trip planning
- [x] Favorites
- [x] Detailed trip plan
- [x] Line search
- [x] Stop search
- [x] Multi-leg trip planning
- [x] Show nearby stops
- [x] Transit service alerts
- [x] Stop/route tile pinning
- [x] Crash reporting
- [x] Analytics

### Planned

- [ ] Subscribe to a line, receive notifications when it stops somewhere
- [ ] Testing (really should write unit tests for the GraphQL parser...)
- [ ] Show city bikes on map
- [ ] Allow city bikes in route planning
- [ ] Show journey-in-progress at top of app in the shell

#### Poorly-documented GraphQL Info

##### Time and date
Time and date parameters, when requested as strings, are expected to look like this:
```
date: "2016-05-20",
time: "23:28:00"
```

I.E. dates are in ISO 8601 format, and times are expressed in 24-hour clock format with colon separators. 

When times are returned by the server, they are usually in UNIX-milliseconds timestamp format, encoded as `long`s.

###### Formatting
When sending JSON requests, the double-quotes around strings must be escaped _in the request_. That means backslashes next to the quotes. _In the request_. Ugh.

Example:
```JSON
{"query": 
	"{ 
		plan (time: \"2:59:0\"){ }	//note the backslashes and quotes next to the numerals
	}"
}
```

##### Transit Modes-The `mode` parameter
Contrary to the documentation, it's not a string that gets coerced to a masked enum (i.e. `"WALK | BUS"`) but rather a comma-separated list of strings with no spaces (i.e. `modes:"BUS,TRAM,RAIL,SUBWAY,FERRY,WALK"`). 

## Stupid Platform Bugs and Gotchas Log
 * When using a `CollectionViewSource` as a `ListView`'s `ItemSource` and a `GroupStyle` with a `GroupStyleHeader` set to `HideIfEmpty=True`, if (any of?) the underlying lists backing the `CollectionViewSource` are ever emptied, the next time an element is added to them, the app will hard crash with no exception. Further information on [StackOverflow](http://stackoverflow.com/questions/24398252/is-there-a-bug-inside-groupstyle-hidesifempty).
 * `AutoSuggestBox` causes a "Catastrophic failure" exception with no further information if its `DisplayMemberPath` is set to anything, and the AutoSuggetBox is using an `ItemTemplate`. Further information found on [TechNet](https://social.msdn.microsoft.com/Forums/sqlserver/en-US/194e87b9-312e-4282-ac5d-a240a917cbaa/uwp-setting-autosuggestbox-items-results-in-catastrophic-failure-because-of-itemtemplate?forum=wpdevelop).
 * When using localized resources in a separate DLL, the project's `.appxmanifest` needs to be edited manually to declare supported languages. The magic `x-generate` token doesn't look in other assemblies to determine what languages the app supports.
 * When using localized resource sin a separate DLL, if you want to use a localized DisplayName or Description or whatever in the `.appxmanifest`, you give to give it the whole path, like so: `ms-resource:DigiTransit10.Localization/AppResources/AppName`.
