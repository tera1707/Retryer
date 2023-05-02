using System;
using System.Threading.Tasks;

namespace SharedRetryProject
{
    public class Retryer
    {
        private int invokeCnt = 0;

        /// <summary>
        /// リトライ処理
        /// </summary>
        /// <typeparam name="T">
        /// retryTarget実施時にリトライさせるべき例外(T以外の例外は、リトライせず普通にthrowする)
        /// ※「Exception」を指定すると、何の例外でもリトライする
        /// </typeparam>
        /// <param name="retryMax">リトライ最大回数(合計実行回数は「最初の1回＋リトライ最大回数」になる)</param>
        /// <param name="interval">リトライ前に待つ時間</param>
        /// <param name="retryTarget">対象のアクション</param>
        /// <param name="onRetryOut">リトライ最大回数分リトライしても正常終了しなかった場合に行うアクション</param>
        /// <param name="throwWhenRetryOut">trueにすると、onRetryOut時に、例外をそのまま上にthrowする</param>
        /// <returns></returns>
        public async Task TryAndThrowWhenRetryOut<T>(int retryMax, TimeSpan interval, Func<Task> retryTarget, Func<Task> onRetryOut, bool throwWhenRetryOut = false)
            where T : Exception
        {
            while (true)
            {
                try
                {
                    await retryTarget.Invoke();
                    break;
                }
                catch (T)
                {
                    if (invokeCnt++ >= retryMax)
                    {
                        await onRetryOut.Invoke();

                        if (throwWhenRetryOut)
                            throw;
                        else
                            break;
                    }
                }
                await Task.Delay(interval);
            }
        }

        /// <summary>
        /// リトライ処理(リトライさせる例外を2つ指定できる版)
        /// </summary>
        public async Task TryAndThrowWhenRetryOut<T1, T2>(int retryMax, TimeSpan interval, Func<Task> retryTarget, Func<Task> onRetryOut, bool throwWhenRetryOut = false)
            where T1 : Exception
            where T2 : Exception
        {
            while (true)
            {
                try
                {
                    await retryTarget.Invoke();
                    break;
                }
                catch (Exception ex) when (ex is T1 || ex is T2)
                {
                    if (invokeCnt++ >= retryMax)
                    {
                        await onRetryOut.Invoke();

                        if (throwWhenRetryOut)
                            throw;
                        else
                            break;
                    }
                }
                await Task.Delay(interval);
            }
        }
    }
}
