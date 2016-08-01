using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace SCMS.ModelBinders
{
    class CustomModelBinder: System.Web.Mvc.DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var holderType = bindingContext.ModelMetadata.ContainerType;
            if (holderType != null)
            {
                var propertyType = holderType.GetProperty(bindingContext.ModelMetadata.PropertyName);
                var attributes = propertyType.GetCustomAttributes(true);
                var hasAttribute = attributes
                  .Cast<Attribute>()
                  .Any(a => a.GetType().IsEquivalentTo(typeof(BLL.Entities.AllowHtmlAttribute)));
                if (hasAttribute)
                {
                    bindingContext.ModelMetadata.RequestValidationEnabled = false;
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
