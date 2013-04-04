Spinnaker
=========

Spinnaker is an Open Source client to [App.net](http://app.net) with the goal of easily posting to your timeline(s). It is inspired by Sail available on the Mac.

It's not meant to fetch timelines or similar - just for fast posting.

I provide a compiled version with a [setup here for download](http://www.li-ghun.de/Downloads/Spinnaker/app.publish/publish.htm).

It's based on my Open Source library to App.net called [AppNet.NET](https://github.com/liGhun/AppNet.NET).

## Notes for developers ##

Spinnaker uses C# / .NET 4.0 with [Visual Studio 2012 for Desktop Express Edition](http://www.microsoft.com/visualstudio/eng/products/visual-studio-express-for-windows-desktop) (which is available for free).

You need a developer account with App.net and if you check out this repository and start Spinnaker by pressing F5 the first window will ask for your client ID and secret. They will be stored on your local %APPDATA% for your convenience. If you want to create an installable file you must enter your client ID and secret in the Common.cs file in order to have them embedded in the binary.

Authorization is done by using a WebBrowser control. The downside is that this control by default only renders HTML in quirks mode which will bring a 404 error on authorization success (as the WebBrowser control is limited) and because of this you won't get the needed access token.

To enable the full browser mode you need to [enter a parameter in the registry as described on this side](http://www.west-wind.com/weblog/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version). Remember to enter both Spinnaker.exe and Spinnaker.vshost.exe in your registry to have the debug version work also. Also remember that if you deliever a setup (I'll include an NSIS example later on) this setup needs to add those registry keys for the user automatically.