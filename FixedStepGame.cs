using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgPong
{
    public class FixedTimestepGame : Microsoft.Xna.Framework.Game
    {
        float UpdateCredit;         // Amount of update credit we currently have  
        float WallClockAvgSum;      // Running average (sum) of time we're taking  
        float WallClockTarget;      // Target time in ms   
        float refreshRate;          // Master refresh "clock" (fps)  
        float stepRate;             // FPS requested from user  
        bool lockStep;
        readonly float[] lockRates = { 0.25f, 1.0f / 3.0f, 0.5f, 2.0f / 3.0f, 0.75f, 1.0f, 1.5f, 2.0f };

        /// <summary>  
        /// This constructor just sets IsFixedTimeStep to false.  
        /// </summary>  
        public FixedTimestepGame()
        {
            // Get our refresh rate  
            refreshRate = (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.RefreshRate;

            // Sanitize the refresh  
            if (refreshRate == 0 || (refreshRate > 58 && refreshRate < 62))
                refreshRate = 60;

            // Set our wall clock target  
            WallClockTarget = 1000.0f / refreshRate;

            // Disable XNA game timing  
            base.IsFixedTimeStep = false;

            // Set a default target elapsed time             
            TargetElapsedTime = TimeSpan.FromTicks((long)(10000000.0f / 60.0f + 0.5f));
        }

        /// <summary>  
        /// We hide the real IsFixedTimeStep, and return fake values  
        /// saying that is is always set.  
        /// </summary>  
        new public bool IsFixedTimeStep
        {
            get { return true; }
            set { base.IsFixedTimeStep = false; }
        }

        /// <summary>  
        // Indicates if Update is in lock-step with Vertical Retrace.  
        /// </summary>  
        public bool LockStep
        {
            get { return lockStep; }
        }

        /// <summary>  
        /// Returns the Step Rate in fps if LockStep==true.  
        /// </summary>  
        public float StepRate
        {
            get { if (lockStep) return stepRate; else return 0; }
        }

        /// <summary>  
        /// Returns the detected monitor refresh rate.  
        /// </summary>  
        public float RefreshRate
        {
            get { return refreshRate; }
        }

        /// <summary>  
        /// We hide the real TargetElapsedTime, so that we can recompute   
        /// 'StepRate' when the value of TargetElapsedTime changes.  
        /// </summary>  
        new public TimeSpan TargetElapsedTime
        {
            get { return base.TargetElapsedTime; }
            set
            {
                int i;
                float ftmp;

                stepRate = ((1000.0f / (float)(value.TotalMilliseconds)) + 0.5f);

                // Try to get in lock-step with the request.  
                // We will try various ratios in an attempt to lock the rate                 
                i = 0;
                lockStep = false;
                while (i < 8 && !lockStep)
                {
                    ftmp = refreshRate * lockRates[i] - stepRate;
                    if (ftmp >= -2 && ftmp <= 2)
                    {
                        lockStep = true;
                        stepRate = refreshRate * lockRates[i];
                    }
                    i++;
                }

                // Use the target elapsed time as passed  
                base.TargetElapsedTime = value;
            }
        }

        /// <summary>  
        /// This function must be used instead of overriding Update(). It  
        /// is not needed for drawable game components.  
        /// </summary>  
        protected virtual void Update2(GameTime gameTime) { }

        /// <summary>  
        /// This function is called where the expected elapsed time is  
        /// about the monitor refresh rate, but can be longer.  
        /// </summary>  
        protected override void Update(GameTime gameTime)
        {
            int UpdateCount = 0;
            bool IsRunningSlowly = false;

            // Tack our moving average  
            WallClockAvgSum -= WallClockAvgSum / 10;
            WallClockAvgSum += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Set running slowly if we're 10% over budget  
            if (((WallClockAvgSum / 10) / WallClockTarget) >= 1.1f)
                IsRunningSlowly = true;

            // We need to create a new GameTime since we don't have  
            // access to the real copy.  
            GameTime myGameTime = new GameTime(gameTime.TotalGameTime, TargetElapsedTime, IsRunningSlowly);

            // Bump our UpdateCredit  
            UpdateCredit += stepRate;

            // See how many times we need to call update (with an allowed margin of error)  
            UpdateCount = (int)(UpdateCredit / refreshRate + 0.5);

            // Call our "Update2" for games that would have normally   
            // used an override on Update(), and call base.Update() for  
            // drawable game components.  
            while (UpdateCount-- > 0)
            {
                Update2(myGameTime);
                base.Update(myGameTime);
                UpdateCredit -= refreshRate;
            }
        }
    }  
}
