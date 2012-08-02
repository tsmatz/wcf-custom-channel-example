using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.BakaChannelDemo
{
    class BakaChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        protected override IRequestChannel OnCreateChannel(System.ServiceModel.EndpointAddress address, Uri via)
        {
            BakaChannel channel = new BakaChannel(this, address);
            return channel;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            // 何もしない . . .
            // (Factory 構築時の初期化処理などは、ここに記述します . . .)
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

    }
}
