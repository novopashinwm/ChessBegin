using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBegin
{
    class Program
    {
        private static string[] letters = 
            new string[8] { "a", "b", "c", "d", "e", "f", "g", "h" };
        
        private static  string[,] board = new string[8, 8];
        private static bool isWhite = true;
        private static Coord Move;
        private static string strFiguraFrom;
        private static string strFiguraTo;
        private static int intFiguraFrom;
        private static int intFiguraTo;
        private static int intFiguraFromDiv;
        private static int intFiguraToDiv;

        private static string[] arrMove;
        const string cstrPawnWhite = "1";
        const string cstrPawnBlack = "2";
        const string cstrBishopWhite = "3";
        const string cstrBishopBlack = "4";

        //Массивы для расстановки фигур
        //Белые
        private static string[,] white = new string[,] { 
                { "a2", cstrPawnWhite },
                { "b2", cstrPawnWhite },
                { "c2", cstrPawnWhite },
                { "d2", cstrPawnWhite },
                { "e2", cstrPawnWhite },
                { "f2", cstrPawnWhite },
                { "g2", cstrPawnWhite },
                { "h2", cstrPawnWhite },
                { "c1", cstrBishopWhite },
                { "f1", cstrBishopWhite},
        };

        //Черные
        private static string[,] black = new string[, ] { 
                { "a7", cstrPawnBlack },
                { "b7", cstrPawnBlack },
                { "c7", cstrPawnBlack },
                { "d7", cstrPawnBlack },
                { "e7", cstrPawnBlack },
                { "f7", cstrPawnBlack },
                { "g7", cstrPawnBlack },
                { "h7", cstrPawnBlack },
                { "c8", cstrBishopBlack },
                { "f8", cstrBishopBlack},

        };

        struct Coord 
        {
            public int colFrom;
            public int rowFrom;
            public int colTo;
            public int rowTo;
        }

        static void Main(string[] args)
        {   
            HelpFirst();
            Console.ReadLine();
            SetFigures(white);
            SetFigures(black);
            string strMove = " ";
                        
            //Здесь находится сердце программы
            while (strMove != "")
            {
                PrintBoard();
                PrintMove();
                strMove = Console.ReadLine();
                if ( !IsMoveValid (strMove) && strMove !="")
                {
                    Console.WriteLine("Не правильный ход!");
                    continue ;
                }
                ReplaceFigure();
                //Передача хода противнику
                isWhite = !isWhite;
                
            }
            Console.WriteLine("Игра закончена!");
            Console.ReadLine();
        }

        static void PrintMove()
        {
            Console.WriteLine();
            Console.WriteLine("Ход {0}.", (isWhite) ? "белых" : "черных");            
            Console.WriteLine("Введите ход:");
            Console.WriteLine();
        }

        /// <summary>
        /// Проверка корректности хода
        /// </summary>
        /// <param name="strMove">Координаты начала и конца хода</param>
        /// <returns></returns>
        static bool IsMoveValid(string strMove)
        {
            bool blnRet = false;
                        
            try
            {
                arrMove = strMove.Split(' ');
                Move.colFrom = Letter2Number(arrMove[0].Substring(0, 1)) ;
                Move.rowFrom = int.Parse(arrMove[0].Substring(1, 1)) - 1;
                Move.colTo = Letter2Number(arrMove[1].Substring(0, 1));
                Move.rowTo = int.Parse(arrMove[1].Substring(1, 1)) - 1;

                #region Проверка на дурака, что координаты не выходят за пределы доски
                //Определяем, что за фигура находится в начальных координатах
                if ((Move.colFrom == -1) || (Move.colTo == -1))
                {
                    Console.WriteLine("Заданы не верные буквенные обозначения колонок доски.");
                    return blnRet;
                }

                if ((Move.rowFrom < 0) || (Move.rowFrom > 7) || (Move.rowTo < 0) || (Move.rowTo > 7))
                {
                    Console.WriteLine("Заданы не верные цифровые обозначения строк доски.");
                    return blnRet;
                }
                #endregion

                strFiguraFrom = board[ Move.rowFrom, Move.colFrom];
                strFiguraTo = board[Move.rowTo, Move.colTo];
                int.TryParse(strFiguraFrom, out intFiguraFrom);
                int.TryParse(strFiguraTo, out intFiguraTo);
                
                intFiguraFromDiv = intFiguraFrom % 2;
                intFiguraToDiv = intFiguraTo % 2;
                
                #region Проверяем, что сейчас ход той фигуры, цвет которой указан
                if (isWhite)
                {
                    if ((intFiguraFromDiv) == 0)
                    {
                        Console.WriteLine("Сейчас ход белых!");
                        return blnRet;
                    }
                }
                else
                {
                    if ((intFiguraFromDiv) != 0)
                    {
                        Console.WriteLine("Сейчас ход черных!");
                        return blnRet;
                    }

                }
                #endregion
                #region Проверяем, что клетка куда пойдет фигура не содержит фигуру того же цвета

                if ( (((intFiguraFromDiv==0) && strFiguraFrom.Trim () !="") && 
                      ((intFiguraToDiv==0 ) && strFiguraTo.Trim () !="" ))
                     || ((intFiguraFromDiv!=0) && (intFiguraToDiv!=0 ))
                    )
                {
                    Console.WriteLine("Фигура пошла на клетку с фигурой того же цвета!");
                    return blnRet;
                }
                #endregion

                switch (strFiguraFrom)                
                {        
                    //Пешки
                    case cstrPawnWhite:
                    case cstrPawnBlack:
                        if (!PawnCheck()) return blnRet;
                        blnRet = true;
                        break;
                    //Слоны
                    case cstrBishopWhite:
                    case cstrBishopBlack :
                        if (!BishopCheck()) return blnRet;
                        blnRet = true;
                        break;
                    case "":                        
                        break;
                }                   

            }
            catch 
            { }
            return blnRet;
        }

        static bool PawnCheck()
        {
            //Проверяем, что на конечной клетке хода нет нашей фигуры                        
            //Если ходим с клетки 2 - допустим a2, то можно передвинуться на одну клетку или сразу на 2 клетки
            bool blnRet = false;
            if (Math.Abs(Move.colFrom - Move.colTo) > 1)
            {
                Console.WriteLine("Пешка не может ходить через 2 столбца");
                return blnRet;
            }

            if ((Move.colFrom != Move.colTo) && strFiguraTo.Trim() == "")
            {
                Console.WriteLine("Ошибочный ход на клетку " + arrMove [1]);
                return blnRet;
            }
            
            //Проверяем ход сначала на 2 клетки
            if (Move.colTo == Move.colFrom &&
                (
                //Белые не могу
                (isWhite && ((Move.rowTo - Move.rowFrom) > 1) && Move.rowFrom !=1)
                ||
                //Черные не могут двигаться дальше 1 клетки
                (!isWhite && ((Move.rowFrom - Move.rowTo) > 1) && Move.rowFrom !=6)
                ||
                //С начальной позиции белые не могут двигаться дальше 2 шагов вперед
                (isWhite && ((Move.rowTo - Move.rowFrom) > 2) && Move.rowFrom == 1)
                ||
                //С начальной позиции черные не могут двигаться далдьше 2 шагов вперед
                (!isWhite && ((Move.rowFrom - Move.rowTo) > 2) && Move.rowFrom == 6)
                ))
                return blnRet  ;

            blnRet = true;
            return blnRet;
        }

        static bool BishopCheck()
        {
            bool blnRet = false;
            if (Math.Abs(Move.colTo - Move.colFrom) != Math.Abs(Move.rowTo - Move.rowFrom))
            {
                Console.WriteLine("Так слон не ходит!");
                return blnRet;
            }

            int begin, end;
            if (Move.rowFrom < Move.rowTo)
            {
                begin = Move.rowFrom;
                end = Move.rowTo;
            }
            else
            {
                begin = Move.rowTo;
                end = Move.rowFrom;
            }
            for (int i = begin; i < end; i++)
            {
                if (board[i, i].Trim() != "")
                {
                    Console.WriteLine("Между началом хода слона " + arrMove[0] + " и окончанием хода " + arrMove[1] + " содержится фигуры!");                    
                    return blnRet;
                }
            }

            blnRet = true;
            return blnRet;
        }

        static void ReplaceFigure ()
        {
            board[Move.rowFrom, Move.colFrom] = " ";
            board[Move.rowTo, Move.colTo] = strFiguraFrom;            
        }

        /// <summary>
        /// Первичная помощь по программе
        /// </summary>
        static void HelpFirst()
        {
            Console.WriteLine();
            Console.WriteLine("Упрощенная шахматная программа");
            Console.WriteLine("1) 1- пешки белых, 2- пешки черных, 3- слон белых, 4 - слон черных;");
            Console.WriteLine("2) пешки ходят на 1 и 2 шага вперед");
            Console.WriteLine("3) пешки умеют бить");
            Console.WriteLine("4) слоны ходят и бьют.");
            Console.WriteLine(" ");
            Console.WriteLine("Для того чтобы сделать ход, нужно ввести через пробел начальные");
            Console.WriteLine ("и конечные координаты фигуры");
            Console.WriteLine("Например:a2  a3");
            Console.WriteLine("Для выхода не вводя никаких координат просто нажмите ввод.");
            Console.WriteLine("Нажмите любую клавишу.....");
            Console.WriteLine();
        }
                
        /// <summary>
        /// Устанавливаем фигуры на доску
        /// figures - фигуры, strSymbol- символ фигур
        /// </summary>
        /// <param name="figures"></param>        
        static void SetFigures(string[,] figures )
        {
            for (int i = 0; i < figures.GetLongLength(0); i++)
                SetCell(figures[i,0], figures[i,1]);            
        }

        static void SetCell(string strCoord, string strValue)
        {
            string horiz = strCoord.Substring(0, 1);
            int vert = int.Parse(strCoord.Substring(1, 1));

            int intH = Letter2Number(horiz);
            board[ vert -1, intH] = strValue;
        }

        /// <summary>
        /// Выводим шахматную доску с фигурами
        /// </summary>
        static void PrintBoard()
        {
            
            string strStroka = "   " + new string('-', 32);
            Console.WriteLine(strStroka );
            for (int i = board.GetLength(0)-1;  i>=0; i--)
            {                
                Console.Write("{0} !", i+1);
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == null ) board[i, j] = " ";
                    Console.Write(" {0} !", board[ i, j]);
                }
                
                Console.WriteLine();
                Console.WriteLine(strStroka );
            }

            Console.Write("   ");
            for (int i = 0; i < 8; i++)            
                Console.Write(" {0}  ", letters [i]) ;
            
            Console.WriteLine();
        }      
        
        static int Letter2Number(string letter)
        {            
            int intFind = -1;
            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == letter.ToLower())
                {
                    intFind = i;
                    break;
                }
            }
            return intFind ;
        }

        static string  Number2Letter(int intI)
        {            
            if (intI>= letters.Length) return "";
            return letters[intI];
        }

    }
}
