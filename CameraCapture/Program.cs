using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace CameraCapture
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CameraCapture());
            //getCameras();
        }

        /*private static int getCameras()
        {
            IntPtr eye = CvInvoke.cvCreateCameraCapture(-1);
            if (eye != null)
            {
                IntPtr frame = CvInvoke.cvQueryFrame(eye);
                CvInvoke.cvShowImage("EYE_TEST", frame);
                CvInvoke.cvWaitKey(0);
                return 0;
            }
            else
            {
                Console.Write("CAMERA NOT DETECTED");
                return 0;
            }
        }*/
    }
}
