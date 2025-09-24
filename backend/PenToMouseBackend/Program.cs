using System.Net.Sockets;
using System.Net;
using System.Text;

namespace PenToMouseBackend
{
    internal class Program
    {
        private const int listenPort = 5000;
        private static void StartListener()     //basic listener for UDP packets, could utilize tcp here for more reliability but udp is faster and we can handle some packet loss
                                                //so now that we have basic listener and we are receiving the packets we need to store/update them and use them to move mouse
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);
                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {Encoding.UTF8.GetString(bytes)}");
                    //so we could do the mouse events here for now
                    //first i would need to create a class for integrating the win32 dll that moves the cursor
                    //getting issues with data stream due to format of data being sent so im refactoring message
                    string message = Encoding.UTF8.GetString(bytes);
                    string[] parts = message.Split(':');
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Invalid Format");
                        continue;
                    }
                    string type = parts[0].ToUpper();       //TOUCH or HOVER
                    string[] coords = parts[1].Split(',');
                    if (coords.Length >= 3)     //for pressure
                    { 
                        //parsing to int
                        //changing to float for more precision, might not be needed but just in case
                        float phoneX = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);       //for za localizations
                        float phoneY = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
                        float pressure = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);
                        /*
                        int phoneX = int.Parse(coords[0]);
                        int phoneY = int.Parse(coords[1]);
                        */
                        //need to convert to screen coords here
                        //for s25 ultra phone screen is 3120×1440 at 6.9inch, my monitor is 2560x1440 at 27inch, not sure how to do the math here
                        //storing the var for now
                        //swapped for now as phone is in portrait mode and monitor in landscape
                        //drawimg might be better to do landscape mode on phone too, even if i swap them landscape still isnt up and down need to figure out math more


                        //could adjust to be more dynamic, would need to read screen actually maybe using pinvoke, and then reading on frontend side also gonna have to research this
                        const int phoneWidth = 1440;
                        const int phoneHeight = 3120;
                        const int monitorWidth = 2560;
                        const int monitorHeight = 1440;


                        // Debug: Check if coordinates are within expected bounds
                        //Console.WriteLine($"Raw phone coords: X: {phoneX} (max: {phoneWidth}), Y: {phoneY} (max: {phoneHeight})");

                        // Clamp coordinates to phone screen bounds to prevent going off-screen -- gpt inspired
                        phoneX = Math.Max(0, Math.Min(phoneX, phoneWidth));
                        phoneY = Math.Max(0, Math.Min(phoneY, phoneHeight));

                        //simple ratio for now, casting to double to avoid int division, and dividing monitor by phone to get scale factor
                        double scaleX = (double)monitorWidth / phoneWidth;
                        double scaleY = (double)monitorHeight / phoneHeight;

                        //applying scale factor to coords
                        int x = (int)(phoneX * scaleX);
                        int y = (int)(phoneY * scaleY);

                        //moving mouse      --refactored
                        //Win32Import.SetCursorPos(x, y);       //removed to improve precision
                        // okay now that we have mouse moving we need to be able to read pressure or taps to simulate clicks
                        //Console.WriteLine($"Type: {type}, Pressure: {pressure}, Scaled to: X: {x}, Y: {y}");
                        //working!!
                        //so now we need to add click events based on pressure or touch/hover, if hover its 0 and if touch its 1


                        //could incorperate some smoothing and dpi sensitivity but need to look this up
                        if (type == "TOUCH" && pressure > 0)      //if touch and pressure is above 0 we click
                        {
                            //Console.WriteLine("Touch detected, simulating mouse down.");
                            Win32Import.SetCursorPos(x, y);     //updating coords on touch
                            Win32Import.mouse_event(Win32Import.MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);

                        }
                        else if (type == "HOVER" || (type == "TOUCH" && pressure == 0))   //if hover or touch with 0 pressure we release
                        {
                            //Console.WriteLine("Hover or touch with zero pressure detected, simulating mouse up.");
                            Win32Import.SetCursorPos(x, y);     //updating coords on hover for enhanced precision
                            Win32Import.mouse_event(Win32Import.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);

                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid data received.");
                    }
                    //Console.WriteLine($"Moved Mouse to: X: {x}, Y: {y}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }
        static void Main(string[] args)
        {
            StartListener();
        }
    }
}


//thinking of adding another project specficially winforms to handle ui and settings, could be useful to have a tray icon and some basic settings, also a nav bar in top center of screen for quick access to paint or settings lets say