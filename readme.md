#DigiTransit 10 (name not final)
<img src=https://mcalistern.visualstudio.com/_apis/public/build/definitions/b45d5b7f-8cbd-4cba-8b35-c9414b788af4/1/badge/>

It's an HSL DigiTransit app for Helsinki. Written as a UWA.

##Building
Requirements
* [Resw Code File Generator](https://reswcodegen.codeplex.com/)

##Feature Epics
- [x] Trip planning
- [x] Favorites (partially completed)
- [x] Detailed trip plan (partially completed, still need markers on map)
- [ ] Line search
- [ ] Stop search
- [ ] Versioned data formats (see [MSDN article](https://msdn.microsoft.com/en-us/windows/uwp/app-settings/store-and-retrieve-app-data) for more details)
- [ ] Multi-leg trip planning
- [ ] Show nearby stops
- [ ] Transit service alerts
- [ ] Stop/route tile pinning
- [ ] Subscribe to a line, receive notifications when it stops somewhere
- [ ] Crash reporting
- [ ] Analytics
- [ ] Testing

##Design Notes
####Searching
The official DigiTransit webapp does two things when the user enters a search term: Makes a GraphQL call for stops based on the search term, and a call to the REST endpoint for geocoding with the search string. It then collects the results, sorts them, and distinguishes between them with different icons in the autosuggest list. We should copy that strategy.

####Poorly-documented GraphQL Info

#####Time and date
Time and date parameters, when requested as strings, are expected to look like this:
```
date: "2016-05-20",
time: "23:28:00"
```

I.E. dates are in ISO 8601 format, and times are expressed in 24-hour clock format with colon separators. 

When times are returned by the server, they are usually in UNIX-milliseconds timestamp format, encoded as `long`s.

#####Formatting
When sending JSON requests, the double-quotes around strings must be escaped _in the request_. That means backslashes next to the quotes. _In the request_. Ugh.

Example:
```JSON
{"query": 
	"{ 
		plan (time: \"2:59:0\"){ }	//note the backslashes and quotes next to the numerals
	}"
}
```

#####Transit Modes-The `mode` parameter
Contrary to the documentation, it's not a string that gets coerced to a masked enum (i.e. `"WALK | BUS"`) but rather a comma-separated list of strings with no spaces (i.e. `modes:"BUS,TRAM,RAIL,SUBWAY,FERRY,WALK"`). 

##Stupid Platform Bugs and Gotchas Log
 * When using a `CollectionViewSource` as a `ListView`'s `ItemSource` and a `GroupStyle` with a `GroupStyleHeader` set to `HideIfEmpty=True`, if (any of?) the underlying lists backing the `CollectionViewSource` are ever emptied, the next time an element is added to them, the app will hard crash with no exception. Further information on [StackOverflow](http://stackoverflow.com/questions/24398252/is-there-a-bug-inside-groupstyle-hidesifempty).
 * `AutoSuggestBox` causes a "Catastrophic failure" exception with no further information if its `DisplayMemberPath` is set to anything, and the AutoSuggetBox is using an `ItemTemplate`. Further information found on [TechNet](https://social.msdn.microsoft.com/Forums/sqlserver/en-US/194e87b9-312e-4282-ac5d-a240a917cbaa/uwp-setting-autosuggestbox-items-results-in-catastrophic-failure-because-of-itemtemplate?forum=wpdevelop).
 * When using localized resources in a separate DLL, the project's `.appxmanifest` needs to be edited manually to declare supported languages. The magic `x-generate` token doesn't look in other assemblies to determine what languages the app supports.