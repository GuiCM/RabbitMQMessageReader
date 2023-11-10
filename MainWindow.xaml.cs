using RabbitMQ.Client;
using RabbitMQMessageReader.Consumer;
using RabbitMQMessageReader.Consumer.Event;
using RabbitMQMessageReader.Consumer.Manager;
using RabbitMQMessageReader.Publisher;
using System;
using System.Text;
using System.Windows;

namespace RabbitMQMessageReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionManager.HasOpenedChannels())
            {
                MessageBoxResult result = MessageBox.Show("Existem canais ainda abertos", "There are channels still opened", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    if (!ConnectionManager.CloseChannels())
                    {
                        MessageBoxResult forceResult = MessageBox.Show("Falha ao fechar os canais abertos, forçar fechamento?", "Falha ao fechar canais abertos", MessageBoxButton.YesNo);

                        if (forceResult == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            string host = txtBoxHost.Text;
            string port = txtBoxPort.Text;
            string user = txtBoxUser.Text;
            string password = txtBoxPassword.Text;

            try
            {
                ConnectionManager.CloseConnection();

                ConnectionManager.OpenConnection(host, Convert.ToInt32(port), user, password);

                txtBlockConnectionLog.Text = $"Conexão aberta! host: {host}, porta: {port}, usuário: {user}, senha: {password}";

                try
                {
                    using IModel channel = ConnectionManager.CreateChannel();

                    ConfigurationManager.Configure(channel);
                }
                catch (Exception configurationException)
                {
                    MessageBox.Show("Fail to configure exchanges and bindings, " + configurationException.Message);
                }
            }
            catch (Exception connectionException)
            {
                txtBlockConnectionLog.Text = "Fail to connect. " + connectionException.Message;
            }
        }

        private void BtnPublishTestMessage_Click(object sender, RoutedEventArgs e)
        {
            using IModel channel = ConnectionManager.CreateChannel();
            TestPublisher publisher = new(channel);

            publisher.PublishTestMessage(10);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ConnectionManager.HasOpenedChannels())
            {
                MessageBoxResult result = MessageBox.Show("Existem canais ainda abertos", "There are channels still opened", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    if (!ConnectionManager.CloseChannels())
                    {
                        MessageBoxResult forceResult = MessageBox.Show("Falha ao fechar os canais abertos, forçar encerramento?", "Falha ao fechar canais abertos", MessageBoxButton.YesNo);

                        if (forceResult == MessageBoxResult.No)
                        {
                            e.Cancel = true;
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }

            ConnectionManager.CloseConnection();
        }

        private void BtnReadMessages_Click(object sender, RoutedEventArgs e)
        {
            //CustomDialogBox messagesCountDialogBox = new CustomDialogBox("Quantidade de mensagens a ler");
            //messagesCountDialogBox.ShowDialog();
            // if (messagesCountDialogBox.ShowDialog() == true)
            /*{
                int messagesCount = Convert.ToInt32(messagesCountDialogBox.ResponseText); */

            IModel channel = ConnectionManager.CreateChannel();
            //IModel secondChannel = ConnectionManager.CreateChannel();
            IModel publisherChannel = ConnectionManager.CreateChannel();
            //IModel secondPublisherChannel = ConnectionManager.CreateChannel();

            Consumer.Consumer readConsumer = new Consumer.Consumer(channel, publisherChannel, new ConsumerOptions() { PrefetchCount = 2, QueueName = "test-deadletter-queue", ReadAmount = 5 });
            Consumer.Consumer secondReadConsumer = new Consumer.Consumer(channel, publisherChannel, new ConsumerOptions() { PrefetchCount = 2, QueueName = "test-deadletter-queue", ReadAmount = 5 });

            /*ConsumerManager consumerManager = new ConsumerManager(new ConsumerManagerOptions()
            {
                messageCount = 10,
                prefetchCount = 2,
                preferredConsumerCount = 1,
                queueName = "test-deadletter-queue"
            }, ProcessFinishConsuming);*/

            IConsumerManager valuedConsumerManager = new ValuedConsumerManager();
            valuedConsumerManager.ConsumersFinished += ValuedConsumerManager_ConsumersFinished;

            valuedConsumerManager.AddConsumer(readConsumer);
            valuedConsumerManager.AddConsumer(secondReadConsumer);
            
            valuedConsumerManager.StartConsumers();
            //}
        }

        private void ValuedConsumerManager_ConsumersFinished(object sender, ConsumersFinishedEventArgs e)
        {
            string result = string.Empty;

            foreach (byte[] item in e.Result)
            {
                result += Encoding.UTF8.GetString(item) + "\n";
            }

            MessageBox.Show(result);
        }

        private void BtnCancelConsumer_Click(object sender, RoutedEventArgs e)
        {
            // channel.BasicCancel(consumerTag);
        }
    }
}
