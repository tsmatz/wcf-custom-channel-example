using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.BakaChannelDemo
{
    public class BakaBindingElement : TransportBindingElement
    {
        public override string Scheme
        {
            get {
                return "baka.file";
            }
        }

        public override BindingElement Clone()
        {
            BakaBindingElement clone = new BakaBindingElement();
            return clone;
        }

        // Request-Reply 型の通信のみサポート
        // (Input/Output, Duplex, Session, などはサポートしない !)
        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if ((typeof(TChannel) != typeof(IRequestChannel)) &&
                (typeof(TChannel) != typeof(IReplyChannel)))
                return false;

            return true;
        }

        // Request-Reply 型の通信のみサポート
        // (Input/Output, Duplex, Session, などはサポートしない !)
        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if( (typeof(TChannel) != typeof(IRequestChannel)) &&
                (typeof(TChannel) != typeof(IReplyChannel)) )
                return false;

            return true;
        }

        // SampleClient が Factory をつかいます !
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            BakaChannelFactory factory = new BakaChannelFactory();
            return (IChannelFactory<TChannel>)factory;
        }

        // SampleService が Listener を使います !
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            BakaChannelListner listener = new BakaChannelListner(new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress));
            return (IChannelListener<TChannel>)listener;
        }

    }

}
