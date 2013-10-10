using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelEstimation.tools;

namespace ModelEstimation
{
    /// <summary>
    /// Этот модуль подбирает параметры модели случайного процесса,
    /// который в каждой точке имеет нормальное распределение
    /// со мат. ожиданием и СКО, определяемыми двумя произвольными функциями.
    /// Функции для вычисления мат.ожидания и СКО передаются в качестве параметров
    /// вместе с выборкой данных.
    /// 
    /// f(x[], c[]) = N(m(x[],c[]),s(x[],c[]))
    /// 
    /// Модуль должен подбирать как все параметры c[] одновременно, 
    /// так и некоторые из них, при известных остальных
    /// </summary>
    public class MLEstimator
    {
        /// <summary>
        /// Индикатор, выводить ли в консоль дополнительную информацию о ходе решения
        /// </summary>
        private bool debugInfo = false;

        /// <summary>
        /// Произвольная функция, которая моделирует среднее значение случайной величины (времени выполнения)
        /// </summary>
        /// <param name="c">массив параметров - переменные, по которым идёт оптимизация</param>
        /// <param name="x">массив - вектор аргументов функции в одной точке (заданные параметры для одного запуска)</param>
        /// <returns>математическое ожидание в заданной точке x</returns>
        public delegate double meanFunction(double[] c, double[] x);
        /// <summary>
        /// Переменная для хранения делегируемой функции математического ожидания
        /// </summary>
        private meanFunction mean;

        /// <summary>
        /// Произвольная функция, которая моделирует СКО случайной величины (отклонение во времени выполнения)
        /// </summary>
        /// <param name="c">массив параметров - переменные, по которым идёт оптимизация</param>
        /// <param name="x">массив - вектор аргументов функции в одной точке (заданные параметры для одного запуска)</param>
        /// <returns>СКО в заданной точке x</returns>
        public delegate double sigmaFunction(double[] c, double[] x);
        /// <summary>
        /// Переменная для хранения делегируемой функции СКО
        /// </summary>
        private sigmaFunction sigma;

        /// <summary>
        /// Выборка данных
        /// Подразумевается, если размер матрицы n*m, то:
        /// n - размер выборки (количество экспериментов);
        /// m-1 - количество аргументов x,
        /// т.е. столбцы 0..m-2 - аргументы, m-1 - значение функции (время выполнения).
        /// </summary>
        private double[,] data;

        /// <summary>
        /// n - размер выборки;
        /// m-1 - количество аргументов функции;
        /// k - количество оптимизируемых параметров;
        /// </summary>
        private int n, m, k;

        /// <summary>
        /// Толерантность методов при вычислениях
        /// </summary>
        private double eps = 1.0e-12;


        ///// <summary>
        ///// Логарифмическая функция правдоподобия,
        ///// используется для оптимизации
        ///// </summary>
        ///// <param name="c">параметры оптимизации</param>
        ///// <param name="L">значение ЛФП</param>
        ///// <param name="obj">доп. объект для alglib</param>
        //public void likelihood(double[] c, ref double L, object obj)
        //{
        //    L = 0;

        //    double[] x = new double[m - 1];
        //    double y, temp, sig;
        //    // параметры оптимизации
        //    double[] a = p.Take(k).ToArray();
        //    double[] b = p.Skip(k).Take(l).ToArray();

        //    // сумма по всем элементам выборки
        //    for (int i = 0; i < n; i++)
        //    {
        //        // копируем строку из data в массив
        //        System.Buffer.BlockCopy(data, sizeof(double) * i * m, x, 0, sizeof(double) * (m - 1));
        //        y = data[i, m - 1];

        //        // считаем ЛФП
        //        sig = sigma(b, x);
        //        sig = sig < eps ? eps : sig;
        //        temp = (mean(a, x) - y) / sig;
        //        L += Math.Log(sig) + temp * temp / 2;
        //    }

        //}

