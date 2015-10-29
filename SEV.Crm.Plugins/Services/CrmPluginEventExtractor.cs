using System;
using Microsoft.Xrm.Sdk;
using SEV.Crm.Business;
using SEV.Crm.Plugins.Properties;

namespace SEV.Crm.Services
{
    internal class CrmPluginEventExtractor : ICrmPluginEventExtractor
    {
        public CrmPluginEvent GetPluginEvent(IPluginExecutorContext executorContext)
        {
            IPluginExecutionContext context = executorContext.PluginExecutionContext;

            var stage = (CrmEventStage)context.Stage;
            string pluginEventText = string.Concat(stage.ToString(), context.MessageName);

            CrmPluginEvent pluginEvent;
            if (Enum.TryParse(pluginEventText, out pluginEvent))
            {
                return pluginEvent;
            }
            throw new InvalidCastException(String.Format(Resources.PluginEventExtractorError, pluginEventText));
        }
    }
}
