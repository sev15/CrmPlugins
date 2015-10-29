
namespace SEV.Crm.Business
{
    internal enum CrmEventStage
    {
        PreValidate = 10,
        Pre = 20,
        Post = 40,
    }

    public enum CrmPluginEvent
    {
        None,
        PreValidateCreate,
        PreCreate,
        PostCreate,
        PreValidateUpdate,
        PreUpdate,
        PostUpdate,
        PreValidateDelete,
        PreDelete,
        PostDelete,
    }
}
