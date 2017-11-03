namespace AleFIT.Workflow.Core
{
    /// <summary>
    /// Defines a state in which execution of workflow is.
    /// </summary>
    public enum ExecutionState
    {
        /// <summary>
        /// Execution is running.
        /// </summary>
        Running = 1,

        /// <summary>
        /// Execution has been paused.
        /// </summary>
        Paused = 2,

        /// <summary>
        /// Execution has successfully competed.
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Execution has failed.
        /// </summary>
        Failed = 4
    }
}
