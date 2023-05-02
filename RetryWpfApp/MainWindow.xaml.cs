using SharedRetryProject;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace RetryWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int cnt = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// リトライ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            cnt = 0;
            var retryer = new Retryer();

            await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(5, TimeSpan.FromSeconds(1), 
                async () =>
                {
                    await ErrorFunc();
                },
                async () =>
                {
                    Debug.WriteLine("リトライ回数超えました。");
                });
        }

        private async void Button_Click2(object sender, RoutedEventArgs e)
        {
            cnt = 0;
            var retryer = new Retryer();

            try
            {
                await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(5, TimeSpan.FromSeconds(1),
                    async () =>
                    {
                        await ErrorFunc();
                    },
                    async () =>
                    {
                        Debug.WriteLine("リトライ回数超えました。");
                    },
                    true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("画面側のtrycatch");
            }

        }


        private async Task ErrorFunc()
        {
            Debug.WriteLine($" 処理 {cnt++} 回目");

            if (cnt < 10)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
