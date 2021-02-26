﻿using System;
using UnityEngine;

namespace Jumps 
{
    public interface IJumpable
    {
        Vector3 Jump();
    }

    /**
     * E.G jump implementation example
     */
    public class JumpBase : IJumpable
    {
        public Vector3 Jump()
        {
            // TODO: store it in const instead of hardcoding it
            return new Vector3(0, (float) Math.Sqrt(2f * -2f * -9.81f), 0);
        }
    }

    public class JumpDouble : IJumpable
    {
        public Vector3 Jump()
        {
            return new Vector3(0, (float) Math.Sqrt(2f * -2f * -9.81f), 0);
        }
    }

    public class JumpNone : IJumpable
    {
        public Vector3 Jump()
        {
            return new Vector3(0, 0, 0);
        }
    }
}