﻿namespace CommonAPI
{
    /// <summary>
    /// Allow this factory system to receive update calls
    /// </summary>
    public interface IPreUpdate
    {
        /// <summary>
        /// This call will happen before main factory update 
        /// </summary>
        void PreUpdate();
    }
}