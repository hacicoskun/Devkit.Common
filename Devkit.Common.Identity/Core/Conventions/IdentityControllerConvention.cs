using Devkit.Common.Identity.Controllers;
using Devkit.Common.Identity.Options;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Devkit.Common.Identity.Core.Conventions
{
    public class IdentityControllerConvention(IdentityOptions options) : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        { 
            if (!options.EnableAuthApi)
            {
                var controller = application.Controllers.FirstOrDefault(c => c.ControllerType == typeof(AuthController));
                if (controller != null)
                {
                    application.Controllers.Remove(controller);
                }
            }

            if (options.EnableUserApi) return;
            {
                var controller = application.Controllers.FirstOrDefault(c => c.ControllerType == typeof(UserController));
                if (controller != null)
                {
                    application.Controllers.Remove(controller);
                }
            }
        }
    }
}