using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.BakaChannelDemo
{
    // このクラスは、構成定義 (.config) で使用されます
    class BakaBindingElementExtensionElement : BindingElementExtensionElement
    {
        public BakaBindingElementExtensionElement() { }

        public override Type BindingElementType
        {
            get
            {
                return typeof(BakaBindingElement);
            }
        }

        protected override BindingElement CreateBindingElement()
        {
            BakaBindingElement bindingElement = new BakaBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }
    }
}
