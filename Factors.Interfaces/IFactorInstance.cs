﻿namespace Factors.Interfaces
{
    public interface IFactorInstance
    {
        string UserAccount { get; set; }

        IFactorConfiguration Configuration { get; set; }
    }
}
