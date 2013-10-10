using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelEstimation.tools
{
    public class MathTool
    {
        //--------Параметры мат. вычислений--------------//

        /// <summary>
        /// приращение для численного получения производной
        /// </summary>
        public static double Delta
        {
            get { return MathTool.delta; }
            set { MathTool.delta = value <= 0 ? 1.0e-6 : value; }
        }
        private static double delta = 1.0e-6;


        
        /// <summary>
        /// Количество точек, используемых при дифференцировании
        /// Поддерживаемые случаи: 2, 3, 5
        /// </summary>
        public static int DerivativePoints
        {
            get { return MathTool.derivativePoints; }
            set { MathTool.derivativePoints = (value != 2 && value != 3) ? 5 : value; }
        }
        private static int derivativePoints = 5;

        //----------Объявление функций-------------------//

        /// <summary>
        /// Произвольная скалярная функция от вектора аргументов
        /// </summary>
        /// <param name="x">аргументы</param>
        /// <returns>скалярное значение</returns>
        public delegate double functionNDto1D(double[] x);

        /// <summary>
        /// Произвольная вектор-функция от вектора аргументов
        /// </summary>
        /// <param name="x">аргументы</param>
        /// <returns>векторное значение</returns>
        public delegate double[] functionNDtoMD(double[] x);

        /// <summary>
        /// Произвольная скалярная функция от скалярнонго аргумента
        /// </summary>
        /// <param name="x">аргумент</param>
        /// <returns>скалярное значение</returns>
        public delegate double function1Dto1D(double x);


        //---------------Общие методы-------------------//

        /// <summary>
        /// Численная оценка якобиана векторной функции
        /// </summary>
        /// <param name="x">вектор аргументов</param>
        /// <param name="function">вектор-функция вектора аргументов</param>
        /// <returns>матрица - якобиан</returns>
        public static double[,] derivative(double[] x, functionNDtoMD function)
        {
            // значение функции в точке x0 = x;
            double[] F0 = function(x);
            // конструируем якобиан
            double[,] dFdx = new double[F0.Length,x.Length];

            switch (DerivativePoints)
            {
                case 2:
                {
                    double[] x1 = (double[])x.Clone();
                    double[] F1;
                    for (int i = 0; i < x.Length; i++)
                    {
                        x1[i] = x[i] + Delta;
                        F1 = function(x1);
                        for (int j = 0; j < F0.Length; j++)
                            dFdx[j, i] = (F1[j] - F0[j]) / Delta;
                        x1[i] = x[i];
                    }
                    break;
                }
                case 3:
                {
                    double[] x1 = (double[])x.Clone(), x2 = (double[])x.Clone();
                    double[] F1, F2;
                    for (int i = 0; i < x.Length; i++)
                    {
                        x1[i] = x[i] + Delta;
                        x2[i] = x[i] - Delta;
                        F1 = function(x1);
                        F2 = function(x2);
                        for (int j = 0; j < F0.Length; j++)
                            dFdx[j, i] = (F1[j] - F2[j]) / Delta / 2;
                        x1[i] = x[i];
                        x2[i] = x[i];
                    }
                    break;
                }
                case 5:
                default:
                {
                    double[] x1 = (double[])x.Clone(), x2 = (double[])x.Clone(), x3 = (double[])x.Clone(), x4 = (double[])x.Clone();
                    double[] F1, F2, F3, F4;
                    for (int i = 0; i < x.Length; i++)
                    {
                        x1[i] = x[i] + Delta;
                        x2[i] = x[i] - Delta;
                        x3[i] = x[i] + 2*Delta;
                        x4[i] = x[i] - 2*Delta;
                        F1 = function(x1);
                        F2 = function(x2);
                        F3 = function(x3);
                        F4 = function(x4);
                        for (int j = 0; j < F0.Length; j++)
                            dFdx[j, i] = (F4[j] - 8*F2[j] + 8*F1[j] - F3[j]) / Delta / 12;
                        x1[i] = x[i];
                        x2[i] = x[i];
                        x3[i] = x[i];
                        x4[i] = x[i];
                    }
                    break;
                }
            }

            // округление значений до точности Delta
            int k = (int)Math.Ceiling(-Math.Log10(Delta));
            for (int i = 0; i < x.Length; i++)
                for (int j = 0; j < F0.Length; j++)
                {
                    dFdx[j, i] = Math.Round(dFdx[j, i], k);
                }

            return dFdx;
        }


        /// <summary>
        /// Численная оценка градиента скалярной функции
        /// </summary>
        /// <param name="x">аргументы функции</param>
        /// <param name="functionNDto1D">делегируемая функция</param>
        /// <returns>градиент функции</returns>
        public static double[] derivative(double[] x, functionNDto1D function)
        {
            var res = derivative(x, delegate(double[] x0) { return new double[] { function(x0) }; });
            var grad = new double[x.Length];
            System.Buffer.BlockCopy(res, 0, grad, 0, sizeof(double) * x.Length);
            return grad;
        }

        /// <summary>
        /// Численная оценка производной скалярной функции скалярного аргумента
        /// </summary>
        /// <param name="x">аргумент функции</param>
        /// <param name="function">делегируемая функция</param>
        /// <returns>скаляр - значение производной в точке x</returns>
        public static double derivative(double x, function1Dto1D function)
        {
            return derivative(new double[] { x }, delegate(double[] x0) { return function(x0[0]); })[0];
        }

        /// <summary>
        /// Проекция функции многих переменных на прямую
        /// (в смысле приведения функции многих переменных к функции одной переменной)
        /// </summary>
        /// <param name="functionNDto1D">исходная функция</param>
        /// <param name="dir">направление для проекции</param>
        /// <param name="start">стартовая точка</param>
        /// <returns>функция одного аргумента</returns>
        public static function1Dto1D projection(functionNDto1D function, double[] dir, double[] start)
        {
            ////нормировка направления
            //double[] parms = (double[])dir.Clone();
            //double temp = 0;
            //foreach (var d in dir)
            //    temp += d * d;
            //temp = Math.Sqrt(temp);
            //for (int i = 0; i < parms.Length; i++)
            //    parms[i] /= temp;

            // функция от одного аргумента x[0]
            return delegate(double x)
            {
                double[] vec = (double[])start.Clone();
                for (int i = 0; i < dir.Length; i++)
                    vec[i] += dir[i] * x;
                return function(vec);
            };
        }

        /// <summary>
        /// скалярное произведение векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double dot(double[] a, double[] b)
        {
            double d = 0;
            for (int i = 0; i < a.Length; i++)
                d += a[i] * b[i];
            return d;
        }
    }
}
