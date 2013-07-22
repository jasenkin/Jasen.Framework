using System;

namespace Jasen.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum ConnectionOperation
    {
        /// <summary>
        /// 
        /// </summary>
        Open, 

        /// <summary>
        /// 
        /// </summary>
        Close, 

        /// <summary>
        /// 
        /// </summary>
        ChangeDatabase,

        /// <summary>
        /// 
        /// </summary>
        BeginTransaction,

        /// <summary>
        /// 
        /// </summary>
        Commit,

        /// <summary>
        /// 
        /// </summary>
        Rollback
    }



}
