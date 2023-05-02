using SharedRetryProject;
using System.Diagnostics;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [Description("�ő�񐔂܂Ń��g���C����ꍇ")]
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
        [Description("���g���C���Ȃ��ꍇ")]
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
        [Description("���g���C�ő�񐔂ɂȂ�O�ɐ���I������ꍇ")]
        public async Task TestMethod3()
        {
            int invokeCtr = 0;
            int fixCtr = 3;//3��ڂŕ���
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
        [Description("���g���C�b���̊m�F")]
        public async Task TestMethod4()
        {
            int invokeCtr = 0;
            int retryMax = 3;

            var retryer = new Retryer();

            // ���s�J�n�O�ɃX�g�b�v�E�H�b�`���X�^�[�g
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
                    // ���g���C�����ׂďI�������X�g�b�v�E�H�b�`��~
                    sw.Stop();
                });

            // ���g���C��3�񂾂ƁA�������s4��A�Ԃ�10�b�҂���3��Ȃ̂ŁA30�b+�������Ԃ�����͂���30�b�}���b�A�ŏI���͂��Ƒz��
            Debug.WriteLine(sw.Elapsed.ToString());
            Assert.IsTrue(TimeSpan.FromSeconds(30 - 2) < sw.Elapsed && sw.Elapsed < TimeSpan.FromSeconds(30 + 2));
        }

        [TestMethod]
        [Description("�w��ȊO�̗�O�ł̓��g���C�����A��O���X���[���邱�Ƃ̊m�F")]
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
        [Description("���g���C�ő�񐔂ɂȂ�O�ɐ���I������ꍇ(2�̗�O���󂯂���ق��̃��\�b�h)")]
        public async Task TestMethod11()
        {
            int invokeCtr = 0;
            int fixCtr = 6;//3��ڂŕ���
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
        [Description("�w��ȊO�̗�O�ł̓��g���C�����A��O���X���[���邱�Ƃ̊m�F(2�̗�O���󂯂���ق��̃��\�b�h)")]
        public async Task TestMethod12()
        {
            int invokeCtr = 0;
            int fixCtr = 6;//3��ڂŕ���
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