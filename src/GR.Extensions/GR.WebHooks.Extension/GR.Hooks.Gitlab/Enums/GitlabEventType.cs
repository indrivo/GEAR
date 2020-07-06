using System.Runtime.Serialization;

namespace GR.Hooks.Gitlab.Enums
{
    public enum GitlabEventType
    {
        Unknown,

        [EnumMember(Value = "Push Hook")]
        Push,

        [EnumMember(Value = "Tag Push Hook")]
        TagPush,

        [EnumMember(Value = "Issue Hook")]
        Issue,

        [EnumMember(Value = "Note Hook")]
        Note,

        [EnumMember(Value = "Merge Request Hook")]
        MergeRequest,

        [EnumMember(Value = "Wiki Page Hook")]
        WikiPage,

        [EnumMember(Value = "Pipeline Hook")]
        Pipeline,

        [EnumMember(Value = "Job Hook")]
        Job
    }
}