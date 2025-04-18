﻿using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEngine;

namespace ModularBuildingsFramework
{
	public interface ILengthScalable 
	{
        /// ======================================================================

        float CalculateLengthScale(float length);

        /// ======================================================================
    }
}