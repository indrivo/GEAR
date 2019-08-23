using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ST.TaskManager.Abstractions.Helpers
{
    /// <summary>
    /// Task module exception messages
    /// </summary>
    public enum ExceptionMessagesEnum
    {
        [Description("Task Not Found")] TaskNotFound,

        [Description("Task is null")] NullTask,

        [Description("Parameter is null")] NullParameter,

        [Description("There was a error on saving Task ")]
        TaskNotSaved,
    }
}
