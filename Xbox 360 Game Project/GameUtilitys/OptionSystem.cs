using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class OptionSystem
    {
        private byte[] subList;
        private byte primaryItemIndex = 0;

        private byte[] MAX_OPTIONS;
        private bool UPDOWN_MOVEMENT;

        private float holdTimeMainList = 0.0f;
        private float holdTimeSubList = 0.0f;
        private bool isHoldingMainList = false;
        private bool isHoldingSubList = false;

        public static float MOVE_SPEED = 1000.0f / 4.0f;

        //list is a size list so we can keep track of max options.
        //Direction is true if we go up/down and false if we go left/right
        public OptionSystem(byte[] list, bool direction)
        {
            this.subList = new byte[list.Length];
            MAX_OPTIONS = list;
            for (int i = 0; i < subList.Length; i++)
                subList[i] = 0;
            UPDOWN_MOVEMENT = direction;
        }

        public byte PrimaryItemIndex()
        {
            return primaryItemIndex;
        }

        public byte[] SubList()
        {
            return subList;
        }

        public void Update(GameTime gameTime)
        {
            if (UPDOWN_MOVEMENT)
            {
                UpdateVertical(gameTime, true);
                UpdatePrimary();
                if (holdTimeMainList == 0.0f)
                {
                    UpdateHorizontal(gameTime, false);
                    UpdateSecondary();
                }
            }
            else
            {
                UpdateHorizontal(gameTime, true);
                UpdatePrimary();
                if (holdTimeMainList == 0.0f)
                {
                    UpdateVertical(gameTime, false);
                    UpdateSecondary();
                }
            }
        }

        public void push()
        {
            if (holdTimeSubList != 0.0f || holdTimeMainList != 0.0f)
                return;
            if (++subList[primaryItemIndex] >= MAX_OPTIONS[primaryItemIndex])
                subList[primaryItemIndex] = 0;
            holdTimeSubList = 0.0f;
        }

        public void pull()
        {
            if (holdTimeSubList != 0.0f || holdTimeMainList != 0.0f)
                return;
            if (--subList[primaryItemIndex] == 255)
                subList[primaryItemIndex] = (byte)(MAX_OPTIONS[primaryItemIndex] - 1);
            holdTimeSubList = 0.0f;
        }

        public void UpdatePrimary()
        {
            if (primaryItemIndex == 255)
                primaryItemIndex = (byte)(subList.Length - 1);
            else if (primaryItemIndex >= subList.Length)
                primaryItemIndex = 0;
        }

        public void UpdateSecondary()
        {
            if (subList[primaryItemIndex] == 255)
                subList[primaryItemIndex] = (byte)(MAX_OPTIONS[primaryItemIndex] - 1);
            else if (subList[primaryItemIndex] >= MAX_OPTIONS[primaryItemIndex])
                subList[primaryItemIndex] = 0;
        }

        private void DecrementMainList(int time, bool released)
        {
            holdTimeMainList += time;
            if (released)
            {
                isHoldingMainList = false;
                holdTimeMainList = 0.0f;
            }
            else
            {
                if (holdTimeMainList == time && !isHoldingMainList)
                    --primaryItemIndex;
            }
            if (holdTimeMainList > MOVE_SPEED)
            {
                --primaryItemIndex;
                isHoldingMainList = true;
                holdTimeMainList = 0.0f;
            }
        }

        private void DecrementSubList(int time, bool released)
        {
            holdTimeSubList += time;
            if (released)
            {
                isHoldingSubList = false;
                holdTimeSubList = 0.0f;
            }
            else
            {
                if (holdTimeSubList == time && !isHoldingSubList)
                    --subList[primaryItemIndex];
            }
            if (holdTimeSubList > MOVE_SPEED)
            {
                --subList[primaryItemIndex];
                isHoldingSubList = true;
                holdTimeSubList = 0.0f;
            }
        }

        private void IncrementMainList(int time, bool released)
        {
            holdTimeMainList += time;
            if (released)
            {
                isHoldingMainList = false;
                holdTimeMainList = 0.0f;
            }
            else
            {
                if (holdTimeMainList == time && !isHoldingMainList)
                    ++primaryItemIndex;
            }
            if (holdTimeMainList > MOVE_SPEED)
            {
                ++primaryItemIndex;
                isHoldingMainList = true;
                holdTimeMainList = 0.0f;
            }
        }

        private void IncrementSubList(int time, bool released)
        {
            holdTimeSubList += time;
            if (released)
            {
                isHoldingSubList = false;
                holdTimeSubList = 0.0f;
            }
            else
            {
                if (holdTimeSubList == time && !isHoldingSubList)
                    ++subList[primaryItemIndex];
            }
            if (holdTimeSubList > MOVE_SPEED)
            {
                ++subList[primaryItemIndex];
                isHoldingSubList = true;
                holdTimeSubList = 0.0f;
            }
        }

        private void UpdateHorizontal(GameTime gameTime, bool primary)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.X >= 0.15f)
            {
                if (primary)
                    IncrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X < 0.15f);
                else
                    IncrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X < 0.15f);
            }
            else if (last.ThumbSticks.Left.X <= -0.15f)
            {
                if (primary)
                    DecrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X > -0.15f);
                else
                    DecrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X > -0.15f);
            }
            else if (last.ThumbSticks.Right.X >= 0.15f)
            {
                if (primary)
                    IncrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X < 0.15f);
                else
                    IncrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X < 0.15f);
            }
            else if (last.ThumbSticks.Right.X <= -0.15f)
            {
                if (primary)
                    DecrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X > -0.15f);
                else
                    DecrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X > -0.15f);
            }
            else if (last.DPad.Right == ButtonState.Pressed)
            {
                if (primary)
                    IncrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Right == ButtonState.Released);
                else
                    IncrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Right == ButtonState.Released);
            }
            else if (last.DPad.Left == ButtonState.Pressed)
            {
                if (primary)
                    DecrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Left == ButtonState.Released);
                else
                    DecrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Left == ButtonState.Released);
            }
            else
            {
                if (primary)
                {
                    holdTimeMainList = 0.0f;
                    isHoldingMainList = false;
                }
                else
                {
                    holdTimeSubList = 0.0f;
                    isHoldingSubList = false;
                }
            }
        }

        private void UpdateVertical(GameTime gameTime, bool primary)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.Y <= -0.15f)
            {
                if (primary)
                    IncrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y > -0.15f);
                else
                    IncrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y > -0.15f);
            }
            else if (last.ThumbSticks.Left.Y >= 0.15f)
            {
                if (primary)
                    DecrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y < 0.15f);
                else
                    DecrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y < 0.15f);
            }
            else if (last.ThumbSticks.Right.Y <= -0.15f)
            {
                if (primary)
                    IncrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y > -0.15f);
                else
                    IncrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y > -0.15f);
            }
            else if (last.ThumbSticks.Right.Y >= 0.15f)
            {
                if (primary)
                    DecrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y < 0.15f);
                else
                    DecrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y < 0.15f);
            }
            else if (last.DPad.Down == ButtonState.Pressed)
            {
                if (primary)
                    IncrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Down == ButtonState.Released);
                else
                    IncrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Down == ButtonState.Released);
            }
            else if (last.DPad.Up == ButtonState.Pressed)
            {
                if (primary)
                    DecrementMainList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Up == ButtonState.Released);
                else
                    DecrementSubList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Up == ButtonState.Released);
            }
            else
            {
                if (primary)
                {
                    holdTimeMainList = 0.0f;
                    isHoldingMainList = false;
                }
                else
                {
                    holdTimeSubList = 0.0f;
                    isHoldingSubList = false;
                }
            }
        }
    }
}
