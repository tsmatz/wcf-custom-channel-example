using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.Xml;

namespace Microsoft.Samples.BakaChannelDemo
{
    // RequestContext はリクエスト交換をおこなう際に必要になります .
    // Service - Client のメッセージ交換ごとに毎回 1 つずつ作成される、
    // メッセージ交換のための仮想的な仲介オブジェクトです
    class BakaRequestContext : RequestContext
    {
        Message message;
        TimeSpan defaultTimeout;
        string filePath;

        public BakaRequestContext(Message msg, string path, TimeSpan timeout)
        {
            this.message = msg;
            filePath = path;
            this.defaultTimeout = timeout;
        }

        // Client が Request をおこなう際に使用されます
        public override Message RequestMessage
        {
            get 
            {
                return this.message;
            }
        }

        // Service が Response を返す際に使用されます
        public override void Reply(Message message, TimeSpan timeout)
        {
            // Reply を渡す (file でバカ通信)
            // (Note !! 実際にはちゃんと Lock をおこなうこと !)
            string action, body;
            action = message.Headers.Action;
            XmlDictionaryReader xmlDicReader = message.GetReaderAtBodyContents();
            body = xmlDicReader.ReadOuterXml();
            using(Stream stream = File.Create(filePath + "_Response"))
            using(StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(action);
                writer.WriteLine(body);
            }
        }

        public override void Reply(Message message)
        {
            this.Reply(message, defaultTimeout);
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void EndReply(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

    }
}
