/*
 * @Author - Jeremy DX
 * @Date Created - May 14th 2012.
 * @File Name - MenuSystem.cs
 */

using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class MenuSystem
    {
        //This is the current selected Item Index.
        private byte selectedItemIndex = 0;

        private bool UPDOWN_MOVEMENT;
        private byte MAX_OPTIONS;
        private bool LOOPING;

        //This hold the total time this button is held down.
        private float holdTime = 0.0f;
        
        //This becomes true when we have moved 1 menu option and becomes false when we release.
        private bool isHolding = false;

        //This is our move speed this means that we will move 4 items per second.
        private static float MOVE_SPEED = 1000.0f / 4.0f;

        //Constructor below for creating the MenuSystem.
        //Takes in primary direction which represents up/down if true and left/right if false;
        //Takes in dual which represents if both directions are used.
        //Takes in options which represents the max options for primary direction.

        public MenuSystem(bool updownDirection, byte options)
        {
            UPDOWN_MOVEMENT = updownDirection;
            MAX_OPTIONS = (byte)(options - 1);
            LOOPING = true;
        }

        public MenuSystem(bool updownDirection, bool looping, byte options)
        {
            UPDOWN_MOVEMENT = updownDirection;
            MAX_OPTIONS = (byte)(options - 1);
            LOOPING = looping;
        }

        //Returns the selected item index (Vertical Index).
        public byte SelectedItemIndex()
        {
            return selectedItemIndex;
        }

        public void SelectedItemIndex(byte index)
        {
            selectedItemIndex = index;
        }

        public void ResizeList(int size)
        {
            MAX_OPTIONS = (byte)(size - 1);
        }

        //This updates the menu's
        //@Warning - After calling this method set ButtonIndex.lastGameState to the current GamePadState.
        public void Update(GameTime gameTime)
        {
            if (UPDOWN_MOVEMENT)
                UpdateUpDownMovement(gameTime);
            else
                UpdateLeftRightMovement(gameTime);
        }

        //This increments the list.
        private void Increment(int time, Boolean released)
        {
            holdTime += time;
            if (released)
            {
                isHolding = false;
                holdTime = 0.0f;
            }
            else
            {
                if (holdTime == time && !isHolding)
                    ++selectedItemIndex;
            }
            if (holdTime > MOVE_SPEED)
            {
                ++selectedItemIndex;
                isHolding = true;
                holdTime = 0.0f;
            }
            if (selectedItemIndex > MAX_OPTIONS)
                selectedItemIndex = LOOPING ? (byte)0 : MAX_OPTIONS;
        }

        //This decrements this list.
        private void Decrement(int time, Boolean released)
        {
            holdTime += time;
            if (released)
            {
                isHolding = false;
                holdTime = 0.0f;
            }
            else
            {
                if (holdTime == time && !isHolding)
                    --selectedItemIndex;
            }
            if (holdTime > MOVE_SPEED)
            {
                --selectedItemIndex;
                isHolding = true;
                holdTime = 0.0f;
            }
            if (selectedItemIndex == 255)
                selectedItemIndex = LOOPING ? MAX_OPTIONS : (byte)0;
        }

        //This decides if the list should be incremented, decrements, or do nothing.
        private void UpdateUpDownMovement(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.Y <= -0.15f)
            {
                Increment(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y > -0.15f);
            }
            else if (last.ThumbSticks.Left.Y >= 0.15f)
            {
                Decrement(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y < 0.15f);
            }
            else if (last.ThumbSticks.Right.Y <= -0.15f)
            {
                Increment(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y > -0.15f);
            }
            else if (last.ThumbSticks.Right.Y >= 0.15f)
            {
                Decrement(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y < 0.15f);
            }
            else if (last.DPad.Down == ButtonState.Pressed)
            {
                Increment(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Down == ButtonState.Released);
            }
            else if (last.DPad.Up == ButtonState.Pressed)
            {
                Decrement(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Up == ButtonState.Released);
            }
        }

        //This decides if the list should be incremented, decrements, or do nothing.
        private void UpdateLeftRightMovement(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.X >= 0.15f)
            {
                Increment(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X < 0.15f);
            }
            else if (last.ThumbSticks.Left.X <= -0.15f)
            {
                Decrement(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X > -0.15f);
            } 
            else if (last.ThumbSticks.Right.X >= 0.15f)
            {
                Increment(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X < 0.15f);
            }
            else if (last.ThumbSticks.Right.X <= -0.15f)
            {
                Decrement(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X > -0.15f);
            }
            else if (last.DPad.Right == ButtonState.Pressed)
            {
                Increment(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Right == ButtonState.Released);
            }
            else if (last.DPad.Left == ButtonState.Pressed)
            {
                Decrement(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Left == ButtonState.Released);
            }
        }

    }
}