        public double likelihood(double[] c)
        {
            double L = 0;

            double[] x = new double[m - 1];
            double y, temp, sig;

            // сумма по всем элементам выборки
            for (int i = 0; i < n; i++)
            {
                // копируем строку из data в массив x
                System.Buffer.BlockCopy(data, sizeof(double) * i * m, x, 0, sizeof(double) * (m - 1));
                y = data[i, m - 1];

                // считаем ЛФП
                sig = sigma(c, x);
                if (sig > eps)
                {
                    temp = (mean(c, x) - y) / sig;
                    L += Math.Log(sig) + temp * temp / 2;
                }
                else // граничные условия sigma > 0
                {
                    temp = (mean(c, x) - y) / eps * (1 + 100 * (eps - sig) );
                    L += Math.Log(eps) + temp * temp / 2;
                }
            }

            return L;
        }

        public double[] likelihoodD(double[] c)
        {
            double[] dL = new double[k];
            for (int j = 0; j < k; j++)
                dL[j] = 0;

            double[] x = new double[m - 1], dm, ds;
            double y, temp, sig;

            
            // сумма по всем элементам выборки
            for (int i = 0; i < n; i++)
            {
                // копируем строку из data в массив x
                System.Buffer.BlockCopy(data, sizeof(double) * i * m, x, 0, sizeof(double) * (m - 1));
                y = data[i, m - 1];

                // производные функций
                dm = MathTool.derivative(c, delegate(double[] x0) { return mean(x0, x); });
                ds = MathTool.derivative(c, delegate(double[] x0) { return sigma(x0, x); });

                // считаем ЛФП
                sig = sigma(c, x);
                sig = sig < eps ? eps : sig;
                temp = (mean(c, x) - y) / sig;

                // заполняем градиент
                for (int j = 0; j < k; j++)
                    dL[j] += dm[j] * temp / sig;

                for (int j = 0; j < k; j++)
                    dL[k + j] += ds[j] / sig * (1 - temp * temp);
            }
            
            return dL;
        }

        

        /// <summary>
        /// Основной конструктор 
        /// </summary>
        /// <param name="data">Выборка данных
        /// Подразумевается, если размер матрицы n*m, то: 
        /// n - размер выборки (количество экспериментов);
        /// m-1 - количество аргументов x;
        /// т.е. столбцы 0..m-2 - аргументы, m-1 - значение функции (время выполнения).</param>
        /// <param name="k">количество параметров оптимизации</param>
        /// <param name="mean">функция мат.ожидания meanFunction(double[] c, double[] x)</param>
        /// <param name="sigma">функция СКО sigmaFunction(double[] c, double[] x)</param>
        /// <param name="eps">точность вычислений</param>
        /// <param name="debugInfo">Нужно ли выводить доп. информацию</param>
        public MLEstimator(double[,] data, int k, meanFunction mean, sigmaFunction sigma, double eps, bool debugInfo)
        {
            this.data = data;
            this.n = data.GetLength(0);
            this.m = data.GetLength(1);
            this.k = k;
            this.mean = mean;
            this.sigma = sigma;
            this.eps = eps;
            this.debugInfo = debugInfo;
            OptimTool.debugInfo = debugInfo;
        }

        /// <summary>
        /// запуск оценки параметров
        /// </summary>
        /// <param name="c">вектор начальных значений</param>
        /// <param name="toSolve">вектор той же размерности, что и c: 
        /// Если toSolve[i] = true, c[i] будет оптимизирован;
        /// Если toSolve[i] = false, c[i] считается константой.</param>
        /// <returns></returns>
        public double[] estimate(double[] c, bool[] toSolve)
        {


            return OptimTool.optimiseND(
                delegate(double[] p0) {
                    for (int i = 0; i < k; i++)
                        if (!toSolve[i])
                            p0[i] = c[i];
                    return likelihood(p0);
                },
                delegate(double[] p0) {
                    for (int i = 0; i < k; i++)
                        if (!toSolve[i])
                            p0[i] = c[i];
                    var rez = MathTool.derivative(p0, (MathTool.functionNDto1D)likelihood);
                    for (int i = 0; i < k; i++)
                        if (!toSolve[i])
                            rez[i] = 0;
                    return rez; 
                }
                , c);

            //return OptimTool.optimiseND(likelihood,
            //    likelihoodD
            //    , c);
        }

        /// <summary>
        /// запуск оценки параметров
        /// </summary>
        /// <param name="c">вектор начальных значений</param>
        /// <returns></returns>
        public double[] estimate(double[] c)
        {
            bool[] toSolve = new bool[c.Length];
            for(int i = 0; i < c.Length; i++)
                toSolve[i] = true;
            return estimate(c, toSolve);
        }
    }
}