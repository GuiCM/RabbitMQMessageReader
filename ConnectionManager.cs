using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace RabbitMQMessageReader
{
    public class ConnectionManager
    {
        private static IConnection Connection;

        private static readonly List<IModel> OpenChannels = new();

        public static void OpenConnection(string host, int port, string user, string password)
        {
            ConnectionFactory connectionFactory = new()
            {
                HostName = host,
                UserName = user,
                Password = password,
                Port = port,
            };

            Connection = connectionFactory.CreateConnection();
        }

        public static bool IsConnectionOpen()
        {
            return Connection != null && Connection.IsOpen;
        }

        public static IModel CreateChannel()
        {
            if (IsConnectionOpen())
            {
                IModel channel = Connection.CreateModel();
                channel.ModelShutdown += (e, a) => OpenChannels.RemoveAll(channel => channel.IsClosed);

                OpenChannels.Add(channel);

                return channel;
            }

            throw new Exception("Connection is not opened.");
        }

        public static void CloseConnection()
        {
            if (IsConnectionOpen())
            {
                Connection.Close();
                Connection = null;
            }
        }

        public static bool HasOpenedChannels()
        {
            return OpenChannels.Any(channel => channel.IsOpen);
        }

        public static bool CloseChannels()
        {
            for (int i = 0; i < OpenChannels.Count; i++)
            {
                if (OpenChannels[i].IsOpen)
                {
                    try
                    {
                        OpenChannels[i].Close();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}