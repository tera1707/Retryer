using SharedRetryProject;
using System.Diagnostics;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [Description("最大回数までリトライする場合")]
        public async Task TestMethod1()
        {
            int invokeCtr = 0;
            int retryMax = 5;
            bool executedOnRetryOut = false;

            var retryer = new Retryer();

            await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(retryMax, TimeSpan.FromSeconds(1),
                async () =>
                {
                    invokeCtr++;
                    throw new InvalidOperationException();
                },
                async () =>
                {
                    executedOnRetryOut = true;
                });

            Assert.AreEqual(retryMax + 1, invokeCtr);
            Assert.AreEqual(true, executedOnRetryOut);
        }

        [TestMethod]
        [Description("リトライがない場合")]
        public async Task TestMethod2()
        {
            int invokeCtr = 0;
            int retryMax = 5;
            bool executedOnRetryOut = false;

            var retryer = new Retryer();

            await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(retryMax, TimeSpan.FromSeconds(1),
                async () =>
                {
                    invokeCtr++;
                },
                async () =>
                {
                    executedOnRetryOut = true;
                });

            Assert.AreEqual(1, invokeCtr);
            Assert.AreEqual(false, executedOnRetryOut);
        }

        [TestMethod]
        [Description("リトライ最大回数になる前に正常終了する場合")]
        public async Task TestMethod3()
        {
            int invokeCtr = 0;
            int fixCtr = 3;//3回目で復活
            int retryMax = 5;
            bool executedOnRetryOut = false;

            var retryer = new Retryer();

            await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(retryMax, TimeSpan.FromSeconds(1),
                async () =>
                {
                    invokeCtr++;

                    if (invokeCtr < fixCtr)
                        throw new InvalidOperationException();
                },
                async () =>
                {
                    executedOnRetryOut = true;
                });

            Assert.AreEqual(fixCtr, invokeCtr);
            Assert.AreEqual(false, executedOnRetryOut);
        }

        [TestMethod]
        [Description("リトライ秒数の確認")]
        public async Task TestMethod4()
        {
            int invokeCtr = 0;
            int retryMax = 3;

            var retryer = new Retryer();

            // 実行開始前にストップウォッチをスタート
            Stopwatch sw = new Stopwatch();
            sw.Start();

            await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(retryMax, TimeSpan.FromSeconds(10),
                async () =>
                {
                    invokeCtr++;
                    throw new InvalidOperationException();
                },
                async () =>
                {
                    // リトライがすべて終わったらストップウォッチ停止
                    sw.Stop();
                });

            // リトライが3回だと、処理実行4回、間に10秒待ちが3回なので、30秒+処理時間かかるはず→30秒±数秒、で終わるはずと想定
            Debug.WriteLine(sw.Elapsed.ToString());
            Assert.IsTrue(TimeSpan.FromSeconds(30 - 2) < sw.Elapsed && sw.Elapsed < TimeSpan.FromSeconds(30 + 2));
        }

        [TestMethod]
        [Description("指定以外の例外ではリトライせず、例外をスローすることの確認")]
        public async Task TestMethod5()
        {
            int invokeCtr = 0;
            int retryMax = 5;
            bool executedOnRetryOut = false;

            var retryer = new Retryer();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await retryer.TryAndThrowWhenRetryOut<InvalidOperationException>(retryMax, TimeSpan.FromSeconds(1),
                    async () =>
                    {
                        invokeCtr++;
                        throw new ArgumentNullException();
                    },
                    async () =>
                    {
                        executedOnRetryOut = true;
                    });
            });

            Assert.AreEqual(1, invokeCtr);
            Assert.AreEqual(false, executedOnRetryOut);
        }

        [TestMethod]
        [Description("リトライ最大回数になる前に正常終了する場合(2つの例外を受けられるほうのメソッド)")]
        public async Task TestMethod11()
        {
            int invokeCtr = 0;
            int fixCtr = 6;//3回目で復活
            int retryMax = 10;
            bool executedOnRetryOut = false;

            var retryer = new Retryer();

            await retryer.TryAndThrowWhenRetryOut<InvalidOperationException, ArgumentNullException>(retryMax, TimeSpan.FromSeconds(1),
                async () =>
                {
                    invokeCtr++;

                    if (invokeCtr < fixCtr)
                    {
                        if (invokeCtr % 2 == 0)
                            throw new InvalidOperationException();
                        else
                            throw new ArgumentNullException();
                    }
                },
                async () =>
                {
                    executedOnRetryOut = true;
                });

            Assert.AreEqual(fixCtr, invokeCtr);
            Assert.AreEqual(false, executedOnRetryOut);
        }


        [TestMethod]
        [Description("指定以外の例外ではリトライせず、例外をスローすることの確認(2つの例外を受けられるほうのメソッド)")]
        public async Task TestMethod12()
        {
            int invokeCtr = 0;
            int fixCtr = 6;//3回目で復活
            int retryMax = 10;
            bool executedOnRetryOut = false;

            var retryer = new Retryer();

            await Assert.ThrowsExceptionAsync<DivideByZeroException>(async () =>
            {
                await retryer.TryAndThrowWhenRetryOut<InvalidOperationException, ArgumentNullException>(retryMax, TimeSpan.FromSeconds(1),
                    async () =>
                    {
                        invokeCtr++;

                        throw new DivideByZeroException();
                    },
                    async () =>
                    {
                        executedOnRetryOut = true;
                    });
            });

            Assert.AreEqual(1, invokeCtr);
            Assert.AreEqual(false, executedOnRetryOut);
        }
    }
}