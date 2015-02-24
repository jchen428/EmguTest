using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //Declare global variables
        private Capture capture;        //Takes images from camera as frames
        private bool captureInProgress;

        public CameraCapture()
        {
            InitializeComponent();
        }

        /**
         * Creates an EmguCV Image, captures a frame from camera and 
         * allocates it to the imageFrame, and displays it in ImageBox
         */
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> imageFrame = capture.QueryFrame();
            CamImageBox.Image = imageFrame;
        }

        /**
         * Toggles captureInProgress and starts/stops capture
         */
        private void btnStart_Click(object sender, EventArgs e)
        {
            #region Instantiate new Capture if one doesn't already exist
            if (capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            #endregion

            if (capture != null)
            {
                if (captureInProgress)      //Stop capture and reset btnStart text to "Start"
                {
                    btnStart.Text = "Start";
                    Application.Idle -= ProcessFrame;
                }
                else                        //Start capture and set btnStart text to "Stop"
                {
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }

                captureInProgress = !captureInProgress;     //Invert captureInProgress
            }
        }

        private void ReleaseData()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }
    }
}
