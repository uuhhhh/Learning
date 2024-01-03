﻿using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity;

public partial class WallDetector : EnvObjectDetector
{
    protected override int GetPriorityOf(EnvObject envObject)
    {
        return envObject.Data.WallPriority;
    }
}