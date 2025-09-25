# PenToMouseApp
### A Simple App that allows for my S25 Stylus to connect to my PC and act as a drawing tablet

*Current Features*
- Sends UDP Packets From Phone(Frontend) to PC(Backend) that contains the coordinates, pressure and touch type from the stylus
- Basic Client Server Setup, Allowing the server to be running at all times, and client to be run when desired
- Allows for movement mimicing from stylus and translates to mouse movements/actions


*Planned Features*
- Integrate a UI (In a form of a nav bar for quick access and perhaps settings for configuration)
- Smoothen out the Scalling
- Adjust for Landscape mode on phone
- Smoothen out the Sensitivity
- Cleanup some code
- Add adjustable screensizes for more device support
- Integrate other connectivity options *optional*
- Add tcp option to mitigate packet loss severe networks *optional*
