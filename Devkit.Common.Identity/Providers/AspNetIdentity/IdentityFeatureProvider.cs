using Devkit.Common.Identity.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Devkit.Common.Identity.Core.Features
{
    public class IdentityFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly bool _enableAuthApi;
        private readonly bool _enableUserApi;

        public IdentityFeatureProvider(bool enableAuthApi, bool enableUserApi)
        {
            _enableAuthApi = enableAuthApi;
            _enableUserApi = enableUserApi;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (!_enableAuthApi)
            {
                var controller = feature.Controllers.FirstOrDefault(c => c.AsType() == typeof(AuthController));
                if (controller != null) feature.Controllers.Remove(controller);
            }

            if (!_enableUserApi)
            {
                var controller = feature.Controllers.FirstOrDefault(c => c.AsType() == typeof(UserController));
                if (controller != null) feature.Controllers.Remove(controller);
            }
        }
    }
}