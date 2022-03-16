using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new char[9][]
            {
                new char[9] {' ', ' ', ' ', ' ', ' ', ' ', ' ', '2', ' ' },
                new char[9] {' ', ' ', '6', '4', ' ', ' ', '1', '3', ' ' },
                new char[9] {'4', ' ', ' ', ' ', '9', ' ', ' ', ' ', ' ' },
                new char[9] {' ', ' ', ' ', '1', ' ', ' ', ' ', ' ', '2' },
                new char[9] {' ', ' ', '8', ' ', ' ', ' ', ' ', ' ', '9' },
                new char[9] {' ', '3', ' ', ' ', '7', ' ', '8', '1', ' ' },
                new char[9] {' ', ' ', '3', '9', ' ', ' ', '6', '4', ' ' },
                new char[9] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '8' },
                new char[9] {' ', '7', ' ', ' ', ' ', '5', ' ', ' ', ' ' }
             };
            var x = new SudokuLibrary.SudokuSolver();
            x.SudokuSolverSolution(input);
            for (int i = 0; i < 9; i++)
            {
                var row = "";
                for (int j = 0; j < 9; j++)
                    row += x.sudokuArrays[i][j] + " ";
                Console.WriteLine(row);
            }
            // for(int i = 0; i < 9; i++)
            // {
            //     for(int j = 0; j < 9; j++)
            //     {
            //         var c = x.sudokuArrays[i][j];
            //         if(c == '\0')
            //             input[i][j] = ' ';
            //         else
            //             input[i][j] = c;
            //     }
            // }
            // x = new SudokuLibrary.SudokuSolver();
            // x.SudokuSolverSolution(input);
            // for (int i = 0; i < 9; i++)
            // {
            //     var row = "";
            //     for (int j = 0; j < 9; j++)
            //         row += x.sudokuArrays[i][j] + " ";
            //     Console.WriteLine(row);
            // }
            var y = 99;
        }
    }
}
namespace SudokuLibrary
{
    public class SudokuSolver
    {
        public char[][] sudokuArrays;
        Dictionary<string, int[]> arrayLinker;
        Dictionary<string, string> notes;
        bool valueAdded;
        static void ArgumentValidator(char[][] board)
        {
            if (board.GetLength(0) != 9)
                throw new ArgumentException("Input must be jagged array of 9 character arrays with lengths of 9.");
            var values = new char[10] { ' ', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < 9; i++)
            {
                if (board[i].Length != 9)
                    throw new ArgumentException("Input must be jagged array of 9 character arrays with lengths of 9.");
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (!values.Contains(board[i][j]))
                        throw new ArgumentException("Each array element must be a number between 1-9 or ' '.");
                }
            }
        }
        void InitializeFields()
        {
            valueAdded = true;
            sudokuArrays = new char[27][];
            arrayLinker = new Dictionary<string, int[]>();
            notes = new Dictionary<string, string>();
            for (int i = 0; i < 27; i++)
                sudokuArrays[i] = new char[9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var blockNum = 18 + (i / 3) * 3 + j / 3;
                    var blockPos = (i % 3) * 3 + j % 3;
                    var iStr = i.ToString();
                    var jStr = j.ToString();
                    arrayLinker.Add
                        (iStr + jStr, new int[4] { j + 9, i, blockNum, blockPos });
                    arrayLinker.Add
                        ((j + 9).ToString() + iStr, new int[4] { i, j, blockNum, blockPos });
                    arrayLinker.Add
                        (blockNum.ToString() + blockPos.ToString(), new int[4] { i, j, j + 9, i });
                    notes.Add(iStr + jStr, "123456789");
		            notes.Add((j + 9).ToString() + iStr, "123456789");
	                notes.Add(blockNum.ToString() + blockPos.ToString(), "123456789");
                }
            }
        }
        void InputBoard(char[][] board)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if(board[i][j] != ' ')
                    {
                        AddNum(board[i][j], i, j);
                    }
                }
            }
        }
        void AddNum(char num, int array, int pos)
        {
            var coPositions = arrayLinker[array.ToString() + pos.ToString()];
            sudokuArrays[array][pos] = num;
            sudokuArrays[coPositions[0]][coPositions[1]] = num;
            sudokuArrays[coPositions[2]][coPositions[3]] = num;
	    notes.Remove(array.ToString() + pos.ToString());
            notes.Remove(coPositions[0].ToString() + coPositions[1].ToString());
	    notes.Remove(coPositions[2].ToString() + coPositions[3].ToString());
            EditNotes(num, array);
            EditNotes(num, coPositions[0]);
            EditNotes(num, coPositions[2]);
            BoardValidator(sudokuArrays);
        }
        void EditNotes(char num, int array, string[] exceptions = null)
        {
            for (int i = 0; i < 9; i++)
            {
                var coPositions = arrayLinker[array.ToString() + i.ToString()];
		var str = array.ToString() + i.ToString();
                if (exceptions != null && exceptions.Contains(str))
                    continue;
                else if (sudokuArrays[array][i] == '\0' && notes[str].Contains(num))
                {
                    var note = notes[str];
                    var index = note.IndexOf(num);
                    note = note.Remove(index, 1);
                    if (note.Length == 1)
                    {
                        AddNum(note[0], array, i);
                        valueAdded = true;
                    }
                    else
                    {
                        notes[str] = note;
			notes[coPositions[0].ToString() + coPositions[1].ToString()] = note;
			notes[coPositions[2].ToString() + coPositions[3].ToString()] = note;
                        valueAdded = true;
                    }
                }
            }
        }
        public static void BoardValidator(char[][] sudokuArrays)
        {
            for (int i = 0; i < 27; i++)
            {
                var currentArray = new char[9];
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuArrays[i][j] != '\0' && currentArray.Contains(sudokuArrays[i][j]))
                        throw new ArgumentException("This is not a valid sudoku board.");
                    currentArray[j] = sudokuArrays[i][j];
                }
            }
        }
        void ScanByNumber(char num)
        {
            var array = 0;
            for (int i = 18; i < 27; i++)
            {   
                var block = sudokuArrays[i];
                if (block.Contains(num))
                    continue;
                var openSpots = new string[9];
                var count = 0;
                string[] exceptions;
                for (int j = 0; j < 9; j++)
                {
                    var str = i.ToString() + j.ToString();
                    var coPositions = arrayLinker[str];
                    var row = coPositions[0];
                    if (block[j] == '\0' && notes[str].Contains(num))
                    {
                        openSpots[count] = str;
                        count++;
                    }
                }
                if (count == 1)
                {
                    var coPositions = arrayLinker[openSpots[0]];
                    AddNum(num, coPositions[0], coPositions[1]);
                    valueAdded = true;
                }
                else if (count == 3 || count == 2)
                {
                    var row = arrayLinker[openSpots[0]][0];
                    var col = arrayLinker[openSpots[0]][1];
                    if ((count == 2 && arrayLinker[openSpots[1]][0] == row) ||
                        (count == 3 && arrayLinker[openSpots[1]][0] == row && arrayLinker[openSpots[2]][0] == row))
                        array = row;
                    else if ((count == 2 && arrayLinker[openSpots[1]][1] == col) ||
                        (count == 3 && arrayLinker[openSpots[1]][1] == col && arrayLinker[openSpots[2]][1] == col))
                        array = col + 9;
                    else
                        continue;
                    exceptions = new string[count];
                    for (int j = 0; j < count; j ++)
                    {
                        var coPositions = arrayLinker[openSpots[j]];
		        if(array < 9)
			    exceptions[j] = coPositions[0].ToString() + coPositions[1].ToString();
                        else
			    exceptions[j] = coPositions[2].ToString() + coPositions[3].ToString();
                    }
                    EditNotes(num, array, exceptions);
                }
            }
        }
        void ScanNotes(int arrayNum)
        {
            var blankCount = 0;
            var array = sudokuArrays[arrayNum];
            string note;
            for (int i = 0; i < 9; i++)
            {
                if (array[i] == '\0')
                    blankCount++;
            }
            for (int i = 0; i < 9; i++)
            {
                var exceptions = new string[9];
                var count = 0;
                if (array[i] != '\0') 
                    continue;
                var str = arrayNum.ToString() + i.ToString();
                exceptions[i] = str;
                count++;
                note = notes[str];
                for (int j = i + 1; j < 9; j++)
                {
                    var similar = false;
                    if(array[j] != '\0')
                        continue;
                    str = arrayNum.ToString() + j.ToString();
                    foreach (char c in note)
                    {
                        if( notes[str].Contains(c) )
                            similar = true;
                    }
                    if(!similar)
                        continue;
                    foreach (char c in notes[str])
                    {
                        if (!note.Contains(c))
                            note += c;
                    }
                    exceptions[j] = str;
                    count++;
                    if(note.Length == count)
                        break;
                }
                if (note.Length < blankCount && note.Length == count)
                {
                    foreach(char c in note)
                    {
                        EditNotes(c, arrayNum, exceptions);
                    }
                }
            }
        }
        
        public void SudokuSolverSolution(char[][] board)
        {
            ArgumentValidator(board);
            InitializeFields();
            InputBoard(board);
            BoardValidator(sudokuArrays);
            while (valueAdded == true)
            {
                valueAdded = false;
                for (int i = 1; i < 10; i++)
                    ScanByNumber(Convert.ToChar(i + '0'));
                if (valueAdded == false)
                {
                    for (int i = 0; i < 27; i++)
                        ScanNotes(i);
                }
            }
        }
    }
}

