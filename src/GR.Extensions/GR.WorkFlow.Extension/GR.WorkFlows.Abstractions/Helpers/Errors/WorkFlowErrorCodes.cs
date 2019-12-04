namespace GR.WorkFlows.Abstractions.Helpers.Errors
{
    public class WorkFlowErrorCodes
    {
        public const string GRWF_0x100 = "Transitions are not allowed, where the starting and ending states are the same";
        public const string GRWF_0x101 = "No more than one transition in one direction is allowed";
        public const string GRWF_0x102 = "This state cannot be deleted because it is used by an entity";
        public const string GRWF_0x103 = "This workflow has at least one contract registered with one entity, delete this contract and then you can delete workflow";
        public const string GRWF_0x104 = "State name is used, choose another state name";
        public const string GRWF_0x105 = "The starting state cannot be set to end";
        public const string GRWF_0x106 = "The end state cannot be set from the beginning";
    }
}
