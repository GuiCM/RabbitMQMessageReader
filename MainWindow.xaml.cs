using RabbitMQ.Client;
using System.Windows;

namespace RabbitMQMessageReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Connector connector = new Connector();
        private Configurer configurer = new RabbitMQMessageReader.Configurer();
        private Publisher publisher = new Publisher();
        private Consumer consumer;

        string consumerTag;

        private IConnection connection;
        private IModel channel;

        string ackTest = "";
        int ackInvoked = 0;

        string nackTest = ";";
        int nackInvoked = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Configure()
        {
            connection = connector.GetConnection();
            channel = connection.CreateModel();
            channel.BasicQos(0, 2, false);

            configurer.Configure(channel);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Configure();

            MessageBox.Show("Configured");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            new CustomPublisher().Publish(channel, publisher);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //using (IModel channel = connection.CreateModel())
            //{
            consumer = new Consumer(channel, connection.CreateModel());
            consumerTag = consumer.Start();
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (channel.IsOpen)
            {
                channel.Close();
            }

            if (connection.IsOpen)
            {
                connection.Close();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            channel.Close();
        }
    }
}
