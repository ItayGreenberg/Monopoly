using System;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace MonopolyClient
{

    public class PictureBoxMovement
    {
        private PictureBox pictureBox;
        private int pixelsPerStep = 8; // Adjust as needed
        private int interval = 20; // Adjust as needed

        public PictureBoxMovement(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
        }
        public async Task MoveLeftAsync(int pixels)
        {
            await MoveAsync(-pixels, 0);
        }
        public async Task MoveTopAsync(int pixels)
        {
            await MoveAsync(0, -pixels);
        }
        public async Task MoveRightAsync(int pixels)
        {
            await MoveAsync(pixels, 0);
        }
        public async Task MoveBottomAsync(int pixels)
        {
            await MoveAsync(0, pixels);
        }
        private async Task MoveAsync(int deltaX, int deltaY) // fill in the end
        {
            int steps = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY)) / pixelsPerStep;

            float stepX = (float)deltaX / steps;
            float stepY = (float)deltaY / steps;

            for (int i = 0; i < steps; i++)
            {
                pictureBox.Left += (int)stepX;
                pictureBox.Top += (int)stepY;
                await Task.Delay(interval);
            }
        }
    }
}