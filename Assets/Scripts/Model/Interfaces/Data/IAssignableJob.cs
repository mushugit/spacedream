﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IAssignableJob : IJob
    {
        void Execute();
        void Assign();
        void Unassign();
    }
}
