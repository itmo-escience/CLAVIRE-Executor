using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelEstimation.tools
{
    public class OptimTool
    {

        static double infinity = 1.0e12;
        static double eps = 1.0e-8;
        static double maxSteps = 150;

        public static bool debugInfo = false;

        /// <summary>
        /// Оптимизация функции одного аргумента.
        /// Простейшая версия, нулевого порядка (без производных).
        /// Изначально подразумевается, что аргумент >= 0, стартовая точка поиска = 0.
        /// (Для получения такой функции используется проекция из MathTool)
        /// </summary>
        /// <param name="functionNDto1D">целевая функция</param>
        /// <returns>точка, в которой наблюдается минимум функции</returns>
        public static double optimise1D(MathTool.function1Dto1D function)
        {
            return optimise1D(function, 1);
        }

        /// <summary>
        /// Оптимизация функции одного аргумента.
        /// Простейшая версия, нулевого порядка (без производных).
        /// Изначально подразумевается, что аргумент >= 0, стартовая точка поиска = 0.
        /// (Для получения такой функции используется проекция из MathTool)
        /// </summary>
        /// <param name="functionNDto1D">целевая функция</param>
        /// <param name="startXMax">характерная длина промежутка неопределённости</param>
        /// <returns>точка, в которой наблюдается минимум функции</returns>
        public static double optimise1D(MathTool.function1Dto1D function, double startXMax)
        {
            // промежуток неопределённости
            double xmax = startXMax;

            // сначала находим начальный промежуток неопределённости, т.е. xmax
            // cначала спускаемся вниз быстро, чтобы не взять слишком большой промежуток
            {
                double f0 = function(0);
                while (function(xmax) > f0)
                    xmax /= 10;
            }
            // потом увеличиваем, если нужно
            {
                double x = xmax;
                int confidence = 5;
                double fLast, fCur = function(x), xlastMax = 1;
                do
                {
                    fLast = fCur;
                    xlastMax *= 2;
                    x = xlastMax;
                    fCur = function(x);
                    if (fCur > fLast * (1 + eps))
                    {
                        confidence = 0;
                    }
                    else if (fCur <= fLast * (1 + eps) && fCur >= fLast * (1 - eps))
                    {
                        confidence--;
                    }
                    else
                    {
                        xmax = xlastMax;
                    }
                } while (confidence > 0 && xlastMax < infinity);
                xmax *= 2;
            }
            if(debugInfo)
                Console.WriteLine("xmax = " + xmax);
            // на текущий момент найден промежуток неопределённости и задан начальный x[0]
            // Ищем минимум методом золотого сечения:
            {
                double xmin = 0;
                double x1, x2;
                double tau = 2 / (Math.Sqrt(5) + 1);
                do
                {
                    x2 = xmin + (xmax - xmin) * tau;
                    x1 = xmax - (xmax - xmin) * tau;
                    if (function(x2) > function(x1))
                        xmax = x2;
                    else
                        xmin = x1;
                } while (Math.Abs(x2 - x1) > eps);

                // округление значений до точности Delta
                int k = (int)Math.Ceiling(-Math.Log10(MathTool.Delta));
                xmax = Math.Round((x1 + x2) / 2, k);
            }

            return xmax;
        }

        /// <summary>
        /// Оптимизация функции нескольких аргументов.
        /// Метод первого порядка
        /// </summary>
        /// <param name="func">функция</param>
        /// <param name="grad">градиент</param>
        /// <param name="start">начальное приближение</param>
        /// <returns>точка минимума</returns>
        public static double[] optimiseND(MathTool.functionNDto1D func, MathTool.functionNDtoMD grad, double[] start)
        {
            double[] x = (double[])start.Clone();
            int n = x.Length;
            double[] G = grad(x), S = new double[n];
            for (int i = 0; i < n; i++)
                S[i] = -G[i];
            double F = func(x), beta, grd = MathTool.dot(G, G), grdOld = grd, alpha = Math.Sqrt(grd), grdS = Math.Sqrt(grd);

            int step = 0;
            if (debugInfo)
            {
                Console.WriteLine("step = " + step);
                Console.WriteLine("alpha = " + alpha * Math.Sqrt(grd));
                Console.WriteLine("p = " + view(x));
                Console.WriteLine("G = {0}, |G|^2 = {1}", view(G), grd);
                Console.WriteLine("S = {0}, |S|^2 = {1}", view(S), MathTool.dot(S, S));
            }

            do {
                step++;
                // одномерная минимизация в направлении сопряжённого градиента
                alpha = optimise1D(MathTool.projection(func, S, x), 1);// / Math.Sqrt(MathTool.dot(S, S)) );
               // Console.WriteLine(Math.Sqrt(MathTool.dot(S,S)));

                // смещение x
                for (int i = 0; i < n; i++)
                    x[i] += alpha * S[i];


                // вычисление нового градиента
                G = grad(x);
                grdOld = grd;
                grd = MathTool.dot(G, G); 
                // коэффициент beta по методу Флетчера-Ривза
                beta = grd / grdOld;

                // обновление сопряжённого градиента
                for (int i = 0; i < n; i++)
                    S[i] = -G[i] / Math.Sqrt(grd) + beta * S[i] / grdS;
                grdS = Math.Sqrt(MathTool.dot(S, S));
                if (debugInfo)
                {
                    Console.WriteLine("step = " + step);
                    Console.WriteLine("alpha = " + alpha * grdS);
                    Console.WriteLine("beta = " + beta);
                    Console.WriteLine("p = " + view(x));
                    Console.WriteLine("G = {0}, |G|^2 = {1}", view(G), grd);
                    Console.WriteLine("S = {0}, |S|^2 = {1}", view(S), MathTool.dot(S, S));
                }
            } while (alpha * grdS > eps && step < maxSteps);


            return x;
        }


        public static string view(double[] x)
        {
            string rez = "{";
            foreach (var val in x)
                rez += " " + Math.Round(val, 4) + ",";
            rez = rez.Substring(0, rez.Length - 1) + " }";
            return rez;
        }
    }
}
