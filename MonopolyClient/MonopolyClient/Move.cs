using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class Move
    {
        public Player player;
        public PictureBox pictureBox;
        public int amountToGo;

        public Move(Player player, int amountToGo)
        {
            this.player = player;
            this.amountToGo = amountToGo;
            this.pictureBox = player.pictureBox;
        }
        public Move(Player player, Square to)
        {
            this.player = player;
            this.pictureBox = player.pictureBox;
            if (to.id > player.currentSquare.id)
            {
                amountToGo = (to.id - player.currentSquare.id);
            }
            else
            {
                amountToGo = (Board.squares.Length - (player.currentSquare.id - to.id));
            }
        }
        public async Task Execute() // moves the player in the way needed + visual effects
        {
            //ClientForm.s_instance.UpdateDebugOutput($"moving {player.name} from square {player.currentSquare.id} {amountToGo} steps");
            PictureBoxMovement pictureBoxMovement = new PictureBoxMovement(pictureBox);
            for(int i = 0; i < amountToGo; i++) 
            {
                int currentSquareId = player.currentSquare.id;
                int currentPart = currentSquareId / 10;
                //ClientForm.s_instance.UpdateDebugOutput($"current square id {currentSquareId}, width:{player.currentSquare.width}, height: {player.currentSquare.height}");

                switch (currentPart) 
                {
                    case 0:
                        {
                            if (currentSquareId == 0)
                                await pictureBoxMovement.MoveLeftAsync(120);
                            else if(currentSquareId == 9)
                                await pictureBoxMovement.MoveLeftAsync(120);
                            else
                                await pictureBoxMovement.MoveLeftAsync(Board.squares[currentSquareId].width);
                            break;
                        }
                    case 1:
                        {
                            if (currentSquareId == 10)
                            {
                                await pictureBoxMovement.MoveTopAsync(104);
                            }
                            else if (currentSquareId == 19)
                            {
                                await pictureBoxMovement.MoveTopAsync(96);
                            }
                            else
                            {
                                await pictureBoxMovement.MoveTopAsync(Board.squares[currentSquareId].height);
                            }
                            break;
                        }
                    case 2:
                        { 
                            if (currentSquareId == 20)
                                await pictureBoxMovement.MoveRightAsync(120);
                            else if (currentSquareId == 29)
                                await pictureBoxMovement.MoveRightAsync(120);
                            else
                                await pictureBoxMovement.MoveRightAsync(Board.squares[currentSquareId].width);
                            break;
                        }
                    case 3:
                        {
                            if (currentSquareId == 30)
                                await pictureBoxMovement.MoveBottomAsync(96);
                            else if (currentSquareId == 39)
                                await pictureBoxMovement.MoveBottomAsync(104);
                            else
                                await pictureBoxMovement.MoveBottomAsync(Board.squares[currentSquareId].height);
                            break;
                        }
                }
                player.currentSquare = Board.squares[(player.currentSquare.id + 1) % 40];
            }
        }
    }
}
