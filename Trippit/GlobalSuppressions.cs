﻿
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no tar    get or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("General", "RCS1025:Format each enum member on separate line.", Justification = "Small enums don't need to have memberso n separate lines.", Scope = "type", Target = "~T:Trippit.ViewModels.TripResultViewModel.TripState")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Potential Code Quality Issues", "RECS0018:Comparison of floating point numbers with equality operator", Justification = "This check is only for rebinding protection. If it changes at all, we DO want to update.", Scope = "member", Target = "~P:Trippit.Controls.TripResultContent.MapWidth")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1102:Mark class as static.", Justification = "Need this to be iniitalizable by XAML, and static classes aren't.", Scope = "type", Target = "~T:Trippit.Helpers.FontIconGlyphs")]

