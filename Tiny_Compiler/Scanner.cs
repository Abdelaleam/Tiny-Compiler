using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Digit, Letter, Number, String, Identifier,
    //ReservedWords
    INTEGER, FLOAT, STRING, READ, WRITE, REPEAT, UNTIL, IF, ELSEIF, ELSE, THEN, RETURN, ENDL, MAIN,END,
    //COMMENT
    Comment,
    //Arth Op
    PlusOp, MinusOp, MultiplyOp, DivideOp,
    //Assign Op
    AssignmentOp,
    //Condition Op
    LessThanOp, GreaterThanOp, EqualOp, NotEqualOp,
    //Boolean Op
    AndOp, OrOp,
    // {}
    Left_Curly_Brace, Right_Curly_Brace,
    Left_Square_Bracket, Right_Square_Bracket,
    left_parenthesis, right_parenthesis,
    //;
    Semicolon,Comma,Dot,

}
namespace Tiny_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }


    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
   
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.INTEGER);
            ReservedWords.Add("float", Token_Class.FLOAT);
            ReservedWords.Add("string", Token_Class.STRING);
            ReservedWords.Add("read", Token_Class.READ);
            ReservedWords.Add("write", Token_Class.WRITE);
            ReservedWords.Add("repeat", Token_Class.REPEAT);
            ReservedWords.Add("until", Token_Class.UNTIL);
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("elseif", Token_Class.ELSEIF);
            ReservedWords.Add("else", Token_Class.ELSE);
            ReservedWords.Add("then", Token_Class.THEN);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("endl", Token_Class.ENDL);
            ReservedWords.Add("main", Token_Class.MAIN);
            ReservedWords.Add("end", Token_Class.END);

            //Arth
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            //Assign
            Operators.Add(":=", Token_Class.AssignmentOp);
            //Conditions
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            //Boolean
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            //{}
            Operators.Add("{", Token_Class.Left_Curly_Brace);
            Operators.Add("}", Token_Class.Right_Curly_Brace);
            Operators.Add("(", Token_Class.left_parenthesis);
            Operators.Add(")", Token_Class.right_parenthesis);
            Operators.Add("[", Token_Class.Left_Square_Bracket);
            Operators.Add("]", Token_Class.Right_Square_Bracket);
            Operators.Add(";",Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add(".", Token_Class.Dot);



        }
        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                //single line
                if (CurrentChar == '/' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '/')
                {
                    j += 2;
                    CurrentLexeme = "//";
                    while (j < SourceCode.Length && SourceCode[j] != '\n' && SourceCode[j] != '\r')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;

                }
                //multiline
                //if (CurrentChar == '/' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '*')
                //{
                //    j += 2;
                //    CurrentLexeme = "/*"; 
                //    while (j + 1 < SourceCode.Length && !(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                //    {
                //        CurrentLexeme += SourceCode[j];
                //        j++;
                //    }
                //    if (j + 1 < SourceCode.Length)
                //    {
                //        CurrentLexeme += "*/";
                //        j++;
                //    }
                //    FindTokenClass(CurrentLexeme);
                //    i = j; 
                //    continue;
                //}
                else if (CurrentChar == '/') //Comment lexeme to disregard
                {
                    bool closed = false;
                    j++;
                    if (j < SourceCode.Length && SourceCode[j] == '*')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        try
                        {
                            while (j < SourceCode.Length)
                            {
                                CurrentLexeme += SourceCode[j].ToString();
                                j++;
                                if (SourceCode[j - 1] == '*' && SourceCode[j] == '/')
                                {
                                    CurrentLexeme += SourceCode[j].ToString();
                                    closed = true;
                                    i = j;
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Errors.Error_List.Add("Comment not closed");
                            closed = true;
                            i = j;
                            continue;
                        }
                        if (!closed)
                        {
                            Errors.Error_List.Add("Comment not closed");
                            i = j;
                            continue;
                        }

                    }
                }
                    //identifier & reserved
                    if ((CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z')) 
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        while (char.IsLetter(CurrentChar) || char.IsDigit(CurrentChar))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                            CurrentChar = SourceCode[j];
                        }
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1 ;
                    
                }
                //number or digit
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        while (char.IsDigit(CurrentChar) || CurrentChar == '.' || char.IsLetter(CurrentChar))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            j++;
                            if (j >= SourceCode.Length)
                                break;
                            CurrentChar = SourceCode[j];
                        }
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j-1;
                }
                //string
                else if (CurrentChar == '\"')
                {

                    j++;
                    CurrentChar = SourceCode[j];
                    CurrentLexeme += CurrentChar.ToString();
                    while (CurrentChar != '\"')
                    {

                        if (j == SourceCode.Length - 1)
                        {
                            break;
                        }
                        j++;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                    }
                    FindTokenClass(CurrentLexeme);

                    i = j;
                }

                //&&
                else if (CurrentChar == '&')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '&')
                        {

                            CurrentLexeme += SourceCode[j];
                        }
                        else
                        {
                            j--;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                //||
                else if (CurrentChar == '|')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '|')
                        {

                            CurrentLexeme += SourceCode[j];
                        }
                        else
                        {
                            j--;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                //<>
                else if (CurrentChar == '<')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '>')
                        {

                            CurrentLexeme += SourceCode[j];
                        }
                        else
                        {
                            j--;
                        }
                        FindTokenClass(CurrentLexeme);

                    }
                    i = j;
                }
                //Assign op
                else if (CurrentChar == ':')
                {
                    j++;
                    if (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '=')
                        {

                            CurrentLexeme += SourceCode[j];
                        }
                        else
                        {
                            j--;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                //Arth op  & others and error
                else
                {
                    FindTokenClass(CurrentLexeme);
                }
            }
          
            Tiny_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                TC = ReservedWords[Lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it an Identifier?
            else if (isIdentifier(Lex))
            {
                TC = Token_Class.Identifier;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                TC = Operators[Lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it a digit?
            //else if (isDigit(Lex))
            //{
            //    TC = Token_Class.Digit;
            //    Tok.token_type = TC;
            //    Tokens.Add(Tok);
            //}
            //Is it a Letter?
            else if (isLetter(Lex))
            {
                TC = Token_Class.Letter;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it a Number?
            else if (isNumber(Lex))
            {
                TC = Token_Class.Number;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            //Is it a String?
            else if (isString(Lex))
            {
                TC = Token_Class.String;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else if (isonelineComment(Lex)||ismultilineComment(Lex))
            {
                TC = Token_Class.Comment;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }

            else
            {
                Errors.Error_List.Add(Lex);
            }
        }
        //digit
        //bool isDigit(string lex)
        //{
        //    bool isValid = true;
        //    var p = new Regex("^[0-9]$");
        //    if (!p.IsMatch(lex))
        //        isValid = false;
        //    return isValid;
        //}
        //letter
        bool isLetter(string lex)
        {
            bool isValid = true;
            var p = new Regex("^[a-z]|[A-Z]$");
            if (!p.IsMatch(lex))
                isValid = false;
            return isValid;
        }
        //Number

        bool isNumber(string lex)
        {
            bool isValid = true;
            var p = new Regex(@"^[0-9]+(\.[0-9]+)?$");
            if (!p.IsMatch(lex))
                isValid = false;
            return isValid;
        }
        //string
        bool isString(string lex)
        {
            bool isValid = true;
           
            var p = new Regex("^\"[a-zA-Z0-9!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~–`\r\n ]+\"$");
            if (!p.IsMatch(lex))
                isValid = false;
            return isValid;
        }

        //identifier
        bool isIdentifier(string lex)
        {
            bool isValid = true;
            var p = new Regex("^([a-z]|[A-Z])+([0-9])*$");
            if (!p.IsMatch(lex))
                isValid = false;
            return isValid;
        }
      //  iscomment 
        bool isonelineComment(string lex)
        {
            bool isValid = true;
            var p = new Regex(@"^//[^\r\n]*$");
            if (!p.IsMatch(lex))
                isValid = false;
            return isValid;
        }
        
        bool ismultilineComment(string lex)
        {
       
            bool isValid = true;
            var P2 = new Regex(@"^/\*[a-zA-Z0-9!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~–’\r\n\t\s\f\v]*\*/$");
            if (!P2.IsMatch(lex))
                isValid = false;
            return isValid;
        }


    }
}