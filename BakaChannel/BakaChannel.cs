using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.Samples.BakaChannelDemo
{
    class BakaChannel : ChannelBase, IReplyChannel, IRequestChannel, IChannel, ICommunicationObject
    {
        EndpointAddress address;
        string fileName;
        FileSystemWatcher watcher;
        bool fileChanged = false;

        public BakaChannel(ChannelManagerBase channelManager, EndpointAddress address)
            : base(channelManager)
        {
            this.address = address;
            this.fileName = address.Uri.LocalPath.Trim(new char[] {'/'});
        }

        // Service から呼ばれます
        // FileSystemWatcher クラスを使って監視します (ここは頭が良い)
        protected override void OnOpen(TimeSpan timeout)
        {
            watcher = new FileSystemWatcher(Path.GetDirectoryName(fileName),
                Path.GetFileName(fileName));
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;
        }

        // Service が Close する際に呼ばれます
        protected override void OnClose(TimeSpan timeout)
        {
            base.OnClosed();
        }

        // FileWatcher クラスのイベントハンドラです (少し良くないコード)
        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Note !!!
            // これは Sample です.
            // (Synchronize のための Lock をおなうよう修正しましょう !)
            fileChanged = true;
            Thread.Sleep(500);
        }

        protected override void OnAbort()
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        #region IReplyChannel メンバ

        protected delegate bool AsyncTryReceiveRequest(TimeSpan timeout, out RequestContext context);
        private AsyncTryReceiveRequest asyncTryReceiveRequest;

        // Service が Request の到着を監視する目的で使います
        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            asyncTryReceiveRequest = new AsyncTryReceiveRequest(TryReceiveRequest);
            RequestContext context;
            return asyncTryReceiveRequest.BeginInvoke(timeout, out context, callback, state);
        }

        // 同上
        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            context = null;
            Thread.Sleep(500);

            // Close のときは監視を終了！
            if (base.State != CommunicationState.Opened)
                return true;

            // ファイルをバカチェックして取得
            // (Note !! 実際にはちゃんと Lock をおこなうこと !)
            if(!fileChanged)
                return false;

            fileChanged = false;
            FileInfo fileinfo = new FileInfo(fileName);
            if (!(fileinfo.Length > 0))
                return false;

            string action, body;
            using (StreamReader reader = new StreamReader(fileName))
            {
                action = reader.ReadLine();
                body = reader.ReadToEnd();
            }
            XmlReader bodyreader = XmlReader.Create(new StringReader(body));
            Message msg = Message.CreateMessage(MessageVersion.Default, action, bodyreader);
            msg.Headers.To = address.Uri;

            context = new BakaRequestContext(msg, fileName, timeout);

            File.Delete(fileName);
            File.Create(fileName).Close();
            fileChanged = false;

            return true;
        }

        // 同上
        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            return asyncTryReceiveRequest.EndInvoke(out context, result);
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public EndpointAddress LocalAddress
        {
            get { throw new NotImplementedException(); }
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public RequestContext ReceiveRequest()
        {
            throw new NotImplementedException();
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRequestChannel メンバ

        // Client から Request を発行する際に呼ばれます
        public Message Request(Message message, TimeSpan timeout)
        {
            // メッセージを送信 (file でバカ通信)
            // (Note !! 実際にはちゃんと Lock をおこなうこと !)
            string action, body;
            action = message.Headers.Action;
            XmlDictionaryReader xmlDicReader = message.GetReaderAtBodyContents();
            body = xmlDicReader.ReadOuterXml();
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(action);
                writer.WriteLine(body);
            }

            // メッセージを受信 (file でバカ受信)
            Thread.Sleep(3000);     // 4 秒待って回答が来なければ失敗 !!??
            string actionResponse, bodyResponse;
            using (StreamReader streamReader = new StreamReader(fileName + "_Response"))
            {
                actionResponse = streamReader.ReadLine();
                bodyResponse = streamReader.ReadToEnd();
            }
            XmlReader xmlReader = XmlReader.Create(new StringReader(bodyResponse));
            Message messageResponse = Message.CreateMessage(MessageVersion.Default, actionResponse, xmlReader);
            File.Delete(fileName + "_Response");

            return messageResponse;
        }

        public Message Request(Message message)
        {
            return this.Request(message, base.DefaultSendTimeout);
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return this.address;
            }
        }

        public Uri Via
        {
            get
            {
                return this.address.Uri;
            }
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public Message EndRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
