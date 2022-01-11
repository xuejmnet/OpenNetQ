using System;

namespace OpenNetQ.Store.Common.Messages;

/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 11 January 2022 21:25:31
* @Email: 326308290@qq.com
*/
/// <summary>
/// message
/// </summary>
public class Message
{
    private string topic;
    private int flag;
    private IDictionary<string, string> properties;
    private byte[] body;
    private string transactionId;

    public Message()
    {
        
    }

    public Message(string topic,byte[] body):this(topic,string.Empty,string.Empty,0,body,true)
    {
        
    }

    public Message(string topic,string tags,byte[] body):this(topic,tags,string.Empty,0,body,true)
    {
        
    }

    public Message(string topic,string tags,string keys,byte[] body):this(topic,tags,keys,0,body,true)
    {
        
    }
    public Message(string topic, string tags, string keys, int flag, byte[] body, bool waitStoreMsgOk)
    {
        this.topic = topic;
        this.flag = flag;
        this.body = body;
        
    }

    public void SetKeys(string keys)
    {
        this.AddProperty(,keys);
    }
    private void AddProperty(string name,string value)
    {
        if (null == this.properties)
        {
            this.properties = new Dictionary<string, string>();
        }
        this.properties.Add(name,value);
    }

}