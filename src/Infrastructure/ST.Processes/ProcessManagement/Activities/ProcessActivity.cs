using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace ST.Procesess.ProcessManagement.Activities
{
    public class ProcessActivity : Activity
    {
        public override void Run(ExecutionContext context)
        {
            Console.WriteLine("Running activity 1. Is it working?");
            ProcessManagerHolder.Instance.HandleActivityCompletion(
                   context.Instance.Id,
                   context.Token.Id,
                   null
               );
            //or
            ProcessManagerHolder.Instance.HandleActivityFailure(
                context.Instance.Id,
                context.Token.Id,
                null
              );
        }
    }
}
