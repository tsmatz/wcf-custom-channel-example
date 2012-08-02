using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.IO;

namespace Microsoft.Samples.BakaChannelDemo
{
    class BakaChannelListner : ChannelListenerBase<IReplyChannel>
    {
        Uri uri;
        BakaChannel channel = null;
        string fileName;

        public BakaChannelListner(Uri uri)
            : base()
        {
            this.uri = uri;
        }

        // Service が Channel を構築する際に呼びます !  (非常に大事)
        protected override IReplyChannel OnAcceptChannel(TimeSpan timeout)
        {
            if (channel != null)
                return default(IReplyChannel);

            channel = new BakaChannel(this, new EndpointAddress(uri));
            return channel;
        }

        // 同上
        private AsyncAcceptChannel asyncAcceptChannel;
        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            asyncAcceptChannel = new AsyncAcceptChannel(_AcceptChannel);
            return asyncAcceptChannel.BeginInvoke(timeout, callback, state);
        }
        protected delegate IReplyChannel AsyncAcceptChannel(TimeSpan timeout);
        protected IReplyChannel _AcceptChannel(TimeSpan timeout)
        {
            return AcceptChannel(timeout);
        }

        // 同上
        protected override IReplyChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return asyncAcceptChannel.EndInvoke(result);
        }

        public override Uri Uri
        {
            get
            {
                return uri;
            }
        }

        // リスナー作成時の初期化処理はここに記述します
        protected override void OnOpen(TimeSpan timeout)
        {
            // 今回は、メッセージ交換用のファイルを作成！
            fileName = uri.LocalPath.Trim(new char[] { '/' });
            string dirName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dirName))
                throw new FaultException(string.Format("Please create folder : {0} !", dirName));
            File.Create(fileName).Close();
        }

        // リスナー終了時のクリーンアップ処理はここに記述します
        protected override void OnClose(TimeSpan timeout)
        {
            // 作成したファイルを削除
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        protected override void OnAbort()
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
