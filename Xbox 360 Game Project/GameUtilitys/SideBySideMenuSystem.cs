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
    class SideBySideMenuSystem
    {
        //This is the current selected Item Index.
        private byte selectedPrimaryIndex = 0;
        private byte selectedSecondaryIndex = 0;
        private bool listPrimaryLocation = true;

        private bool UPDOWN_MOVEMENT;
        private byte PRIMARY_MAX_OPTIONS;
        private byte SECONDARY_MAX_OPTIONS;
        private ushort ORDER;

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

        public SideBySideMenuSystem(bool updownDirection, byte p_options, byte s_options, ushort order)
        {
            UPDOWN_MOVEMENT = updownDirection;
            PRIMARY_MAX_OPTIONS = (byte)(p_options - 1);
            SECONDARY_MAX_OPTIONS = (byte)(s_options - 1);
            ORDER = order;
        }

        public void reset()
        {
            selectedPrimaryIndex = 0;
            selectedSecondaryIndex = 0;
            listPrimaryLocation = true;
        }

        public void reset(byte p, byte s)
        {
            selectedPrimaryIndex = p;
            selectedSecondaryIndex = s;
            listPrimaryLocation = true;
        }

        public void resizeList(bool locationEqualPrimary, int size)
        {
            if (locationEqualPrimary)
            {
                PRIMARY_MAX_OPTIONS = (byte)(size - 1);
                if (selectedPrimaryIndex > PRIMARY_MAX_OPTIONS)
                    selectedPrimaryIndex = PRIMARY_MAX_OPTIONS;
            }
            else
            {
                SECONDARY_MAX_OPTIONS = (byte)(size - 1);
                if (selectedSecondaryIndex > SECONDARY_MAX_OPTIONS)
                    selectedSecondaryIndex = SECONDARY_MAX_OPTIONS;
            }
        }

        //Returns the selected item index (Vertical Index).
        public byte PrimaryIndex()
        {
            return selectedPrimaryIndex;
        }

        public byte SecondaryIndex()
        {
            return selectedSecondaryIndex;
        }

        public bool PrimaryLocationSelected()
        {
            return listPrimaryLocation;
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
        private void IncrementPrimaryList(int time, Boolean released)
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
                {
                    ++selectedPrimaryIndex;
                }
            }
            if (holdTime > MOVE_SPEED)
            {
                ++selectedPrimaryIndex;
                isHolding = true;
                holdTime = 0.0f;
            }
            if (selectedPrimaryIndex > PRIMARY_MAX_OPTIONS)
                selectedPrimaryIndex = 0;
        }

        private void IncrementSecondaryList(int time, Boolean released)
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
                    ++selectedSecondaryIndex;
            }
            if (holdTime > MOVE_SPEED)
            {
                ++selectedSecondaryIndex;
                isHolding = true;
                holdTime = 0.0f;
            }
            if (selectedSecondaryIndex > SECONDARY_MAX_OPTIONS)
                selectedSecondaryIndex = 0;
        }

        //This decrements this list.
        private void DecrementPrimaryList(int time, Boolean released)
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
                    --selectedPrimaryIndex;
            }
            if (holdTime > MOVE_SPEED)
            {
                --selectedPrimaryIndex;
                isHolding = true;
                holdTime = 0.0f;
            }
            if (selectedPrimaryIndex == 255)
                selectedPrimaryIndex = PRIMARY_MAX_OPTIONS;
        }

        private void DecrementSecondaryList(int time, Boolean released)
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
                    --selectedSecondaryIndex;
            }
            if (holdTime > MOVE_SPEED)
            {
                --selectedSecondaryIndex;
                isHolding = true;
                holdTime = 0.0f;
            }
            if (selectedSecondaryIndex == 255)
                selectedSecondaryIndex = SECONDARY_MAX_OPTIONS;
        }

        private void ChangeTrinaryList(int time, Boolean released)
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
                {
                    listPrimaryLocation = !listPrimaryLocation;
                    Swap();
                }
            }
            if (holdTime > MOVE_SPEED)
            {
                listPrimaryLocation = !listPrimaryLocation;
                Swap();
                isHolding = true;
                holdTime = 0.0f;
            }
        }

        private void Swap()
        {
            int s_start = ORDER >> 8;
            int p_start = ORDER - (s_start << 8);
            if (s_start == p_start)
                return;
            int s_end;
            int p_end;

            if (s_start > p_start)
            {
                s_end = s_start + PRIMARY_MAX_OPTIONS;
                p_end = p_start + PRIMARY_MAX_OPTIONS;
            }
            else
            {
                s_end = s_start + SECONDARY_MAX_OPTIONS;
                p_end = p_start + SECONDARY_MAX_OPTIONS;
            }

            if (s_end > SECONDARY_MAX_OPTIONS)
                s_end = SECONDARY_MAX_OPTIONS;
            if (p_end > PRIMARY_MAX_OPTIONS)
                p_end = PRIMARY_MAX_OPTIONS;
            if (s_end <= s_start || p_end <= p_start)
                return;
            if (listPrimaryLocation)
            {
                if (s_start > p_start)
                {
                    if (selectedSecondaryIndex >= s_start)
                    {
                        if (selectedSecondaryIndex <= s_end && selectedPrimaryIndex + s_start <= s_end)
                        {
                            selectedPrimaryIndex = (byte)(selectedSecondaryIndex - s_start);
                        } 
                        else
                            selectedPrimaryIndex = (byte)p_end;
                    }
                    else
                    {
                        selectedPrimaryIndex = (byte)p_start;
                    }
                }
            }
            else
            {
                if (s_start > p_start)
                {
                    if (selectedSecondaryIndex >= s_start || selectedPrimaryIndex != p_start) {
                        if (selectedSecondaryIndex <= s_end && selectedPrimaryIndex <= p_end)
                        {
                            selectedSecondaryIndex = (byte)(selectedPrimaryIndex + s_start);
                        }
                    }
                }
                else
                {
                    if (selectedPrimaryIndex >= p_start)
                    {
                        if (selectedPrimaryIndex <= p_end)
                            selectedSecondaryIndex = (byte)(selectedPrimaryIndex - p_start);
                        else
                            selectedSecondaryIndex = (byte)s_end;
                    }
                    else
                    {
                        selectedSecondaryIndex = (byte)s_start;
                    }
                }
            }
            if (selectedSecondaryIndex > SECONDARY_MAX_OPTIONS)
                selectedSecondaryIndex = SECONDARY_MAX_OPTIONS;
            if (selectedPrimaryIndex > PRIMARY_MAX_OPTIONS)
                selectedPrimaryIndex = PRIMARY_MAX_OPTIONS;
        }

        //This decides if the list should be incremented, decrements, or do nothing.
        private void UpdateUpDownMovement(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.Y <= -0.15f)
            {
                if (listPrimaryLocation)
                    IncrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y > -0.15f);
                else
                    IncrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y > -0.15f);
            }
            else if (last.ThumbSticks.Left.Y >= 0.15f)
            {
                if (listPrimaryLocation)
                    DecrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y < 0.15f);
                else
                    DecrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y < 0.15f);
            }
            else if (last.ThumbSticks.Right.Y <= -0.15f)
            {
                if (listPrimaryLocation)
                    IncrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y > -0.15f);
                else
                    IncrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y > -0.15f);
            }
            else if (last.ThumbSticks.Right.Y >= 0.15f)
            {
                if (listPrimaryLocation)
                    DecrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y < 0.15f);
                else
                    DecrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y < 0.15f);
            }
            else if (last.DPad.Down == ButtonState.Pressed)
            {
                if (listPrimaryLocation)
                    IncrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Down == ButtonState.Released);
                else
                    IncrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Down == ButtonState.Released);
            }
            else if (last.DPad.Up == ButtonState.Pressed)
            {
                if (listPrimaryLocation)
                    DecrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Up == ButtonState.Released);
                else
                    DecrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Up == ButtonState.Released);
            }
            else
            {
                UpdateTrinaryLeftRight(gameTime);
            }
        }

        //This decides if the list should be incremented, decrements, or do nothing.
        private void UpdateLeftRightMovement(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.X >= 0.15f)
            {
                if (listPrimaryLocation)
                    IncrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X < 0.15f);
                else
                    IncrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X < 0.15f);
            }
            else if (last.ThumbSticks.Left.X <= -0.15f)
            {
                if (listPrimaryLocation)
                    DecrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X > -0.15f);
                else
                    DecrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X > -0.15f);
            }
            else if (last.ThumbSticks.Right.X >= 0.15f)
            {
                if (listPrimaryLocation)
                    IncrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X < 0.15f);
                else
                    IncrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X < 0.15f);
            }
            else if (last.ThumbSticks.Right.X <= -0.15f)
            {
                if (listPrimaryLocation)
                    DecrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X > -0.15f);
                else
                    DecrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X > -0.15f);
            }
            else if (last.DPad.Right == ButtonState.Pressed)
            {
                if (listPrimaryLocation)
                    IncrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Right == ButtonState.Released);
                else
                    IncrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Right == ButtonState.Released);
            }
            else if (last.DPad.Left == ButtonState.Pressed)
            {
                if (listPrimaryLocation)
                    DecrementPrimaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Left == ButtonState.Released);
                else
                    DecrementSecondaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Left == ButtonState.Released);
            }
            else
            {
                UpdateTrinaryUpDown(gameTime);
            }
        }

        public void UpdateTrinaryLeftRight(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.X >= 0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X < 0.45f);
            }
            else if (last.ThumbSticks.Left.X <= -0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.X > -0.45f);
            }
            else if (last.ThumbSticks.Right.X >= 0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X < 0.45f);
            }
            else if (last.ThumbSticks.Right.X <= -0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.X > -0.45f);
            }
            else if (last.DPad.Right == ButtonState.Pressed)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Right == ButtonState.Released);
            }
            else if (last.DPad.Left == ButtonState.Pressed)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Left == ButtonState.Released);
            }
        }

        public void UpdateTrinaryUpDown(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (last.ThumbSticks.Left.Y <= -0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y > -0.45f);
            }
            else if (last.ThumbSticks.Left.Y >= 0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Left.Y < 0.45f);
            }
            else if (last.ThumbSticks.Right.Y <= -0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y > -0.45f);
            }
            else if (last.ThumbSticks.Right.Y >= 0.45f)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.ThumbSticks.Right.Y < 0.45f);
            }
            else if (last.DPad.Down == ButtonState.Pressed)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Down == ButtonState.Released);
            }
            else if (last.DPad.Up == ButtonState.Pressed)
            {
                ChangeTrinaryList(gameTime.ElapsedGameTime.Milliseconds, current.DPad.Up == ButtonState.Released);
            }
        }

    }
}