using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Matrix M = new Matrix(3);
                Vector b = new Vector(3);

                /*M[0, 0] = 9; M[0, 1] = -2; M[0, 2] = 3; M[0, 3] = 2;
                M[1, 0] = 2; M[1, 1] = 8; M[1, 2] = -2; M[1, 3] = 3;
                M[2, 0] = -3; M[2, 1] = 2; M[2, 2] = 11; M[2, 3] = -4;
                M[3, 0] = -2; M[3, 1] = 3; M[3, 2] = 2; M[3, 3] = 10;

                b[0] = 54.5; b[1] = -14; b[2] = 12.5; b[3] = -21;*/

                M[0, 0] = 5; M[0, 1] = 1; M[0, 2] = 2;
                M[1, 0] = -3; M[1, 1] = 9; M[1, 2] = 4;
                M[2, 0] = 1; M[2, 1] = 2; M[2, 2] = -7;

                b[0] = 10; b[1] = -14; b[2] = -33;

                Console.WriteLine("The matrix M is {0}", M);
                Console.WriteLine("The vector b is {0}", b);

                LinSolve l = new LinSolve();
                l.Tol = 0.0001;
                l.Maxiters = 100;

                Vector ans = l.Solve(M, b);
                Console.WriteLine("The solution to Mx = b is {0}", ans);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error encountered: {0}", e.Message);
            }
        }
    }

    class LinSolve
    {
        // data
        private Matrix L;
        private Matrix D;
        private Matrix U;

        private Vector b;

        private double tol = 0.001;
        private int maxiters = 1000;
        private int count; // count of iterations taken to solve

        public double Tol { get => tol; set => tol = value; }

        public int Maxiters { get => maxiters; set => maxiters = value; }

        // constructor
        public LinSolve()
        {

        }

        // methods

        private Matrix D_invert(Matrix d) // invert diagonal matrix
        {
            Matrix temp = new Matrix(d.Size);

            for (int i = 0; i < d.Size; i++)
            {
                temp[i, i] = 1.0 / d[i, i];
            }

            return temp;
        }

        public Vector Solve(Matrix m, Vector v) // solve system
        {
            b = new Vector(v);

            L = new Matrix(m.Size); // create L/D/U submatrices
            D = new Matrix(m.Size);
            U = new Matrix(m.Size);

            for (int i = 0; i < m.Size; i++)
            {
                for (int j = 0; j < m.Size; j++)
                {
                    if (i > j) // L submatrix
                    {
                        L[i, j] = m[i, j];
                    }
                    else if (i == j) // D submatrix
                    {
                        D[i, j] = m[i, j];
                    }
                    else // U submatrix
                    {
                        U[i, j] = m[i, j];
                    }
                }
            }

            count = 0;

            Matrix D_inv = D_invert(D); // these 3 components stay constant between iterations
            Matrix T = D_inv * (L + U);
            Vector c = D_inv * b;

            Vector xold = new Vector(b.Length); // initial "guess" of all zeros
            Vector xnew = c - (T * xold);

            double e = (xnew - xold).Norm / xold.Norm; // error

            while (e > tol && count < maxiters)
            {
                count++;

                xold = xnew;
                xnew = c - (T * xold);

                e = (xnew - xold).Norm / xold.Norm;
            }

            return xnew;
        }
    }

    public class Matrix
    {
        // data
        private double[,] data;
        private int size;

        public int Size { get => size; }

        // constructors

        public Matrix() // default constructor
        {
            size = 3;
            data = new double[size, size];
        }

        public Matrix(int n) // create square matrix by size
        {
            if (n > 1)
            {
                size = n;
            }
            else
            {
                throw new Exception("Matrix size must be positive integer > 1");
            }

            data = new double[size, size];
        }

        public Matrix(Matrix a) // create matrix from another matrix
        {
            size = a.Size;

            this.data = new double[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    this.data[i, j] = a.data[i, j];
        }

        // overloaded operators
        public static Matrix operator +(Matrix a, Matrix b) // binary addition
        {
            if (a.Size != b.Size)
            {
                throw new Exception("Matrix dimensions do not agree.");
            }

            Matrix tmp = new Matrix(a.Size);

            for (int i = 0; i < a.Size; i++)
                for (int j = 0; j < a.Size; j++)
                    tmp.data[i, j] = a.data[i, j] + b.data[i, j];

            return tmp;
        }

        public static Matrix operator -(Matrix a, Matrix b) // binary subtraction
        {
            if (a.Size != b.Size)
            {
                throw new Exception("Matrix dimensions do not agree.");
            }

            Matrix tmp = new Matrix(a.Size);

            for (int i = 0; i < a.Size; i++)
                for (int j = 0; j < a.Size; j++)
                    tmp.data[i, j] = a.data[i, j] - b.data[i, j];

            return tmp;
        }

        public static Matrix operator *(Matrix a, Matrix b) // binary multiplication
        {
            if (a.Size != b.Size)
            {
                throw new Exception("Matrix dimensions do not agree.");
            }

            Matrix tmp = new Matrix(a.Size);

            for (int i = 0; i < a.Size; i++)
            {
                for (int j = 0; j < a.Size; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < a.Size; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }

                    tmp.data[i, j] = sum;
                }
            }

            return tmp;
        }

        public static Vector operator *(Matrix m, Vector a) // matrix by vector multiplication
        {
            int m_rows = m.Size;
            int m_cols = m.Size;
            int a_l = a.Length;

            Vector temp = new Vector(a_l);

            if (m_cols != a_l)
            {
                throw new Exception("Cannot multiply Matrix by Vector: size of Matrix not equal to length of Vector");
            }

            for (int i = 0; i < a_l; i++)
            {
                double sum = 0;

                for (int j = 0; j < a_l; j++) // running sum of multiplied terms
                {
                    sum += m[i, j] * a[j];
                }

                temp[i] = sum;
            }

            return temp;
        }

        public double this[int rowindex, int colindex] // indexing operator
        {
            get
            {
                if (rowindex >= 0 && rowindex < size && colindex >= 0 && colindex < size)
                {
                    return data[rowindex, colindex];
                }
                else
                {
                    string e = String.Format("Invalid Matrix index specified: row must be >= 0 and < {0} and column must be >= 0 and < {0}", size);
                    throw new Exception(e);
                }
            }
            set
            {
                if (rowindex >= 0 && rowindex < size && colindex >= 0 && colindex < size)
                {
                    data[rowindex, colindex] = value;
                }

                else
                {
                    string e = String.Format("Invalid Matrix index specified: row must be >= 0 and < {0} and column must be >= 0 and < {0}", size);
                    throw new Exception(e);
                }
            }
        }

        // methods
        public override string ToString()
        {
            string s = "\n"; // rows, be separated by \n

            for (int i = 0; i < size; i++)
            {
                string r = "{"; // "row" of values

                for (int j = 0; j < size - 1; j++)
                {
                    r += String.Format("{0}", data[i, j]) + ", ";
                }

                r += String.Format("{0}", data[i, size - 1]) + "}"; // add last term in row

                s += r + "\n"; // create new "row" in total string
            }

            return s;
        }
    }

    public class Vector
    {
        // data
        private double[] data;
        private int length;

        public int Length { get => this.length; }

        public double Norm // norm of vector
        {
            get
            {
                double max1 = 0;

                for (int i = 0; i < length; i++)
                {
                    if (Math.Abs(data[i]) > max1)
                    {
                        max1 = Math.Abs(data[i]);
                    }
                }

                return max1;
            }
        }

        public double Sum // sum of all values of vector
        {
            get
            {
                double sum = 0;

                for (int i = 0; i < length; i++)
                {
                    sum += data[i];
                }

                return sum;
            }
        }

        // constructors
        public Vector() // default (vector of length 3 with zero values)
        {
            length = 3;
            data = new double[length];
        }

        public Vector(int s) // vector of length l with zero values
        {
            if (s > 1)
            {
                length = s;
                data = new double[length];
            }
            else
            {
                throw new Exception("Vector length must be an integer > 1");
            }
        }

        public Vector(double[] a) // create vector from an array
        {
            length = a.GetLength(0);
            data = new double[length];

            for (int i = 0; i < length; i++)
            {
                data[i] = a[i];
            }
        }

        public Vector(Vector a) // create vector from another vector
        {
            this.length = a.Length;
            data = new double[this.length];

            for (int i = 0; i < this.length; i++)
            {
                data[i] = a[i];
            }
        }

        // overloaded operators
        public double this[int i] // indexing operator
        {
            get
            {
                if (i >= 0 & i < length)
                {
                    return data[i];
                }
                else
                {
                    string e = String.Format("Invalid Vector index specified, position must be >= 0 and < {0}", length);
                    throw new Exception(e);
                }
            }
            set
            {
                if (i >= 0 & i < length)
                {
                    data[i] = value;
                }
                else
                {
                    string e = String.Format("Invalid Vector index specified, position must be >= 0 and < {0}", length);
                    throw new Exception(e);
                }
            }
        }

        public static Vector operator +(Vector a, Vector b) // addition of 2 vectors
        {
            int a_l = a.Length;
            int b_l = b.Length;

            if (a_l != b_l) // check if vectors same size
            {
                throw new Exception("Cannot add vectors: Lengths not equal!");
            }

            Vector temp = new Vector(a_l);

            for (int i = 0; i < a_l; i++) // add corresponding elements
            {
                temp[i] = a[i] + b[i];
            }

            return temp;
        }

        public static Vector operator -(Vector a, Vector b) // subtraction of 2 vectors
        {
            int a_l = a.Length;
            int b_l = b.Length;

            if (a_l != b_l) // check if vectors same size
            {
                throw new Exception("Cannot subtract vectors: Lengths not equal!");
            }

            Vector temp = new Vector(a_l);

            for (int i = 0; i < a_l; i++) // subtract corresponding elements
            {
                temp[i] = a[i] - b[i];
            }

            return temp;
        }

        public static double operator *(Vector a, Vector b) // dot product multiplication of 2 vectors
        {
            int a_l = a.Length;
            int b_l = b.Length;

            if (a_l != b_l) // check if vectors same size
            {
                throw new Exception("Cannot dot multiply vectors: Lengths not equal!");
            }

            double sum = 0;

            for (int i = 0; i < a_l; i++) // running sum of multiplied terms
            {
                sum = sum + (a[i] * b[i]);
            }

            return sum;
        }

        public static Vector operator *(Vector a, double x) // multiplication of vector by constant
        {
            int a_l = a.Length;

            Vector temp = new Vector(a_l);

            for (int i = 0; i < a_l; i++) // scale each term by constant
            {
                temp[i] = a[i] * x;
            }

            return temp;
        }

        public static Vector operator *(double x, Vector a) // multiplication of constant by vector
        {
            int a_l = a.Length;

            Vector temp = new Vector(a_l);

            for (int i = 0; i < a_l; i++) // scale each term by constant
            {
                temp[i] = a[i] * x;
            }

            return temp;
        }

        // methods
        public override string ToString()
        {
            string r = "{"; // "row" of values

            for (int i = 0; i < length - 1; i++)
            {
                r += String.Format("{0:0.000}", data[i]) + ", ";
            }

            r += String.Format("{0:0.000}", data[length - 1]) + "}"; // add last term in row

            return r;
        }
    }
}
