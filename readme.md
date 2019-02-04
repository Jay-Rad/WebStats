# WebStats

[![Build status](https://ci.appveyor.com/api/projects/status/jf6qqqgmc31qqmfw?svg=true)](https://ci.appveyor.com/project/Jay-Rad/webstats)

An HttpModule that gathers statistics about request processing times and response sizes.  For web page requests, results are injected into the response bodies as a small widget.

![WebStats Widget](https://github.com/Jay-Rad/WebStats/raw/master/Solution%20Items/Widget.gif "WebStats Widget")

## Why I Built This
I created this as part of a coding challenge for an open position at a really awesome company.

## Getting Started
* The entry point of the module is \WebStats\StatsModule.cs.
* Run WebStats.SampleSite for a working demo.
* Run Install-WebStats.ps1 from an elevated PowerShell console to install WebStats module on all local IIS sites.
    * Note: The module DLL is embedded as a Base64 string in the script and is updated every build.  You can distribute the script by itself for installation on other machines.
* To install on a single site, put the WebStats.dll file in the site's Bin directory, then modify the web.config's modules section to include type "WebStats.StatsModule".
* You can customize the widget by modifying \WebStats\Resources\StatsWidget.html.

## Prerequisites
* Requires .NET Framework 4.5.2 or higher.
* IIS Express is required for E2E tests.

## Running the Tests
* Standard unit tests are in WebStats.UnitTests.
* WebStats.E2E contains end-to-end tests.
    * Launches the sample site in IIS Express.
    * Uses Selenium to browse the site and ensure the widget is working properly.
* If the tests aren't running, try rebuilding the solution a few times.
* If there are exceptions, try running "Update-Package -Reinstall" in the NuGet Package Manager console.